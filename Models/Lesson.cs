using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Lesson
    {
        protected int _id;
        protected int _courseId;
        protected String _title;
        protected DateTime _lessonDate;
        public Lesson(String[] adatok)
        {
            _id = Int32.Parse(adatok[0]);
            _courseId = Int32.Parse(adatok[1]);
            _title = adatok[2];
            //ezt még ki kell javítani
            _lessonDate = new DateTime();
        }
        public int getId() { return _id; }
    }
}
