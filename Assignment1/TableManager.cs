﻿using Assignment1.SqlObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment1
{
    public enum Operators
    {
        equal, not_equal, less, greater, none//add by fong 
    }
    public enum OperatorLink
    {
        OR, AND
    }

    public enum OperatorsType
    {
        attr2attr, attr2constant, constant2constant, onlyOne
    }
    public enum Aggregation
    {
        count, sum
    }
    class table_attribute_pair
    {
        public string tableName;
        public string Name;
        public string attributeName;
        public table_attribute_pair(string table, string attr)
        {
            tableName = table;
            attributeName = attr;
        }
    }

    /*
     * where attr1 = value1 and attr2 > value2 or attr3 < value3
     * ----------------------------------------------------------
     * image that => where (and attr1 == value1) (and attr2 > value2) (or attr3 < value3)
     * where w1 = new where(attr1,Operators.equal,value1,OperatorLink.AND);
     * where w2 = new where(attr2,Operators.greater,value2,OperatorLink.AND);
     * where w3 = new where(attr3,Operators.less,value3,OperatorLink.OR);
     */
    class where
    {
        public Operators oper;
        public OperatorsType operType;
        public KeyValuePair<string, string> tableAttrPair1;
        public KeyValuePair<string, string> tableAttrPair2;
        public dynamic con1 = null;
        public dynamic con2 = null;
        public OperatorLink operLink;

        public HashSet<Dictionary<string, Guid>> elementSet;
        public List<string> attrName; // store table name in elementSet

        public where(string table1, string attr1, OperatorsType OperatorsType)
        {
            this.tableAttrPair1 = new KeyValuePair<string, string>(table1, attr1);
            con1 = null;
            this.operType = OperatorsType;
        }

        public where(dynamic con1, OperatorsType OperatorsType)
        {
            this.con1 = con1;
            this.operType = OperatorsType;
        }

        public where(HashSet<Dictionary<string, Guid>> elementSet, List<string> attrName)
        {
            this.elementSet = elementSet;
            this.attrName = attrName;
        }
        public where(where preWhere)
        {
            oper = preWhere.oper;
            operLink = preWhere.operLink;
            tableAttrPair1 = preWhere.tableAttrPair1;
            tableAttrPair2 = preWhere.tableAttrPair2;
        }
        public void setElementSet(HashSet<Dictionary<string, Guid>> elementSet)
        {
            this.elementSet = elementSet;
        }

        public where(string table1, string attr1, string table2, string attr2, Operators op, OperatorLink opLink, OperatorsType opType)
        {
            operType = opType;
            oper = op;
            operLink = opLink;
            tableAttrPair1 = new KeyValuePair<string, string>(table1, attr1);
            tableAttrPair2 = new KeyValuePair<string, string>(table2, attr2);
        }

        public where(dynamic con1, dynamic con2, Operators op, OperatorLink opLink, OperatorsType opType)
        {
            this.con1 = con1;
            this.con2 = con2;
            oper = op;
            operLink = opLink;
            operType = opType;
        }

        public where(string table1, string attr1, dynamic con1, Operators op, OperatorLink opLink, OperatorsType opType)
        {
            tableAttrPair1 = new KeyValuePair<string, string>(table1, attr1);
            this.con1 = con1;
            oper = op;
            operLink = opLink;
            operType = opType;
        }

    }

    class outputPair
    {
        public string tableName;
        public string aliName;
        public string attr;
        public bool hasTable;

        public Aggregation aggregation;
        public Boolean hasAggregation;

        public outputPair(string aliName, string tableName, string attr, bool hasTable, Aggregation aggregation, Boolean hasAggregation)
        {
            this.aliName = aliName;
            this.tableName = tableName;
            this.attr = attr;
            this.hasTable = hasTable;
            this.aggregation = aggregation;
            this.hasAggregation = hasAggregation;
        }
    }

    [Serializable]
    class TableManager
    {
        public InstructionResult createTable(string name, List<string> TableAttributesOrder, Dictionary<string, TableAttribute> TableAttributes)
        {
            if (tables.ContainsKey(name)) return InstructionResult.TABLE_NAME_DUPLICATE;    //table name duplicated
            else
            {
                tables.Add(name, new Table(TableAttributesOrder, TableAttributes));
                ParserTest.println("Success");
                return InstructionResult.SUCCESS;   //Success
            }
        }

        public void insert(string name, Dictionary<string, dynamic> ele)
        {
            if (tables.ContainsKey(name) == true)
            {
                InstructionResult res = tables[name].insert(ele);
                if (res != InstructionResult.SUCCESS)
                {
                    string errorString;
                    switch (res)
                    {
                        case InstructionResult.PRIMARY_KEY_DUPLICATE:
                            errorString = "Error : Primary key duplicated";
                            break;
                        case InstructionResult.VARCHAR_TOO_SHORT:
                            errorString = "Error : Varchar is too short";
                            break;
                        case InstructionResult.INCORRECT_TYPE:
                            errorString = "Error : Type is incorrected";
                            break;
                        case InstructionResult.NULL_PRIMARY_KEY:
                            errorString = "Error : Primary key can not be null";
                            break;
                        default:
                            errorString = "Error : Unknown error";
                            break;
                    }
                    Console.WriteLine(errorString);
                }
                else
                {
                    ParserTest.println("Success");
                }
            }
            else
            {
                string errorString = "Error : Table is not exist";
                Console.WriteLine(errorString);
            }
        }

        public string attrToTable(string attrName, SqlObjects.Sql_Select sqlSelect)
        {
            foreach (SqlObjects.Sql_Select_Table table in sqlSelect.from.tables)
            {
                Table exTable = getTable(table.name);

                if (exTable.hasAttr(attrName) == true)
                {
                    Console.WriteLine(table.hasAlias ? table.alias : table.name);
                    return table.hasAlias ? table.alias : table.name;
                }
            }
            return null;

            //need to handle error
        }
        public void parseToSelect(SqlObjects.Sql_Select sqlSelect)
        {
            this.aliaName = new Dictionary<string, string>();
            /*
             * Add alia/tablename pair in DIC
             */
            foreach (SqlObjects.Sql_Select_Table table in sqlSelect.from.tables)
            {
                if ((table.hasAlias))
                {
                    if (!aliaName.ContainsKey(table.alias))
                    {
                        aliaName.Add(table.alias, table.name);
                    }
                }

                if (!aliaName.ContainsKey(table.name))
                {
                    aliaName.Add(table.name, table.name);
                }

            }

            List<outputPair> outputOrderList = new List<outputPair>();
            outputPair[] outputOrder = new outputPair[sqlSelect.attrs.Count];
            for (int i = 0; i < sqlSelect.attrs.Count; i++)
            {
                if (sqlSelect.attrs[i].name.CompareTo("*") == 0 && sqlSelect.attrs[i].hasAggregation == false)
                {
                    if (sqlSelect.attrs[i].hasTable == false)
                    {
                        foreach (SqlObjects.Sql_Select_Table table in sqlSelect.from.tables)
                        {
                            List<string> order = getTable(table.name).getAttributesOrder();

                            foreach (string att in order)
                            {
                                outputOrderList.Add(new outputPair((table.hasAlias ? table.alias : table.name), table.name, att, false, sqlSelect.attrs[i].aggregation, sqlSelect.attrs[i].hasAggregation));
                            }
                        }
                    }
                    else
                    {
                        List<string> order = getTable(sqlSelect.attrs[i].table.name).getAttributesOrder();

                        foreach (string att in order)
                        {
                            outputOrderList.Add(new outputPair((sqlSelect.attrs[i].table.hasAlias ? sqlSelect.attrs[i].table.alias : sqlSelect.attrs[i].table.name), sqlSelect.attrs[i].table.name, att, true, sqlSelect.attrs[i].aggregation, sqlSelect.attrs[i].hasAggregation));
                        }
                    }

                }
                else
                {
                    if (sqlSelect.attrs[i].hasTable)
                    {
                        outputOrderList.Add(new outputPair(sqlSelect.attrs[i].tableAlias, sqlSelect.attrs[i].table.name, sqlSelect.attrs[i].name, sqlSelect.attrs[i].hasTable, sqlSelect.attrs[i].aggregation, sqlSelect.attrs[i].hasAggregation));
                    }
                    else
                    {
                        if (sqlSelect.attrs[i].name == "*" && sqlSelect.attrs[i].hasAggregation)
                        {
                            //outputOrderList.Add(new outputPair(sqlSelect.attrs[i].tableAlias, aliaName[sqlSelect.attrs[i].tableAlias], sqlSelect.attrs[i].name, false, sqlSelect.attrs[i].aggregation, sqlSelect.attrs[i].hasAggregation));
                            //outputOrder = outputOrderList.ToArray();
                            isAllstarCount = true;
                        }
                        else if (attrToTable(sqlSelect.attrs[i].name, sqlSelect) != null)
                        {
                            sqlSelect.attrs[i].tableAlias = attrToTable(sqlSelect.attrs[i].name, sqlSelect);
                            outputOrderList.Add(new outputPair(sqlSelect.attrs[i].tableAlias, aliaName[sqlSelect.attrs[i].tableAlias], sqlSelect.attrs[i].name, sqlSelect.attrs[i].hasTable, sqlSelect.attrs[i].aggregation, sqlSelect.attrs[i].hasAggregation));
                        }
                    }
                }
            }
            outputOrder = outputOrderList.ToArray();
            if (!sqlSelect.where.isEmpty)
            {
                where[] tables = new where[sqlSelect.where.listOfConditions.conditionNum];
                if (sqlSelect.where.listOfConditions.conditionNum >= 1)
                {
                    SqlObjects.Sql_Operand leftOp = sqlSelect.where.listOfConditions.firstCondition.leftOpd;

                    if (leftOp.type == OperandType.attr)
                    {
                        if (leftOp.content.hasTable == false)
                        {
                            leftOp.content.tableAlias = attrToTable(leftOp.content.name, sqlSelect);
                        }
                    }
                    SqlObjects.Sql_Operand rightOp = sqlSelect.where.listOfConditions.firstCondition.rightOpd;

                    if (sqlSelect.where.listOfConditions.firstCondition.opType != OperatorsType.onlyOne)
                    {
                        if (rightOp.type == OperandType.attr)
                        {
                            if (rightOp.content.hasTable == false)
                            {
                                rightOp.content.tableAlias = attrToTable(rightOp.content.name, sqlSelect);
                            }
                        }
                    }


                    if (sqlSelect.where.listOfConditions.firstCondition.opType == OperatorsType.attr2attr)
                    {
                        Sql_Condition condition = sqlSelect.where.listOfConditions.firstCondition;
                        tables[0] = new where(condition.leftOpd.content.tableAlias,
                                                 condition.leftOpd.content.name,
                                                 condition.rightOpd.content.tableAlias,
                                                 condition.rightOpd.content.name,
                                                 condition.op,
                                                 OperatorLink.AND,
                                                 OperatorsType.attr2attr);

                    }
                    else if (sqlSelect.where.listOfConditions.firstCondition.opType == OperatorsType.attr2constant)
                    {

                        Sql_Condition condition = sqlSelect.where.listOfConditions.firstCondition;
                        Console.WriteLine("Name = " + condition.leftOpd.content.tableAlias);

                        tables[0] = new where(condition.leftOpd.content.tableAlias,
                                                condition.leftOpd.content.name,
                                                condition.rightOpd.getOperand(),
                                                condition.op,
                                                OperatorLink.AND,
                                                OperatorsType.attr2constant);
                    }
                    else if (sqlSelect.where.listOfConditions.firstCondition.opType == OperatorsType.constant2constant)
                    {
                        Sql_Condition condition = sqlSelect.where.listOfConditions.firstCondition;
                        tables[0] = new where(condition.leftOpd.getOperand(),
                                                condition.rightOpd.getOperand(),
                                                condition.op,
                                                OperatorLink.AND,
                                                OperatorsType.constant2constant);
                    }
                    else if (sqlSelect.where.listOfConditions.firstCondition.opType == OperatorsType.onlyOne)
                    {
                        Sql_Condition condition = sqlSelect.where.listOfConditions.firstCondition;
                        if (condition.leftOpd.type == OperandType.attr)
                        {
                            tables[0] = new where(condition.leftOpd.content.tableAlias, condition.leftOpd.content.name, OperatorsType.onlyOne);
                        }
                        else
                        {
                            tables[0] = new where(condition.leftOpd.content, OperatorsType.onlyOne);
                        }

                    }
                }
                if (sqlSelect.where.listOfConditions.conditionNum == 2)
                {
                    SqlObjects.Sql_Operand leftOp = sqlSelect.where.listOfConditions.secondCondition.leftOpd;

                    if (leftOp.type == OperandType.attr)
                    {
                        if (leftOp.content.hasTable == false)
                        {
                            leftOp.content.tableAlias = attrToTable(leftOp.content.name, sqlSelect);
                        }
                    }

                    SqlObjects.Sql_Operand rightOp = sqlSelect.where.listOfConditions.secondCondition.rightOpd;
                    if (sqlSelect.where.listOfConditions.secondCondition.opType != OperatorsType.onlyOne)
                    {
                        if (rightOp.type == OperandType.attr)
                        {
                            if (rightOp.content.hasTable == false)
                            {
                                rightOp.content.tableAlias = attrToTable(rightOp.content.name, sqlSelect);
                            }
                        }
                    }
                    if (sqlSelect.where.listOfConditions.secondCondition.opType == OperatorsType.attr2attr)
                    {
                        Sql_Condition condition = sqlSelect.where.listOfConditions.secondCondition;
                        tables[1] = new where(condition.leftOpd.content.tableAlias,
                                                condition.leftOpd.content.name,
                                                condition.rightOpd.content.tableAlias,
                                                condition.rightOpd.content.name,
                                                condition.op,
                                                (String.Compare(sqlSelect.where.listOfConditions.conjunction.ToLower(), "and", true) == 0 ? OperatorLink.AND : OperatorLink.OR),
                                                OperatorsType.attr2attr);

                    }
                    else if (sqlSelect.where.listOfConditions.secondCondition.opType == OperatorsType.attr2constant)
                    {

                        Sql_Condition condition = sqlSelect.where.listOfConditions.secondCondition;
                        tables[1] = new where(condition.leftOpd.content.tableAlias,
                                                condition.leftOpd.content.name,
                                                condition.rightOpd.getOperand(),
                                                condition.op,
                                                 (String.Compare(sqlSelect.where.listOfConditions.conjunction.ToLower(), "and", true) == 0 ? OperatorLink.AND : OperatorLink.OR),
                                                OperatorsType.attr2constant);
                    }
                    else if (sqlSelect.where.listOfConditions.secondCondition.opType == OperatorsType.constant2constant)
                    {
                        Sql_Condition condition = sqlSelect.where.listOfConditions.secondCondition;
                        tables[1] = new where(condition.leftOpd.getOperand(),
                                                condition.rightOpd.getOperand(),
                                                condition.op,
                                                (String.Compare(sqlSelect.where.listOfConditions.conjunction.ToLower(), "and", true) == 0 ? OperatorLink.AND : OperatorLink.OR),
                                                OperatorsType.constant2constant);
                    }
                    else if (sqlSelect.where.listOfConditions.secondCondition.opType == OperatorsType.onlyOne)
                    {
                        Sql_Condition condition = sqlSelect.where.listOfConditions.secondCondition;
                        if (condition.leftOpd.type == OperandType.attr)
                        {
                            tables[1] = new where(condition.leftOpd.content.tableAlias, condition.leftOpd.content.name, OperatorsType.onlyOne);
                        }
                        else
                        {
                            tables[1] = new where(condition.leftOpd.content, OperatorsType.onlyOne);
                        }
                    }

                }
                select(aliaName, tables, outputOrder);
            }
            else
            {
                select(aliaName, new where[0], outputOrder);
            }

        }

        public dynamic onlyoneOper(Dictionary<string, string> aliaName, where table)
        {
            if (table.con1 != null)
            {
                return table.con1 != 0;
            }
            else
            {
                return true;
            }
        }
        public HashSet<Dictionary<string, Guid>> attr2attrOper(Dictionary<string, string> aliaName, where table)
        {
            //Build a hashset to store ans
            HashSet<Dictionary<string, Guid>> elementSet = new HashSet<Dictionary<string, Guid>>();

            // Get 2 Tables
            Table table1 = getTable(aliaName[table.tableAttrPair1.Key]);
            Table table2 = getTable(aliaName[table.tableAttrPair2.Key]);
            // Get TableDatas from 2 tables
            Dictionary<Guid, List<dynamic>> attribIndex1 = table1.getTableData();
            Dictionary<Guid, List<dynamic>> attribIndex2 = table2.getTableData();
            // Get AllIndex form 2 tables
            HashSet<Guid> dataKeys1 = table1.getAllIndex();
            HashSet<Guid> dataKeys2 = table2.getAllIndex();
            // Set attribute index
            int index1;
            int index2;
            List<string> TableAttributesOrder1 = table1.getAttributesOrder();
            index1 = TableAttributesOrder1.FindIndex(x => x == table.tableAttrPair1.Value);
            List<string> TableAttributesOrder2 = table2.getAttributesOrder();
            index2 = TableAttributesOrder2.FindIndex(x => x == table.tableAttrPair2.Value);

            if (table.oper == Operators.equal)
            {

                HashSet<dynamic> ans = new HashSet<dynamic>();

                bool ll = dataKeys1.Count <= dataKeys2.Count;
                HashSet<Guid> tk1 = ll ? dataKeys1 : dataKeys2;
                HashSet<Guid> tk2 = ll ? dataKeys2 : dataKeys1;
                Table t1 = ll ? table1 : table2;
                Table t2 = ll ? table2 : table1;
                string ak1 = table.tableAttrPair1.Key;
                string ak2 = table.tableAttrPair2.Key;
                string at1 = ll ? table.tableAttrPair1.Value : table.tableAttrPair2.Value;
                string at2 = ll ? table.tableAttrPair2.Value : table.tableAttrPair1.Value;

                var tans =
                    from data1 in t1.getAttribIndexKeys(at1)
                    from data2 in t2.getAttribIndexKeys(at2)
                    where data1 == data2
                    select data1;

                foreach (var val1 in tans)
                {
                    foreach(Guid g1 in t1.getAttribIndex(at1, val1))
                    {
                        foreach(Guid g2 in t2.getAttribIndex(at2, val1))
                        {
                            Guid gg1 = ll ? g1 : g2;
                            Guid gg2 = ll ? g2 : g1;
                            Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                            aliasGidPair.Add(ak1, gg1);
                            if (!aliasGidPair.ContainsKey(ak2))
                            {
                                aliasGidPair.Add(ak2, gg2);
                            }
                            elementSet.Add(aliasGidPair);
                        }
                    }
                }
            }
            else if (table.oper == Operators.greater)
            {
                var ans =
                    from data1 in dataKeys1
                    from data2 in dataKeys2
                    where attribIndex1[data1][index1] > attribIndex2[data1][index2]
                    select new { d1 = data1, d2 = data2 };
                foreach (var dataPair in ans)
                {
                    Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                    aliasGidPair.Add(table.tableAttrPair1.Key, dataPair.d1);
                    if (!aliasGidPair.ContainsKey(table.tableAttrPair2.Key))
                    {
                        aliasGidPair.Add(table.tableAttrPair2.Key, dataPair.d2);
                    }
                    elementSet.Add(aliasGidPair);
                }
            }
            else if (table.oper == Operators.less)
            {
                var ans =
                    from data1 in dataKeys1
                    from data2 in dataKeys2
                    where attribIndex1[data1][index1] < attribIndex2[data1][index2]
                    select new { d1 = data1, d2 = data2 };
                foreach (var dataPair in ans)
                {
                    Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                    aliasGidPair.Add(table.tableAttrPair1.Key, dataPair.d1);
                    if (!aliasGidPair.ContainsKey(table.tableAttrPair2.Key))
                    {
                        aliasGidPair.Add(table.tableAttrPair2.Key, dataPair.d2);
                    }
                    elementSet.Add(aliasGidPair);
                }
            }
            else
            {
                var ans =
                    from data1 in dataKeys1
                    from data2 in dataKeys2
                    where attribIndex1[data1][index1] != attribIndex2[data1][index2]
                    select new { d1 = data1, d2 = data2 };
                foreach (var dataPair in ans)
                {
                    Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                    aliasGidPair.Add(table.tableAttrPair1.Key, dataPair.d1);
                    if (!aliasGidPair.ContainsKey(table.tableAttrPair2.Key))
                    {
                        aliasGidPair.Add(table.tableAttrPair2.Key, dataPair.d2);
                    }
                    elementSet.Add(aliasGidPair);
                }
            }
            return elementSet;

        }

        public bool stringEqualCheck(string s1, string s2)
        {
            //Console.WriteLine("s1 = " + s1 + " s2 = " + s2.Trim(new Char[] { '\'', '"' }));
            if (string.Compare(s1, s2.Trim(new Char[] { '\'', '"' }), true) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public HashSet<Dictionary<string, Guid>> attr2conOper(Dictionary<string, string> aliaName, where table)
        {
            //Build a hashset to store ans
            HashSet<Dictionary<string, Guid>> elementSet = new HashSet<Dictionary<string, Guid>>();

            // Get 2 Tables
            Table table1 = getTable(aliaName[table.tableAttrPair1.Key]);
            // Get TableDatas from 2 tables
            Dictionary<Guid, List<dynamic>> attribIndex1 = table1.getTableData();
            // Get AllIndex form 2 tables
            HashSet<Guid> dataKeys1 = table1.getAllIndex();
            // Set attribute index
            int index1;
            List<string> TableAttributesOrder1 = table1.getAttributesOrder();
            index1 = TableAttributesOrder1.FindIndex(x => x == table.tableAttrPair1.Value);
            if (table.oper == Operators.equal && table.con1.GetType() == typeof(int))
            {
                HashSet<Guid> ans = null;
                if (table1.isAttribIndexContains(table.tableAttrPair1.Value, table.con1))
                {
                    ans = table1.getAttribIndex(table.tableAttrPair1.Value, table.con1);
                }
                else
                {
                    ans = new HashSet<Guid>();
                }
                foreach (Guid g in ans)
                {
                    Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                    aliasGidPair.Add(table.tableAttrPair1.Key, g);
                    elementSet.Add(aliasGidPair);
                }
            }
            else if (table.oper == Operators.equal && table.con1.GetType() == typeof(string))
            {

                //var ans =
                //    from data1 in dataKeys1
                //    where stringEqualCheck(attribIndex1[data1][index1], table.con1)
                //    select new { d1 = data1 };
                HashSet<Guid> ans = null;
                if (table1.isAttribIndexContains(table.tableAttrPair1.Value, table.con1.Trim(new Char[] { '\'', '"' })))
                {
                    ans = table1.getAttribIndex(table.tableAttrPair1.Value, table.con1.Trim(new Char[] { '\'', '"' }));
                }
                else
                {
                    ans = new HashSet<Guid>();
                }
                foreach (Guid g in ans)
                {
                    Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                    aliasGidPair.Add(table.tableAttrPair1.Key, g);
                    elementSet.Add(aliasGidPair);
                }
            }
            else if (table.oper == Operators.greater)
            {
                if (table1.isAttrIndexing(table.tableAttrPair1.Value))
                {
                    //Console.WriteLine("A");
                    //HashSet<Guid> ans = table1.findBoundSet(table.tableAttrPair1.Value, table.con1, table.oper);
                    HashSet<dynamic> ans = table1.getBoundinfSet(table.tableAttrPair1.Value, table.con1, table.oper);
                    foreach (Guid dataPair in ans)
                    {
                        Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                        aliasGidPair.Add(table.tableAttrPair1.Key, dataPair);
                        elementSet.Add(aliasGidPair);
                    }
                }
                else
                {
                    var ans =
                        from data1 in dataKeys1
                        where attribIndex1[data1][index1] > table.con1
                        select new { d1 = data1 };
                    foreach (var dataPair in ans)
                    {
                        Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                        aliasGidPair.Add(table.tableAttrPair1.Key, dataPair.d1);
                        elementSet.Add(aliasGidPair);
                    }
                }


            }
            else if (table.oper == Operators.less)
            {
                if (table1.isAttrIndexing(table.tableAttrPair1.Value))
                {
                    //Console.WriteLine("A");
                    //HashSet<Guid> ans = table1.findBoundSet(table.tableAttrPair1.Value, table.con1, table.oper);
                    HashSet<dynamic> ans = table1.getBoundinfSet(table.tableAttrPair1.Value, table.con1, table.oper);
                    foreach (Guid dataPair in ans)
                    {
                        Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                        aliasGidPair.Add(table.tableAttrPair1.Key, dataPair);
                        elementSet.Add(aliasGidPair);
                    }
                }
                else
                {
                    var ans =
                        from data1 in dataKeys1
                        where attribIndex1[data1][index1] < table.con1
                        select new { d1 = data1 };
                    foreach (var dataPair in ans)
                    {
                        Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                        aliasGidPair.Add(table.tableAttrPair1.Key, dataPair.d1);
                        elementSet.Add(aliasGidPair);
                    }
                }
            }
            else if (table.oper == Operators.not_equal && table.con1.GetType() == typeof(int))
            {
                var ans =
                    from data1 in dataKeys1
                    where attribIndex1[data1][index1] != table.con1
                    select new { d1 = data1 };
                foreach (var dataPair in ans)
                {
                    Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                    aliasGidPair.Add(table.tableAttrPair1.Key, dataPair.d1);
                    elementSet.Add(aliasGidPair);
                }
            }
            else
            {
                var ans =
                    from data1 in dataKeys1
                    where !stringEqualCheck(attribIndex1[data1][index1], table.con1)
                    select new { d1 = data1 };
                foreach (var dataPair in ans)
                {
                    Dictionary<string, Guid> aliasGidPair = new Dictionary<string, Guid>();
                    aliasGidPair.Add(table.tableAttrPair1.Key, dataPair.d1);
                    elementSet.Add(aliasGidPair);
                }
            }
            return elementSet;

        }

        public bool checkAggr(outputPair[] outputOrder)
        {
            bool hasAggr = false;
            bool hasNonAggr = false;
            foreach (outputPair op in outputOrder)
            {
                if (op.hasAggregation)
                {
                    hasAggr = true;
                }
                if (!op.hasAggregation)
                {
                    hasNonAggr = true;
                }
            }

            if (hasNonAggr && hasAggr)
            {
                Console.WriteLine("error");
                return false;
            }
            else if (hasAggr)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void printAggr(outputPair[] outputOrder, HashSet<Dictionary<string, Guid>> data, HashSet<string> total)
        {
            List<KeyValuePair<string, int>> AggrOut = new List<KeyValuePair<string, int>>();

            if (isAllstarCount)
            {
                AggrOut.Add(new KeyValuePair<string, int>("Count(*)", allstarCount));
            }

            foreach (outputPair op in outputOrder)
            {

                string tablename = op.aliName;
                if (!op.hasAggregation)
                {
                    Console.WriteLine("Error: mix Aggeration wiht non-Aggr");
                    return;
                }
                int ans = 0;
                Table table = getTable(aliaName[tablename]);
                Dictionary<Guid, List<dynamic>> tabledata = table.getTableData();
                int index = 0;
                foreach (string at in table.getAttributesOrder())
                {
                    if (at == op.attr)
                    {
                        index = table.getAttributesOrder().IndexOf(at);
                        break;
                    }
                }
                switch (op.aggregation)
                {
                    case Aggregation.count:
                        if (total.Contains(tablename))
                        {
                            foreach (Dictionary<string, Guid> dic in data)
                            {
                                if (tabledata[dic[tablename]][index] != null)
                                {
                                    ans++;
                                }
                            }
                            AggrOut.Add(new KeyValuePair<string, int>("Count(" + op.attr + ")", (op.attr != "*" ? (int)ans : tabledata.Count)));

                        }
                        else
                        {
                            foreach (Guid id in table.getAllIndex())
                            {
                                if (tabledata[id][index] != null)
                                {
                                    ans++;
                                }
                            }
                            AggrOut.Add(new KeyValuePair<string, int>("Count(" + op.attr + ")", (op.attr != "*" ? (int)ans : tabledata.Count)));

                        }
                        break;
                    case Aggregation.sum:
                        try
                        {

                            if (total.Contains(tablename))
                            {
                                foreach (Dictionary<string, Guid> dic in data)
                                {
                                    if (tabledata[dic[tablename]][index] != null)
                                    {
                                        ans += tabledata[dic[tablename]][index];
                                    }
                                }
                                AggrOut.Add(new KeyValuePair<string, int>("Sum(" + op.attr + ")", (int)ans));

                            }
                            else
                            {
                                foreach (Guid id in table.getAllIndex())
                                {
                                    if (tabledata[id][index] != null)
                                    {
                                        ans += tabledata[id][index];
                                    }
                                }
                                AggrOut.Add(new KeyValuePair<string, int>("Sum(" + op.attr + ")", (int)ans));

                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Aggregation Sum with wrong type");
                            return;
                        }
                        break;
                }
            }

            System.IO.StreamWriter file = new System.IO.StreamWriter("../../output.csv");
            string attrOut = "";
            string valueOut = "";
            foreach (KeyValuePair<string, int> d in AggrOut)
            {
                attrOut += d.Key.ToString() + ",";
                valueOut += d.Value.ToString() + ",";

            }

            file.WriteLine(attrOut);
            file.WriteLine(valueOut);
            file.Close();


        }
        public void printSelect(outputPair[] outputOrder, HashSet<Dictionary<string, Guid>> data, HashSet<string> total)
        {



            //the final output format
            List<string> attribute = new List<string>();
            List<List<dynamic>> ans = new List<List<dynamic>>();

            //check the tables we need
            HashSet<string> tableDistinct = new HashSet<string>();
            List<string> tableList = new List<string>();
            HashSet<Dictionary<string, Guid>> exData = null;
            //find the table we need
            for (int i = 0; i < outputOrder.Length; i++)
            {
                attribute.Add(outputOrder[i].attr);
                if (!tableDistinct.Contains(outputOrder[i].aliName))
                {
                    tableDistinct.Add(outputOrder[i].aliName);
                    tableList.Add(outputOrder[i].aliName);
                }
            }

            //if no where use crossProduct add in exData
            if (data.Count == 0 && total.Count == 0)
            {
                //the final tuple pairs 
                exData = new HashSet<Dictionary<string, Guid>>();
                crossProductRecur(exData, new Dictionary<string, Guid>(), tableList, 0, tableList.Count);
            }
            else
            {
                //the table doesn't use where filter
                List<string> tableNoWhere = new List<string>();
                foreach (string s in tableList)
                {
                    if (!total.Contains(s)) tableNoWhere.Add(s);
                }
                exData = new HashSet<Dictionary<string, Guid>>();
                if (tableNoWhere.Count != 0)
                {
                    //need to use data cross product table which doesn't use where filter

                    foreach (Dictionary<string, Guid> d in data)
                    {
                        //the final tuple pairs 
                        crossProductRecur(exData, d, tableNoWhere, 0, tableNoWhere.Count);
                    }
                }
                else
                {
                    exData = data;
                }
            }

            //retrive data and add in list

            if (exData != null)
            {
                allstarCount = exData.Count;
                if (checkAggr(outputOrder) || isAllstarCount)
                {
                    printAggr(outputOrder, data, total);
                    return;
                }

                foreach (Dictionary<string, Guid> tuple in exData)
                {
                    List<dynamic> tmp = new List<dynamic>();
                    foreach (outputPair op in outputOrder)
                    {
                        //Console.WriteLine(op.tableName);
                        Table targetTable = getTable(op.tableName);
                        dynamic tt = targetTable.getTableOnlyOneData(tuple[op.aliName], op.attr);
                        tmp.Add(tt);
                    }
                    ans.Add(tmp);
                }
            }
            //final output

            System.IO.StreamWriter file = new System.IO.StreamWriter("../../output.csv");

            string attributeOutput = "";
            foreach (string s in attribute)
            {
                attributeOutput += s + ",";
            }
            file.WriteLine(attributeOutput);


            foreach (List<dynamic> s in ans)
            {
                string dataOutput = "";
                foreach (dynamic d in s)
                {
                    dataOutput += d.ToString() + ",";
                }
                file.WriteLine(dataOutput);
            }
            file.Close();
        }

        private void crossProductRecur(HashSet<Dictionary<string, Guid>> data, Dictionary<string, Guid> dictionary, List<string> tableList, int v, int count)
        {
            if (v == count)
            {
                data.Add(dictionary);
                return;
            }
            else
            {
                HashSet<Guid> tmp = getTable(aliaName[tableList[v]]).getAllIndex();
                foreach (Guid t in tmp)
                {
                    Dictionary<string, Guid> tmp2 = new Dictionary<string, Guid>();
                    tmp2.Add(tableList[v], t);
                    foreach (KeyValuePair<string, Guid> p in dictionary)
                    {
                        tmp2.Add(p.Key, p.Value);
                    }
                    crossProductRecur(data, tmp2, tableList, v + 1, count);
                }
            }
        }

        public bool con2conOper(Dictionary<string, string> aliaName, where table)
        {
            switch (table.oper)
            {
                case Operators.equal:
                    if (table.con1.GetType() == typeof(int))
                    {
                        return table.con1 == table.con2;
                    }
                    else
                    {
                        return (string.Compare(table.con1, table.con2, true) == 0 ? true : false);
                    }

                case Operators.greater:
                    return table.con1 > table.con2;

                case Operators.less:
                    return table.con1 < table.con2;

                case Operators.not_equal:
                    if (table.con1.GetType() == typeof(int))
                    {
                        return table.con1 != table.con2;
                    }
                    else
                    {
                        return (string.Compare(table.con1, table.con2, true) != 0 ? true : false);
                    }
            }
            return false;
        }

        public void select(Dictionary<string, string> aliaName, where[] tables, outputPair[] outputOrder)
        {
            //Dictionary<string, string> aliaNameDic = aliaName;

            HashSet<Dictionary<string, Guid>> selectAns = new HashSet<Dictionary<string, Guid>>();

            if (tables.Length == 0)
            {
                printSelect(outputOrder, selectAns, new HashSet<string>());
                return;
            }

            if (tables.Length == 1)
            {
                switch (tables[0].operType)
                {
                    case OperatorsType.attr2attr:
                        try
                        {
                            printSelect(outputOrder, attr2attrOper(aliaName, tables[0]), new HashSet<string> { tables[0].tableAttrPair1.Key, tables[0].tableAttrPair2.Key });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error Compare type error");
                            return;
                        }

                        return;
                    case OperatorsType.attr2constant:
                        try
                        {
                            printSelect(outputOrder, attr2conOper(aliaName, tables[0]), new HashSet<string> { tables[0].tableAttrPair1.Key });
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error Compare type error");
                            return;
                        }
                        return;
                    case OperatorsType.constant2constant:
                        if (con2conOper(aliaName, tables[0]))
                        {
                            try
                            {
                                printSelect(outputOrder, new HashSet<Dictionary<string, Guid>>(), new HashSet<string>());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error Compare type error");
                                return;
                            }
                            return;
                        }
                        else
                        {
                            return;
                        }
                    case OperatorsType.onlyOne:
                        if (onlyoneOper(aliaName, tables[0]).GetType() != typeof(bool))
                        {
                            printSelect(outputOrder, onlyoneOper(aliaName, tables[0]), new HashSet<string> { tables[0].tableAttrPair1.Key, tables[0].tableAttrPair2.Key });
                            return;
                        }
                        else
                        {
                            if (onlyoneOper(aliaName, tables[0]))
                            {
                                printSelect(outputOrder, new HashSet<Dictionary<string, Guid>>(), new HashSet<string>());
                                return;
                            }
                            else
                            {
                                return;
                            }
                        }
                }
            }
            //check where is attr or not
            bool isAttr1 = tables[0].operType != OperatorsType.constant2constant;
            bool isAttr2 = tables[1].operType != OperatorsType.constant2constant;

            HashSet<string> left = new HashSet<string>();
            HashSet<string> right = new HashSet<string>();
            HashSet<string> total = new HashSet<string>();
            if (isAttr1)
            {
                if (tables[0].tableAttrPair1.Key != null)
                {
                    left.Add(tables[0].tableAttrPair1.Key);
                    total.Add(tables[0].tableAttrPair1.Key);
                }
                if (tables[0].tableAttrPair2.Key != null)
                {
                    left.Add(tables[0].tableAttrPair2.Key);
                    total.Add(tables[0].tableAttrPair2.Key);
                }

            }
            if (isAttr2)
            {
                if (tables[1].tableAttrPair1.Key != null)
                {
                    right.Add(tables[1].tableAttrPair1.Key);
                    total.Add(tables[1].tableAttrPair1.Key);
                }
                if (tables[1].tableAttrPair2.Key != null)
                {
                    right.Add(tables[1].tableAttrPair2.Key);
                    total.Add(tables[1].tableAttrPair2.Key);
                }
            }

            List<dynamic> data = new List<dynamic>();
            Console.WriteLine("table size = " + tables.Length);
            for (int i = 0; i < tables.Length; i++)
            {
                switch (tables[i].operType)
                {
                    case OperatorsType.attr2attr:
                        try
                        {
                            data.Add(attr2attrOper(aliaName, tables[i]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error Compare type error");
                            return;
                        }
                        break;
                    case OperatorsType.attr2constant:
                        try
                        {
                            data.Add(attr2conOper(aliaName, tables[i]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error Compare type error");
                            return;
                        }
                        break;
                    case OperatorsType.constant2constant:
                        try
                        {
                            data.Add(con2conOper(aliaName, tables[i]));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error Compare type error");
                            return;
                        }
                        break;
                    case OperatorsType.onlyOne:
                        if (onlyoneOper(aliaName, tables[i]).GetType() != typeof(bool))
                        {
                            data.Add(onlyoneOper(aliaName, tables[i]));
                        }
                        else
                        {
                            if (onlyoneOper(aliaName, tables[i]))
                            {
                                data.Add(onlyoneOper(aliaName, tables[i]));
                            }
                            else
                            {
                                return;
                            }
                        }
                        break;
                }

            }

            // 2 conditions are const2const
            if (data[0].GetType() == typeof(Boolean) && data[1].GetType() == typeof(Boolean))
            {
                if (tables[1].operLink == OperatorLink.AND)
                {
                    if ((bool)data[0] && (bool)data[1])
                    {
                        printSelect(outputOrder, new HashSet<Dictionary<string, Guid>>(), new HashSet<string>());
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    if ((bool)data[0] || (bool)data[1])
                    {
                        printSelect(outputOrder, new HashSet<Dictionary<string, Guid>>(), new HashSet<string>());
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (data[0].GetType() != typeof(Boolean) && data[1].GetType() != typeof(Boolean))
            {
                //2 conditions are attr2attr

                if (tables[1].operLink == OperatorLink.AND)
                {
                    selectAns = intersect(data[0], left, data[1], right);
                }
                else if (tables[1].operLink == OperatorLink.OR)
                {
                    selectAns = union(aliaName, data[0], left, data[1], right);
                }

                printSelect(outputOrder, selectAns, total);

            }
            else if (data[0].GetType() == typeof(Boolean) && data[1].GetType() != typeof(Boolean))
            {
                //first condition = con2con
                //second condition != const2const

                if (tables[1].operLink == OperatorLink.AND && (bool)data[1] == false)
                {
                    return;
                }
                else
                {
                    printSelect(outputOrder, data[1], total);
                }
            }
            else if (data[0].GetType() != typeof(Boolean) && data[1].GetType() == typeof(Boolean))
            {
                //first condition != con2con
                //second condition = const2const

                if (tables[1].operLink == OperatorLink.AND && (bool)data[0] == false)
                {
                    return;
                }
                else
                {
                    printSelect(outputOrder, data[0], total);
                }
            }

        }

        class HashEqualityComparer : IEqualityComparer<Dictionary<string, Guid>>
        {

            public bool Equals(Dictionary<string, Guid> b1, Dictionary<string, Guid> b2)
            {

                foreach (KeyValuePair<string, Guid> kv in b1)
                {
                    if (!b2.ContainsKey(kv.Key))
                    {
                        return false;
                    }
                    if (kv.Value != b2[kv.Key])
                    {
                        return false;
                    }


                }
                return true;
            }
            public int GetHashCode(Dictionary<string, Guid> bx)
            {
                int hCode = bx.Count;
                return hCode.GetHashCode();
            }

        }

        private HashSet<Dictionary<string, Guid>> union(Dictionary<string, string> aliaName, HashSet<Dictionary<string, Guid>> elementSet1, HashSet<string> tableAttrHashSet1, HashSet<Dictionary<string, Guid>> elementSet2, HashSet<string> tableAttrHashSet2)
        {
            //Find duplicate table
            HashSet<string> dupAttr = new HashSet<string>();
            var ans =
                from d1 in tableAttrHashSet1
                from d2 in tableAttrHashSet2
                where d1 == d2
                select d1;

            foreach (string attr in ans)
            {
                dupAttr.Add(attr);
            }

            HashSet<Dictionary<string, Guid>> temp_eleSet1 = new HashSet<Dictionary<string, Guid>>();
            foreach (Dictionary<string, Guid> data in elementSet1)
            {
                Dictionary<string, Guid> temp_dic = new Dictionary<string, Guid>(data);
                foreach (string attr in tableAttrHashSet2)
                {
                    if (!dupAttr.Contains(attr))
                    {
                        string table_name = aliaName[attr];
                        foreach (Guid id in getTable(table_name).getAllIndex())
                        {
                            temp_dic.Add(attr, id);
                        }
                    }
                }
                temp_eleSet1.Add(temp_dic);
            }

            HashSet<Dictionary<string, Guid>> temp_eleSet2 = new HashSet<Dictionary<string, Guid>>();
            foreach (Dictionary<string, Guid> data in elementSet2)
            {
                Dictionary<string, Guid> temp_dic = new Dictionary<string, Guid>(data);
                foreach (string attr in tableAttrHashSet1)
                {
                    if (!dupAttr.Contains(attr))
                    {
                        string table_name = aliaName[attr];
                        foreach (Guid id in getTable(table_name).getAllIndex())
                        {
                            temp_dic.Add(attr, id);
                        }
                    }
                }
                temp_eleSet2.Add(temp_dic);
            }

            //HashSet<Dictionary<string, Guid>> result = (HashSet<Dictionary<string, Guid>>)temp_eleSet1.Union((HashSet<Dictionary<string, Guid>>)temp_eleSet2);
            HashEqualityComparer tempCom = new HashEqualityComparer();
            HashSet<Dictionary<string, Guid>> result = new HashSet<Dictionary<string, Guid>>();

            var union = temp_eleSet1.Union(temp_eleSet2, tempCom);

            foreach (var aa in union)
            {
                result.Add(aa);
            }
            return result;
            //return result;
        }

        private Boolean checkIntersect(Dictionary<string, Guid> element1, Dictionary<string, Guid> element2, List<string> dupAttr)
        {
            foreach (string tableName in dupAttr)
            {
                if (element1[tableName] != element2[tableName])
                {
                    return false;
                }
            }
            return true;
        }
        private HashSet<Dictionary<string, Guid>> intersect(HashSet<Dictionary<string, Guid>> elementSet1, HashSet<string> tableAttrHashSet1, HashSet<Dictionary<string, Guid>> elementSet2, HashSet<string> tableAttrHashSet2)
        {
            HashSet<Dictionary<string, Guid>> result = new HashSet<Dictionary<string, Guid>>();

            List<string> dupAttr = new List<string>();
            var ans1 =
                from d1 in tableAttrHashSet1
                from d2 in tableAttrHashSet2
                where d1 == d2
                select d1;

            foreach (string attr in ans1)
            {
                dupAttr.Add(attr);
            }

            var ans2 =
                from d1 in elementSet1
                from d2 in elementSet2
                where checkIntersect(d1, d2, dupAttr)
                select new { d1 = d1, d2 = d2 };
            foreach (var data in ans2)
            {
                Dictionary<string, Guid> temp_result = new Dictionary<string, Guid>();
                foreach (var dd in data.d1)
                {
                    if (!temp_result.ContainsKey(dd.Key))
                    {
                        temp_result.Add(dd.Key, dd.Value);
                    }
                }
                foreach (var dd in data.d2)
                {
                    if (!temp_result.ContainsKey(dd.Key))
                    {
                        temp_result.Add(dd.Key, dd.Value);
                    }
                }
                result.Add(temp_result);
            }

            return result;
        }

        public Table getTable(string name)
        {
            return tables[name];
        }


        public void print_table_context() //Print table data to csv file
        {
            var csv = new StringBuilder();
            foreach (System.Collections.Generic.KeyValuePair<string, Table> tablePair in tables)
            {

                Dictionary<Guid, List<dynamic>> ans = tablePair.Value.getTableData();
                List<string> attr_order = tablePair.Value.getAttributesOrder();

                //Print table name
                var newLine = string.Format(tablePair.Key);
                csv.AppendLine(newLine);

                //Print attribute order
                var attr = String.Join(", ", attr_order.ToArray());
                csv.AppendLine(attr);

                foreach (System.Collections.Generic.KeyValuePair<Guid, List<dynamic>> ele in ans)
                {

                    //Print each tuple
                    var eleattr = String.Join(", ", ele.Value.ToArray());
                    csv.AppendLine(eleattr);
                }



            }
            File.WriteAllText("../../table_conetxt.csv", csv.ToString());
        }


        public void turnOnIndexing(string tableName,string attriName)
        {
            tables[tableName].turnOnIndexing(attriName);
        }

        private Dictionary<string, Table> tables = new Dictionary<string, Table>(1000000);
        public Dictionary<string, string> aliaName;
        public int allstarCount;
        public bool isAllstarCount;
    }
}

