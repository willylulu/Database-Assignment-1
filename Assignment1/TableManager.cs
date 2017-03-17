using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    enum operators
    {
        equal,not_equal,less,greater
    }
    enum operatorLink
    {
        OR,AND
    }
    class where
    {
        public string attribute;
        public operators operation;
        public dynamic value;
        public operatorLink operationLink;
        public where(string a, operators b, dynamic c, operatorLink d)
        {
            attribute = a; operation = b; value = c; operationLink = d;
        }
    }

    class whereJoin
    {
        public KeyValuePair<string,string> attribut1;
        public operators operation;
        public KeyValuePair<string, string> attribut2;
        public operatorLink operationLink;
        public whereJoin(string a, string b, operators c, string d, string e, operatorLink f)
        {
            attribut1 = new KeyValuePair<string, string>(a, b);
            attribut2 = new KeyValuePair<string, string>(d, e);
            operation = c;
            operationLink = f;
        }
    }

    class tableSelect
    {
        public string name;
        public List<where> wheres;
        public tableSelect(string a, List<where> b)
        {
            name = a;
            wheres = b;
        }
    }

    class TableManager
    {
        public InstructionResult createTable(string name, List<string> TableAttributesOrder, Dictionary<string, TableAttribute> TableAttributes)
        {
            if (tables.ContainsKey(name)) return InstructionResult.TABLE_NAME_DUPLICATE;    //table name duplicated
            else
            {
                tables.Add(name, new Table(TableAttributesOrder, TableAttributes));
                ParserTest.println("Success");
                return InstructionResult.SUCCESS;   //Success
            }
        }

        public void insert(string name, Dictionary<string, dynamic> ele)
        {
           if (tables.ContainsKey(name) == true)
            {
                InstructionResult res = tables[name].insert(ele);
                if (res != InstructionResult.SUCCESS)
                {
                    string errorString;
                    switch (res)
                    {
                        case InstructionResult.PRIMARY_KEY_DUPLICATE:
                            errorString = "Error : Primary key duplicated";
                            break;
                        case InstructionResult.VARCHAR_TOO_SHORT:
                            errorString = "Error : Varchar is too short";
                            break;
                        case InstructionResult.INCORRECT_TYPE:
                            errorString = "Error : Type is incorrected";
                            break;
                        case InstructionResult.NULL_PRIMARY_KEY:
                            errorString = "Error : Primary key can not be null";
                            break;
                        default:
                            errorString = "Error : Unknown error";
                            break;
                    }
                    Console.WriteLine(errorString);
                }
                else
                {
                    ParserTest.println("Success");
                }
            }
            else
            {
                string errorString = "Error : Table is not exist";
                Console.WriteLine(errorString);
            }
        }

        public List<List<dynamic>> select(string[] outputOrder,tableSelect[] tables,whereJoin[] whereJoins)
        {
            Dictionary<string, HashSet<Guid>> tableCandidates = new Dictionary<string, HashSet<Guid>>();
            foreach(tableSelect ts in tables)
            {
                HashSet<Guid> temp = new HashSet<Guid>(getTable(ts.name).getAllIndex());
                foreach(where w in ts.wheres)
                {
                    HashSet<Guid> rt = getTable(ts.name).getAttribIndexWithOper(w.attribute,w.value,w.operation);
                    if (w.operationLink == operatorLink.AND) temp = intersect(temp, rt);
                    else if (w.operationLink == operatorLink.OR) temp = union(temp, rt);
                }
                tableCandidates.Add(ts.name,temp);
            }
            return null;
        }

        private HashSet<Guid> union(HashSet<Guid> a, HashSet<Guid> b)
        {
            HashSet<Guid> ans = new HashSet<Guid>();
            foreach (Guid iter in a) ans.Add(iter);
            foreach (Guid iter in b) ans.Add(iter);
            return ans;
        }

        private HashSet<Guid> intersect(HashSet<Guid> a, HashSet<Guid> b)
        {
            HashSet<Guid> ans = new HashSet<Guid>();
            HashSet<Guid> c,d;
            if (a.Count < b.Count)
            {
                c = a;d = b;
            }
            else
            {
                c = b; d = a;
            }
            foreach(Guid iter in c)
            {
                if (d.Contains(iter)) ans.Add(iter);
            }
            return ans;
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
                    //Console.WriteLine(ele.Value.ToArray());
                    var eleattr = String.Join(", ", ele.Value.ToArray());
                    csv.AppendLine(eleattr);
                }



            }
            File.WriteAllText("../../table_conetxt.csv", csv.ToString());




        }
        private Dictionary<string, Table> tables = new Dictionary<string, Table>(1000000);
    }
}
