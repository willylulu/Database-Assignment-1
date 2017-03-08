using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    enum InstructionResult
    {
        success,
        primaryKeyDuplicate,
        varcharTooShort,
        incorrectType,
        nullPrimaryKey
    }

    class Table
    {

        public Table(List<string> TableAttributesOrder, Dictionary<string, TableAttributeInfo> TableAttributesInfo)
        {
            this.TableAttributesOrder = TableAttributesOrder;
            this.TableAttributesInfo = TableAttributesInfo;
            foreach(string s in TableAttributesOrder)
            {
                attribIndex.Add(s,new Dictionary<dynamic, List<Guid>>());
            }
        }

        public InstructionResult insert(Dictionary<string,dynamic> turbel)
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
                    if(info.isPrimery) return InstructionResult.nullPrimaryKey;   //primary key can not be null
                    else
                    {
                        switch(info.type)
                        {
                            case TableAttributeInfo.varchar:
                                turbel.Add(name,string.Empty);
                                break;
                            case TableAttributeInfo.integer:
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
                        if (keyRepeatTimes.ContainsKey(value) && keyRepeatTimes[value] > 0) return InstructionResult.primaryKeyDuplicate; //primary key duplicated
                        else keyRepeatTimes.Add(value, 1);
                    }
                    if (info.type == "String")
                    {
                        if (value.Length > info.maxStringLength) return InstructionResult.varcharTooShort;    //varchar is too short
                    }
                    if (value.GetType().Name != info.type)
                    {
                        return InstructionResult.incorrectType;  //type is incorrected
                    }
                }
                
            }

            //Check Success, Add in database
            //because map is not order garented, so we need a list defined the order in database
            List<dynamic> row_data = new List<dynamic>();
            Guid guid = Guid.NewGuid();
            foreach (string s in TableAttributesOrder)
            {
                row_data.Add(turbel[s]);

                //let the every element in turbel put in the attribIndex for selection
                if (!attribIndex[s].ContainsKey(turbel[s]))
                    attribIndex[s].Add(turbel[s],new List<Guid>());
                attribIndex[s][turbel[s]].Add(guid);
            }

            data.Add(guid,row_data);

            return InstructionResult.success;   //Success
        }

        public Dictionary<Guid, List<dynamic>> getTableData()
        {
            return data;
        }

        private List<string> TableAttributesOrder = new List<string>(10);
        private Dictionary<string,TableAttributeInfo> TableAttributesInfo = new Dictionary<string,TableAttributeInfo>(10);
        private Dictionary<dynamic, int> keyRepeatTimes = new Dictionary<dynamic, int>(1000000);
        private Dictionary<Guid,List<dynamic>> data = new Dictionary<Guid, List<dynamic>>(1000000);
        private Dictionary<string, Dictionary<dynamic, List<Guid>>> attribIndex = new Dictionary<string, Dictionary<dynamic, List<Guid>>>(10);
    }
}
