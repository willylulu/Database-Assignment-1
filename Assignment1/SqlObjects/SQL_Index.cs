using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1.SqlObjects
{
    class SQL_Index
    {
        public String index;
        public bool isUnique;
        public String Table;
        public List<String> attrs;


        public SQL_Index(String index, bool isUnique, String Table, List<String> attrs)
        {
            this.index = index;
            this.isUnique = isUnique;
            this.Table = Table;
            this.attrs = attrs;
        }



    }
}
