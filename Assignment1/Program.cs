using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class Program
    {
        static void Main(string[] args)
        {
            Table table = new Table("Dick");

            //Test element for inserting data in table
            List<dynamic> tableElement = new List<dynamic>();
            tableElement.Add("Fuck");
            tableElement.Add(123456);
            tableElement.Add("Jerk");
            tableElement.Add(8888);
            table.insert(tableElement);

            //print all of data in table
            List<List<dynamic>> ans = table.getTableData();

            foreach(List<dynamic> ele in ans)
            {
                foreach(dynamic eleAttr in ele)
                {
                    Console.Write(eleAttr + " ");
                }
                Console.Write("\n");
            }
        }
    }
}
