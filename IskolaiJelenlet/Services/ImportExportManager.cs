using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Excel = ClosedXML.Excel;
using Microsoft.Data.Sqlite;
using Queries = IskolaiJelenlet.Services.SQLQueries_xlsxExporter;

namespace IskolaiJelenlet.Services
{
    public class ImportExportManager
    {

        public void MenuHandler_feladatok()
        {
            /*
             opcio1: CreateExcelTemplate()
            opcio2: ImportDataAsync()
            opcio3: ExportDataAsync()
             */
        }
        public void CreateExcelTemplate()
        {
            //ask for the target directory. if left empty, or not a valid path, use C:\Temp\ as default
            Console.WriteLine("Enter target directory for Excel template (default: C:\\Temp\\):");
            string targetDirectory = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                Console.WriteLine($"No target specified, Using default directory: {targetDirectory}");
                targetDirectory = @"C:\Temp\";
                //if c:\Temp\ does not exist, create it
            }
            else if (!Directory.Exists(targetDirectory))
            {
                Console.WriteLine($"Warning: Directory {targetDirectory} does not exist. Using default directory: C:\\Temp\\");
                targetDirectory = @"C:\Temp\";
            }
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            //append "school_template.xlsx" to targetdirectory
            targetDirectory = Path.Combine(targetDirectory, "school_template.xlsx");

            var workbook = new Excel.XLWorkbook();

            var attendanceSheet = workbook.Worksheets.Add("Attendance");
            attendanceSheet.Cell(1, 1).Value = "student_id";
            attendanceSheet.Cell(1, 2).Value = "lesson_id";
            attendanceSheet.Cell(1, 3).Value = "status";

            var courseSheet = workbook.Worksheets.Add("Course");
            courseSheet.Cell(1, 1).Value = "course_name";
            courseSheet.Cell(1, 2).Value = "description";

            var gradeSheet = workbook.Worksheets.Add("Grade");
            gradeSheet.Cell(1, 1).Value = "student_id";
            gradeSheet.Cell(1, 2).Value = "course_id";
            gradeSheet.Cell(1, 3).Value = "grade_value";
            gradeSheet.Cell(1, 4).Value = "grade_date";

            var lessonSheet = workbook.Worksheets.Add("Lesson");
            lessonSheet.Cell(1, 1).Value = "course_id";
            lessonSheet.Cell(1, 2).Value = "title";
            lessonSheet.Cell(1, 3).Value = "lesson_date";

            var noteSheet = workbook.Worksheets.Add("Note");
            noteSheet.Cell(1, 1).Value = "student_id";
            noteSheet.Cell(1, 2).Value = "lesson_id";
            noteSheet.Cell(1, 3).Value = "content";
            noteSheet.Cell(1, 4).Value = "created_at";

            var studentSheet = workbook.Worksheets.Add("Student");
            studentSheet.Cell(1, 1).Value = "first_name";
            studentSheet.Cell(1, 2).Value = "last_name";
            studentSheet.Cell(1, 3).Value = "email";
            studentSheet.Cell(1, 4).Value = "enrollment_date";

            // --- ADD THIS HELPER METHOD ---
            void FormatAsDateColumn(Excel.IXLWorksheet sheet, int colNumber)
            {
                var col = sheet.Column(colNumber);
                col.Style.DateFormat.Format = "yyyy-mm-dd";
                
                // Add strict data validation from row 2 downwards
                var dateRange = sheet.Range(2, colNumber, 1048576, colNumber);
                
                // Fix for the Obsolete warning: Use CreateDataValidation()
                var validation = dateRange.CreateDataValidation();
                validation.AllowedValues = Excel.XLAllowedValues.Date;
                validation.ShowErrorMessage = true;
                validation.ErrorTitle = "Invalid Date";
                validation.ErrorMessage = "Please enter a valid date (e.g., yyyy-mm-dd).";
            }

            // Apply it ONLY to the 4 specific date columns:
            FormatAsDateColumn(gradeSheet, 4);   // grade_date (Column 4)
            FormatAsDateColumn(lessonSheet, 3);  // lesson_date (Column 3)
            FormatAsDateColumn(noteSheet, 4);    // created_at (Column 4)
            FormatAsDateColumn(studentSheet, 4); // enrollment_date (Column 4)

            // Save the template
            workbook.SaveAs(targetDirectory);
        }



        private async Task ProcessImportFileAsync(string filePath)
        {
            string? extension = Path.GetExtension(filePath)?.ToLower();

            if (extension != ".xlsx")
            {
                Console.WriteLine($"Error: Unsupported file format '{extension}'. Please provide an .xlsx file.");
                return;
            }

            // Using tuple with ErrorMessage to capture the exact SQL exception
            var importResults = new Dictionary<string, (int Success, List<(int Row, string ErrorMessage)> FailedRows)>();

            try
            {
                Console.WriteLine("\nConnecting to the database and starting import...");

                // Get the directory where the .exe currently lives
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                string dbPath = Path.Combine(baseDir, "Database", "iskola.db");

                string connectionString = $"Data Source={dbPath}";
                using var connection = new SqliteConnection(connectionString);
                await connection.OpenAsync();

                using var workbook = new Excel.XLWorkbook(filePath);

                foreach (var sheet in workbook.Worksheets)
                {
                    string tableName = sheet.Name;
                    int successCount = 0;
                    var failedRows = new List<(int Row, string ErrorMessage)>();

                    var headerRow = sheet.Row(1);
                    int columnCount = headerRow.LastCellUsed()?.Address.ColumnNumber ?? 0;

                    if (columnCount == 0) continue;

                    // Extract columns from Header (Row 1)
                    var columnNames = new List<string>();
                    for (int i = 1; i <= columnCount; i++)
                    {
                        string colName = headerRow.Cell(i).GetString().Trim();
                        if (!string.IsNullOrWhiteSpace(colName))
                        {
                            columnNames.Add(colName);
                        }
                    }

                    if (columnNames.Count == 0) continue;

                    //DEBUG START
                    try
                    {
                        using var pragmaCmd = new SqliteCommand($"PRAGMA foreign_key_list([{tableName}])", connection);
                        using var reader = await pragmaCmd.ExecuteReaderAsync();
                        Console.WriteLine($"\n--- DEBUG: Foreign keys for {tableName} ---");
                        bool hasFks = false;
                        while (await reader.ReadAsync())
                        {
                            hasFks = true;
                            string toTable = reader.GetString(2); 
                            string fromCol = reader.GetString(3); 
                            string toCol = reader.GetString(4);   
                            Console.WriteLine($"  Column '{fromCol}' MUST exist in table '{toTable}' column '{toCol}'");
                        }
                        if (!hasFks) Console.WriteLine("  No foreign keys found for this table.");
                    }
                    catch (Exception ex) { Console.WriteLine($"  Error reading Pragma: {ex.Message}"); }
                    //DEBUG END

                    // Dynamically build the INSERT INTO query
                    string columnsJoined = string.Join(", ", columnNames);
                    string parametersJoined = string.Join(", ", columnNames.Select(c => $"@{c}"));
                    string query = $"INSERT INTO [{tableName}] ({columnsJoined}) VALUES ({parametersJoined})";

                    // Start from Row 2 and read down until we hit an empty cell in column 1
                    int currentRowNumber = 2;
                    while (true)
                    {
                        var firstCell = sheet.Cell(currentRowNumber, 1);
                        if (firstCell.IsEmpty())
                        {
                            break; // Stop reading this worksheet once the first column is empty
                        }

                        var row = sheet.Row(currentRowNumber);

                        try
                        {
                            using var command = new SqliteCommand(query, connection);

                            for (int i = 0; i < columnNames.Count; i++)
                            {
                                var cell = row.Cell(i + 1);
                                string colName = columnNames[i];
                                string paramName = $"@{colName}";
                                
                                // Define our specific date columns
                                var dateColumns = new[] { "grade_date", "lesson_date", "created_at", "enrollment_date" };
                                bool isDateColumn = dateColumns.Contains(colName.ToLower());

                                object sqlValue;
                                if (cell.IsEmpty()) 
                                {
                                    sqlValue = DBNull.Value;
                                }
                                else if (isDateColumn && cell.TryGetValue(out DateTime dateValue)) 
                                {
                                    sqlValue = dateValue.ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                else 
                                {
                                    // Treat as standard string/number
                                    sqlValue = cell.GetString().Trim(); 
                                }

                                command.Parameters.AddWithValue(paramName, sqlValue);
                            }

                            //DEBUG START
                            if (tableName == "Note")
                            {
                                Console.WriteLine($"\n--- DEBUG: Inserting into {tableName} (Row {currentRowNumber}) ---");
                                foreach (SqliteParameter param in command.Parameters)
                                {
                                    Console.WriteLine($"  {param.ParameterName} = '{param.Value}' (C# Type: {param.Value?.GetType().Name})");
                                }
                            }
                            //DEBUG END

                            await command.ExecuteNonQueryAsync();
                            successCount++;
                        }
                        catch (Exception ex)
                        {
                            // Capture the exact exception message!
                            failedRows.Add((currentRowNumber, ex.Message));
                        }

                        currentRowNumber++;
                    }

                    importResults.Add(tableName, (successCount, failedRows));
                }

                // Print the summary
                Console.WriteLine("\n=== IMPORT RESULTS ===");
                foreach (var result in importResults)
                {
                    string tableName = result.Key;
                    int successful = result.Value.Success;
                    var failures = result.Value.FailedRows;

                    Console.WriteLine($"Successfully imported {successful} rows of data to [{tableName}].");

                    if (failures.Count > 0)
                    {
                        Console.WriteLine($"Unsuccessfully imported the following rows to [{tableName}]:");
                        foreach (var failure in failures)
                        {
                            Console.WriteLine($"  - Row {failure.Row}: {failure.ErrorMessage}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nA critical error occurred during the database import: {ex.Message}");
            }
        }

        public async Task ImportDataAsync(IEnumerable<string> filePaths)
        {
            string? selectedFilePath = null;
            int attempts = 0;

            while (attempts < 3)
            {
                Console.WriteLine("Enter the file path to import (or a directory containing .xlsx files):");
                // Trim quotes in case the user pastes using "Copy as path"
                string? inputPath = Console.ReadLine()?.Trim('"', ' ');

                if (string.IsNullOrWhiteSpace(inputPath))
                {
                    Console.WriteLine("Warning: Input empty. Please try again.");
                    attempts++;
                    continue;
                }

                if (File.Exists(inputPath))
                {
                    selectedFilePath = inputPath;
                    break;
                }
                else if (Directory.Exists(inputPath))
                {
                    var excelFiles = Directory.GetFiles(inputPath, "*.xlsx");
                    
                    if (excelFiles.Length == 0)
                    {
                        Console.WriteLine($"Warning: No .xlsx files found in directory '{inputPath}'.");
                        attempts++;
                        continue;
                    }

                    Console.WriteLine("\nFound the following Excel files:");
                    foreach (var file in excelFiles)
                    {
                        Console.WriteLine($"- {Path.GetFileName(file)}");
                    }

                    bool fileSelected = false;
                    while (!fileSelected)
                    {
                        Console.WriteLine("\nPlease type the exact name of the file you want to import (e.g. 'data.xlsx'), or type 'cancel' to pick a new path:");
                        string? fileName = Console.ReadLine()?.Trim('"', ' ');

                        if (fileName?.Equals("cancel", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            break; // Break the inner loop, continue outer path prompt
                        }

                        if (!string.IsNullOrWhiteSpace(fileName))
                        {
                            string potentialPath = Path.Combine(inputPath, fileName);
                            // Verify it actually exists (handling exactly what they typed)
                            if (File.Exists(potentialPath))
                            {
                                selectedFilePath = potentialPath;
                                fileSelected = true;
                            }
                            else
                            {
                                Console.WriteLine($"Typo? The file '{fileName}' was not found in the directory. Try again.");
                            }
                        }
                    }

                    if (fileSelected)
                    {
                        break; // Step out of the main retry loop
                    }
                    else
                    {
                        attempts++; // Count "cancel" as an attempt
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: The path '{inputPath}' does not exist. Please try again.");
                    attempts++;
                }
            }

            if (selectedFilePath == null)
            {
                Console.WriteLine("Error: Maximum attempts reached or cancelled. Exiting import process.");
                return;
            }

            Console.WriteLine($"\nStarting import for: {selectedFilePath}");
            await ProcessImportFileAsync(selectedFilePath);
        }
        public async Task ExportDataToExcel(IEnumerable<string> coursesToExport, string targetDirectory, string format)
        {
            //connect to iskola.db and ask the user multiple times to select courses, students or grades to export for a specific student, or students on a course. when a selection is finished, go back to "do you want to add another item to the export?" with 0 meaning no, export.

        }

        private Task ProcessExportCourse()
        {
            // Placeholder for export logic, e.g., fetching course data and writing to Excel


            // Returning completed task as a stub
            return Task.CompletedTask;
        }

        public async Task InteractiveExportMenuAsync()
        {
            int currentPage = 1;
            bool exitMenu = false;

            while (!exitMenu)
            {
                Console.Clear();
                Console.WriteLine("=== EXPORT DATA TO EXCEL ===");
                
                if (currentPage == 1)
                {
                    Console.WriteLine("Page 1 of 2");
                    Console.WriteLine("1: Attendance (All statuses with Student & Lesson info)");
                    Console.WriteLine("2: Attendance (Filter by Student)");
                    Console.WriteLine("3: Attendance (Filter by Lesson)");
                    Console.WriteLine("4: All Courses");
                    Console.WriteLine("5: Grades (All)");
                    Console.WriteLine("6: Grades (Filter by Student)");
                    Console.WriteLine("7: Grades (Filter by Course & Student)");
                    Console.WriteLine("8: Grades (Filter by Course for all students)");
                    Console.WriteLine("9: Next Page --->");
                    Console.WriteLine("0: Exit Export Menu");
                }
                else if (currentPage == 2)
                {
                    Console.WriteLine("Page 2 of 2");
                    Console.WriteLine("1: All Lessons");
                    Console.WriteLine("2: Notes (All)");
                    Console.WriteLine("3: Notes (Filter by Student)");
                    Console.WriteLine("4: All Students");
                    Console.WriteLine("9: <--- Previous Page (Same as 0)");
                    Console.WriteLine("0: <--- Previous Page");
                }

                Console.Write("\nSelect an option: ");
                var key = Console.ReadKey(true).KeyChar;

                if (currentPage == 1)
                {
                    switch (key)
                    {
                        case '1': await RunExportAsync("AttendanceAll"); break;
                        case '2': await RunExportAsync("AttendanceByStudent"); break;
                        case '3': await RunExportAsync("AttendanceByLesson"); break;
                        case '4': await RunExportAsync("CoursesAll"); break;
                        case '5': await RunExportAsync("GradesAll"); break;
                        case '6': await RunExportAsync("GradesByStudent"); break;
                        case '7': await RunExportAsync("GradesByCourseAndStudent"); break;
                        case '8': await RunExportAsync("GradesByCourse"); break;
                        case '9': currentPage = 2; break;
                        case '0': exitMenu = true; break;
                    }
                }
                else if (currentPage == 2)
                {
                    switch (key)
                    {
                        case '1': await RunExportAsync("LessonsAll"); break;
                        case '2': await RunExportAsync("NotesAll"); break;
                        case '3': await RunExportAsync("NotesByStudent"); break;
                        case '4': await RunExportAsync("StudentsAll"); break;
                        case '9':
                        case '0': currentPage = 1; break;
                    }
                }
            }
        }

        private async Task RunExportAsync(string exportType)
        {
            Console.Clear();
            Console.WriteLine($"=== EXPORTING: {exportType} ===");

            string query = "";
            string? studentId = null;
            string? courseId = null;
            string? lessonId = null;

            // Connect to the database FIRST so we can query available options
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string dbPath = Path.Combine(baseDir, "Database", "iskola.db");
            using var connection = new SqliteConnection($"Data Source={dbPath}");
            await connection.OpenAsync();

            // 1. Map the query and prompt for required parameters (showing lists)
            switch (exportType)
            {
                case "AttendanceAll": query = Queries.AttendanceAll; break;
                case "AttendanceByStudent":
                    query = Queries.AttendanceByStudent;
                    await DisplayAvailableStudentsAsync(connection);
                    studentId = PromptForInput("Enter Student ID: ");
                    break;
                case "AttendanceByLesson":
                    query = Queries.AttendanceByLesson;
                    await DisplayAvailableLessonsAsync(connection);
                    lessonId = PromptForInput("Enter Lesson ID: ");
                    break;
                case "CoursesAll": query = Queries.CoursesAll; break;
                case "GradesAll": query = Queries.GradesAll; break;
                case "GradesByStudent":
                    query = Queries.GradesByStudent;
                    await DisplayAvailableStudentsAsync(connection);
                    studentId = PromptForInput("Enter Student ID: ");
                    break;
                case "GradesByCourse":
                    query = Queries.GradesByCourse;
                    await DisplayAvailableCoursesAsync(connection);
                    courseId = PromptForInput("Enter Course ID: ");
                    break;
                case "GradesByCourseAndStudent":
                    query = Queries.GradesByCourseAndStudent;
                    await DisplayAvailableCoursesAsync(connection);
                    courseId = PromptForInput("Enter Course ID: ");
                    await DisplayAvailableStudentsAsync(connection);
                    studentId = PromptForInput("Enter Student ID: ");
                    break;
                case "LessonsAll": query = Queries.LessonsAll; break;
                case "NotesAll": query = Queries.NotesAll; break;
                case "NotesByStudent":
                    query = Queries.NotesByStudent;
                    await DisplayAvailableStudentsAsync(connection);
                    studentId = PromptForInput("Enter Student ID: ");
                    break;
                case "StudentsAll": query = Queries.StudentsAll; break;
                default:
                    Console.WriteLine("Unknown export type.");
                    return;
            }

            // 2. Bind parameters to the main query
            using var command = new SqliteCommand(query, connection);
            if (studentId != null) command.Parameters.AddWithValue("@student_id", studentId);
            if (courseId != null) command.Parameters.AddWithValue("@course_id", courseId);
            if (lessonId != null) command.Parameters.AddWithValue("@lesson_id", lessonId);

            Console.WriteLine("\nGenerating Excel file, please wait...");

            // 3. Execute query and write to Excel
            using var reader = await command.ExecuteReaderAsync();
            using var workbook = new Excel.XLWorkbook();
            var sheet = workbook.Worksheets.Add("Exported Data");

            // Write Headers
            for (int i = 0; i < reader.FieldCount; i++)
            {
                sheet.Cell(1, i + 1).Value = reader.GetName(i);
                sheet.Cell(1, i + 1).Style.Font.Bold = true;
            }

            // Write Data Rows
            int currentRow = 2;
            bool hasData = false;
            while (await reader.ReadAsync())
            {
                hasData = true;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    object value = reader.GetValue(i);
                    sheet.Cell(currentRow, i + 1).Value = value == DBNull.Value ? "" : value.ToString();
                }
                currentRow++;
            }

            if (!hasData)
            {
                Console.WriteLine("\nNo data found for this selection! File was not generated.");
            }
            else
            {
                // Format file name with date to avoid overwriting or file lock exceptions
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string savePath = $@"C:\Temp\Export_{exportType}_{timestamp}.xlsx";
                
                if (!Directory.Exists(@"C:\Temp")) Directory.CreateDirectory(@"C:\Temp");

                sheet.Columns().AdjustToContents();
                workbook.SaveAs(savePath);

                Console.WriteLine($"\nSUCCESS: Exported {currentRow - 2} rows to {savePath}");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey(true);
        }

        // --- HELPER METHODS FOR INTERACTIVE LISTS ---
        
        private string PromptForInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }

        private async Task DisplayAvailableStudentsAsync(SqliteConnection connection)
        {
            Console.WriteLine("\n--- Available Students ---");
            using var cmd = new SqliteCommand("SELECT student_id, first_name, last_name FROM Student", connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"[ID: {reader["student_id"]}] {reader["first_name"]} {reader["last_name"]}");
            }
            Console.WriteLine("--------------------------");
        }

        private async Task DisplayAvailableCoursesAsync(SqliteConnection connection)
        {
            Console.WriteLine("\n--- Available Courses ---");
            using var cmd = new SqliteCommand("SELECT course_id, course_name FROM Course", connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"[ID: {reader["course_id"]}] {reader["course_name"]}");
            }
            Console.WriteLine("-------------------------");
        }

        private async Task DisplayAvailableLessonsAsync(SqliteConnection connection)
        {
            Console.WriteLine("\n--- Available Lessons ---");
            using var cmd = new SqliteCommand("SELECT lesson_id, title, lesson_date FROM Lesson", connection);
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"[ID: {reader["lesson_id"]}] {reader["title"]} ({reader["lesson_date"]})");
            }
            Console.WriteLine("-------------------------");
        }
    }
}