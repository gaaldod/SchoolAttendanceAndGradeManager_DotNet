using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace IskolaiJelenlet.Services
{
    public class ImportExportManager
    {

        /// Mass import courses and their data from multiple files in parallel using TPL.

        public async Task ImportDataAsync(IEnumerable<string> filePaths)
        {
            // Process files in parallel
            await Parallel.ForEachAsync(filePaths, async (filePath, cancellationToken) =>
            {
                await ProcessImportFileAsync(filePath);
            });
        }


        /// Mass export courses to multiple files in parallel using TPL.

        public async Task ExportDataAsync(IEnumerable<string> coursesToExport, string targetDirectory, string format)
        {
            // Ensure target directory exists
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Process exports in parallel
            await Parallel.ForEachAsync(coursesToExport, async (course, cancellationToken) =>
            {
                await ProcessExportCourseAsync(course, targetDirectory, format);
            });
        }

        private Task ProcessImportFileAsync(string filePath)
        {
            string extension = Path.GetExtension(filePath)?.ToLower();

            // Setup format handling
            switch (extension)
            {
                case ".xls":
                case ".xlsx":
                    // TODO: Implement actual Excel import logic (e.g. using ClosedXML or ExcelDataReader)
                    Console.WriteLine($"Importing Excel file: {filePath}");
                    break;
                case ".xml":
                    // TODO: Implement actual XML import logic
                    Console.WriteLine($"Importing XML file: {filePath}");
                    break;
                default:
                    Console.WriteLine($"Warning: Unsupported file format {extension} for file {filePath}");
                    break;
            }

            // Returning completed task as a stub
            return Task.CompletedTask;
        }

        private Task ProcessExportCourseAsync(string courseInfo, string targetDirectory, string format)
        {
            string targetPath = Path.Combine(targetDirectory, $"{courseInfo}_export.{format.TrimStart('.')}");

            // Setup format handling
            switch (format.ToLower().TrimStart('.'))
            {
                case "xls":
                case "xlsx":
                    // TODO: Implement actual Excel export logic
                    Console.WriteLine($"Exporting {courseInfo} to Excel: {targetPath}");
                    break;
                case "xml":
                    // TODO: Implement actual XML export logic
                    Console.WriteLine($"Exporting {courseInfo} to XML: {targetPath}");
                    break;
                default:
                    Console.WriteLine($"Warning: Unsupported export format {format}");
                    break;
            }

            // Returning completed task as a stub
            return Task.CompletedTask;
        }
    }
}
