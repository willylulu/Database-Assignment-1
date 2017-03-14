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
            input = "INSERT INTO losing_attrs_lP   age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing attribute name )
            input = "INSERT INTO losing_attrs_rP   (age, studentId, gender, name \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing attribute value(
            input = "INSERT INTO losing_values_lP   (age, studentId, gender, name) \n vaLUES 20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing attribute value )
            input = "INSERT INTO losing_values_rP   (age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra';";
            Parser.Insert(input);

            //Error case, losing redundent_comma_in_attrs
            input = "INSERT INTO redundent_comma_in_attrs   ( age, studentId, gender, name, ) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, losing , in attrs
            input = "INSERT INTO losing_comma_in_attrs (age, studentId gender, name) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, redundent '
            input = "INSERT INTO redundent_Q_in_values ( age, studentId, gender, name) \n vaLUES (20, 13, 'M', 'Fernand'o Sierra');";
            Parser.Insert(input);

            //Error case, mismatching arguments 
            input = "INSERT INTO mismatching_arguments  ( age, studentId, gender, name, haha, zz) \n vaLUES (20, 13, 'M', 'Fernando Sierra');";
            Parser.Insert(input);

            //Error case, empty values 
            input = "INSERT INTO empty_values  ( age, studentId, gender, name, haha, zz) \n vaLUES ();";
            Parser.Insert(input);

            //Error case, empty values & attrs 
            input = "INSERT INTO empty_attrs_values () \n vaLUES ();";
            Parser.Insert(input);

            //Error case, empty attrs 
            input = "INSERT INTO empty_attrs_values () \n vaLUES (1, 2, 3);";
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
            input = "CREATE tAbLe losing_comma\n  ( department_name varchar(20) \n type varchar(20), \n   num_employees int ) ";
            Parser.CreateTable(input);

            //Error case, losing (              |
            input = "CREATE tAbLe losing_lP\n   department_name varchar(20), \n type varchar(20), \n   num_employees int ) ";
            Parser.CreateTable(input);
            
            //Error case, more ,                                                                                          |
            input = "CREATE tAbLe redundant_comma\n (  department_name varchar(20), \n type varchar(20), \n   num_employees int, ) ";
            Parser.CreateTable(input);

            //Error case, unkown type date                                             |
            input = "CREATE tAbLe unkown_Type_date\n (  department_name varchar(20), \n type DATE, \n   num_employees int, ) ";
            Parser.CreateTable(input);

            //more )                                                      |   
            input = "CreaTe tABle Redundant_rP(stUDENTId int PRimaRY KEY, )\n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //more (                                                           |
            input = "CreaTe tABle rEdundant_lP(stUDENTId int PRimaRY KEY, \n nAM(e vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
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
            input = "CreaTe tABle primary_key_error_PRmaRY(stUDENTId int PRmaRY KEY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string 'primary key' wrong                              |
            input = "CreaTe tABle primary_key_error_noKey(stUDENTId int PRmaRY, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string 'primary key' wrong                                  |
            input = "CreaTe tABle primary_key_error_REdundant_words(stUDENTId int PRimaRY primary key, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);

            //string 'primary key' wrong                                  |
            input = "CreaTe tABle primary_key_error_wrong_words(stUDENTId int          zzz key, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
            Parser.CreateTable(input);


            //string 'primary key' wrong                                  |
            input = "CreaTe tABle primary_key_error_wrong_word(stUDENTId int z, \n nAMe vARCHar(15), \n   gENDEr vaRCHar(1), age vaRCHAr(22))";
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
        public static bool debugFlag = false;
        public static void println(string str)
        {
            if(debugFlag) Console.WriteLine(str);
        }
    }
}
