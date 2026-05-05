using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using Controllers;
using Models;

namespace IskolaiJelenlet
{
    public class Program
    {
        static void Main(string[] args)
        {
            //automatikus hiány számitás
           
            var Controllerek = new Feladatok();
            List<Lesson> Orak = new List<Lesson>();
            Controllerek.OrakListaFeltoltes();
            List<String> menuPontok = new List<string>();
            Tanar tanar;
            menuPontok.Add("Kilépés");
            menuPontok.Add("Óra indítás");
            menuPontok.Add("Diákok kezelése");
            menuPontok.Add("Jegy kezelő");
            menuPontok.Add("Jelenlét kezelő");
            menuPontok.Add("Jegyzet hozzáadása");



             String[] test = {"12","Kiss","Bela","asd@faszom.org"};
             String[] test2 = {"1343","Nagy","Bsdfsdela","asdfsdfsd@faszom.org"};
             String[] test3 = {"125","Kiss","Belsdfa","asd@sdffaszom.org"};
             var testHallgato = new Hallgato(test);
             var testHallgato2 = new Hallgato(test2);
             var testHallgato3 = new Hallgato(test3);
            List<Hallgato> hallgatok = new List<Hallgato>();
            hallgatok.Add(testHallgato);
            hallgatok.Add(testHallgato2);
            hallgatok.Add(testHallgato3);

            


            var oktato = new Tanar(hallgatok);
            /*
            var testLesson = new Lesson(1,"asdasd",DateTime.Now);
            Console.WriteLine(testLesson.ToString());
            string[] asd = testLesson.ConvertToRekord();
            Console.WriteLine(asd[2]);
            */
            var menu = 0;
            do
            {
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
                        break;
                    case 3:
                        Controllerek.JegyKezeles();
                        break;
                    case 4:
                        Controllerek.JelenletKezeles(oktato);

                        break;
                    case 5:
                        var Jegyzet = Controllerek.JegyzetHozzaadas();
                        Console.Clear();
                        Console.WriteLine("Jegyzet mentve!");
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
