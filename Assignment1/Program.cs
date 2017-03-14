using System;
using System.Collections.Generic;
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

            string text = System.IO.File.ReadAllText(@"../../sql_query.sql");
            Parser.sql_parser(text,tableManager);

            tableManager.print_table_context();
            //TestCreateTable();
            //TestInsertion();
            Console.WriteLine("End");
            Console.ReadKey(true);
        }
    }
}
