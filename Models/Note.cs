using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Note
    {
        protected String _student;
        protected String _lesson;
        protected String _content;
        protected String _createdTime;

        public Note(String studentName, String lesson, String content)
        {
            _student = studentName;
            _lesson = lesson;
            _content = content;
            _createdTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public string getStudent() { return _student; }
        public string getContent() { return _content; }
        public string getLesson() { return _lesson; }

        public string getCreatedTime() { return _createdTime; }
        public override string ToString()
        {
            return _student+" "+_lesson+" "+_content+" "+_createdTime;
        }

    }
}
