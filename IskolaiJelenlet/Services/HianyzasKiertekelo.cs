using System.Text.Json;
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

                if (absentPercentage > 30)
                {
                    warnings.Add(new EvaluationResult(studentId, studentName, courseId, courseName, "Absent", absentPercentage));
                }
                if (latePercentage > 45)
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

            // Filter skipped warnings - because the percentage is in the key, if the % changes, it shows up again!
            var activeWarnings = warnings
                .Where(w => !suppressed.Contains(w.GetUniqueKey()))
                .OrderByDescending(w => w.Percentage)
                .ToList();

            if (activeWarnings.Count == 0) return;

            Console.WriteLine("\n=== AUTOMATIC ATTENDANCE WARNINGS ===");
            foreach (var w in activeWarnings)
            {
                Console.WriteLine($"- {w.StudentName} ({w.CourseName}): {w.Percentage:F1}% {w.Type}");
            }

            Console.WriteLine("\nWhat would you like to do with these warnings?");
            Console.WriteLine("1: Save list to C:\\Temp and suppress them (until the percentage changes)");
            Console.WriteLine("2: Just suppress them (until the percentage changes)");
            Console.WriteLine("3: Skip to main menu (will show again next time)");
            
            bool validChoice = false;
            while (!validChoice)
            {
                Console.Write("Select an option (1-3): ");
                var key = Console.ReadLine()?.Trim();
                
                switch (key)
                {
                    case "1":
                        SaveWarningsToTemp(activeWarnings);
                        SuppressWarnings(activeWarnings, suppressed);
                        Console.WriteLine("List saved and warnings suppressed. Loading main menu...");
                        System.Threading.Thread.Sleep(1500); // Pause so they can read the message
                        validChoice = true;
                        break;
                    case "2":
                        SuppressWarnings(activeWarnings, suppressed);
                        Console.WriteLine("Warnings suppressed. Loading main menu...");
                        System.Threading.Thread.Sleep(1500); // Pause so they can read the message
                        validChoice = true;
                        break;
                    case "3":
                        validChoice = true; // Instantly go to menu
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }
            }
        }

        private void SaveWarningsToTemp(List<EvaluationResult> activeWarnings)
        {
            string targetDir = @"C:\Temp";
            if (!Directory.Exists(targetDir))
            {
                Directory.CreateDirectory(targetDir);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string filePath = Path.Combine(targetDir, $"AttendanceWarnings_{timestamp}.txt");

            using StreamWriter writer = new StreamWriter(filePath);
            writer.WriteLine($"Attendance Warnings Report - {DateTime.Now}");
            writer.WriteLine(new string('-', 50));
            foreach (var w in activeWarnings)
            {
                writer.WriteLine($"{w.StudentName} | Course: {w.CourseName} | Issue: {w.Type} | Percentage: {w.Percentage:F1}%");
            }
            
            Console.WriteLine($"\nFile successfully saved to: {filePath}");
        }

        private void SuppressWarnings(List<EvaluationResult> activeWarnings, HashSet<string> suppressed)
        {
            foreach (var w in activeWarnings)
            {
                suppressed.Add(w.GetUniqueKey());
            }
            // Save updated suppressed warnings to file, in the directory the program is launched from
            File.WriteAllText(SuppressedWarningsFile, JsonSerializer.Serialize(suppressed));
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

        // By putting the formatted percentage inside the key, the key changes if the percentage changes!
        public string GetUniqueKey() => $"{StudentId}_{CourseId}_{Type}_{Percentage:F1}";
    }
}