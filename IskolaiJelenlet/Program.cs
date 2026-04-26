using System.Security.Cryptography.X509Certificates;
using Controllers;
using Models;

namespace IskolaiJelenlet
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var menu = 0;
            var Controllerek = new Feladatok();

            /* String[] test = {"12","Kiss","Bela","asd@faszom.org"};
             String[] test2 = {"1343","Nagy","Bsdfsdela","asdfsdfsd@faszom.org"};
             String[] test3 = {"125","Kiss","Belsdfa","asd@sdffaszom.org"};
             var testHallgato = new Hallgato(test);
             var testHallgato2 = new Hallgato(test2);
             var testHallgato3 = new Hallgato(test3);
             Console.WriteLine(testHallgato.ToString());
             Console.WriteLine(testHallgato2.ToString());
             Console.WriteLine(testHallgato3.ToString());
            
            var testLesson = new Lesson(1,"asdasd",DateTime.Now);
            Console.WriteLine(testLesson.ToString());
            string[] asd = testLesson.ConvertToRekord();
            Console.WriteLine(asd[2]);
            */


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
