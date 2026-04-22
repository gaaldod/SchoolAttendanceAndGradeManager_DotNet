using Controllers;

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
