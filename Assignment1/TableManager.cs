using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{

    class TableManager
    {
        public InstructionResult createTable(string name, List<string> TableAttributesOrder, Dictionary<string, TableAttributeInfo> TableAttributesInfo)
        {
            if (tables.ContainsKey(name)) return InstructionResult.tableNameDuplicate;    //table name duplicated
            else tables.Add(name,new Table(TableAttributesOrder, TableAttributesInfo));
            return InstructionResult.success;   //Success
        }

        public void insert(string name, Dictionary<string, dynamic> ele)
        {
            InstructionResult res = tables[name].insert(ele);
            if(res!= InstructionResult.success)
            {
                string errorString;
                switch (res)
                {
                    case InstructionResult.primaryKeyDuplicate:
                        errorString = "Primary key duplicated";
                        break;
                    case InstructionResult.varcharTooShort:
                        errorString = "Varchar is too short";
                        break;
                    case InstructionResult.incorrectType:
                        errorString = "Type is incorrected";
                        break;
                    case InstructionResult.nullPrimaryKey:
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
