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
        protected Lesson _tanora;

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
        public Lesson GetLesson() { return _tanora; }
        public void AddOra(Lesson ls)
        {
            _tanora = ls;
        }



    }
}
