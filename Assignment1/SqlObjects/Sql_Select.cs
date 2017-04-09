using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;


namespace Assignment1.SqlObjects
{

    public class Sql_Select
    {
        private List<Sql_Select_Attr> attrs;
        private Sql_From from;   
        private Sql_Where where;  
        private Sql_Select_Table[] tables;          //tables from from
        private Sql_ListOfConditions conditions;    //conditions from where

        public Sql_Select(List<Sql_Select_Attr>attrs, Sql_From from, Sql_Where where)
        {
            this.attrs = attrs;
            this.from = from;
            this.where = where;

            //find the table that match's attr's tableAlias 
            //by using attr.setTable(Table)
            this.tables = this.from.tables.ToArray();
            setAttrsSelect();

            if(!this.where.isEmpty)     //if no where clause, dont do
                setConditionSelect(this.tables);
        }

        public void setAttrsSelect()
        {
            foreach (Sql_Select_Attr attr in attrs)
                attr.setTable(getMatchedTable(attr));

        }
        public void setConditionSelect(Sql_Select_Table[] tables)
        {
            this.conditions = this.where.listOfConditions;
            this.conditions.setAttrsTable(tables);
        }

        public Sql_Select_Table getMatchedTable(Sql_Select_Attr attr)
        {
            foreach (Sql_Select_Table table in tables)
            {
                ////
                if (table.alias.Equals(attr.tableAlias) && attr.tableAlias != null)
                {
                    ////
                    return table;
                }
            }
            return null;

        }

        public override string ToString()
        {
            String output = "";
            output += "[Select] \n";
            foreach(Sql_Select_Attr attr in attrs)
            {
                output += attr.ToString() ;
            }
            output += from.ToString() ;

            if (!where.isEmpty)
                output += where.ToString();

            output += "\n\n";
            return output;
        }

    }


    public class Sql_Select_Attr
    {
        public Sql_Select_Table table;
        public String tableAlias;
        public String name;
        public Boolean hasTable;

        public Aggregation aggregation;
        public Boolean hasAggregation;
        public Sql_Select_Attr(String val1, String val2, Boolean hasTable, String aggregation, Boolean hasAggregation)
        {
            this.hasAggregation = hasAggregation;
            if (this.hasAggregation)
            {
                if (aggregation.ToLower().Equals(Aggregation.count.ToString()))
                    this.aggregation = Aggregation.count;
                else if (aggregation.ToLower().Equals(Aggregation.sum.ToString()))
                    this.aggregation = Aggregation.sum;
                else
                    throw new DbException.UnkownKeyword("Unkown aggregation function: " + aggregation);
            }
            if (hasTable)
            {
                //if has Table, ex:  val.val2
                this.name = val2.Replace(".", "");  //replace the first dot
                this.tableAlias = val1;
            }
            else
                //else: val
                this.name = val1;
            
        }

        public void setTable(Sql_Select_Table table)
        {
            this.table = table;
        }

        public override string ToString()
        {
            String output = String.Format("    (Attr) {0}  {1}{2}\n",
                                          (hasAggregation)? aggregation+"("+name+")" : name, 
                                          (hasTable) ? "in " + tableAlias : "", 
                                          (hasTable) ? " ("+table.name+")" : "");
            return output;
        }
    }



}
