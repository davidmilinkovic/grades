using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ocene
{
    class Polje
    {
        public string naziv { get; set; }
        public string vrednost { get; set; }

        public Polje(string naz, string vred)
        {
            naziv = naz;            
            vrednost = vred;
        }
    }
}
