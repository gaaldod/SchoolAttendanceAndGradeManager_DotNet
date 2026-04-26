using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Lesson
    {
        
        protected int _courseId;
        protected String _title;
        protected DateTime _lessonDate;
        public Lesson(int id,string title,DateTime startTime)
        {

            _courseId = id;
            _title = title;
            _lessonDate = startTime;
        }
        public int getCourseId() { return _courseId; }
        public String getTitle() { return _title; }
        
        public DateTime GetDate() { return _lessonDate; }

        public string[] ConvertToRekord()
        {
            string[] tmb = new string[3];
            tmb[0] = Convert.ToString(_courseId);
            tmb[1] = _title;
            tmb[2] = _lessonDate.ToString("yyyy-MM-dd HH:mm:ss"); ;


            return tmb;
        
        }
        public override string ToString()
        {
            string kezdesFormatted = _lessonDate.ToString("yyyy-MM-dd HH:mm:ss");
            return String.Format("{0,-8:d}{1,-15:s}{2}",_courseId,_title,kezdesFormatted);
        }


    }
}
