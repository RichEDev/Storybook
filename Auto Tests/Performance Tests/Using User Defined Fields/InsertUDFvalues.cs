
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class InsertUDFvalues
    {
        public static FormPostHttpBody AddAllUDFs(FormPostHttpBody requestBody, string UDFname, bool editing)
        {
            return AddAllUDFs(requestBody, UDFname, editing, false);
        }

        public static FormPostHttpBody AddAllUDFs(FormPostHttpBody requestBody, string UDFname, bool editing, bool selfReg)
        {                                                      
            // Pull out all items for every List UDF
            cDatabaseConnection dbData = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = dbData.GetReader("SELECT userdefineid, item, valueid FROM userdefined_list_items");

            SortedList<int, SortedList<int, string>> lstAllListItemValues = new SortedList<int, SortedList<int, string>>();
            int userdefineID;
            int valueID;
            string item;
            string userdefineName;

            while (reader.Read())
            {
                userdefineID = reader.GetInt32(reader.GetOrdinal("userdefineid"));
                valueID = reader.GetInt32(reader.GetOrdinal("valueid"));
                if (reader.IsDBNull(reader.GetOrdinal("item")) == false)
                {
                    item = reader.GetString(reader.GetOrdinal("item"));
                }
                else
                {
                    item = string.Empty;
                }

                if (lstAllListItemValues.ContainsKey(userdefineID) == false)
                {
                    lstAllListItemValues.Add(userdefineID, new SortedList<int, string>());
                }

                lstAllListItemValues[userdefineID].Add(valueID, item);
            }

            reader.Close();

            string editText = "Testing ";
            string editInt = "86";
            string editDateTime = "09/01/2002 03:04";
            string editTime = "05:06";
            string editDate = "06/07/2008";
            int editList = 0;
            string editYesNoY = "0";
            string editYesNoN = "1";
            string editCurrency = "9.99";
            string editDecimal = "1.2345";
            string udf = "";
            string content = "contentmain";
            string formattedBox = "contentmain";

            if (editing == true) 
            {
                editText = "EDITED ";
                editInt = "66";
                editDateTime = "06/06/2006 06:06";
                editTime = "06:06";
                editDate = "06/06/2006";
                editList = 1;
                editYesNoY = "1";
                editYesNoN = "0";
                editCurrency = "6.66";
                editDecimal = "6.2345";
            }

            if (UDFname == "AutoExpenseItem" || UDFname == "AutoEmployees")
            {
                udf = "udf";
            }

            if (selfReg == true)
            {
                content = "pageContents$wizregister";
                formattedBox = "pageContents_wizregister";
            }

            // Connect to database
            cDatabaseConnection dbConnect = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            reader = dbConnect.GetReader("select * from userdefined where attribute_name like '" + UDFname + "%' order by fieldtype");
            while (reader.Read())            
            {
                userdefineID = reader.GetInt32(reader.GetOrdinal("userdefineid"));
                userdefineName = reader.GetValue(reader.GetOrdinal("attribute_name")).ToString();

                switch (reader.GetValue(reader.GetOrdinal("fieldtype")).ToString())
                {
                    case "1": // If the UDF is 'Text'                        
                        requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editText + userdefineName);                        
                        break;

                    case "2": // If the UDF is 'Int'
                        requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editInt);                        
                        break;

                    case "3": // If the UDF is 'Date'

                        if (userdefineName.Contains("Time"))
                        {
                            if (userdefineName.Contains("DateandTime"))
                            {
                                requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editDateTime);
                            }
                            else
                            {
                                requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editTime);
                            }
                        }
                        else 
                        {
                            requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editDate);
                        }                       
                        break;

                    case "4": // If the UDF is 'List'

                        if (lstAllListItemValues.ContainsKey(userdefineID) && lstAllListItemValues[userdefineID].Values.Count > 1)
                        {
                            // Has items                            
                            requestBody.FormPostParameters.Add("ctl00$" + content + "$cmb" + udf + userdefineID, lstAllListItemValues[userdefineID].Keys[editList].ToString());
                        }
                        else
                        {
                            // Has no items
                            requestBody.FormPostParameters.Add("ctl00$" + content + "$cmb" + udf + userdefineID, "0");
                        }
                       
                        break;

                    case "5": // If the UDF is 'Yes No'

                        if (userdefineName.Contains("YesNoN"))
                        {
                            requestBody.FormPostParameters.Add("ctl00$" + content + "$cmb" + udf + userdefineID, editYesNoN);
                        }
                        else
                        {
                            requestBody.FormPostParameters.Add("ctl00$" + content + "$cmb" + udf + userdefineID, editYesNoY);
                        }                        

                        break;

                    case "6": // If the UDF is 'Currency'
                        requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editCurrency);

                        break;

                    case "7": // If the UDF is 'Decimal'
                        requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editDecimal);

                        break;

                    case "10": // If the UDF is 'Formatted Text'

                        if (userdefineName.Contains("Formatted"))
                        {
                            if (udf == "udf")
                            {
                                requestBody.FormPostParameters.Add("_contentChanged_ctl00_" + formattedBox + "_rtfudf" + userdefineID + "_ctl02", "");
                                requestBody.FormPostParameters.Add("_contentForce_ctl00_" + formattedBox + "_rtfudf" + userdefineID + "_ctl02", "1");
                                requestBody.FormPostParameters.Add("_content_ctl00_" + formattedBox + "_rtfudf" + userdefineID + "_ctl02", editText + userdefineName);
                                requestBody.FormPostParameters.Add("_activeMode_ctl00_" + formattedBox + "_rtfudf" + userdefineID + "_ctl02", "0");
                            }
                            requestBody.FormPostParameters.Add("_contentChanged_ctl00_" + formattedBox + "_txt" + userdefineID + "_ctl02", "");
                            requestBody.FormPostParameters.Add("_contentForce_ctl00_" + formattedBox + "_txt" + userdefineID + "_ctl02", "1");
                            requestBody.FormPostParameters.Add("_content_ctl00_" + formattedBox + "_txt" + userdefineID + "_ctl02", editText + userdefineName);
                            requestBody.FormPostParameters.Add("_activeMode_ctl00_" + formattedBox + "_txt" + userdefineID + "_ctl02", "0");
                        }
                        else
                        {
                            requestBody.FormPostParameters.Add("ctl00$" + content + "$txt" + udf + userdefineID, editText + userdefineName);
                        }                        

                        break;

                    default:
                        
                        break;
                }
            }
            reader.Close();
            return requestBody;
        }

        public static System.EventHandler<Microsoft.VisualStudio.TestTools.WebTesting.ValidationEventArgs> ValidateAllUDFs(WebTestRequest requestOutcome, string findText)
        {
            ValidationRuleFindText validationRule = new ValidationRuleFindText();
            validationRule.FindText = findText;               
            validationRule.IgnoreCase = false;
            validationRule.UseRegularExpression = false;
            validationRule.PassIfTextFound = true;
            return validationRule.Validate;
        }
    }
}
