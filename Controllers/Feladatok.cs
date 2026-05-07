using System.ComponentModel.DataAnnotations;
using Models;
using Microsoft.Data.Sqlite;
using System.IO;

namespace Controllers
{
    public class Feladatok

    {
       
        public Lesson OraInditas()
        {
            
            Console.WriteLine("Tanóra indítás:");
            Console.WriteLine("1: Testnevelés\n2:Informatika\n3:Magyar irodalom\n4:Angol nyelv");
            Console.Write("Kérem adja meg kurzus sorszámát:");
            var oraKod = Convert.ToInt32(Console.ReadLine());

            Console.Write("Kérem adja meg az tanóra címét:");
            #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            string oraCim = Console.ReadLine();
            #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
            Console.Write("Kérem adja meg az tanóra kezdetének évét:");
            var ev = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kérem adja meg az tanóra kezdetének hónapját:");
            var honap  = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kérem adja meg az tanóra kezdetének napját:");
            var nap = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kérem adja meg az tanóra kezdetének óráját:");
            var ora = Convert.ToInt32(Console.ReadLine());
            Console.Write("Kérem adja meg az tanóra kezdetének percét:");
            var perc = Convert.ToInt32(Console.ReadLine());

            string kezdesFormatted = DatumKonverter(new DateTime(ev, honap, nap, ora, perc, 00));

            var Tanora = new Lesson(oraKod,oraCim, new DateTime(ev, honap, nap, ora, perc, 00));
            
            return Tanora;

        }
        public void OraRiportGeneralas()
        {
            Console.WriteLine("Tanóra riport generálása:");
        }
        public void JelenletKezeles(Tanar tanar)
        {
            Console.Clear();
            Console.WriteLine("Jelenlét nyilvantartó:");
            
            Console.Write("Adja meg a hiányzó diákok nevét vesszővel elválasztva:");
            var sc = Console.ReadLine();
            string[] hianyzoDiakok = sc.Split(",");
            for (int i = 0; i < hianyzoDiakok.Length; i++)
            {
                hianyzoDiakok[i] = (hianyzoDiakok[i].StartsWith(" ")) ? hianyzoDiakok[i].Substring(1, hianyzoDiakok[i].Length)  : hianyzoDiakok[i];
                
            }
            Console.ReadKey();

        }
        public void JegyKezeles()
        {
            Console.Clear();
            Console.WriteLine("=== Jegyek Kezelése (Modosítás / Törlés) ===");
            Console.Write("Kérem adja meg a diák azonosítóját (Student ID): ");
            var studentId = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(studentId)) return;

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDir, "Database", "iskola.db");

            using var connection = new SqliteConnection($"Data Source={dbPath}");
            connection.Open();

            // 1. List the grades for the student
            string selectQuery = @"
                SELECT g.course_id, c.course_name, g.grade_value, g.grade_date 
                FROM Grade g
                JOIN Course c ON g.course_id = c.course_id
                WHERE g.student_id = @student_id
                ORDER BY g.grade_date DESC";

            using var selectCmd = new SqliteCommand(selectQuery, connection);
            selectCmd.Parameters.AddWithValue("@student_id", studentId);

            var availableGrades = new List<(string CourseId, string CourseName, string GradeValue, string GradeDate)>();
            using var reader = selectCmd.ExecuteReader();

            while (reader.Read())
            {
                availableGrades.Add((
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetString(3)
                ));
            }
            if (availableGrades.Count == 0)
            {
                Console.WriteLine("Ennek a diáknak még nincsenek beírt jegyei.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nTalált jegyek:");
            for (int i = 0; i < availableGrades.Count; i++)
            {
                var g = availableGrades[i];
                Console.WriteLine($"{i + 1}. Tantárgy: {g.CourseName} | Jegy: {g.GradeValue} | Dátum: {g.GradeDate}");
            }

            // 2. Select a grade to modify/delete
            Console.Write("\nKérem válassza ki a módosítani/törölni kívánt jegy sorszámát (kilépéshez hagyja üresen): ");
            var listIndexInput = Console.ReadLine();
            if (!int.TryParse(listIndexInput, out int listIndex) || listIndex < 1 || listIndex > availableGrades.Count) return;

            var targetGrade = availableGrades[listIndex - 1];

            // 3. Choose action
            Console.WriteLine($"\nKiválasztva: {targetGrade.CourseName} - {targetGrade.GradeValue} ({targetGrade.GradeDate})");
            Console.WriteLine("M: Módosítás");
            Console.WriteLine("T: Törlés");
            Console.Write("Válasszon műveletet: ");
            var action = Console.ReadLine()?.Trim().ToUpper();

            if (action == "T") // DELETE
            {
                string deleteQuery = @"
                    DELETE FROM Grade 
                    WHERE student_id = @student_id 
                    AND course_id = @course_id 
                    AND grade_date = @grade_date";

                using var deleteCmd = new SqliteCommand(deleteQuery, connection);
                deleteCmd.Parameters.AddWithValue("@student_id", studentId);
                deleteCmd.Parameters.AddWithValue("@course_id", targetGrade.CourseId);
                deleteCmd.Parameters.AddWithValue("@grade_date", targetGrade.GradeDate);

                deleteCmd.ExecuteNonQuery();
                Console.WriteLine("\nSikeres törlés!");
            }
            else if (action == "M") // UPDATE
            {
                Console.Write("Kérem adja meg az új jegyet: ");
                var newGrade = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(newGrade)) return;

                string updateQuery = @"
                    UPDATE Grade 
                    SET grade_value = @new_grade 
                    WHERE student_id = @student_id 
                    AND course_id = @course_id 
                    AND grade_date = @grade_date";

                using var updateCmd = new SqliteCommand(updateQuery, connection);
                updateCmd.Parameters.AddWithValue("@new_grade", newGrade);
                updateCmd.Parameters.AddWithValue("@student_id", studentId);
                updateCmd.Parameters.AddWithValue("@course_id", targetGrade.CourseId);
                updateCmd.Parameters.AddWithValue("@grade_date", targetGrade.GradeDate);

                updateCmd.ExecuteNonQuery();
                Console.WriteLine("\nSikeres módosítás!");
            }

            Console.WriteLine("Nyomjon meg egy gombot a visszatéréshez...");
            Console.ReadKey();
        }

        public void DiakokKezelese()
        {
            Console.WriteLine("A diákok kezelése jelenleg az Excel Import/Export fülön lehetséges!");
        }
        public void OrakListaFeltoltes()
        {

        }

        public void HianyzasSzamitas()
        {

        }

        public Note JegyzetHozzaadas()
        {
            Console.Clear();   
            Console.WriteLine("Jegyzet írása:");
            Console.Write("Adja meg a diák nevét: ");
            var diakneve = Console.ReadLine();
            Console.Write("Adja meg az óra nevét: ");
            var Oranev = Console.ReadLine();
            Console.Write("Adja meg a jegyzetet ");
            var jegyezet = Console.ReadLine();
            return new Note(diakneve,Oranev,jegyezet);
        }

        public String DatumKonverter(DateTime ido)
        {
            return ido.ToString("yyyy-MM-dd HH:mm:ss");
        }




    }


    enum Hianyzas { 
        Present,Absent,Late
    }


}
