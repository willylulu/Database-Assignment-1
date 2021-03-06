﻿using LinqKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment1
{
    class Program
    {

        static Boolean isSame(Guid id1, Guid id2, Dictionary<Guid, dynamic> G1, Dictionary<Guid, dynamic> G2)
        {
            if (G1[id1] == G2[id2])
            {
                return true;
            }else
            {
                return false;
            }
        }
        static void Main4(string[] args)
        {
            Guid id1 = Guid.NewGuid();
            Guid id2 = Guid.NewGuid();
            int hash1 = id1.GetHashCode();
            int hash2 = id2.GetHashCode();
            Console.WriteLine(id1);
            Console.WriteLine(id2);
            Console.WriteLine(hash1);
            Console.WriteLine(hash2);
            int hash = (hash1 + hash2).GetHashCode();
            Console.WriteLine(hash);
            Console.ReadKey(true);
        }
        static void Main3(string[] args)
        {
            HashSet<Dictionary<string, Guid>> elementSet = new HashSet<Dictionary<string, Guid>>();

            Dictionary<string, Guid> D1 = new Dictionary<string, Guid>();
            Dictionary<Guid, dynamic> G1 = new Dictionary<Guid, dynamic>();
            Dictionary<Guid, dynamic> G2 = new Dictionary<Guid, dynamic>();

            
            Guid id = Guid.NewGuid();
            G1.Add(id, 123);
            D1.Add("A1", id);
            id = Guid.NewGuid();
            G2.Add(id, 123);
            D1.Add("A2", id);

            Dictionary<string, Guid> D2 = new Dictionary<string, Guid>();
            
            id = Guid.NewGuid();
            G1.Add(id, 123);
            D2.Add("A1", id);
            id = Guid.NewGuid();
            G2.Add(id, 124);
            D2.Add("A2", id);

            elementSet.Add(D1);
            elementSet.Add(D2);

            var predicate = PredicateBuilder.New<Dictionary<string, Guid>>();
            
            predicate = predicate.Or(X => isSame(X["A1"],X["A2"],G1,G2) );
            

            var ans = elementSet.Where(predicate);
            
            foreach(var data in ans)
            {
                foreach(KeyValuePair<string, Guid> dd in data)
                {
                    Console.WriteLine(dd);
                }
            }
                
            Console.ReadKey(true);
        }
        /*static void Main(string[] args)
        {
            TableManager tableManager = new TableManager();

            //table1
            string table1_name = "Mockdata";
            List<string> table1_attrorder = new List<string>();
            table1_attrorder.Add("id");
            table1_attrorder.Add("height");
            table1_attrorder.Add("salary");
            table1_attrorder.Add("age");
            table1_attrorder.Add("FirstName");
            Dictionary<string, TableAttribute> table1_attr = new Dictionary<string, TableAttribute>();
            table1_attr.Add("id", new TableAttribute("Int32",false,0));
            table1_attr.Add("height", new TableAttribute("Int32", false, 0));
            table1_attr.Add("salary", new TableAttribute("Int32", false, 0));
            table1_attr.Add("age", new TableAttribute("Int32", false, 0));
            table1_attr.Add("FirstName", new TableAttribute("String", false, 30));
            tableManager.createTable(table1_name, table1_attrorder, table1_attr);
            Dictionary<string, dynamic> insert_data = new Dictionary<string, dynamic>();
            insert_data.Add("id", 100);
            insert_data.Add("height", 175);
            insert_data.Add("salary", 40000);
            insert_data.Add("age", 25);
            insert_data.Add("FirstName", "Jhon");
            tableManager.insert(table1_name,insert_data);
            insert_data = new Dictionary<string, dynamic>();
            insert_data.Add("id", 102);
            insert_data.Add("height", 180);
            insert_data.Add("salary", 42000);
            insert_data.Add("age", 30);
            insert_data.Add("FirstName", "Mike");
            tableManager.insert(table1_name, insert_data);

            //table2
            string table2_name = "Mockdata2";
            List<string> table2_attrorder = new List<string>();
            table2_attrorder.Add("eid");
            table2_attrorder.Add("eheight");
            table2_attrorder.Add("esalary");
            Dictionary<string, TableAttribute> table2_attr = new Dictionary<string, TableAttribute>();
            table2_attr.Add("eid", new TableAttribute("Int32", false, 0));
            table2_attr.Add("eheight", new TableAttribute("Int32", false, 0));
            table2_attr.Add("esalary", new TableAttribute("Int32", false, 0));
            tableManager.createTable(table2_name, table2_attrorder, table2_attr);
            insert_data = new Dictionary<string, dynamic>();
            insert_data.Add("eid", 100);
            insert_data.Add("eheight", 175);
            insert_data.Add("esalary", 40000);
            tableManager.insert(table2_name, insert_data);
            insert_data = new Dictionary<string, dynamic>();
            insert_data.Add("eid", 103);
            insert_data.Add("eheight", 156);
            insert_data.Add("esalary", 42000);
            tableManager.insert(table2_name, insert_data);

            table_attribute_pair[] tap1 = new table_attribute_pair[1];
            tap1[0] = new table_attribute_pair(table1_name, "height");
            where table1_new = new where("salary",Operators.greater,39000,OperatorLink.AND);
            List<where> table1_where = new List<where>();
            table1_where.Add(table1_new);
            tableSelect[] table1Select = new tableSelect[1];
            table1Select[0] = new tableSelect(table1_name, table1_where);
            whereJoin[] wj = new whereJoin[1];
            wj[0] = new whereJoin(table1_name,"salary",Operators.equal,table2_name,"esalary",OperatorLink.AND);
            tableManager.select(tap1, table1Select, wj);


            tableManager.print_table_context();
            Console.ReadKey(true);
        }*/
        static void Main(string[] args)
        {
            tableManager = new TableManager();
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            Stopwatch sw = new Stopwatch();
            //TestCase -- open for updating
            //1. Insertion that has no values
            //2. Insertion that values('1'1, '2'3' ) raise exception

            //To use Create Table , just call CreateTable(str sql)
            //To use Insert Table , just call Insert(str sql)
            //To determine which instruction, use  getInstruction(str str)

            if(args.Length == 1)
            {
                if (args[0] == "-r")
                {
                    Console.WriteLine("Read File "+ args[1]);
                    file = File.Open(args[1], FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter reader = new BinaryFormatter();
                    tableManager = (TableManager)reader.Deserialize(file);
                    file.Dispose();
                    file.Close();
                }
            }
            else
            {
                Console.WriteLine("Cold Start");
            }
            while (true)
            {
                string cmd = Console.ReadLine();
                string[] cmd_split = cmd.Split(' ');
                if (cmd == "exit")
                {
                    break;
                }
                else if (cmd == "t")
                {
                    sw.Reset();
                    sw = Stopwatch.StartNew();
                    string text = "";
                    text = System.IO.File.ReadAllText(@"../../testcase/createTables.sql");
                    Parser.sql_parser(text, tableManager);

                    text = System.IO.File.ReadAllText(@"../../testcase/author.sql");
                    Parser.sql_parser(text, tableManager);

                    text = System.IO.File.ReadAllText(@"../../testcase/book.sql");
                    Parser.sql_parser(text, tableManager);
                    //
                    tableManager.print_table_context();


                    //text = System.IO.File.ReadAllText(@"../../testcase/select_test.sql");
                    //Parser.sql_parser(text, tableManager);
                    sw.Stop();
                    long ms = sw.ElapsedMilliseconds;
                    Console.WriteLine("Cost " + ms + " ms");
                }
                else if (cmd == "t2")
                {
                    sw.Reset();
                    sw = Stopwatch.StartNew();
                    string text = "";
                    text = System.IO.File.ReadAllText(@"../../testcase/createTables.sql");
                    Parser.sql_parser(text, tableManager);

                    text = System.IO.File.ReadAllText(@"../../testcase/autuor2.sql");
                    Parser.sql_parser(text, tableManager);

                    text = System.IO.File.ReadAllText(@"../../testcase/book3.sql");
                    Parser.sql_parser(text, tableManager);
                    //
                    tableManager.print_table_context();


                    //text = System.IO.File.ReadAllText(@"../../testcase/select_test.sql");
                    //Parser.sql_parser(text, tableManager);
                    sw.Stop();
                    long ms = sw.ElapsedMilliseconds;
                    Console.WriteLine("Cost " + ms + " ms");
                }
                else if(cmd == "c")
                {
                    tableManager.turnOnIndexing("book", "pages");
                }
                else if(cmd_split[0] == "read")
                {
                    string input = "";
                    if (cmd_split[1] == "default")
                    {
                        input = "data.db2";
                    }
                    else input = cmd_split[1];
                    Console.WriteLine("Read File " + input);
                    sw.Reset();
                    sw = Stopwatch.StartNew();
                    file = File.Open(input, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    BinaryFormatter reader = new BinaryFormatter();
                    tableManager = (TableManager)reader.Deserialize(file);
                    file.Dispose();
                    file.Close();
                    sw.Stop();
                    long ms = sw.ElapsedMilliseconds;
                    Console.WriteLine("Cost " + ms + " ms");
                }
                else if(cmd_split[0] == "sql")
                {
                    sw.Reset();
                    sw = Stopwatch.StartNew();
                    string text = "";
                    text = System.IO.File.ReadAllText(cmd_split[1]);
                    Parser.sql_parser(text, tableManager);
                    sw.Stop();
                    long ms = sw.ElapsedMilliseconds;
                    Console.WriteLine("Cost " + ms + " ms");
                }
                else
                {
                    sw.Reset();
                    sw = Stopwatch.StartNew();
                    Parser.sql_parser(cmd, tableManager);
                    sw.Stop();
                    long ms = sw.ElapsedMilliseconds;
                    Console.WriteLine("Cost " + ms + " ms");
                }
            }

            //string text = "";
            //text = System.IO.File.ReadAllText(@"../../testcase/createTables.sql");
            //Parser.sql_parser(text, tableManager);

            //text = System.IO.File.ReadAllText(@"../../testcase/author.sql");
            //Parser.sql_parser(text, tableManager);

            //text = System.IO.File.ReadAllText(@"../../testcase/book.sql");
            //Parser.sql_parser(text, tableManager);

            //text = System.IO.File.ReadAllText(@"../../testcase/student.sql");
            //Parser.sql_parser(text, tableManager);

            //tableManager.print_table_context();


            //text = System.IO.File.ReadAllText(@"../../testcase/select_test.sql");
            //Parser.sql_parser(text, tableManager);

            /* do
             {
                 sql_path = Console.ReadLine();
                 text = System.IO.File.ReadAllText(sql_path);
                 Parser.sql_parser(text, tableManager);
             } while (sql_path != null);*/

            //text = System.IO.File.ReadAllText(args.Length==0? "../../testcase/createTables.sql" : args[0]);
            //string text = System.IO.File.ReadAllText(@"../../sql_error3.sql");


            //string[] seperated_query = text.Split(';');
            //foreach (string s in seperated_query)
            //{
            //    console.writeline(s);
            //    parser.sql_selector(s.trimstart());
            //}
            //sw.Stop();
            //long ms = sw.ElapsedMilliseconds;
            //Console.WriteLine("Cost "+ms+" ms");
            //tableManager.print_table_context();
            //TestCreateTable();
            //TestInsertion();
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            tableManager.print_table_context();
            file = File.Open("data.db2", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            BinaryFormatter writer = new BinaryFormatter();
            writer.Serialize(file, tableManager);
            file.Dispose();
            file.Close();
        }

        static public FileStream file = null;
        static public TableManager tableManager;
    }
}
