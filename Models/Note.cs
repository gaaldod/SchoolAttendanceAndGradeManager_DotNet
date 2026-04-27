using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Note
    {
        protected int _studentId;
        protected int _lessonId;
        protected String _content;
        protected DateTime _createdTime;

        public Note(int studentid, int lessonod, String content)
        {
            _studentId = studentid;
            _lessonId = lessonod;
            _content = content;
            _createdTime = DateTime.Now ;
        }






    }
}
