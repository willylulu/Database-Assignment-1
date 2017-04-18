using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    [Serializable]
    class TableAttribute
    {
        public const string STRING_TYPE = "String";
        public const string INT_TYPE = "Int32";
        public const int MAX_STRING_LENGTH = 40;

        public TableAttribute(string type, bool isPrimary, int maxStringLength)
        {
            this.type = type;
            this.isPrimary = isPrimary;
            this.maxStringLength = maxStringLength;
        }

        public string type;
        public bool isPrimary;
        public int maxStringLength;
    }
}
