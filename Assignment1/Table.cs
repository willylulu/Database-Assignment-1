﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    enum InstructionResult
    {
        SUCCESS,
        PRIMARY_KEY_DUPLICATE,
        VARCHAR_TOO_SHORT,
        INCORRECT_TYPE,
        NULL_PRIMARY_KEY,
        TABLE_NAME_DUPLICATE,
    }

    static class Constants
    {
        public const int MAX_ATTR_NUM = 10;
        public const int DEFAULT_SPACE = 1000000;
        public const int DEFAULT_SPACE_SM = 100000;
        public const int DEFAULT_SPACE_SM_COMP = 10;
    }

    class nullEle
    {
        override public string ToString()
        {
            return "(null)";
        }
    }

    [Serializable]
    class Table
    {

        public Table(List<string> TableAttributesOrder, Dictionary<string, TableAttribute> TableAttributes)
        {
            this.TableAttributesOrder = TableAttributesOrder;
            this.TableAttributes= TableAttributes;
            foreach(string s in TableAttributesOrder)
            {
                attribIndex.Add(s,new Dictionary<dynamic, HashSet<Guid>>(Constants.DEFAULT_SPACE_SM));
                indexingLookupTable.Add(s,false);
                attriSortedIndex.Add(s,new BPlusTree());
            }
        }

        public InstructionResult insert(Dictionary<string,dynamic> tuple)
        {
            //retrive each attribute in table
            foreach(KeyValuePair<string, TableAttribute> infoPair in TableAttributes)
            {
                String name = infoPair.Key;
                TableAttribute info = infoPair.Value;
                //check is every value in attribute is defined in tuple
                //if not replace by default value
                if (!tuple.ContainsKey(name))
                {
                    if(info.isPrimary) return InstructionResult.NULL_PRIMARY_KEY;   //primary key can not be null
                    else
                    {
                        tuple.Add(name, new nullEle());
                    }
                }
                else
                {
                    //Check format legality
                    dynamic value = tuple[name];
                    if (info.isPrimary)
                    {
                        if (attribIndex[name].ContainsKey(value)) return InstructionResult.PRIMARY_KEY_DUPLICATE; //primary key duplicated
                    }
                    if (info.type == "String")
                    {
                        //Console.WriteLine("Attrname = " + name);
                        if (value.Length > info.maxStringLength) return InstructionResult.VARCHAR_TOO_SHORT;    //varchar is too short
                    }
                    if (value.GetType().Name != info.type)
                    {
                        return InstructionResult.INCORRECT_TYPE;  //type is incorrected
                    }
                }
                
            }

            //Check Success, Add in database
            //because map is not order garented, so we need a list defined the order in database
            List<dynamic> row_data = new List<dynamic>(Constants.MAX_ATTR_NUM);
            Guid guid = Guid.NewGuid();
            foreach (string s in TableAttributesOrder)
            {
                row_data.Add(tuple[s]);

                //let the every element in tuple put in the attribIndex for selection
                setAttribIndex(s,tuple[s],guid);
            }

            dataKeys.Add(guid);
            data.Add(guid,row_data);

            return InstructionResult.SUCCESS;   //Success
        }

        public List<string> getAttributesOrder()
        {
            return this.TableAttributesOrder;
        }

        public Dictionary<Guid, List<dynamic>> getTableData()
        {
            return data;
        }

        private void setAttribIndex(string name, dynamic value, Guid address)
        {
            if (!attribIndex[name].ContainsKey(value))
            {
                HashSet<Guid> temp = new HashSet<Guid>();
                attribIndex[name].Add(value, temp);
                if(value.GetType().ToString()=="System.Int32" && isAttrIndexing(name))
                    attriSortedIndex[name].insert(value);
            }
            attribIndex[name][value].Add(address);
        }

        public HashSet<Guid> getAttribIndex(string name, dynamic value)
        {
            return attribIndex[name][value];
        }

        public Dictionary<dynamic,HashSet<Guid>>.KeyCollection getAttribIndexKeys(string name)
        {
            return attribIndex[name].Keys;
        }

        public bool isAttribIndexContains(string name, dynamic value)
        {
            return attribIndex[name].ContainsKey(value);
        }

    

        public Dictionary<string, Dictionary<dynamic, HashSet<Guid>>> getAllAttribIndex()
        {
            return attribIndex;
        }

        public HashSet<Guid> getAllIndex()
        {
            return dataKeys;
        }

        public Dictionary<string, TableAttribute> getTableAttributes()
        {
            return TableAttributes;
        }

        public dynamic getTableOnlyOneData(Guid address, string attribute)
        {
            int index = TableAttributesOrder.IndexOf(attribute);
            if (index == -1)
            {
                return new nullEle();
            }
            else
            {
                return data[address][index];
            }
        }

        public bool hasAttr( string attrName)
        {
            foreach(string tableAttr in TableAttributesOrder)
            {
                if( attrName.CompareTo(tableAttr) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public void turnOnIndexing(string name)
        {
            if (!indexingLookupTable[name])
            {
                indexingLookupTable[name] = true;
                foreach (KeyValuePair<dynamic, HashSet<Guid>> k in attribIndex[name])
                {
                    attriSortedIndex[name].insert(k.Key);
                }
            }
        }

        public bool isAttrIndexing(string name)
        {
            return indexingLookupTable[name];
        }

        public HashSet<dynamic> getBoundinfSet(string name, dynamic value, Operators oper)
        {
            HashSet<dynamic> h = new HashSet<dynamic>();
            if (oper == Operators.greater)
            {
                attriSortedIndex[name].findLowerBound(h,value);
            }
            else
            {
                attriSortedIndex[name].findUpperBound(h, value);
            }
            return h;
        }

        private List<string> TableAttributesOrder = new List<string>(Constants.MAX_ATTR_NUM);
        private Dictionary<string,TableAttribute> TableAttributes= new Dictionary<string,TableAttribute>(Constants.MAX_ATTR_NUM);
        private HashSet<Guid> dataKeys = new HashSet<Guid>();
        private Dictionary<Guid,List<dynamic>> data = new Dictionary<Guid, List<dynamic>>(Constants.DEFAULT_SPACE);
        private Dictionary<string, Dictionary<dynamic, HashSet<Guid>>> attribIndex = new Dictionary<string, Dictionary<dynamic, HashSet<Guid>>>(Constants.MAX_ATTR_NUM);
        private Dictionary<string, bool> indexingLookupTable = new Dictionary<string, bool>();
        private Dictionary<string, BPlusTree> attriSortedIndex = new Dictionary<string, BPlusTree>();
    }
}
