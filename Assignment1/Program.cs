using LinqKit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;
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

            string sql_path;
            string text;
            text = System.IO.File.ReadAllText("../../testcase/createTables.sql");
            Parser.sql_parser(text, tableManager);

            text = System.IO.File.ReadAllText("../../testcase/author.sql");
            Parser.sql_parser(text, tableManager);

            text = System.IO.File.ReadAllText("../../testcase/book.sql");
            Parser.sql_parser(text, tableManager);

            text = System.IO.File.ReadAllText("../../testcase/student.sql");
            Parser.sql_parser(text, tableManager);

            text = System.IO.File.ReadAllText("../../testcase/select_test.sql");
            Parser.sql_parser(text, tableManager);

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
