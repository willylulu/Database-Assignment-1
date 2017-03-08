using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class TableAttributeInfo
    {
        public const string varchar = "String";
        public const string integer = "Int32";

        public TableAttributeInfo(string type, bool isPrimery, int maxStringLength)
        {
            this.type = type;
            this.isPrimery = isPrimery;
            this.maxStringLength = maxStringLength;
        }

        public string type;
        public bool isPrimery;
        public int maxStringLength;
    }
}
