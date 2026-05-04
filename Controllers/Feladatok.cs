using System.ComponentModel.DataAnnotations;
using Models;

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
            var oraCim = Console.ReadLine();

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

            var kezdes = new DateTime(ev,honap,nap,ora,perc,00);
            string kezdesFormatted = DatumKonverter(kezdes);

            var Tanora = new Lesson(oraKod,oraCim,kezdes);
            
            return Tanora;

        }
        public void OraRiportGeneralas()
        {
            Console.WriteLine("Tanóra riport generálása:");
        }
        public void JelenletKezeles(Tanar tanar)
        {
            
            Console.WriteLine("Jelenlét nyilvantartó:");



        }
        public void JegyKezeles()
        {
            Console.WriteLine("Jegyek kezelése:");
        }
        public void DiakokKezelese()
        {
            Console.WriteLine("Diákok kezelése:");
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
            Console.Write("Adja meg a diákk nevét: ");
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
