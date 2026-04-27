using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Tanar
    {
        
        
        protected List<Hallgato> _diakok;

        public Tanar(List<Hallgato> diakok) 
        { 
            
            _diakok = diakok;

        }

        public void KiirHallgatokListaja()
        {
            Console.WriteLine("Diákok:");
            foreach (var item in _diakok)
            {
                Console.WriteLine(item.ToString());
            }
        }



    }
}
