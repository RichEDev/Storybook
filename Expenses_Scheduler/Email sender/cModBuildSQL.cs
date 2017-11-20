using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Data;
using SpendManagementLibrary;
using System.Diagnostics;
using System.IO;

namespace Expenses_Scheduler
{

   public enum csvJOIN_Fields
   {
        RowStatus = 0,
        JoinID = 1,
        JoinSQL = 2,
        JoinTable = 3
   }

   public class cModBuildSQL
   {
       cAccount cAccount;
       private cEmailLog clslog;
       public cAccount Account
       {
           get { return cAccount; }
       }

       public cModBuildSQL(ref cEmailLog log)
       {
           clslog = log;
       }
       public string ConstructQuery(List<String> fields, string baseTable, string keyField, string keyValue, string includeFields)
       {
           clslog.AddToLog("Generating query to fetch fields from database...", Account);
           StringBuilder sFields = new StringBuilder();
           StringBuilder sQuery = new StringBuilder();
           NameValueCollection usedFields = new NameValueCollection();
           NameValueCollection usedJoins = new NameValueCollection();

           cCSV csv = new cCSV();

           DataSet ds = csv.CSVToDataset(System.AppDomain.CurrentDomain.BaseDirectory + "email sender\\database.csv");

           sFields.Append("[" + baseTable + "].[" + keyField + "]");

           foreach (string field in fields)
           {
               foreach (DataRow dRow in ds.Tables[0].Rows)
               {
                   bool isnull = false;
                   if (field == "[*" + (string)dRow[(int)csvDB_Fields.FriendlyName] + "*]")
                   {
                       if (usedFields[(string)dRow[(int)csvDB_Fields.FriendlyName]] != "y")
                       {
                           if (sFields.Length > 0)
                           {
                               sFields.Append(", ");
                           }

                           sFields.Append(dRow[(int)csvDB_Fields.TableName] + "." + dRow[(int)csvDB_Fields.FieldName]);

                           if ((string)dRow[(int)csvDB_Fields.TableName] == "dbo")
                           {
                               sFields.Append(" AS [" + dRow[(int)csvDB_Fields.FieldAlias].ToString() + "]");
                           }

                           usedFields.Add((string)dRow[(int)csvDB_Fields.FriendlyName], "y");
                       }

                       if (dRow[(int)csvDB_Fields.JoinsReq] == DBNull.Value)
                       {
                           isnull = true;
                       }
                       if (isnull == false)
                       {
                           if ((string)dRow[(int)csvDB_Fields.JoinsReq] == "")
                           {
                               isnull = true;
                           }
                       }

                       if (isnull == false)
                       {
                           string joinField = (string)dRow[(int)csvDB_Fields.JoinsReq];
                           string[] joinFields;

                           if (joinField.IndexOf(",") > -1)
                           {
                               joinFields = joinField.Split(',');
                           }
                           else
                           {
                               joinFields = new string[1];
                               joinFields[0] = joinField;
                           }

                           foreach(string t in joinFields)
                           {
                               if (usedJoins[t] != "y")
                               {
                                   usedJoins.Add(t, "y");
                               }
                           }
                       }
                   }
               }
           }

           sQuery.Append("SELECT " + includeFields + sFields.ToString() + " FROM [" + baseTable + "] " + GetJoins(usedJoins, baseTable) + " WHERE [" + baseTable + "].[" + keyField + "] = " + keyValue + "");
           clslog.AddToLog("Query built.", Account);
           return sQuery.ToString();
       }

       private string GetJoins(NameValueCollection selectedJoins, string baseTable)
       {
           clslog.AddToLog("Generating joins to relate tables...", Account);
           cCSV csvIn = new cCSV();
           NameValueCollection builtJoins = new NameValueCollection();
           StringBuilder joins = new StringBuilder();
           DataSet ds = new DataSet();

           ds = csvIn.CSVToDataset(System.AppDomain.CurrentDomain.BaseDirectory + "email sender\\joins.csv");
           IEnumerator sjEnum = selectedJoins.GetEnumerator();
           //selectedJoins.Set("", "")

           foreach (DataRow dRow in ds.Tables[0].Rows)
           {
               sjEnum.Reset();
               while (sjEnum.MoveNext())
               {
                   string sI = (string)sjEnum.Current;
                   if(builtJoins[sI] == "y") continue;

                   if((string) dRow[(int) csvJOIN_Fields.JoinID] != sI) continue;

                   if((string) dRow[(int) csvJOIN_Fields.JoinTable] == baseTable) continue;

                   joins.Append(dRow[(int)csvJOIN_Fields.JoinSQL] + " ");
                   builtJoins.Add(sI, "y");
               }
           }

           clslog.AddToLog("Joins done.", Account);

           return joins.ToString();
       }
   }
}
