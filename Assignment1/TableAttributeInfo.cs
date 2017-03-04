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

        private string name;
        private int type;   //0:int, 1:varchar
        private bool isPrimery;
        private int maxStringLength;
    }
}
