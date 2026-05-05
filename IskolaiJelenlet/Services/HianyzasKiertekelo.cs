using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace IskolaiJelenlet.Services
{
    public class HianyzasKiertekelo
    {
        private const string SuppressedWarningsFile = "warnings.json";
        private readonly string _dbPath;

        public HianyzasKiertekelo()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _dbPath = Path.Combine(baseDir, "Database", "iskola.db");
        }

        public async Task EvaluateAbsencesAsync()
        {
            var warnings = new List<EvaluationResult>();

            string query = @"
                SELECT 
                    s.student_id, s.last_name, s.first_name, 
                    c.course_id, c.description AS course_name,
                    COUNT(a.lesson_id) AS total_lessons,
                    SUM(CASE WHEN a.status = 'Absent' THEN 1 ELSE 0 END) AS absent_count,
                    SUM(CASE WHEN a.status = 'Late' THEN 1 ELSE 0 END) AS late_count
                FROM Attendance a
                JOIN Student s ON a.student_id = s.student_id
                JOIN Lesson l ON a.lesson_id = l.lesson_id
                JOIN Course c ON l.course_id = c.course_id
                GROUP BY s.student_id, s.last_name, s.first_name, c.course_id, c.description
                HAVING COUNT(a.lesson_id) > 0";

            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            await connection.OpenAsync();
            using var command = new SqliteCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                string studentId = reader.GetString(0);
                string studentName = $"{reader.GetString(1)} {reader.GetString(2)}";
                string courseId = reader.GetString(3);
                string courseName = reader.GetString(4);

                int totalLessons = reader.GetInt32(5);
                int absentCount = reader.GetInt32(6);
                int lateCount = reader.GetInt32(7);

                double absentPercentage = (double)absentCount / totalLessons * 100;
                double latePercentage = (double)lateCount / totalLessons * 100;

                if (absentPercentage > 20)
                {
                    warnings.Add(new EvaluationResult(studentId, studentName, courseId, courseName, "Absent", absentPercentage));
                }
                if (latePercentage > 50)
                {
                    warnings.Add(new EvaluationResult(studentId, studentName, courseId, courseName, "Late", latePercentage));
                }
            }

            ProcessAndDisplayWarnings(warnings);
        }

        private void ProcessAndDisplayWarnings(List<EvaluationResult> warnings)
        {
            if (warnings.Count == 0) return;

            // Load suppressed warnings
            HashSet<string> suppressed = new();
            if (File.Exists(SuppressedWarningsFile))
            {
                string json = File.ReadAllText(SuppressedWarningsFile);
                suppressed = JsonSerializer.Deserialize<HashSet<string>>(json) ?? new HashSet<string>();
            }

            // Filter skipped warnings
            var activeWarnings = warnings.Where(w => !suppressed.Contains(w.GetUniqueKey())).OrderByDescending(w => w.Percentage).ToList();

            if (activeWarnings.Count == 0) return;

            Console.WriteLine("\n=== AUTOMATIC ATTENDANCE WARNINGS ===");

            bool hasLateWarnings = false;
            foreach (var w in activeWarnings)
            {
                Console.WriteLine($"- {w.StudentName} ({w.CourseName}): {w.Percentage:F1}% {w.Type}");
                if (w.Type == "Late") hasLateWarnings = true;
            }

            if (hasLateWarnings)
            {
                Console.Write("\nWould you like to suppress these 'Late' warnings for future runs? (y/n): ");
                var key = Console.ReadLine()?.Trim().ToLower();
                if (key == "y")
                {
                    foreach (var w in activeWarnings.Where(x => x.Type == "Late"))
                    {
                        suppressed.Add(w.GetUniqueKey());
                    }
                    File.WriteAllText(SuppressedWarningsFile, JsonSerializer.Serialize(suppressed));
                    Console.WriteLine("Late warnings suppressed.");
                }
            }

            Console.WriteLine("Press any key to continue to the main menu...");
            Console.ReadKey(true);
        }
    }

    public class EvaluationResult
    {
        public string StudentId { get; }
        public string StudentName { get; }
        public string CourseId { get; }
        public string CourseName { get; }
        public string Type { get; }
        public double Percentage { get; }

        public EvaluationResult(string studentId, string studentName, string courseId, string courseName, string type, double percentage)
        {
            StudentId = studentId;
            StudentName = studentName;
            CourseId = courseId;
            CourseName = courseName;
            Type = type;
            Percentage = percentage;
        }

        public string GetUniqueKey() => $"{StudentId}_{CourseId}_{Type}";
    }
}