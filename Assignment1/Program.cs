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
            TableManager tableManager = new TableManager();

            tableManager.createTable("Dick");

            //Test element for inserting data in table
            List<dynamic> tableElement = new List<dynamic>();
            tableElement.Add("Fuck");
            tableElement.Add(8888);
            tableElement.Add("Jerk");
            tableElement.Add(8888);
            tableManager.insertTable("Dick",tableElement);

            //print all of data in table
            List<List<dynamic>> ans = tableManager.getTable("Dick").getTableData();

            foreach(List<dynamic> ele in ans)
            {
                foreach(dynamic eleAttr in ele)
                {
                    Console.Write(eleAttr + "/" + eleAttr.GetType().Name + " ");
                }
                Console.Write("\n");
            }

        }
    }
}
