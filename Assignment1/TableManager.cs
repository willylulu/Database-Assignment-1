using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class TableManager
    {
        public int createTable(string name)
        {
            if (tables.ContainsKey(name)) return -1;    //table name duplicated
            else tables.Add(name,new Table(name));
            return 1;   //Success
        }

        public int insertTable(string name, List<dynamic> ele)
        {
            return tables[name].insert(ele);
        }

        public Table getTable(string name)
        {
            return tables[name];
        }

        private Dictionary<string, Table> tables = new Dictionary<string, Table>();
    }
}
