using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Tanar
    {
        
        protected Lesson _lesson;
        protected List<Hallgato> _diakok;

        public Tanar(Lesson lsn, List<Hallgato> diakok) 
        { 
            _lesson = lsn;
            _diakok = diakok;

        }

        public void KiirHallgatokListaja()
        {
            foreach (var item in _diakok)
            {
                Console.WriteLine(item.ToString());
            }
        }



    }
}
