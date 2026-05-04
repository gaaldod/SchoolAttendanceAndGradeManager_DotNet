using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Kurzus
    {
        
        protected int _id;
        protected String _courseName;
        protected String _description;
        public Kurzus(String[] adatok) 
        {
            _id = Int32.Parse(adatok[0]);
            _courseName = adatok[1];
            _description = adatok[2];
        }

        //getterek
        public int getId() { return _id; }
        public String getCourseName() { return _courseName; }
        public String getDescription() { return _description; }
        

    }
}
