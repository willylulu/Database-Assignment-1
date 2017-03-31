﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class Program
    {

        static void Main(string[] args)
        {
            List<Object> arr = new List<object>(10);
            var aa = new where("t1", "a1", "t2", "a2", Operators.equal, OperatorLink.AND);
            arr.Add(12);
            arr[0] = aa;
            arr.Add(aa);
            Console.WriteLine(arr[0].GetType());
            Console.WriteLine(arr[1].GetType());
            Console.WriteLine(arr[1].GetType() == typeof(where));

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
        static void Main2(string[] args)
        {
            TableManager tableManager = new TableManager();
            //TestCase -- open for updating
            //1. Insertion that has no values
            //2. Insertion that values('1'1, '2'3' ) raise exception

            //To use Create Table , just call CreateTable(str sql)
            //To use Insert Table , just call Insert(str sql)
            //To determine which instruction, use  getInstruction(str str)

            Stopwatch sw = new Stopwatch();
            sw.Reset();
            sw = Stopwatch.StartNew();

            string text = System.IO.File.ReadAllText(args.Length==0?@"../../sql_query.sql": args[0]);
            //string text = System.IO.File.ReadAllText(@"../../sql_error3.sql");
            Parser.sql_parser(text,tableManager);

            //string[] seperated_query = text.Split(';');
            //foreach (string s in seperated_query)
            //{
            //    console.writeline(s);
            //    parser.sql_selector(s.trimstart());
            //}
            sw.Stop();
            long ms = sw.ElapsedMilliseconds;
            Console.WriteLine("Cost "+ms+" ms");
            tableManager.print_table_context();
            //TestCreateTable();
            //TestInsertion();
            Console.ReadKey(true);
        }
    }
}
