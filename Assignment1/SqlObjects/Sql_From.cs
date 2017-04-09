using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1.SqlObjects
{
    public class Sql_Select_Table
    {
        public String name;
        public Boolean hasAlias;
        public String alias;
        public Sql_Select_Table(String name, Boolean hasAlias, String alias)
        {
            //
            this.name = name;
            
            this.hasAlias = hasAlias;
            this.alias = (this.hasAlias) ? alias : name;

        }
        public override string ToString()
        {
            String output = String.Format("    (Table) {0} {1}\n",
                                          (hasAlias) ?  alias: name, 
                                          ": " + name + " ");
            return output;
        }
    }
    public class Sql_From
    {
        public List<Sql_Select_Table> tables;
        public Sql_From(List<Sql_Select_Table> tables)
        {
            //
            this.tables = tables;
        }
        public override string ToString()
        {
            String output = "[From]: \n";
            foreach(Sql_Select_Table table in tables){
                output += table.ToString();
            }
            return output;
        }

    }
}
