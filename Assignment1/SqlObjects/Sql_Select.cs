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
        public List<Sql_Select_Attr> attrs;
        public Sql_From from;
        public Sql_Where where;  

        public Sql_Select_Table[] Tables;          //tables from from
        public Sql_ListOfConditions Conditions;    //conditions from where

        public String errorMsg = "";

        public Sql_Select(List<Sql_Select_Attr>attrs, Sql_From from, Sql_Where where)
        {
            this.attrs = attrs;
            this.from = from;
            this.where = where;

            //find the table that match's attr's tableAlias 
            //by using attr.setTable(Table)
            this.Tables = this.from.tables.ToArray();
            setAttrsSelect();

            if(!this.where.isEmpty)     //if no where clause, dont do
                setConditionSelect(this.Tables);

            if(testHasDuplicateAlias())
                throw new DbException.InvalidKeyword(errorMsg);
        }

        private void setAttrsSelect()
        {
            foreach (Sql_Select_Attr attr in attrs)
                attr.setTable(getMatchedTableOrRaiseException(attr));
        }
        private void setConditionSelect(Sql_Select_Table[] tables)
        {
            this.Conditions = this.where.listOfConditions;
            this.Conditions.setAttrsTable(tables);
        }
        private Sql_Select_Table getMatchedTableOrRaiseException(Sql_Select_Attr attr)
        {
            if (attr.tableAlias == null)
                return null;
            foreach (Sql_Select_Table table in Tables)
                if (table.alias.Equals(attr.tableAlias) && attr.tableAlias != null)
                    return table;

            throw new DbException.InvalidKeyword("Invalid key error - table alias " + attr.tableAlias + " doesn't exist");
            
        }
        private bool testHasDuplicateAlias()
        {
            foreach (Sql_Select_Table table in Tables)
            {
                foreach(Sql_Select_Table table_compare in Tables)
                {
                    if (table_compare == table)
                        continue;
                    if (table.alias.Equals(table_compare.alias))
                    {
                        errorMsg = "Invalid key error - there are duplicated table aliases: "
                                                            + table.alias;
                        return true;
                    }
                }
            }

            return false;
        }
        public override string ToString()
        {
            String output = "";
            output += "[Select] \n";
            foreach(Sql_Select_Attr attr in attrs)
            {
                output += attr.ToString() +"\n";
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

            this.hasTable = hasTable;
            this.hasAggregation = hasAggregation;
            if (this.hasAggregation)
                this.aggregation = getMatchedAggregatioinOrRaiseException(aggregation);

            if (hasTable)       //have parsed a table from attr
            {
                //if has Table, ex:  val.val2
                this.name = val2;  //replace the first dot
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
            String output = String.Format("    (Attr) {0}  {1}{2}",
                                          (hasAggregation)? aggregation+"("+name+")" : name, 
                                          (hasTable) ? "in " + tableAlias : "", 
                                          (hasTable) ? " ("+table.name+")" : "");
            return output;
        }
        
        private Aggregation getMatchedAggregatioinOrRaiseException(String str)
        {
            str = str.ToLower();
            Console.WriteLine(str);
            if (str.Equals(Aggregation.count.ToString()))
                return Aggregation.count;
            else if (str.Equals(Aggregation.sum.ToString()))
                return Aggregation.sum;
            else
                throw new DbException.UnkownKeyword("Unkown aggregation function: " + aggregation);
        }
    }



}
