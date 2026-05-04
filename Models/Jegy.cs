using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Jegy
    {
        protected String _targy;
        protected int _erdemjegy;


        public Jegy(string targy, int jegy) 
        {
            _targy=targy;
            _erdemjegy=jegy;
        }

        public override string ToString()
        {
            return String.Format("{0,3:d}{1,-5:s}",_erdemjegy, _targy);
        }





    }
}
