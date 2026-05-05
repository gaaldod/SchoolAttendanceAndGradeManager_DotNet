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
