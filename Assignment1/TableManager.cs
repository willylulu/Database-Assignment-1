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

    class table_attribute_pair
    {
        public string tableName;
        public string attributeName;
        public table_attribute_pair(string table, string attr)
        {
            tableName = table;
            attributeName = attr;
        }
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
        public Operators oper;
        public KeyValuePair<string, string> tableAttrPair1;
        public KeyValuePair<string, string> tableAttrPair2;
        public OperatorLink operLink;

        public HashSet<Dictionary<string, Guid>> elementSet;
        public List<string> attrName; // store table name in elementSet

        public where(HashSet<Dictionary<string, Guid>> elementSet, List<string> attrName)
        {
            this.elementSet = elementSet;
            this.attrName = attrName;
        }
        public where( where preWhere)
        {
            oper = preWhere.oper;
            operLink = preWhere.operLink;
            tableAttrPair1 = preWhere.tableAttrPair1;
            tableAttrPair2 = preWhere.tableAttrPair2;
        }
        public void setElementSet(HashSet<Dictionary<string, Guid>> elementSet)
        {
            this.elementSet = elementSet;
        }

        public where(string table1, string attr1, string table2, string attr2, Operators op, OperatorLink opLink)
        {
            oper = op;
            operLink = opLink;
            tableAttrPair1 = new KeyValuePair<string, string>(table1, attr1);
            tableAttrPair2 = new KeyValuePair<string, string>(table2, attr2);
        }

    }

    class where_set
    {
        public List<object> where_group;

        public where_set(List<object> where_group)
        {
            this.where_group = where_group;
        }
    }
    


    class outputPair
    {
        string tableName;
        string attr;
        public outputPair(string tableName, string attr)
        {
            this.tableName = tableName;
            this.attr = attr;
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

        public where generateWhere( Dictionary<string, string> aliaName, where inWhere)
        {
            where outWhere = new where(inWhere);
            Table table1 = getTable(aliaName[outWhere.tableAttrPair1.Key]);
            Table table2 = getTable(aliaName[outWhere.tableAttrPair2.Key]);
            Dictionary<Guid, List<dynamic>> attribIndex1 = table1.getTableData();
            Dictionary<Guid, List<dynamic>> attribIndex2 = table2.getTableData();
            HashSet<Guid> dataKeys1 = table1.getAllIndex();
            HashSet<Guid> dataKeys2 = table2.getAllIndex();
            int index1;
            int index2;
            List<string> TableAttributesOrder1 = table1.getAttributesOrder();
            index1 = TableAttributesOrder1.FindIndex(x => x == outWhere.tableAttrPair1.Value);
            List<string> TableAttributesOrder2 = table2.getAttributesOrder();
            index2 = TableAttributesOrder2.FindIndex(x => x == outWhere.tableAttrPair2.Value);

            var ans =
                from data1 in dataKeys1
                from data2 in dataKeys2
                where attribIndex1[data1][index1] == attribIndex2[data1][index2]
                select new { d1 = data1, d2 = data2 };
            foreach (var dataPair in ans)
            {
                Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                aliasGidPair.Add(outWhere.tableAttrPair1.Key, dataPair.d1);
                if (!aliasGidPair.ContainsKey(outWhere.tableAttrPair2.Key))
                {
                    aliasGidPair.Add(outWhere.tableAttrPair2.Key, dataPair.d2);
                }
                outWhere.elementSet.Add(aliasGidPair);
            }

            return outWhere;
        }
        public where select(Dictionary<string, string> aliaName ,where_set[] tables, outputPair[] outputOrder)
        {
            Dictionary<string, string> aliaNameDic = aliaName;
            for (int i = 0 ; i<tables.Length; i++)
            {
                
                if(tables.Length == 1 && tables[i].where_group.Count == 1) //convert to tableSelect
                {
                    if (tables[i].where_group[0].GetType() == typeof(where))
                    {
                        return generateWhere(aliaName, (where)tables[i].where_group[0]);
                    }
                }
                else
                {
                    for(int j = 0; j<tables[i].where_group.Count; j++)
                    {
                        if(tables[i].where_group[j].GetType() == typeof(where))
                        {
                            tables[i].where_group[j] = generateWhere(aliaName, (where)tables[i].where_group[j]);
                        }else if(tables[i].where_group[j].GetType() == typeof(where_set))
                        {
                            where_set[] temp_set = new where_set[1];
                            temp_set[0] = (where_set)tables[i].where_group[j];
                            tables[i].where_group[j] = select(aliaName, temp_set, outputOrder);
                        }
                    }
                }
            }
            HashSet<Dictionary<string, Guid>> temp_result = new HashSet<Dictionary<string, Guid>>();
            temp_result = ((where)tables[0].where_group[0]).elementSet;
            List<string> temp_attr = ((where)tables[0].where_group[0]).attrName;
            for (int i = 0; i < tables.Length; i++)
            {
                if( ((where)tables[i].where_group[0]).operLink == OperatorLink.AND)
                {
                    temp_result = union(aliaName, ((where)tables[i].where_group[0]).elementSet, ((where)tables[i].where_group[0]).attrName, temp_result , temp_attr);
                    List<string> attrName = ((where)tables[i].where_group[0]).attrName;
                    List<string> dupAttr = new List<string>();
                    HashSet<string> attrSet = new HashSet<string>();
                    foreach(string a in attrName)
                    {
                        attrSet.Add(a);
                    }
                    foreach (string b in temp_attr)
                    {
                        attrSet.Add(b);
                    }
                    temp_attr = new List<string>();
                    foreach (string attr in attrSet)
                    {
                        temp_attr.Add(attr);
                    }
                }else if(((where)tables[i].where_group[0]).operLink == OperatorLink.OR)
                {
                    temp_result = intersect(((where)tables[i].where_group[0]).elementSet, ((where)tables[i].where_group[0]).attrName, temp_result, temp_attr);
                    List<string> attrName = ((where)tables[i].where_group[0]).attrName;
                    List<string> dupAttr = new List<string>();
                    HashSet<string> attrSet = new HashSet<string>();
                    foreach (string a in attrName)
                    {
                        attrSet.Add(a);
                    }
                    foreach (string b in temp_attr)
                    {
                        attrSet.Add(b);
                    }
                    temp_attr = new List<string>();
                    foreach (string attr in attrSet)
                    {
                        temp_attr.Add(attr);
                    }
                }
                 
            }
            return new where( temp_result, temp_attr);
        }

        class HashEqualityComparer : IEqualityComparer<Dictionary<string, Guid>>
        {

            public bool Equals(Dictionary<string, Guid> b1, Dictionary<string, Guid> b2)
            {
                foreach( KeyValuePair<string, Guid> kv in b1)
                {
                    if (!b2.ContainsKey(kv.Key))
                    {
                        return false;
                    }else if (kv.Value != b2[kv.Key])
                    {
                        return false;
                    }
                }
                return true;
            }
            public int GetHashCode(Dictionary<string, Guid> bx)
            {
                int hCode = 0;
                foreach(KeyValuePair<string,Guid> kv in bx)
                {
                    hCode += kv.Value.GetHashCode();
                }
                return hCode;
            }

        }

            private HashSet<Dictionary<string, Guid>> union(Dictionary<string, string> aliaName, HashSet<Dictionary<string, Guid>> elementSet1, List<string> tableAttrHashSet1,HashSet<Dictionary<string, Guid>> elementSet2, List<string> tableAttrHashSet2)
        {
            
            HashSet<string> dupAttr = new HashSet<string>();
            var ans =
                from d1 in tableAttrHashSet1
                from d2 in tableAttrHashSet2
                where d1 == d2
                select d1;

            foreach (string attr in ans){
                dupAttr.Add(attr);
            }

            HashSet<Dictionary<string, Guid>> temp_eleSet1 = new HashSet<Dictionary<string, Guid>> ();
            foreach (Dictionary<string,Guid> data in elementSet1)
            {
                Dictionary<string, Guid> temp_dic = new Dictionary<string, Guid>(data);
                foreach (string attr in tableAttrHashSet2)
                {   
                    if( !dupAttr.Contains(attr))
                    {
                        string table_name = aliaName[attr];
                        foreach (Guid id in getTable(table_name).getAllIndex())
                        {
                            temp_dic.Add(attr, id);
                        }
                    }
                }
                temp_eleSet1.Add(temp_dic);
            }

            HashSet<Dictionary<string, Guid>> temp_eleSet2 = new HashSet<Dictionary<string, Guid>>();
            foreach (Dictionary<string, Guid> data in elementSet2)
            {
                Dictionary<string, Guid> temp_dic = new Dictionary<string, Guid>(data);
                foreach (string attr in tableAttrHashSet1)
                {
                    if (!dupAttr.Contains(attr))
                    {
                        string table_name = aliaName[attr];
                        foreach (Guid id in getTable(table_name).getAllIndex())
                        {
                            temp_dic.Add(attr, id);
                        }
                    }
                }
                temp_eleSet2.Add(temp_dic);
            }

            HashSet<Dictionary<string, Guid>> result = (HashSet<Dictionary<string, Guid>>)temp_eleSet1.Union(temp_eleSet2);

            return result;
        }

        private Boolean checkIntersect( Dictionary<string, Guid> element1 , Dictionary<string, Guid> element2 , List<string> dupAttr)
        {
            foreach(string tableName in dupAttr)
            {
                if(element1[tableName] != element2[tableName])
                {
                    return false;
                }
            }
            return true;
        }
        private HashSet<Dictionary<string, Guid>> intersect(HashSet<Dictionary<string, Guid>> elementSet1, List<string> tableAttrHashSet1, HashSet<Dictionary<string, Guid>> elementSet2, List<string> tableAttrHashSet2)
        {
            HashSet<Dictionary<string, Guid>> result = new HashSet<Dictionary<string, Guid>>();

            List<string> dupAttr = new List<string>();
            var ans1 =
                from d1 in tableAttrHashSet1
                from d2 in tableAttrHashSet2
                where d1 == d2
                select d1;

            foreach (string attr in ans1)
            {
                dupAttr.Add(attr);
            }

            var ans2 =
                from d1 in elementSet1
                from d2 in elementSet2
                where checkIntersect(d1, d2, dupAttr)
                select new { d1 = d1, d2 = d2 };
            foreach (var data in ans2)
            {
                Dictionary<string, Guid> temp_result = new Dictionary<string, Guid>();
                foreach(var dd in data.d1)
                {
                    if (!temp_result.ContainsKey(dd.Key))
                    {
                        temp_result.Add(dd.Key,dd.Value);
                    } 
                }
                foreach (var dd in data.d2)
                {
                    if (!temp_result.ContainsKey(dd.Key))
                    {
                        temp_result.Add(dd.Key, dd.Value);
                    }
                }
            }

            return result;
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
                    var eleattr = String.Join(", ", ele.Value.ToArray());
                    csv.AppendLine(eleattr);
                }



            }
            File.WriteAllText("../../table_conetxt.csv", csv.ToString());




        }
        private Dictionary<string, Table> tables = new Dictionary<string, Table>(1000000);
        }
}

class ProductComparer : IEqualityComparer< Dictionary<string, Guid> >
{
    // Products are equal if their names and product numbers are equal. 
    public bool Equals(Dictionary<string, Guid>  x, Dictionary<string, Guid> y)
    {

        return x.SequenceEqual(y);
    }

    // If Equals() returns true for a pair of objects, 
    // GetHashCode must return the same value for these objects. 

    public int GetHashCode(Dictionary<string, Guid> x)
    {
        return x.Count.GetHashCode();
    }

}
