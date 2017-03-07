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

        public void insert(string name, Dictionary<string, dynamic> ele)
        {
            int res = tables[name].insert(ele);
            if(res!=1)
            {
                string errorString;
                switch (res)
                {
                    case -1:
                        errorString = "Primary key duplicated";
                        break;
                    case -2:
                        errorString = "Varchar is too short";
                        break;
                    case -3:
                        errorString = "Type is incorrected";
                        break;
                    case -4:
                        errorString = "Primary key can not be null";
                        break;
                    default:
                        errorString = "Unknown error";
                        break;
                }
                Console.WriteLine(errorString);
            }
        }

        public Table getTable(string name)
        {
            return tables[name];
        }

        private Dictionary<string, Table> tables = new Dictionary<string, Table>(1000000);
    }
}
