using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    class Table
    {

        public Table(List<string> TableAttributesOrder, Dictionary<string, TableAttributeInfo> TableAttributesInfo)
        {
            this.TableAttributesOrder = TableAttributesOrder;
            this.TableAttributesInfo = TableAttributesInfo;
        }

        public int insert(Dictionary<string,dynamic> turbel)
        {
            //retrive each attribute in table
            foreach(KeyValuePair<string, TableAttributeInfo> infoPair in TableAttributesInfo)
            {
                String name = infoPair.Key;
                TableAttributeInfo info = infoPair.Value;
                
                //check is every value in attribute is defined in turbel
                //if not replace by default value
                if (!turbel.ContainsKey(name))
                {
                    if(info.isPrimery) return -4;
                    else
                    {
                        switch(info.type)
                        {
                            case "String":
                                turbel.Add(name,"N/A");
                                break;
                            case "Int32":
                                turbel.Add(name, 0);
                                break;
                        }
                    }
                }
                else
                {
                    //Check format legality
                    dynamic value = turbel[name];
                    if (info.isPrimery)
                    {
                        if (keyRepeatTimes.ContainsKey(value) && keyRepeatTimes[value] > 0) return -1; //primary key duplicated
                        else keyRepeatTimes.Add(value, 1);
                    }
                    if (info.type == "String")
                    {
                        if (value.Length > info.maxStringLength) return -2;    //varchar is too short
                    }
                    if (value.GetType().Name != info.type)
                    {
                        return -3;  //type is incorrected
                    }
                }
                
            }

            //because map is not order garented, so we need a list defined the order in database
            List<dynamic> row_data = new List<dynamic>();
            foreach(string s in TableAttributesOrder)
            {
                row_data.Add(turbel[s]);
            }
            //Check Success, Add in database
            Guid guid = Guid.NewGuid();
            data.Add(guid,row_data);

            return 1;   //Success
        }

        public Dictionary<Guid, List<dynamic>> getTableData()
        {
            return data;
        }

        private Dictionary<string,TableAttributeInfo> TableAttributesInfo = new Dictionary<string,TableAttributeInfo>(10);
        private List<string> TableAttributesOrder = new List<string>(10);
        private Dictionary<Guid,List<dynamic>> data = new Dictionary<Guid, List<dynamic>>(1000000);
        private Dictionary<dynamic, int> keyRepeatTimes = new Dictionary<dynamic, int>(1000000);
    }
}
