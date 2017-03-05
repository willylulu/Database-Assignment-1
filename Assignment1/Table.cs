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

        public int insert(List<dynamic> element)
        {
            //Check format legality
            for (int i=0;i<TableAttributesInfo.Count;i++)
            {
                TableAttributeInfo info = TableAttributesInfo[i];
                if(info.isPrimery)
                {
                    if (keyRepeatTimes[element[i]] > 0) return -1; //primary key duplicated
                }
                if(element[i].GetType().Name == "String")
                {
                    if (element[i].Length > info.maxStringLength) return -2;    //varchar is too short
                }
            }

            //Check Success, Add in database
            data.Add(element);
            //Add in Dictionary(map) to check for primary key
            foreach(dynamic o in element)
            {
                if (keyRepeatTimes.ContainsKey(o)) keyRepeatTimes[o] = keyRepeatTimes[o] + 1;
                else keyRepeatTimes.Add(o, 1);
            }

            return 1;   //Success
        }

        public List<List<dynamic>> getTableData()
        {
            return data;
        }

        private string name;
        private List<TableAttributeInfo> TableAttributesInfo = new List<TableAttributeInfo>();
        private List<List<dynamic>> data = new List<List<dynamic>>();
        private Dictionary<dynamic, int> keyRepeatTimes = new Dictionary<dynamic, int>();
    }
}
