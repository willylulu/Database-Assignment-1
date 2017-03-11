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

            List<string> order = new List<string>();
            order.Add("studentId");
            order.Add("name");
            order.Add("gender");
            order.Add("age");

            TableAttribute studentIdProp = new TableAttribute("Int32", true, 0);
            TableAttribute nameProp = new TableAttribute("String", false, 15);
            TableAttribute genderProp = new TableAttribute("String", false, 1);
            TableAttribute ageProp = new TableAttribute("Int32", false, 0);

            Dictionary<string, TableAttribute> atributes = new Dictionary<string, TableAttribute>(order.Count);
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

            //print table data to csv file
            tableManager.print_table_context();
            Console.ReadLine();
        }
    }
}
