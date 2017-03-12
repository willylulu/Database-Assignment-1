﻿
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
        static void Main2(string[] args)
        {
            TableManager tableManager = new TableManager();
            //TestCase -- open for updating
            //1. Insertion that has no values
            //2. Insertion that values('1'1, '2'3' ) raise exception

            //To use Create Table , just call CreateTable(str sql)
            //To use Insert Table , just call Insert(str sql)
            //To determine which instruction, use  getInstruction(str str)

            string text = System.IO.File.ReadAllText(@"../../sql_query.txt");
            //text = text.ToLower();
            string[] seperated_query = text.Split(';');
            foreach (string s in seperated_query)
            {
                sql_selector(s.TrimStart(), tableManager);
            }

            tableManager.print_table_context();
            //TestCreateTable();
            //TestInsertion();
            Console.ReadKey(true);
        }
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
                    if (Insertion.AttrNames == null) //AttrNames equal null
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
                sql = sql.ToLower();
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
                sql = sql.ToLower();
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

        }


        private static string getInstruction(string str)
        {
            string instruction = SqlGrammar.Instruction.Parse(str);
            if (!instruction.Equals("create table") && !instruction.Equals("insert into"))
                throw new DbException.UnkownKeyword(" undefined instruction: '" + instruction + "'");

            return instruction;
        }

        private static void TestInsertionError()
        {
            string input = "";

            //Error case, losing attribute name (
            input = "INSERT INTO Student   age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, losing attribute name )
            input = "INSERT INTO Student   (age, studentId, gender, name \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, losing attribute value(
            input = "INSERT INTO Student   (age, studentId, gender, name) \n vaLUES 20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, losing attribute value )
            input = "INSERT INTO Student   (age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra';";
            Insert(input);

            //Error case, losing attribute value )
            input = "INSERT INTO Student   ( age, studentId, gender, name, ) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, losing , -- to be sloved
            input = "INSERT INTO Student (age, studentId gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, arguments not match 
            input = "INSERT INTO Student (age, studentId gender, name) \n vaLUES (13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, redundent '
            input = "INSERT INTO Student  ( age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernand'o Sierra');";
            Insert(input);

        }
        private static void TestInsertion()
        {
            string input = "";
            //Normal test, with () in string
            input = "INSERT INTO Student \n VALUES(10,   'John(- Smith'   , 'M', 22)";
            Insert(input);

            //Normal test
            input = "INSERT INTO Student \n VALUES(10,   'John- Smith'   , 'M', 22)";
            Insert(input);

            //Case-Sensitive test
            input = "INsErT InTo sTUDEnt \n ValUEs(112, 'cCcCc+  |+ccccc', 'F', 22);";
            Insert(input);

            // () values () test
            input = "INSERT INTO Student (age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //ADD \n in every place
            input = "INSERT \n INTO \nStudent \n\n(name, \n age, \nstudentId, gender) \nVAlues\n ('Ai>> Toshiko', 21\n, 12, 'F');";
            Insert(input);

        }

        private static void TestCreateTableError()
        {
            string input = "";
            //Error case, losing ,                                           |   
            input = "CREATE tAbLe department\n  ( department_name varchar(20) \n type varchar(20), \n   num_employees int ) ";
            CreateTable(input);

            //Error case, losing (              |
            input = "CREATE tAbLe losing_left_P\n   department_name varchar(20), \n type varchar(20), \n   num_employees int ) ";
            CreateTable(input);

            //Error case, more ,                                                                                          |
            input = "CREATE tAbLe more_comma\n (  department_name varchar(20), \n type varchar(20), \n   num_employees int, ) ";
            CreateTable(input);

            //Error case, unkown type date                                             |
            input = "CREATE tAbLe unkown_Type_date\n (  department_name varchar(20), \n type DATE, \n   num_employees int, ) ";
            CreateTable(input);

            //more )                                                      |   
            input = "CreaTe tABle MORE_right_P(stUDENTId int PRimaRY KEY, )\n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //more (                                                           |
            input = "CreaTe tABle MORE_left_P(stUDENTId int PRimaRY KEY, \n nAM(e vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //string in () is not a number                                                             | 
            input = "CreaTe tABle varchar_Length_not_number(stUDENTId int PRimaRY KEY, \n nAMe vARCHar(fifty), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //varchar oversize                                                                   | 
            input = "CreaTe tABle varchar_oversize_50(stUDENTId int PRimaRY KEY, \n nAMe vARCHar(50), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //int has length                                   | 
            input = "CreaTe tABle int_has_length(stUDENTId int (12) PRimaRY KEY, \n nAMe vARCHar(50), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //string 'primary key' not complete                     |
            input = "CreaTe tABle primary_key_error(stUDENTId int PRmaRY KEY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //string 'primary key' wrong                              |
            input = "CreaTe tABle primary_key_error(stUDENTId int PRmaRY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //string 'primary key' wrong                                  |
            input = "CreaTe tABle primary_key_error(stUDENTId int PRimaRY primary key, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //string 'primary key' wrong                                  |
            input = "CreaTe tABle primary_key_error(stUDENTId int          zzz key, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

        }
        private static void TestCreateTable()
        {
            string input = "";
            //Normal Test + Case Sensitive
            input = "CreaTe tABle stUDent(stUDENTId int PRimaRY KEY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //Primary Key 
            input = "CREATE tAbLe PerSon(\n   personId int PRIMARY KEY, \n name varchar(20) PRiMArY KEY, \n   m_gender varchar(1)   )";
            CreateTable(input);

            //varchr (2) -- space between
            input = "CREATE tAbLe spece_between_name_andP(\n   department_name varchar (20), \n type varchar(20), \n   num_employees int ) ";
            CreateTable(input);

            // Primary key test                                                  |
            input = "CREATE tAbLe Primary_key_test(\n   personId int    PRIMARY     KEY, \n name varchar(20) PRiMArY KEY, \n   m_gender varchar(1)   )";
            CreateTable(input);

            // Primary key test                                                  |
            input = "CREATE tAbLe Primary_key_test(\n   personId int   , \n name varchar(20) PRiMArY KEY, \n   m_gender varchar(1)   )";
            CreateTable(input);
        }
        private static void println(dynamic str)
        {
            Console.WriteLine(str.ToString());
        }

    }
}