using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class TableAttributeInfo
    {
        public TableAttributeInfo(string name, int type, bool isPrimery, int maxStringLength)
        {
            this.name = name;
            this.type = type;
            this.isPrimery = isPrimery;
            this.maxStringLength = maxStringLength;
        }

        public string name;
        public int type;   //0:int, 1:varchar
        public bool isPrimery;
        public int maxStringLength;
    }
}
