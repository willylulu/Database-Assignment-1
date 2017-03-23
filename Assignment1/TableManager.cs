using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    enum Operators
    {
        equal,not_equal,less,greater
    }
    enum OperatorLink
    {
        OR,AND
    }
    /*
     * where attr1 = value1 and attr2 > value2 or attr3 < value3
     * ----------------------------------------------------------
     * image that => where (and attr1 == value1) (and attr2 > value2) (or attr3 < value3)
     * where w1 = new where(attr1,Operators.equal,value1,OperatorLink.AND);
     * where w2 = new where(attr2,Operators.greater,value2,OperatorLink.AND);
     * where w3 = new where(attr3,Operators.less,value3,OperatorLink.OR);
     */
    class where
    {
        public string attribute;
        public Operators operation;
        public dynamic value;
        public OperatorLink operationLink;
        public where(string attr, Operators operation, dynamic value, OperatorLink operatorLink)
        {
            this.attribute = attr; this.operation = operation; this.value = value; this.operationLink = operatorLink;
        }
    }
    /*
     * where table1.attr1 = table2.attr2 and table3.attr3 > table4.attr4 or table5.attr5 > table6.attr6
     * -------------------------------------------------------------------
     * image that => where (and table1.attr1 == table2.attr2) (and table3.attr3 > table4.attr4) (or table5.attr5 > table6.attr6) 
     * whereJoin wJ1 = new whereJoin(table1,attr1,Operators.equal,table2,attr2,OperatorLink.AND);
     * whereJoin wJ1 = new whereJoin(table3,attr3,Operators.greater,table4,attr4,OperatorLink.AND);
     * whereJoin wJ1 = new whereJoin(table5,attr5,Operators.less,table6,attr6,OperatorLink.OR);
     */
    class whereJoin
    {
        public KeyValuePair<string,string> attribut1;
        public Operators operation;
        public KeyValuePair<string, string> attribut2;
        public OperatorLink operationLink;
        public whereJoin(string table1, string attr1, Operators operation, string table2, string attr2, OperatorLink operationLink)
        {
            this.attribut1 = new KeyValuePair<string, string>(table1, attr1);
            this.attribut2 = new KeyValuePair<string, string>(table2, attr2);
            this.operation = operation;
            this.operationLink = operationLink;
        }
    }
    /* selece * from table1 as ta,table2 as tb where ta.attr1 = value1 AND tb.attr2 > value2 OR OR ta.attr3 < value3
     * --------------------------------------------------------------------------------------
     * where w1 = new where(attr1,Operators.equal,value1,OperatorLink.AND);
     * where w2 = new where(attr2,Operators.greater,value2,OperatorLink.AND);
     * where w3 = new where(attr3,Operators.less,value3,OperatorLink.OR);
     * List<where> wheresForTable1 = new List<where>({w1,w3});
     * List<where> wheresForTable2 = new List<where>({w2});
     * tableSelect ts1 = new tableSelect(table1,wheresForTable1);
     * tableSelect ts2 = new tableSelect(table2,wheresForTable2);
     */
    class tableSelect
    {
        public string name;
        public List<where> wheres;
        public tableSelect(string tableName, List<where> wheres)
        {
            this.name = tableName;
            this.wheres = wheres;
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
                    if(w.operationLink == OperatorLink.AND)
                    {
                        HashSet<Guid> rt = getTable(ts.name).getAttribIndexWithOper(w.attribute, w.value, w.operation);
                        temp = intersect(temp, rt);
                    }
                }
                foreach (where w in ts.wheres)
                {
                    if (w.operationLink == OperatorLink.OR)
                    {
                        HashSet<Guid> rt = getTable(ts.name).getAttribIndexWithOper(w.attribute, w.value, w.operation);
                        temp = union(temp, rt);
                    }
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
