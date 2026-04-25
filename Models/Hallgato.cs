using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Models
{
    public class Hallgato
    {
        protected int _id;
        protected String _firstName;
        protected String _lastName;
        protected String _email;
        
        public Hallgato(String[] adatok) 
        {
            _id = Int32.Parse(adatok[0]);
            _firstName = adatok[1];
            _lastName = adatok[2];
            _email = adatok[3];      
        }
        //getterek
        public int getId() { return _id; }
        public String getFirstName() { return _firstName; }
        public String getLastName() { return _lastName; }
        public String getEmail() { return _email; }
                



    }
}
