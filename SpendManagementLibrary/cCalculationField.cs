using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Configuration;
using Infragistics.WebUI.UltraWebCalcManager;

namespace SpendManagementLibrary
{
    /// <summary>
    /// Calculates a column using UltraCalcValue
    /// </summary>
    public class cCalculationField
    {
        private int nAccountID;

        public cCalculationField(int accountID)
        {
            nAccountID = accountID;
        }

        /// <summary>
        /// Replaces Guids and evaluates the forumla
        /// </summary>
        /// <param name="valueFormula"></param>
        /// <param name="baseTable"></param>
        /// <param name="entityID"></param>
        /// <param name="clsFields"></param>
        /// <param name="ConnectionString"></param>
        /// <param name="clsBaseTables"></param>
        /// <returns></returns>
        public string CalculatedColumn(string valueFormula, cTable baseTable, int entityID, cFields clsFields, string ConnectionString, cTables clsBaseTables)
        {
            if (string.IsNullOrEmpty(valueFormula) == false)
            {
                Regex regex = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                MatchCollection matches = regex.Matches(valueFormula);

                List<cField> lstMatchedFields = new List<cField>();
                string newDynamicValue = valueFormula;
                if (matches.Count > 0)
                {
                    cField reqField;
                    cQueryBuilder clsQueryBuilder = new cQueryBuilder(nAccountID, ConnectionString, ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, baseTable, clsBaseTables, clsFields);
                    clsQueryBuilder.addFilter(clsFields.GetFieldByID(baseTable.PrimaryKeyID), ConditionType.Equals, new object[] { entityID }, null, ConditionJoiner.And, null); // null as always on bt? !!!!!!!!

                    int i;

                    for (i = 0; i < matches.Count; i++)
                    {
                        reqField = clsFields.GetFieldByID(new Guid(matches[i].Value));

                        if (reqField != null)
                        {
                            lstMatchedFields.Add(reqField);
                            clsQueryBuilder.addColumn(reqField);
                        }
                    }

                    DataSet ds = clsQueryBuilder.getDataset();


                    if (ds.Tables[0].Rows.Count > 0 && ds.Tables[0].Columns.Count > 0)
                    {
                        int matchedFieldCounter = -1;
                        for (i = 0; i < ds.Tables[0].Columns.Count; i++)
                        {
                            if (i >= lstMatchedFields.Count)
                            {
                                break;
                            }
                            if (ds.Tables[0].Columns[i].ColumnName.Contains("_text") == false)
                            {
                                matchedFieldCounter++;
                                newDynamicValue = newDynamicValue.Replace("[" + lstMatchedFields[matchedFieldCounter].FieldID.ToString() + "]", Convert.ToString(ds.Tables[0].Rows[0][i]));
                            }

                        }
                    }
                }

                Infragistics.WebUI.UltraWebCalcManager.UltraWebCalcManager calcman = new UltraWebCalcManager();
                cText clstext = new cText();
                cExcel clsexcel = new cExcel();
                cRowFunction clsRow = new cRowFunction();
                cColumnFunction clsColumn = new cColumnFunction();
                ColFunction column = new ColFunction();
                cAddressFunction clsAddress = new cAddressFunction();

                calcman.RegisterUserDefinedFunction(clstext);
                calcman.RegisterUserDefinedFunction(clsexcel);
                calcman.RegisterUserDefinedFunction(clsRow);
                calcman.RegisterUserDefinedFunction(clsColumn);
                calcman.RegisterUserDefinedFunction(column);
                calcman.RegisterUserDefinedFunction(clsAddress);
                Infragistics.WebUI.CalcEngine.UltraCalcValue val;
                try
                {
                    val = calcman.Calculate(newDynamicValue);
                    newDynamicValue = val.Value.ToString();
                }
                catch (Exception ex)
                {
                    newDynamicValue = "#ERROR#";
                }
                return newDynamicValue;
            }
            return string.Empty;
        }
    }
}
