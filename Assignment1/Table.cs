using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class Table
    {
        public Table(string name)
        {
            this.name = name;
        }

        public Table(string name, List<TableAttributeInfo> TableAttributesInfo)
        {
            this.name = name;
            this.TableAttributesInfo = TableAttributesInfo;
        }

        public void insert(List<dynamic> element)
        {
            data.Add(element);
        }

        public List<List<dynamic>> getTableData()
        {
            return data;
        }

        private string name;
        private List<TableAttributeInfo> TableAttributesInfo;
        private List<List<dynamic>> data = new List<List<dynamic>>();
    }
}
