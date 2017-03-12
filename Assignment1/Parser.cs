
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
        static void Main(string[] args)
        {
            //TestCase -- open for updating
            // is ' == " ?? "asdf" is a string??
            // ( ')' ) is valid but in the code would throw exception -- to be fixed
            //double primary key?

            //To use Create Table , just call CreateTable(str sql)
            //To use Insert Table , just call Insert(str sql)
            //To determine which instruction, use  getInstruction(str str)


            /*string text = System.IO.File.ReadAllText(@"../../sql_query.txt");
            text = text.ToLower();
            string[] seperated_query = text.Split(';');
            foreach (string s in seperated_query)
            {
                sql_selector(s.TrimStart());
            }*/


            //ParserTest.TestCreateTable();
            //ParserTest.TestCreateTableError();
            ParserTest.TestInsertion();
            //ParserTest.TestInsertionError();
            Console.ReadKey(true);
            Console.ReadKey(true);
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
        private static void println(dynamic str)
        {
            Console.WriteLine(str.ToString());
        }

    }
}