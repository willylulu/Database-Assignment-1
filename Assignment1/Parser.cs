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
            TestCreateTable();



            Console.ReadKey(true);
        }
        private static void TestInsertion()
        {
            string input_insert1 = "INSERT INTO Student \n VALUES(10, 'John Smith', 'M', 22)";
            input_insert1 = input_insert1.ToLower();
            println(input_insert1);

            var insertion = SqlGrammar.Insertion.Parse(input_insert1);
            printList(insertion);
        }

        private static void TestCreateTable()
        {
            //string input_table = "CREATE Table student(\n   studentId in PRIMARY KEY, \n name varchar(15), \n   gender varchar(1), age int)";
            string input_table = "CREATE Table student(studentId int PRIMARY KEY, \n name varchar(15), \n   gender varchar(1), age varchar(22))";
            input_table = input_table.ToLower();
            println(input_table);

            //var input_attribute = "name varchar(15) primary key,";
            //printList(attribute);*/
            var table = SqlGrammar.Table.Parse(input_table);
            printTable(table);


           input_table = "CREATE tAbLe PerSon(\n   personId int PRIMARY KEY, \n name varchar(20), \n   m_gender varchar(1)   )";
            input_table = input_table.ToLower();
            println(input_table);
            table = SqlGrammar.Table.Parse(input_table);
            printTable(table);

            input_table = "CREATE tAbLe department(\n   department_name varchar(20), \n type varchar(20), \n   num_employees int ) ";
            input_table = input_table.ToLower();
            println(input_table);
            table = SqlGrammar.Table.Parse(input_table);
            printTable(table);

        }
        private static void println(dynamic str)
        {
            Console.WriteLine(str.ToString());
        }
        private static void printList(List<dynamic> list)
        {
            println("--------------------");
            println("List:");
            for (int i=0; i<list.Count; i++)
            {
                println(list[i].ToString());
            }
        }
        private static void printTable(List<dynamic> table)
        {
            println("Table:");
            for (int i=0; i<table[1].Count; i++)
            {
                for(int j=0; j<table[1][i].Count; j++)
                {
                    Console.Write(table[1][i][j]+", ");
                }
                Console.WriteLine("");

            }

            println("--------------------");

        }
    }
}
