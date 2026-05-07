using Controllers;
using IskolaiJelenlet.Services;
using Models;
using ImportExportManager = IskolaiJelenlet.Services.ImportExportManager;

namespace IskolaiJelenlet
{
    public class Program
    {
        static async Task Main(string[] args) //changed it to async so the HianyzasKiertekelo can run asynchronously and not block the main thread
        {         
            var Controllerek = new Feladatok();
            List<Lesson> Orak = new List<Lesson>();
            Controllerek.OrakListaFeltoltes();
            List<String> menuPontok = new List<string>();
            Tanar tanar;
            menuPontok.Add("Kilépés");
            menuPontok.Add("Óra indítás");
            menuPontok.Add("Diák adatainak módosítása/Törlése");
            menuPontok.Add("Jegy módosítása/törlése");
            menuPontok.Add("Jegyzet hozzáadása");
            menuPontok.Add("Excel import/export");


            List<Hallgato> hallgatok = new List<Hallgato>();


            var oktato = new Tanar(hallgatok);
            /*
            var testLesson = new Lesson(1,"asdasd",DateTime.Now);
            Console.WriteLine(testLesson.ToString());
            string[] asd = testLesson.ConvertToRekord();
            Console.WriteLine(asd[2]);
            */

            var evaluator = new HianyzasKiertekelo();
            await evaluator.EvaluateAbsencesAsync();



            var menu = 0;
            do
            {
                Console.Clear();
                for(int i = 0; i < menuPontok.Count; i++)
                {
                    Console.WriteLine(i+": "+ menuPontok[i]);
                }
                Console.Write("Kérem válasszon menüpontot: ");
                var scanner = Console.ReadLine();
                menu = Convert.ToInt32(scanner);
                switch (menu)
                {
                    case 0:
                        Console.Clear();
                        Console.WriteLine("Viszont látásra!");
                        System.Threading.Thread.Sleep(1000);
                        break;
                        
                    case 1:
                        oktato.AddOra(Controllerek.OraInditas());
                       // Orak.Add(oktato.GetLesson);
                        Console.WriteLine("Óra rögzítve!");
                        System.Threading.Thread.Sleep(2000);
                        break;
                    case 2:
                        Controllerek.DiakokKezelese();
                        Console.WriteLine("Feladat végrehajtva, vissza a főmenübe!");
                        System.Threading.Thread.Sleep(2000);
                        break;
                    case 3:
                        Controllerek.JegyKezeles();
                        Console.WriteLine("Feladat végrehajtva, vissza a főmenübe!");
                        System.Threading.Thread.Sleep(2000);
                        break;
                    case 4:
                        var Jegyzet = Controllerek.JegyzetHozzaadas();
                        Console.Clear();
                        Console.WriteLine("Jegyzet mentve!");
                        System.Threading.Thread.Sleep(2000);
                        break;
                    case 5:
                        var importExportManager = new ImportExportManager();
                        importExportManager.MenuHandler_feladatok();
                        Console.WriteLine("Feladat végrehajtva, vissza a főmenübe!");
                        System.Threading.Thread.Sleep(2000);
                        break;
                    default:
                        Console.Clear();
                        Console.WriteLine("Nincs ilyen menüpont, próbálja újra!");
                        System.Threading.Thread.Sleep(3000);
                        break;
                }
                Console.Clear();
            } while (menu !=0);
        }
    }
}
