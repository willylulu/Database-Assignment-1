﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;
using System.Text.RegularExpressions;


namespace Assignment1
{
    class SqlGrammar
    {
        /**********************
        * Parse a string (including number, letter, _ ) until space
        * Return: string
        **********************/
        public static readonly Parser<string> Identifier =
           (from first in Parse.LetterOrDigit.Once().XOr(Parse.Char('_').Once())
            //can parse A-Z a-z 0-9 _ 
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('_')).Many()
            select new string(first.Concat(rest).ToArray())).Token();

        /**********************
        * Parse the string (including any character except ')') between the  ()
        * Return: string
        **********************/
        public static Parser<string> ParenthesisedText =
           (from lparenthesis in Parse.Char('(')
            from content in Parse.CharExcept(')').Many().Text()
            from rparenthesis in Parse.Char(')')
            select content);

        /**********************
        * Parse the string (including any character except ')') between the  ()
        * Return: string
        **********************/
        public static Parser<string> QuotedText =
        (from lquot in Parse.Char('\'')
             // from content in Parse.CharExcept('"').Many().Text()
             from content in Parse.CharExcept('\'').Many().Text().Token()
         from rquot in Parse.Char('\'')
             //prefix "(str)" to indicate that it is a string format
             select "'" + content + "'").Token();

        /**********************
        * Parse a Csv element (',' + a string) 
        * Return: string
        **********************/
        internal static Parser<string> CsvElement =
            (from comma in Parse.Char(',').Once().Token()
             from content in QuotedText.Token().Or(Parse.LetterOrDigit.Many().Text().Token())
             select content);


        /**********************
        * Parse a List of string between () and each string is seperated by ',' 
        * Return: List<string>
        **********************/
        internal static Parser<List<string>> ParenthsisedElements =
           (from lparenthesis in Parse.Char('(').Once().Token()
            from first_content in QuotedText.Token().Or(Parse.LetterOrDigit.Many().Text().Token())
            from contents in CsvElement.Many()
            from rparenthesis in Parse.Char(')').Once().Token()
            select (new List<string> { first_content }.Concat(contents.ToList())).ToList());

        /**********************
        * Parse a string of top 2 words of the parameter
        * Return: string
        **********************/
        public static Parser<string> Instruction =
            (from instruction_1 in Identifier
             from instruction_2 in Identifier
             select instruction_1 + " " + instruction_2

             ).Token();

        /**********************
        * Parse a string of "primary" and "key" (non case-sensitive)
        * Return: string
        **********************/
        public static Parser<string> PrimaryKey = (
            from leading in Parse.WhiteSpace.Many().Or(Parse.Return(""))
            from i1 in Parse.Chars("primaryPRIMARY").Many().Text().Or(Parse.Return(""))
            from mid in Parse.WhiteSpace.Once().Text().Or(Parse.Return(""))
            from i2 in Parse.Chars("keyKEY").Many().Text().Token().Or(Parse.Return(""))
            from trailing in Parse.WhiteSpace.Many().Or(Parse.Return(""))
            select i1 + mid + i2
            ).Token();

        /**********************
        * Parse a Sql_TableAttribute
        * Return: Sql_TableAttribute
        **********************/
        public static Parser<Sql_TableAttribute> TableAttribute =
            (from name in Identifier
             from type in Parse.LetterOrDigit.XOr(Parse.Char('_')).Many().Text()
             from maxLength in ParenthesisedText.Token().Or(Parse.Return(Sql_TableAttribute.STRING_LENGTH_FOR_INT))
                 //from isPrimary in Parse.Chars("primary key").Many().Token().Text().Or(Parse.Return(""))
             from isPrimary in PrimaryKey
             from endComma in Parse.Char(',').Once().Text().Or(Parse.Return(Sql_TableAttribute.LAST_ONE))
             //select new Sql_TableAttribute(name, type, maxLength, isPrimary_1+isPrimary_2, (endComma==Sql_TableAttribute.LAST_ONE))
             select new Sql_TableAttribute(name, type, maxLength, isPrimary, (endComma==Sql_TableAttribute.LAST_ONE))
             ).Token();

        /**********************
        * Parse a Sql_Table
        * Return: Sql_Table
        **********************/
        public static Parser<Sql_Table> Table =
            (from instruction in Instruction
             from name in Identifier
             from startParenthesis in Parse.Char('(').Once().Token()
             from attributes in TableAttribute.Many()
             from endParenthesis in Parse.Char(')').Once().Token()
             select new Sql_Table(name, attributes.ToList())
            ).Token();


        /**********************
        * Parse a Sql_Insertion
        * Return: Sql_Insertion
        **********************/
        public static Parser<Sql_Insertion> Insertion =
            (from instruction in Instruction
             from name in Identifier
             from attributes in ParenthsisedElements.Or(Parse.Return(new List<string>()))
             from values_word in Parse.IgnoreCase("values").Once().Token()
             from values in ParenthsisedElements
             select new Sql_Insertion(name, attributes, values)
             ).Token();


        /**********************
        * Check the string is a valid variable name or not, if not, throw exception
        * Return: Boolean, or throw exception
        *Valid: start with letter or '_', then with any amount of letter, number, '_'
        **********************/
        public static Boolean checkVariableNameValidOrThrowException(string str)
        {
            
            Regex validVariableRgx = new Regex(@"^[a-zA-Z_$][a-zA-Z_$0-9]*$");

            if (!validVariableRgx.IsMatch(str))
                throw new DbException.InvalidKeyword("Invalid Keyword Error: '" + str + "' is invalid variable name");
            return true;
        }


        public class Sql_Item
        {
            public string name;
        }

        public class Sql_TableAttribute : Sql_Item
        {
            public string type;
            public bool isPrimary = false;
            public int maxStringLength = 0; //-1 if type is int
            public bool isLastOne;
            private const string TEXT_FOR_PRIAMRY = "primary key";
            public const string TYPE_STRING = "varchar";
            public const string TYPE_INT = "int";
            public const string LAST_ONE = "END";
            public const string STRING_LENGTH_FOR_INT = "-2147483640";

            public Sql_TableAttribute(string name, string type, string maxStringLength, string PrimaryStr, bool isLastOne)
            {
                name = name.ToLower();
                type = type.ToLower();
                maxStringLength = maxStringLength.ToLower();
                PrimaryStr = PrimaryStr.ToLower();
                this.name = name.Trim();
                this.type = type;
                //Console.WriteLine("~~~~" + name + " : " + PriamryStr + " , type:"+type);
                if (PrimaryStr.Equals(TEXT_FOR_PRIAMRY))
                    this.isPrimary = true;
                else if (PrimaryStr == "")
                    this.isPrimary = false;
                else if (PrimaryStr.Equals(""))
                    this.isPrimary = false;
                else
                    throw new DbException.InvalidKeyword("Invalid Keyword: 'primary key' typo or other parsing error '" + PrimaryStr + "'");



                this.isLastOne = isLastOne;

                //not number string in () error ... 
                if(!Int32.TryParse(maxStringLength, out this.maxStringLength))
                    throw new FormatException("Improper Arguments Error: The Length argument of '" + this.name + "' in () should be a number");
                //Int type has string length error
                if(type.Equals(TYPE_INT) && !maxStringLength.Equals(STRING_LENGTH_FOR_INT))
                    throw new FormatException("Improper Arguments Error: Int type '" + this.name + "' should not has string length" +maxStringLength);
                //string type wrong length error
                if(type.Equals(TYPE_STRING) && (this.maxStringLength < 0 || this.maxStringLength > 40)) 
                    throw new FormatException("Improper Arguments Error: String type '" + this.name + "' has invalid length" +
                                                this.maxStringLength + ", Size of string length should located in 0 between 40");
                //Unkown type
                if (!type.Equals(TYPE_STRING) && !type.Equals(TYPE_INT))
                    throw new DbException.UnkownKeyword("Unkown Data Type: " + type);
                //Invalid name
                checkVariableNameValidOrThrowException(this.name);
            }

            public override string ToString()
            {
                string intro = "(Attr) ";
                return intro + name +": "+ type + "("+Convert.ToString(maxStringLength)+") "+((isPrimary == true)? " isPriamry":"");
            }
        }

        public class Sql_Table : Sql_Item
        {
            public List<Sql_TableAttribute> tableAttributes;
            public Sql_Table(string name, List<Sql_TableAttribute> attributes)
            {
                name = name.ToLower();
                this.name = name.Trim();
                this.tableAttributes = attributes;
                
                //Check variable name is valid or not, if not, rasie exception
                checkVariableNameValidOrThrowException(this.name);

                //check more or less comma
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (i != attributes.Count - 1 && attributes[i].isLastOne == true)
                        throw new ParseException("Invalid Comma Error: Missing Comma"); 
                    else if (i == attributes.Count - 1 && attributes[i].isLastOne != true)
                        throw new ParseException("Invalid Comma Error: Redundent Comma"); 
                }
            }
            

            public override string ToString()
            {
                string intro = "(Table) ";
                string stringForElements = "";
                for (int attrCount = 0; attrCount < tableAttributes.Count; attrCount++)
                {
                    stringForElements += "  "+tableAttributes[attrCount].ToString();
                    stringForElements += "\n";
                }
                return intro + name + "\n" + stringForElements;

            }
        }

        public class Sql_Insertion
        {
            public string table;
            public List<string> AttrNames;
            public List<dynamic> AttrValues;
            public const string NO_ATTRIBUTE_NAME = "NO_ATTRIBUTE_NAME";
            public const string NO_ATTRIBUTE_VALUE = "NO_ATTRIBUTE_VALUE";

            public Sql_Insertion(string table, List<string> attrs, List<string> values)
            {
                Console.WriteLine("~~~~~~~attr: " + attrs.Count + ", values: " + values.Count);
                table = table.ToLower();
                this.table = table;
                this.AttrNames = attrs.Select(t => (checkVariableNameValidOrThrowException(t.Trim()))? t.ToLower().Trim() : null).ToList<string>();
                this.AttrValues = values.Select(t => checkQuoted(t)).ToList<dynamic>();

                if (this.AttrNames.Count != 0 && this.AttrValues.Count != this.AttrNames.Count)
                    throw new DbException.MismatchingArguments("Mismatch of arguments: " + AttrNames.Count +
                        " attributes can't match " + AttrValues.Count + " values");
            }


            /**********************
            * check the string in the Quoted is valid or not
            * If the string (not in Quotes) is number, check and return number
            * If the string is null or '',  check and return null
            * EX:
            * 'asdf'asdf'   --- (X)
            * 'asdf         --- (X)
            * asdf'         --- (X)
            **********************/
            private dynamic checkQuoted(string str)
            {
                //IF WRONG, raise exception.... like 'asdf'asdf, 'asf''', 
                int number;
                if (Int32.TryParse(str, out number))
                    return number;

                //Check null 
                str = str.Trim();
                if (str.Equals(""))
                    return null;
                if (str.Equals("null"))
                    return null;

                //Check if text is wraped by ''
                int startQ = str.IndexOf('\'');
                int endQ = str.LastIndexOf('\'');
                if (startQ != 0 || endQ != str.Length - 1)
                    throw new ParseException("mismatch of ' or  uncompleted () or missing ',' ");
                str = str.Substring(1, str.Length - 2);

                //Check no more ' in the string
                if (str.IndexOf('\'') != -1)
                    throw new ParseException("mismatch of ' or  uncompleted () or missing ',' ");

                return str;
            }
            public override string ToString()
            {
                string intro = "(Insertion) ";
                string stringForAttrName = "";
                string stringForAttrValues = "";

                if (AttrNames == null || AttrNames.Count == 0)
                    stringForAttrName = "NO Attribute Name, ";
                else
                {
                    stringForAttrName += "(";
                    for (int attrNameCount=0; attrNameCount < AttrNames.Count; attrNameCount++)
                    {
                        stringForAttrName += AttrNames[attrNameCount].ToString();
                        if (attrNameCount != AttrNames.Count-1)
                            stringForAttrName += ",";
                    }
                    stringForAttrName += ")";
                }
                if (AttrValues == null)
                    stringForAttrName = "NO Attribute Values, ";
                else
                {
                    stringForAttrValues += "(";
                    for (int valuesCount=0; valuesCount < AttrValues.Count; valuesCount++)
                    {
                        stringForAttrValues += AttrValues[valuesCount].ToString();
                        if (valuesCount != AttrValues.Count-1)
                            stringForAttrValues += ",";
                    }
                    stringForAttrValues += ")";
                       
                }
                return intro + table + "\n  " + stringForAttrName + "\n  " +stringForAttrValues + "\n";

            }
        }

    }
}