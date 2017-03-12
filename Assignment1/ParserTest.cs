using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class ParserTest
    {
        public static void TestInsertionError()
        {
            // only consider '
            // ( ')' ) is valid but in the code would throw exception --fixed
            //double primary key? not my consideration
            
            string input = "";

            //Error case, losing attribute name (
            input = "INSERT INTO Student7   age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing attribute name )
            input = "INSERT INTO Student6   (age, studentId, gender, name \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing attribute value(
            input = "INSERT INTO Student5   (age, studentId, gender, name) \n vaLUES 20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing attribute value )
            input = "INSERT INTO Student4   (age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra';";
            Parser.Insert(input);

            //Error case, losing attribute value )
            input = "INSERT INTO Student3   ( age, studentId, gender, name, ) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing , -- to be sloved
            input = "INSERT INTO Student2 (age, studentId gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, arguments not match 
            input = "INSERT INTO Student1 (age, studentId gender, name) \n vaLUES (13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, redundent '
            input = "INSERT INTO REDUNDENT ( age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernand'o Sierra');";
            Parser.Insert(input);

        }
        public static void TestInsertion()
        {
            string input = "";
            //Normal test
            input = "INSERT INTO Student \n VALUES(10,   'John(- Smith'   , 'M', 22)";
            Parser.Insert(input);

            //Normal test
            input = "INSERT INTO Student \n VALUES(10,   'John- Smith'   , 'M', 22)";
            Parser.Insert(input);

            //Case-Sensitive test
            input = "INsErT InTo sTUDEnt \n ValUEs(112, 'cCcCc+  |+ccccc', 'F', 22);";
            Parser.Insert(input);

            // () values () test
            input = "INSERT INTO Student (age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //ADD \n in every place
            input = "INSERT \n INTO \nStudent \n\n(name, \n age, \nstudentId, gender) \nVAlues\n ('Ai>> Toshiko', 21\n, 12, 'F');";
            Parser.Insert(input);

            //Normal test, with ( in string
            input = "INSERT INTO Student \n VALUES(10,   'John(- Smith'   , 'M', 22)";
            Parser.Insert(input);

            //Normal test, with ) in string
            input = "INSERT INTO Student \n VALUES(10,   'John)- Smith'   , 'M', 22)";
            Parser.Insert(input);

            //Normal test, with ) in string
            input = "INSERT INTO Student \n VALUES(10,   'John, Smith'   , 'M', 22)";
            Parser.Insert(input);
        }

        public static void TestCreateTableError()
        {
            string input = "";
            //Error case, losing ,                                           |   
            input = "CREATE tAbLe department\n  ( department_name varchar(20) \n type varchar(20), \n   num_employees int ) ";
            Parser.CreateTable(input);

            //Error case, losing (              |
            input = "CREATE tAbLe losing_left_P\n   department_name varchar(20), \n type varchar(20), \n   num_employees int ) ";
            Parser.CreateTable(input);
            
            //Error case, more ,                                                                                          |
            input = "CREATE tAbLe more_comma\n (  department_name varchar(20), \n type varchar(20), \n   num_employees int, ) ";
            Parser.CreateTable(input);

            //Error case, unkown type date                                             |
            input = "CREATE tAbLe unkown_Type_date\n (  department_name varchar(20), \n type DATE, \n   num_employees int, ) ";
            Parser.CreateTable(input);

            //more )                                                      |   
            input = "CreaTe tABle MORE_right_P(stUDENTId int PRimaRY KEY, )\n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //more (                                                           |
            input = "CreaTe tABle MORE_left_P(stUDENTId int PRimaRY KEY, \n nAM(e vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string in () is not a number                                                             | 
            input = "CreaTe tABle varchar_Length_not_number(stUDENTId int PRimaRY KEY, \n nAMe vARCHar(fifty), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //varchar oversize                                                                   | 
            input = "CreaTe tABle varchar_oversize_50(stUDENTId int PRimaRY KEY, \n nAMe vARCHar(50), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //int has length                                   | 
            input = "CreaTe tABle int_has_length(stUDENTId int (12) PRimaRY KEY, \n nAMe vARCHar(50), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string 'primary key' not complete                     |
            input = "CreaTe tABle primary_key_error(stUDENTId int PRmaRY KEY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string 'primary key' wrong                              |
            input = "CreaTe tABle primary_key_error(stUDENTId int PRmaRY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string 'primary key' wrong                                  |
            input = "CreaTe tABle primary_key_error(stUDENTId int PRimaRY primary key, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string 'primary key' wrong                                  |
            input = "CreaTe tABle primary_key_error(stUDENTId int          zzz key, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

        }
        public static void TestCreateTable()
        {
            string input = "";
            //Normal Test + Case Sensitive
            input = "CreaTe tABle stUDent(stUDENTId int PRimaRY KEY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //Primary Key 
            input = "CREATE tAbLe PerSon(\n   personId int PRIMARY KEY, \n name varchar(20) PRiMArY KEY, \n   m_gender varchar(1)   )";
            Parser.CreateTable(input);

            //varchr (2) -- space between
            input = "CREATE tAbLe spece_between_name_andP(\n   department_name varchar (20), \n type varchar(20), \n   num_employees int ) ";
            Parser.CreateTable(input);

            // Primary key test                                                  |
            input = "CREATE tAbLe Primary_key_test(\n   personId int    PRIMARY     KEY, \n name varchar(20) PRiMArY KEY, \n   m_gender varchar(1)   )";
            Parser.CreateTable(input);

            // Primary key test                                                  |
            input = "CREATE tAbLe Primary_key_test(\n   personId int   , \n name varchar(20) PRiMArY KEY, \n   m_gender varchar(1)   )";
            Parser.CreateTable(input);
        }
        public static void println(dynamic str)
        {
            Console.WriteLine(str.ToString());
        }


    }
}
