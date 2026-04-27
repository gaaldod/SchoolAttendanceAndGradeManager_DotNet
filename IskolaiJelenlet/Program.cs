using System.Security.Cryptography.X509Certificates;
using Controllers;
using Models;

namespace IskolaiJelenlet
{
    public class Program
    {
        static void Main(string[] args)
        {

           
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
            tanar = new Tanar(hallgatok);
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
                
                Console.Clear();
            } while ((menu < 0) || (menu > menuPontok.Count));
            switch (menu)
            {
                case 1:
                    Orak.Add(Controllerek.OraInditas());
                    break;
                case 2:
                    Controllerek.DiakokKezelese();
                    break;
                case 3:
                    Controllerek.JegyKezeles();
                    break;
                case 4:
                    Controllerek.JelenletKezeles(tanar);
                    
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Viszont látásra!");
                    break;
            }


            












        }
    }
}
