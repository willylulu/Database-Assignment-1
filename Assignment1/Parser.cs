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

        public static void sql_parser(string text,TableManager tableManager)
        {
            string[] seperated_query = text.Split(';');
            KeyValuePair<string,dynamic>[] commands = new KeyValuePair<string, dynamic>[seperated_query.Length];
            Parallel.For(0,seperated_query.Length,(i)=> {
                if (seperated_query[i] != string.Empty)
                {
                    commands[i] = Parser.sql_selector(seperated_query[i].TrimStart());
                }
            });
            foreach(KeyValuePair<string, dynamic> com in commands)
            {
                switch (com.Key)
                {
                    case "create": Parser.transCreateTable(tableManager,com.Value); break;
                    case "Inserting": Parser.transInserting(tableManager, com.Value); break;
                    case "select":
                        tableManager.parseToSelect(com.Value);
                        break;
                    case "index":
                        foreach (string attr in com.Value.attrs)
                        {
                            tableManager.turnOnIndexing(com.Value.Table, attr);
                        }
                        break;
                    case "unKnown": Console.WriteLine("Unknown keywords"); break;
                }
            }
            //foreach (string s in seperated_query)
            //{
            //    Console.WriteLine(s);
            //    Parser.sql_selector(s.TrimStart(), tableManager);
            //}
        }

        public static void transCreateTable(TableManager tableManager, SqlGrammar.Sql_Table table)
        {
            List<string> order = new List<string>(Constants.MAX_ATTR_NUM);
            if (table != null)
            {
                Dictionary<string, TableAttribute> atributes = new Dictionary<string, TableAttribute>(Constants.MAX_ATTR_NUM);
                foreach (var item in table.tableAttributes)
                {
                    order.Add(item.name);
                    Parser.println("type = " + item.type);
                    switch (item.type)
                    {
                        case "int":
                            item.type = "Int32";
                            break;
                        case "varchar":
                            item.type = "String";
                            break;
                    }
                    Parser.println("size = " + item.maxStringLength);
                    atributes.Add(item.name, new TableAttribute(item.type, item.isPrimary, item.maxStringLength));
                }
                tableManager.createTable(table.name, order, atributes);
            }
        }

        public static void transInserting(TableManager tableManager, SqlGrammar.Sql_Insertion Insertion)
        {
            Dictionary<string, dynamic> tuple = new Dictionary<string, dynamic>(Constants.MAX_ATTR_NUM);
            if (Insertion != null)
            {
                if (Insertion.AttrNames.Count == 0) //AttrNames equal null
                {
                    List<string> AttrOrder = tableManager.getTable(Insertion.table).getAttributesOrder();
                    for(int i=0;i< Insertion.AttrValues.Count; i++)
                    {
                        tuple.Add(AttrOrder[i], Insertion.AttrValues[i]);
                    }
                }
                else
                {
                    for (int i = 0; i < Insertion.AttrNames.Count; i++)
                    {
                        tuple.Add(Insertion.AttrNames[i], Insertion.AttrValues[i]);
                    }
                }
                tableManager.insert(Insertion.table, tuple);
            }
        }

        public static KeyValuePair<string, dynamic> sql_selector(string sql)
        {
            string[] seperated_query = sql.Split();
            if (string.Compare(seperated_query[0].ToLower(), "create", true) == 0)
            {
                SqlGrammar.Sql_Table table = CreateTable(sql);
                return new KeyValuePair<string, dynamic>("create", table);
            }
            else if (string.Compare(seperated_query[0].ToLower(), "insert", true) == 0)
            {
                //Console.WriteLine("Inserting");
                SqlGrammar.Sql_Insertion Insertion = Insert(sql);
                return new KeyValuePair<string, dynamic>("Inserting", Insertion);
                //public string table;
                //public List<string> AttrNames;
                //public List<dynamic> AttrValues;
            }
            else if (string.Compare(seperated_query[0].ToLower(), "select", true) == 0)
            {
                //Console.WriteLine("Inserting");
                SqlObjects.Sql_Select selection = Parser.Select(sql + ";");
                return new KeyValuePair<string, dynamic>("select", selection);
                //public string table;
                //public List<string> AttrNames;
                //public List<dynamic> AttrValues;
            }
            else if(string.Compare(seperated_query[0].ToLower(),"create index") == 0)
            {
                SqlObjects.SQL_Index Indexing = Parser.Index(sql);
                return new KeyValuePair<string, dynamic>("index", Indexing);
            }
            else
            {
                if (sql == string.Empty)
                {
                    return new KeyValuePair<string, dynamic>("empty", null);
                }
                else return new KeyValuePair<string, dynamic>("unKnown", null);
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
        public static SqlObjects.Sql_Select Select(string sql)
        {
            string error_prefix = "[SELECTION ERROR] - ";
            try
            {
                println(sql);
                var selection = SqlGrammar.Select.Parse(sql);
                println(selection + "\n");
                return selection;
            }
            catch(DbException.InvalidKeyword e)
            {
                Console.WriteLine(error_prefix + e.Message + "\n");
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(error_prefix + e.Message + "\n" + e.StackTrace);
                return null;
            }
            return null;
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


        public static SqlObjects.SQL_Index Index(string sql)
        {
            string error_prefix = "[INSERTION ERROR] - ";
            try
            {
                //sql = sql.ToLower();
                println(sql);
                var index = SqlGrammar.Index.Parse(sql);
                println(index + "\n");
                return index;
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