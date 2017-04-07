using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Assignment1.SqlObjects
{
    public enum OperandType
    {
        attr, str, num
    }

    public class Sql_Select
    {
        private List<Sql_Select_Attr> attrs;
        private Sql_From from;  //get all the tables from 
        private Sql_Where where;    //get the conditions
        private Sql_Select_Table[] tables;          //tables from from
        private Sql_ListOfConditions conditions;    //conditions from where

        public Sql_Select(List<Sql_Select_Attr>attrs, Sql_From from, Sql_Where where)
        {
            Console.WriteLine("Select " + attrs.Count() + ", " + from.tables.Count() );
            this.attrs = attrs;
            this.from = from;
            this.where = where;

            setSelect();
        }

        public void setSelect()
        {
            this.tables = this.from.tables.ToArray();
            this.conditions = this.where.listOfConditions;

            foreach (Sql_Select_Attr attr in attrs)
            {
                foreach (Sql_Select_Table table in tables)
                {
                    //Console.WriteLine("--" + attr.name + " : "+ attr.tableAlias + " ....table: " + table.alias);
                    if (table.alias.Equals(attr.tableAlias) && attr.tableAlias != null)
                    {
                        //Console.WriteLine("---- " + attr.name + "'s table is " + table.name);
                        attr.setTable(table);
                        break;
                    }
                }
            }


            if(conditions.conditionNum > 0)
            {
                if(conditions.firstCondition.leftOpd.attr_IfTypeisAttr != null)
                    conditions.firstCondition.leftOpd.attr_IfTypeisAttr.setTable(
                        getMatchedTable(conditions.firstCondition.leftOpd.attr_IfTypeisAttr));
                if(conditions.firstCondition.rightOpd.attr_IfTypeisAttr != null)
                    conditions.firstCondition.rightOpd.attr_IfTypeisAttr.setTable(
                        getMatchedTable(conditions.firstCondition.rightOpd.attr_IfTypeisAttr));
                if(conditions.conditionNum == 2)
                {
                    if(conditions.secondCondition.leftOpd.attr_IfTypeisAttr != null)
                        conditions.secondCondition.leftOpd.attr_IfTypeisAttr.setTable(
                            getMatchedTable(conditions.secondCondition.leftOpd.attr_IfTypeisAttr));
                    if(conditions.secondCondition.rightOpd.attr_IfTypeisAttr != null)
                        conditions.secondCondition.rightOpd.attr_IfTypeisAttr.setTable(
                            getMatchedTable(conditions.secondCondition.rightOpd.attr_IfTypeisAttr));

                }

            }

        }

        public Sql_Select_Table getMatchedTable(Sql_Select_Attr attr)
        {
            foreach (Sql_Select_Table table in tables)
            {
                //Console.WriteLine("--" + attr.name + " : "+ attr.tableAlias + " ....table: " + table.alias);
                if (table.alias.Equals(attr.tableAlias) && attr.tableAlias != null)
                {
                    //Console.WriteLine("---- " + attr.name + "'s table is " + table.name);
                    return table;
                }
            }
            return null;

        }
            
    }


    public class Sql_Select_Attr
    {
        public Sql_Select_Table table;
        public String tableAlias;
        public String name;
        public Boolean hasTable;
        public Sql_Select_Attr(String val1, String val2, Boolean hasTable)
        {
            if (hasTable)
            {
                //if has Table, ex:  val.val2
                this.tableAlias = val1;
                this.name = val2.Replace(".", "");  //replace the first dot
                Console.WriteLine("    Select Attr: " + val1 + "." + val2 +" , hasTable: " + hasTable);
            }
            else
            {
                //else: val
                this.name = val1;
                Console.WriteLine("    Select Attr: " + val1 +" " +val2 +"             (No Table Name)");
            }
        }

        public void setTable(Sql_Select_Table table)
        {
            this.table = table;
        }
    }

    public class Sql_Select_Table
    {
        public String name;
        public Boolean hasAlias;
        public String alias;
        public Sql_Select_Table(String name, Boolean hasAlias, String alias)
        {
            Console.WriteLine("    Select Table: " + name + " alias: " + alias);
            this.name = name;
            
            this.hasAlias = hasAlias;
            this.alias = (this.hasAlias) ? alias : null;

        }
    }
    public class Sql_From
    {
        public List<Sql_Select_Table> tables;
        public Sql_From(List<Sql_Select_Table> tables)
        {
            Console.WriteLine("Select from: table number: " + tables.Count());
            this.tables = tables;
        }

    }

    public class Sql_Where
    {
        public Sql_ListOfConditions listOfConditions;
        public Sql_Where(String keyword, Sql_ListOfConditions listOfConditions)
        {
            Console.WriteLine("where ends ");
            this.listOfConditions = listOfConditions;
        }
    }


    public class Sql_Operand
    {
        public String content;
        public OperandType type;
        public Sql_Select_Attr attr_IfTypeisAttr;   //if type id attr, the object would have a Select_attr

        public Sql_Operand(String content, OperandType type)
        {
            this.content = content;
            this.type = type;
            if (this.type == OperandType.attr)
            {
                int dotIndex = content.IndexOf(".");
                if (dotIndex == -1)
                    attr_IfTypeisAttr = new Sql_Select_Attr(content, "", false);
                else
                    //T.ATTR length=6, dotIndex=1
                    attr_IfTypeisAttr = new Sql_Select_Attr(content.Substring(0, dotIndex),
                                                            content.Substring(dotIndex + 1, content.Length - dotIndex - 1 ),
                                                            true);
            }
        }

        /****
        *if type is attr, get the name string
        *if type is num, get the int number
        *if type is str, get the string without leading and trailing ' 
        */
        public dynamic getOperand()
        {
            if (type.Equals(OperandType.str))
                return content.Replace(",", "");
            else if (type.Equals(OperandType.num))
                return Int32.Parse(content);
            else
                return content;

        }
    }

    public class Sql_Condition
    {
        public Sql_Operand leftOpd;
        public Sql_Operand  rightOpd;
        public const String NULL_OPERATION = "Null_Operation";
        public Operators op;         //Change Operator access ability to public 
        public OperatorsType opType;

        public Sql_Condition(String leftOpd_str,  String op, String rightOpd_str)
        {
            if (leftOpd_str.Equals("") && op.Equals("") && rightOpd_str.Equals(""))
                return;
            Console.WriteLine("    Condition: " + leftOpd_str + " " + op + " " + rightOpd_str);
            
            Regex stringTypeRgx = new Regex(@"^'.*'$");

            OperandType leftType = getOperandType(leftOpd_str);
            OperandType rightType = getOperandType(rightOpd_str);
            this.leftOpd = new Sql_Operand(leftOpd_str, leftType);
            this.rightOpd = new Sql_Operand(rightOpd_str, rightType);

            if (leftType.Equals(rightType) && leftType.Equals(OperandType.attr))
                opType = OperatorsType.attr2attr;
            else if (!leftType.Equals(OperandType.attr) && !rightType.Equals(OperandType.attr))
                opType = OperatorsType.constant2constant;
            else if (leftType.Equals(OperandType.attr))
                opType = OperatorsType.attr2constant;
            else if (!leftType.Equals(OperandType.attr))
            {
                swapTwo(leftOpd, rightOpd);
                opType = OperatorsType.attr2constant;
            }

            if (op.Equals("="))
                this.op = Operators.equal;
            else if (op.Equals(">"))
                this.op = Operators.greater;
            else if (op.Equals("<"))
                this.op = Operators.less;
            else if (op.Equals("><"))
                this.op = Operators.not_equal;
            else
                throw new DbException.InvalidKeyword();
        }
        public static OperandType getOperandType(String opd)
        {
            
            Regex stringTypeRgx = new Regex(@"^'[^']*'$");
            int num_tmp = 0;
            Console.WriteLine("        getOpd type: " +opd);
            if (stringTypeRgx.IsMatch(opd))
                return OperandType.str;
            else if (Int32.TryParse(opd, out num_tmp))
                return OperandType.num;
            else if (SqlGrammar.checkVariableNameValidOrThrowException(opd))
                return OperandType.attr;
            else
                throw new DbException.InvalidKeyword("Wrong condition clause.");
        }
        public static void swapTwo(dynamic a, dynamic b)
        {
            var tmp = a;
            a = b;
            b = tmp;
        }
        public override string ToString()
        {
            return  leftOpd.content + " " + op + " " + rightOpd.content;
        }

    }

    public class Sql_ListOfConditions
    {
        public int conditionNum;
        public Sql_Condition firstCondition;
        public Sql_Condition secondCondition;
        public String conjunction;

        public Sql_ListOfConditions(Sql_Condition firstCondition, String conjunction,  Sql_Condition secondCondition)
        {
            this.firstCondition = firstCondition;
            this.conditionNum = (conjunction.Equals("")) ? 1 : 2;  //1: only firstCondition exists, 2: both two constions exist

            this.conjunction = (this.conditionNum == 2) ? conjunction : null;
            this.secondCondition = (this.conditionNum == 2) ? secondCondition : null;

            Console.WriteLine("    Conditions: " + firstCondition + " " + conjunction + " " + secondCondition);

        }
    }

    public class Sql_Agggregation
    {

    }
}
