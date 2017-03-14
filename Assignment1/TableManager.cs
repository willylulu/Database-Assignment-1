using System;
using System.Collections.Generic;
using System.IO;
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
            else
            {
                tables.Add(name, new Table(TableAttributesOrder, TableAttributes));
                Console.WriteLine("Success");
                return InstructionResult.SUCCESS;   //Success
            }
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
            else
            {
                Console.WriteLine("Success");
            }
        }

        public Table getTable(string name)
        {
            return tables[name];
        }


        public void print_table_context() //Print table data to csv file
        {
            var csv = new StringBuilder();
            foreach (System.Collections.Generic.KeyValuePair<string, Table> tablePair in tables)
            {

                Dictionary<Guid, List<dynamic>> ans = tablePair.Value.getTableData();
                List<string> attr_order = tablePair.Value.getAttributesOrder();

                //Print table name
                var newLine = string.Format(tablePair.Key);
                csv.AppendLine(newLine);

                //Print attribute order
                var attr = String.Join(", ", attr_order.ToArray());
                csv.AppendLine(attr);

                foreach (System.Collections.Generic.KeyValuePair<Guid, List<dynamic>> ele in ans)
                {

                    //Print each tuple
                    Console.WriteLine(ele.Value.ToArray());
                    var eleattr = String.Join(", ", ele.Value.ToArray());
                    csv.AppendLine(eleattr);
                }



            }
            File.WriteAllText("../../table_conetxt.csv", csv.ToString());




        }
        private Dictionary<string, Table> tables = new Dictionary<string, Table>(1000000);
    }
}
