using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1.SqlObjects
{
    public class Sql_Select
    {
        public List<Sql_Select_Attr> attrs;
        public Sql_Select_Table[] tables;

        public Sql_Select(List<Sql_Select_Attr>attrs, Sql_From from, Sql_Where where)
        {

        }

        public void setSelect()
        {

        }
            
    }
    public class Sql_Select_Attr
    {
        public String table;
        public String name;
        public Boolean hasTable;
        public Sql_Select_Attr(String val1, String val2, Boolean hasTable)
        {
            if (hasTable)
            {
                this.table = val1;
                this.name = val2;
            }
            else
                this.name = val1;
        }
    }

    public class Sql_Select_Table
    {
        public String table;
        public Boolean hasAlias;
        public String alias;
        public Sql_Select_Table(String table, Boolean hasAlias, String alias)
        {
            this.table = table;
            this.alias = alias;
            this.hasAlias = hasAlias;
        }
    }
    public class Sql_From
    {
        public List<Sql_Select_Table> tables;
        public Sql_From(List<Sql_Select_Table> tables)
        {
            this.tables = tables;
        }

    }

    public class Sql_Where
    {
        Sql_ListOfConditions listOfConditions;
        public Sql_Where(String keyword, Sql_ListOfConditions listOfConditions)
        {
            this.listOfConditions = listOfConditions;
        }

    }

    public class Sql_Condition
    {
        public String leftOperand;
        public String rightOperand;
        public String operation;
        public const String NULL_OPERATION = "Null_Operation";

        public Sql_Condition(String leftOperand, String rightOperand, String operation)
        {
            this.leftOperand = leftOperand;
            this.rightOperand = rightOperand;
            this.operation = operation;
        }

    }

    public class Sql_ListOfConditions
    {
        public Sql_Condition firstCondition;
        public Sql_Condition[] conditions;
        public String[] conjunctions;

        public Sql_ListOfConditions(Sql_Condition firestCondition, Sql_Condition[] conditions, String[] conjunctions)
        {
            this.conditions = conditions;
            this.conjunctions = conjunctions;
        }
    }


    public class Sql_Agggregation
    {

    }
}
