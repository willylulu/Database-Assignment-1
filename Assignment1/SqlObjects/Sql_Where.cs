using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Assignment1.SqlObjects
{
    public enum OperandType     //use in where condition
    {
        attr,   //variables
        str,    //constant: String
        num     //constant: number
    }
    public class Sql_Where
    {
        public bool isEmpty;
        public Sql_ListOfConditions listOfConditions;
        public Sql_Where(String keyword, Sql_ListOfConditions listOfConditions)
        {
            if (keyword.Equals(""))
            {
                isEmpty = true;
                return;
            }
            isEmpty = false;
            //
            this.listOfConditions = listOfConditions;
        }
        public override string ToString()
        {
            if (isEmpty)
                return "";

            return "[Where]  " +  listOfConditions.ToString();
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
                    attr_IfTypeisAttr = new Sql_Select_Attr(content, "", false, "", false);
                else
                    //T.ATTR length=6, dotIndex=1
                    attr_IfTypeisAttr = new Sql_Select_Attr(content.Substring(0, dotIndex),
                                                            content.Substring(dotIndex + 1, content.Length - dotIndex - 1 ),
                                                            true, "", false);
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
            //
            

            OperandType leftType = getOperandType(leftOpd_str);
            this.leftOpd = new Sql_Operand(leftOpd_str, leftType);

            if (rightOpd_str.Equals("") && op.Equals(NULL_OPERATION))
            {
                this.op = Operators.none;
                this.opType = OperatorsType.onlyOne;
                return;
            }
            OperandType rightType = getOperandType(rightOpd_str);
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
            else if (op.Equals("<>"))
                this.op = Operators.not_equal;
            else if (op.Equals(NULL_OPERATION) && !leftOpd_str.Equals("") && leftOpd_str.Equals(""))
                this.op = Operators.none;
            else
                throw new DbException.InvalidKeyword("Unkown : " + op);
        }
        public static OperandType getOperandType(String opd)
        {
            
            Regex stringTypeRgx = new Regex(@"^'[^']*'$");
            int num_tmp = 0;
            //
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
        public void setOperandAttrTableIfAvaliable(Sql_Select_Table[] tables)
        {
           if(leftOpd != null && leftOpd.type == OperandType.attr)
                foreach (Sql_Select_Table table in tables)
                    if (table.alias.Equals(leftOpd.attr_IfTypeisAttr.tableAlias) 
                        && leftOpd.attr_IfTypeisAttr.tableAlias != null)
                        leftOpd.attr_IfTypeisAttr.setTable(table);

           if(rightOpd != null && rightOpd.type == OperandType.attr)
                foreach (Sql_Select_Table table in tables)
                    if (table.alias.Equals(rightOpd.attr_IfTypeisAttr.tableAlias) 
                        && rightOpd.attr_IfTypeisAttr.tableAlias != null)
                        rightOpd.attr_IfTypeisAttr.setTable(table);

        }
        public override string ToString()
        {
            if (leftOpd == null)
                return "";
            else if (rightOpd == null)
                return leftOpd.content;
            

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

            //

        }
        public override string ToString()
        {
            if (conditionNum <= 0)
                return "";

            if (conditionNum == 1)
                return firstCondition.ToString();
            else
                return String.Format("{0} {1} {2}",
                                      firstCondition,
                                      conjunction,
                                      secondCondition);

        }

        public void setAttrsTable(Sql_Select_Table[] tables)
        {
            firstCondition.setOperandAttrTableIfAvaliable(tables);
            if (conditionNum == 2)
                secondCondition.setOperandAttrTableIfAvaliable(tables); 
            

        }
    }
}
