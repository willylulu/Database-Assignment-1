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
             select instruction_1 + instruction_2
             ).Token();

        internal static Parser<Sql_TableAttribute> TableAttribute =
            (from name in Identifier
             from type in Parse.LetterOrDigit.XOr(Parse.Char('-')).XOr(Parse.Char('_')).Many().Text()
             from maxLength in ParenthsisedText.Or(Parse.Return("-1"))
             from isPrimary in Parse.Chars("primary key").Many().Token().Text().Or(Parse.Return("NO"))
             from end in Parse.Char(',').Once().Text().Or(Parse.Return("END"))
             select new Sql_TableAttribute(name, type, maxLength, isPrimary)
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
            private const string TEXT_FOR_PRIAMRY = "primary key";

            public Sql_TableAttribute(string name, string type, string maxStringLength, string isPriamry)
            {
                this.name = name;
                this.type = type;
                this.maxStringLength = Int32.Parse(maxStringLength);
                if (isPriamry.Equals(TEXT_FOR_PRIAMRY))
                    this.isPrimary = true;
                else
                    this.isPrimary = false;
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
            public List<string> AttrItem;
            public List<string> AttrValues;
            //public string AttrItem;
            //public string AttrValues;

            public Sql_Insertion(string table, List<string> attrs, List<string> values)
            {
                this.table = table;
                //this.AttrValues = values;
                //this.AttrItem = attrs;
            }
            public Sql_Insertion(string table, string attrs, string values)
            {
                this.table = table;
                if (attrs.Equals("NO_ATTRIBUTE_NAME"))
                {
                    this.AttrItem = new List<string>();
                }
                else
                {
                    string[] attrsArray = attrs.Split(',');
                    this.AttrItem = attrsArray.ToList();
                }
                string[] valuesArray = values.Split(',');
                this.AttrValues = valuesArray.ToList();
                

            }
            public override string ToString()
            {
                string intro = "(Insertion) ";
                return intro + table + "\n  " + AttrItem + "\n  " +AttrValues + "\n";

            }
        }

    }
}
