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
            //TestCase -- open for updating
            //1. Insertion that has no values
            //2. Insertion that values('1'1, '2'3' ) raise exception

            //To use Create Table , just call CreateTable(str sql)
            //To use Insert Table , just call Insert(str sql)
            //To determine which instruction, use  getInstruction(str str)

            string text = System.IO.File.ReadAllText(@"../../sql_query.txt");
            text = text.ToLower();
            string[] seperated_query = text.Split(';');
            foreach (string s in seperated_query)
            {
                sql_selector(s.TrimStart());
            }

            //TestCreateTable();
            //TestInsertion();
            Console.ReadKey(true);
        }
        public static SqlGrammar.Sql_Table CreateTable(string sql)
        {
            try
            {
                sql = sql.ToLower();
                println(sql);
                var table = SqlGrammar.Table.Parse(sql);
                println(table+"\n");
                //TODO: connect with API
                return table;
            }
            catch(ParseException e)
            {
                Console.WriteLine("[CREATE TABLE ERROR] INCORRECT SQL: " + e.Message +"\n");
                return null;
            }
            catch(DbException.UnknownTypeException e)
            {
                Console.WriteLine("[CREATE TABLE ERROR] Unkown Type: " + e.Message +"\n");
                return null;
            }catch(DbException.InvalidCommaException e)
            {
                Console.WriteLine("[CREATE TABLE ERROR] Invalid Comma: " + e.Message +"\n");
                return null;
            }

        }

        public static void sql_selector(string sql)
        {
            string[] seperated_query = sql.Split();
            if (string.Compare(seperated_query[0], "create", true) == 0)
            {
                CreateTable(sql);
            }
            else if (string.Compare(seperated_query[0], "insert", true) == 0)
            {
                Insert(sql);
            }
        }

        public static SqlGrammar.Sql_Insertion Insert(string sql)
        {
            try
            {
                sql = sql.ToLower();
                println(sql);
                var insertion = SqlGrammar.Insertion.Parse(sql);
                println(insertion+"\n");
                //TODO: connect with API
                return insertion;
            }
            catch(ParseException e)
            {
                Console.WriteLine("[INSERT ERROR] - Invalid SQL: " + e.Message + "\n");
                return null;
            }
            catch(DbException.InvalidSQLArguments e)
            {
                //Console.WriteLine("[INSERT ERROR] - Invalid SQL Argument: " + e.Message + "\n");
                Console.WriteLine("[INSERT ERROR] - Invalid SQL: " + "\n");
                return null;
            }

        }


        private static string getInstruction(string str)
        {
            string instruction = SqlGrammar.Instruction.Parse(str);
            if (!instruction.Equals("create table") && !instruction.Equals("insert into"))
                throw new DbException.UnkownInstruction("undefined instruction " + instruction);

            return instruction;
        }
        private static void TestInsertion()
        {
            string input = "";
            //Normal test
            input = "INSERT INTO Student \n VALUES(10,   'John- Smith'   , 'M', 22)";
            Insert(input);

            //Case-Sensitive test
            input = "INsErT InTo sTUDEnt \n ValUEs(112, 'cCcCc+  |+ccccc', 'F', 22);" ;
            Insert(input);

            // () values () test
            input = "INSERT INTO Student (age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //ADD \n in every place
            input = "INSERT \n INTO \nStudent \n\n(name, \n age, \nstudentId, gender) \nVAlues\n ('Ai>> Toshiko', 21\n, 12, 'F');";
            Insert(input);

            //Error case, losing (
            input = "INSERT INTO Student age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, losing , -- to be sloved
            input = "INSERT INTO Student (age, studentId gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Insert(input);

            //Error case, more ,  -- to be sloved
            input = "INSERT \n INTO \nStudent \n\n(name, \n age, \nstudentId, gender,) \nVAlues\n ('Ai>> Toshiko', 21\n, 12, 'F');";
            Insert(input);
        }

        private static void TestCreateTable()
        {
            string input = "";
            //Normal Test + Case Sensitive
            input = "CreaTe tABle stUDent(stUDENTId int PRimaRY KEY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            CreateTable(input);

            //Primary Key 
            input= "CREATE tAbLe PerSon(\n   personId int PRIMARY KEY, \n name varchar(20) PRiMArY KEY, \n   m_gender varchar(1)   )";
            CreateTable(input);

            input = "CREATE tAbLe department(\n   department_name varchar(20), \n type varchar(20), \n   num_employees int ) ";
            CreateTable(input);

            //Error case, losing ,
            input = "CREATE tAbLe department\n  ( department_name varchar(20) \n type varchar(20), \n   num_employees int ) ";
            CreateTable(input);

            //Error case, losing (
            input = "CREATE tAbLe department\n   department_name varchar(20), \n type varchar(20), \n   num_employees int ) ";
            CreateTable(input);

            //Error case, more ,
            input = "CREATE tAbLe department\n (  department_name varchar(20), \n type varchar(20), \n   num_employees int, ) ";
            CreateTable(input);

            //Error case, unkown type date
            input = "CREATE tAbLe department\n (  department_name varchar(20), \n type DATE, \n   num_employees int, ) ";
            CreateTable(input);


        }
        private static void println(dynamic str)
        {
            Console.WriteLine(str.ToString());
        }
    }
}
