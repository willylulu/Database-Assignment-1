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

            List<string> order = new List<string>();
            order.Add("studentId");
            order.Add("name");
            order.Add("gender");
            order.Add("age");

            TableAttributeInfo studentIdProp = new TableAttributeInfo("Int32", true, 0);
            TableAttributeInfo nameProp = new TableAttributeInfo("String", false, 15);
            TableAttributeInfo genderProp = new TableAttributeInfo("String", false, 1);
            TableAttributeInfo ageProp = new TableAttributeInfo("Int32", false, 0);

            Dictionary<string, TableAttributeInfo> atributes = new Dictionary<string, TableAttributeInfo>(order.Count);
            atributes.Add("studentId", studentIdProp);
            atributes.Add("name", nameProp);
            atributes.Add("gender", genderProp);
            atributes.Add("age", ageProp);
            //Important！！！Order and Attributes must be Aligned.

            tableManager.createTable("student", order, atributes);

            //Test element for inserting data in table
            Dictionary<string, dynamic> turbel = new Dictionary<string, dynamic>();
            turbel.Add("studentId", 1);
            turbel.Add("name", "Willy");
            turbel.Add("gender", "M");
            turbel.Add("age", 21);
            tableManager.insert("student", turbel);

            //print all of data in table
            Dictionary<Guid, List<dynamic>> ans = tableManager.getTable("student").getTableData();

            foreach(System.Collections.Generic.KeyValuePair<Guid, List<dynamic>> ele in ans)
            {
                Console.Write(ele.Key.ToString() +"\n");
                foreach (dynamic eleAttr in ele.Value)
                {
                    Console.Write(eleAttr + "(" + eleAttr.GetType().Name + ") ");
                }
                Console.Write("\n");
            }

            Console.ReadLine();
        }
    }
}
