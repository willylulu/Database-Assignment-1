using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sprache;


namespace Assignment1
{
    class SqlGrammar
    {
        public static readonly Parser<string> Identifier =
           (from first in Parse.Letter.Once()
                //can parse letter, digit, -, _ for many
            from rest in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many()
            select new string(first.Concat(rest).ToArray())).Token();

        internal static Parser<string> ParenthsisedText =
           (from lparenthesis in Parse.Char('(')
            from content in Parse.CharExcept(')').Many().Text()
            from rparenthesis in Parse.Char(')')
            select content);

        internal static Parser<string> QuotedText =
            (from lquot in Parse.Char('\'')
                 // from content in Parse.CharExcept('"').Many().Text()
             from content in Parse.CharExcept('\'').Many().Text().Token()
             from rquot in Parse.Char('\'')
                 //prefix "(str)" to indicate that it is a string format
             select "(str)" + content).Token();

        internal static Parser<string> CsvElement =
            (from content in QuotedText.Or(Identifier)
             from comma in Parse.Char(',').Once().Token().Or(Parse.Return("END"))
             select content);

        internal static Parser<List<string>> ParenthsisedElements =
           (from lparenthesis in Parse.Char('(')
            from contents in CsvElement.Many()
            from rparenthesis in Parse.Char(')')
            select contents.ToList());



        public static Parser<string> Instruction =
            (from instruction_1 in Identifier
             from instruction_2 in Identifier
             select instruction_1 + " " + instruction_2

             ).Token();

        internal static Parser<Sql_TableAttribute> TableAttribute =
            (from name in Identifier
             from type in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many().Text()
             from maxLength in ParenthsisedText.Or(Parse.Return("-1"))
             from isPrimary in Parse.Chars("primary key").Many().Token().Text().Or(Parse.Return(""))
             from end in Parse.Char(',').Once().Text().Or(Parse.Return("END"))
             select new Sql_TableAttribute(name, type, maxLength, isPrimary, (end=="END"))
             //select new List<string> { name, type, maxLength, isPrimary, end}
             ).Token();

        internal static Parser<Sql_Table> Table =
            (from instruction in Instruction
                 //what is ok in table name?
             from name in Identifier
             from startParenthesis in Parse.Char('(').Once().Token().Text()
             from attributes in TableAttribute.Many()
             from endParenthesis in Parse.Char(')').Once().Token().Text()
                 //select new List<dynamic> { name, attributes}
                 //select new List<dynamic> {name, attributes, startParenthesis, endParenthesis}
             select new Sql_Table(name, attributes.ToList())
            ).Token();


        internal static Parser<Sql_Insertion> Insertion =
            (from instruction in Instruction
                 //What is ok in table name?
             from name in Identifier
             from attributes in ParenthsisedText.Or(Parse.Return("NO_ATTRIBUTE_NAME"))
             //from attributes in ParenthsisedElements.Or(Parse.Return(new List<string>()))
             from values_word in Parse.Chars("values").Many().Token().Text()
             //from values in ParenthsisedElements 
             from values in ParenthsisedText.Or(Parse.Return("NO_ATTRIBUTE_VALUE"))
             //select new List<dynamic> { name, attributes}
             //select new List<dynamic> {name, attributes, values_word, values}
             select new Sql_Insertion(name, attributes, values)
             ).Token();


        public Boolean checkVariableName(string str)
        {
            char[] invalidChar = {'!', '@', '#', '$', '%', '^', '&', '*', '(',
                                    ')', '+', '=', '[', ']', '{', '}', '<', '>', ';', ':', '~'};
            //check if space in it
            if (str.IndexOf(' ') != -1)
                return false;
            if (str.IndexOfAny(invalidChar) != -1)
                return false;

            //number cannot be the first
            int number;
            if (Int32.TryParse(str[0].ToString(), out number))
                return false;

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
            //public int maxStringLength = 0; //-1 if type is int
            public int maxStringLength = 0; //-1 if type is int
            public bool isLastOne;
            private const string TEXT_FOR_PRIAMRY = "primary key";
            public const string TYPE_STRING = "varchar";
            public const string TYPE_INT = "int";

            public Sql_TableAttribute(string name, string type, string maxStringLength, string isPriamry, bool isLastOne)
            {
                this.name = name;
                this.type = type;
                this.maxStringLength = Int32.Parse(maxStringLength);
                if (isPriamry.Equals(TEXT_FOR_PRIAMRY))
                    this.isPrimary = true;
                else
                    this.isPrimary = false;
                this.isLastOne = isLastOne;

                if (!type.Equals(TYPE_STRING) && !type.Equals(TYPE_INT))
                    throw new DbException.UnknownTypeException("Unkown Data Type: " + type);
                    
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
                this.name = name;
                this.tableAttributes = attributes;

                //check more or less comma
                for (int i = 0; i < attributes.Count; i++)
                {
                    if (i != attributes.Count - 1 && attributes[i].isLastOne == true)
                        throw new DbException.InvalidCommaException("less comma"); 
                    else if (i == attributes.Count - 1 && attributes[i].isLastOne != true)
                        throw new DbException.InvalidCommaException("More comma"); 
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
            //public string AttrItem;
            //public string AttrValues;

            public Sql_Insertion(string table, string attrs, string values)
            {
                this.table = table;
                this.AttrNames = (attrs.Equals(NO_ATTRIBUTE_NAME)) ? null : attrs.Split(',').Select(t => t.Trim()).ToList();
                this.AttrValues = values.Split(',').Select(t => checkQuoted(t)).ToList<dynamic>();
            }

            private dynamic checkQuoted(string str)
            {
                //IF WRONG, raise exception.... like 'asdf'asdf, 'asf''', 
                int number;
                if (Int32.TryParse(str, out number))
                    return number;

                str = str.Trim();
                if (str.Equals(""))
                    throw new DbException.InvalidSQLArguments("empty argument...");


                int startQ = str.IndexOf('\'');
                int endQ = str.LastIndexOf('\'');
                //Check if text is wraped by ''
                if (startQ != 0 || endQ != str.Length - 1)
                    throw new DbException.InvalidSQLArguments("mismatch of ' ");

                str = str.Substring(1, str.Length - 2);
                //Check no more ' in the string
                if (str.IndexOf('\'') != -1)
                    throw new DbException.InvalidSQLArguments("mismatch of ' ");

                return str;
            }
            public override string ToString()
            {
                string intro = "(Insertion) ";
                string stringForAttrName = "";
                string stringForAttrValues = "";

                if (AttrNames == null)
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
