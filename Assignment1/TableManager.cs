using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{

    class TableManager
    {
        public InstructionResult createTable(string name, List<string> TableAttributesOrder, Dictionary<string, TableAttribute> TableAttributes)
        {
            if (tables.ContainsKey(name)) return InstructionResult.TABLE_NAME_DUPLICATE;    //table name duplicated
            else tables.Add(name,new Table(TableAttributesOrder, TableAttributes));
            return InstructionResult.SUCCESS;   //Success
        }

        public void insert(string name, Dictionary<string, dynamic> ele)
        {
            InstructionResult res = tables[name].insert(ele);
            if(res!= InstructionResult.SUCCESS)
            {
                string errorString;
                switch (res)
                {
                    case InstructionResult.PRIMARY_KEY_DUPLICATE:
                        errorString = "Primary key duplicated";
                        break;
                    case InstructionResult.VARCHAR_TOO_SHORT:
                        errorString = "Varchar is too short";
                        break;
                    case InstructionResult.INCORRECT_TYPE:
                        errorString = "Type is incorrected";
                        break;
                    case InstructionResult.NULL_PRIMARY_KEY:
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
