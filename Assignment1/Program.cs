using System;
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
