using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class TableManager
    {
        public int createTable(string name, List<string> TableAttributesOrder, Dictionary<string, TableAttributeInfo> TableAttributesInfo)
        {
            if (tables.ContainsKey(name)) return -1;    //table name duplicated
            else tables.Add(name,new Table(TableAttributesOrder, TableAttributesInfo));
            return 1;   //Success
        }

        public void insertTable(string name, Dictionary<string, dynamic> ele)
        {
            int res = tables[name].insert(ele);
        }

        public Table getTable(string name)
        {
            return tables[name];
        }

        private Dictionary<string, Table> tables = new Dictionary<string, Table>(1000000);
    }
}
