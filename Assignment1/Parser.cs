
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;

namespace Assignment1
{
    class Parser
    {
        
        public static void sql_selector(string sql, TableManager tableManager)
        {
            string[] seperated_query = sql.Split();
            if (string.Compare(seperated_query[0].ToLower(), "create", true) == 0)
            {
                SqlGrammar.Sql_Table table = CreateTable(sql);
                List<string> order = new List<string>();
                if (table != null)
                {
                    Dictionary<string, TableAttribute> atributes = new Dictionary<string, TableAttribute>(table.tableAttributes.Count);
                    foreach (var item in table.tableAttributes)
                    {
                        order.Add(item.name);
                        Console.WriteLine("type = " + item.type);
                        switch (item.type)
                        {
                            case "int":
                                item.type = "Int32";
                                break;
                            case "varchar":
                                item.type = "String";
                                break;
                        }
                        Console.WriteLine("size = " + item.maxStringLength);
                        atributes.Add(item.name, new TableAttribute(item.type, item.isPrimary, item.maxStringLength));
                    }
                    tableManager.createTable(table.name, order, atributes);
                }
            }
            else if (string.Compare(seperated_query[0].ToLower(), "insert", true) == 0)
            {
                Console.WriteLine("Inserting");
                SqlGrammar.Sql_Insertion Insertion = Insert(sql);
                //public string table;
                //public List<string> AttrNames;
                //public List<dynamic> AttrValues;
                Dictionary<string, dynamic> tuple = new Dictionary<string, dynamic>();
                if (Insertion != null)
                {
                    if (Insertion.AttrNames.Count == 0) //AttrNames equal null
                    {
                        List<string> AttrOrder = tableManager.getTable(Insertion.table).getAttributesOrder();
                        int i = 0;
                        foreach (dynamic Attrvalue in Insertion.AttrValues)
                        {
                            tuple.Add(AttrOrder[i], Attrvalue);
                            i++;
                        }
                    }
                    else
                    {
                        int i = 0;
                        foreach (dynamic Attrname in Insertion.AttrNames)
                        {
                            tuple.Add(Attrname, Insertion.AttrValues[i]);
                            i++;
                        }
                    }
                    tableManager.insert(Insertion.table, tuple);
                }
            }
            else
            {
                Console.WriteLine("Unknown keywords");
            }
        }
        public static SqlGrammar.Sql_Table CreateTable(string sql)
        {
            string error_prifix = "[CREATEION ERROR] - ";
            try
            {
                //sql = sql.ToLower();
                println(sql);
                var table = SqlGrammar.Table.Parse(sql);
                println(table + "\n");
                return table;
            }
            catch (ParseException e)
            {
                Console.WriteLine(error_prifix + "Invalid Sql Syntax: " + e.Message + "\n");
                return null;
            }
            catch (FormatException e)
            {
                Console.WriteLine(error_prifix + e.Message);
                return null;
            }
            catch (DbException.UnkownKeyword e)
            {
                Console.WriteLine(error_prifix + e.Message + "\n");
                return null;
            }
            catch (DbException.OtherSyntaxError e)
            {
                Console.WriteLine(error_prifix + e.Message + "\n");
                return null;
            }
            catch (DbException.InvalidKeyword e)
            {
                Console.WriteLine(error_prifix + e.Message + "\n");
                return null;
            }
        }
        public static SqlGrammar.Sql_Insertion Insert(string sql)
        {
            string error_prefix = "[INSERTION ERROR] - ";
            try
            {
                //sql = sql.ToLower();
                println(sql);
                var insertion = SqlGrammar.Insertion.Parse(sql);
                println(insertion + "\n");
                return insertion;
            }
            catch (ParseException e)
            {
                Console.WriteLine(error_prefix + "Invalid SQL syntax: mismatch of ' or  uncompleted () or unexpected character~~\n" + e.Message + "\n");
                return null;
            }
            catch (DbException.InvalidKeyword e)
            {
                //Console.WriteLine("[INSERT ERROR] - Invalid SQL Argument: " + e.Message + "\n");
                Console.WriteLine(error_prefix + e.Message + "\n");
                return null;
            }
            catch (DbException.MismatchingArguments e)
            {
                Console.WriteLine(error_prefix + e.Message + "\n");
                return null;

            }

        }



        private static string getInstruction(string str)
        {
            string instruction = SqlGrammar.Instruction.Parse(str);
            if (!instruction.Equals("create table") && !instruction.Equals("insert into"))
                throw new DbException.UnkownKeyword(" undefined instruction: '" + instruction + "'");

            return instruction;
        }
        private static void println(dynamic str)
        {
            //Console.WriteLine(str.ToString());
        }

    }
}