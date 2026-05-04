using Controllers;
using System;
using System.IO;
using Microsoft.Data.Sqlite;
using IskolaiJelenlet.Services;
using ImportExportManager = IskolaiJelenlet.Services.ImportExportManager;
using System.Linq;

// Get the directory where the .exe currently lives
string baseDir = AppDomain.CurrentDomain.BaseDirectory;
string dbPath = Path.Combine(baseDir, "Database", "iskola.db");

string connectionString = $"Data Source={dbPath}";
// --- DIAGNOSTIC TEST: Check iskola.db tables ---
Console.WriteLine($"Looking for DB at: {Path.GetFullPath(dbPath)}");

if (File.Exists(dbPath))
{
    Console.WriteLine("Database file exists! Reading tables...");
    using var connection = new SqliteConnection($"Data Source={dbPath}");
    connection.Open();

    using var command = connection.CreateCommand();
    command.CommandText = "SELECT name FROM sqlite_master WHERE type='table'";
    
    using var reader = command.ExecuteReader();
    bool hasTables = false;
    while (reader.Read())
    {
        hasTables = true;
        Console.WriteLine($"- Found Table: {reader.GetString(0)}");
    }

    if (!hasTables)
    {
        Console.WriteLine("The database exists, but it contains NO tables.");
    }
}
else
{
    Console.WriteLine("CRITICAL: The database file does NOT exist at this location.");
}
Console.WriteLine("--------------------------------------------------\n");

var manager = new ImportExportManager();

// Test 1: Create Template
// manager.CreateExcelTemplate();

// Test 2: Import Data 
// (Passes an empty list since the method asks for the file/directory path in the console anyway)
//await manager.ImportDataAsync(Enumerable.Empty<string>());

/*
// Test 3: Export Data (using dummy properties for testing)
var coursesToExport = new[] { "Math101", "History101" };
await manager.ExportDataAsync(coursesToExport, @"C:\Temp\Export", "xlsx");
*/
// Return immediately for testing purposes, ignoring the main menu

//test 4: Export Data with user input
await manager.InteractiveExportMenuAsync();


return 0;

namespace IskolaiJelenlet
{
    internal class Program
    {
        static void Main(string[] args)
        {
            

            var menu = 0;
            var Controllerek = new Feladatok();
            
            do
            {
                Console.Write("1: Óra indítás\n2: Óra befejezés" +
                    "\n3: Diákok kezelése\n4: Jegy kezelő\n5: Jelenlét kezelő\n0: Kilépés\nKérem válasszon menüpontot:");
                var scanner = Console.ReadLine();
                menu=Convert.ToInt32(scanner);
                Console.Clear();
            } while ( (menu < 0) || (menu > 5) );
            Console.Clear();
            switch (menu)
            {
                case 1:
                    Controllerek.OraInditas();
                    break;
                case 2:
                    Controllerek.OraBefejezes();
                    break;
                case 3:
                    Controllerek.DiakokKezelese();
                    break;
                case 4:
                    Controllerek.JegyKezeles();
                    break;
                case 5:
                    Controllerek.JelenletKezeles();
                    break;
                default:
                    break;
            }
        }
    }
}
