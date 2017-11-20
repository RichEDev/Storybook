
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditUDF : WebTest
    {

        public EditUDF()
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/tailoringmenu.aspx"));
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "accountid = ";
            extractionRule2.EndsWith = ";";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "accountID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;
            string accountID = this.Context["accountID"].ToString();
            string udfID = "";
            SortedList<int, string> udfListItems = new SortedList<int,string>();
            
            // Loop through the database, use values to edit each UDF
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM UserDefinedFieldsEdit");

            while (reader.Read())
            {
                WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcGrid.asmx/filterGrid"));
                request8.ThinkTime = 4;
                request8.Method = "POST";
                StringHttpBody request8Body = new StringHttpBody();
                request8Body.ContentType = "application/json; charset=utf-8";
                request8Body.InsertByteOrderMark = false;
                request8Body.BodyString = @"{""accountid"":" + accountID + @",""gridid"":""gridFields"",""filter"":" + "\"" + reader.GetValue(0).ToString() + "\"" + @",""gridDetails"":[""gridFields"",true,""aeuserdefined.aspx?userdefineid={userdefineid}"",true,""javascript:deleteUserdefined({userdefineid});"",true,20,true,false,""b384f241-1cf5-4c9e-8bbe-07880ab2a0a4"",[true,""0f133d88-d784-4cf9-92ce-5aa378932121"",[],0,"""",false,""b384f241-1cf5-4c9e-8bbe-07880ab2a0a4"",[],0,"""",false,""f48c4391-2360-4e01-9d06-2cebdeea701f"",[],0,"""",false,""c8b01e80-df69-4d44-9d77-118c1f71ad23"",[1,""Text"",2,""Integer"",3,""Date"",4,""List"",5,""Tick Box"",6,""Currency"",7,""Number"",8,""Hyperlink"",9,""Relationship"",10,""Large Text""],0,"""",false,""2cf396de-8db6-4c79-a30d-a44584609351"",[],0,""""],""f07e080b-f2ae-452b-90b3-7d2530baae17"",false,"""","""",""Ascending"",""userdefineid"",true,[],[],false,""CheckBox"",false,""cGrid""]}";                
                request8.Body = request8Body;
                ExtractText extractionRule3 = new ExtractText();
                extractionRule3.StartsWith = "href=\\\"aeuserdefined.aspx?userdefineid=";
                extractionRule3.EndsWith = "\\\"";
                extractionRule3.IgnoreCase = false;
                extractionRule3.UseRegularExpression = false;
                extractionRule3.Required = true;
                extractionRule3.ExtractRandomMatch = false;
                extractionRule3.Index = 0;
                extractionRule3.HtmlDecode = false;
                extractionRule3.ContextParameterName = "";
                extractionRule3.ContextParameterName = "udfID";
                request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
                yield return request8;
                request8 = null;

                udfID = this.Context["udfID"].ToString();

                WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeuserdefined.aspx"));
                request10.ThinkTime = 14;
                request10.QueryStringParameters.Add("userdefineid", udfID, false, false);
                ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
                extractionRule4.Required = true;
                extractionRule4.HtmlDecode = true;
                extractionRule4.ContextParameterName = "1";
                request10.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
                yield return request10;
                request10 = null;

                WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeuserdefined.aspx"));
                request9.Method = "POST";
                request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminuserdefined.aspx");
                request9.QueryStringParameters.Add("userdefineid", udfID, false, false);
                FormPostHttpBody request9Body = new FormPostHttpBody();
                request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtattributename", reader.GetValue(1).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$vcattrributename_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$vcattrributename_ClientState"].ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtorder", "");
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtattributedescription", reader.GetValue(2).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtattributetooltip", reader.GetValue(3).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$ddlstgroup", "0");
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtmaxlength", reader.GetValue(4).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$cmbtextformat", reader.GetValue(6).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtmaxlengthlarge", reader.GetValue(5).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$cmbtextformatlarge", "2");
                request9Body.FormPostParameters.Add("ctl00$contentmain$cmbdefaultvalue", reader.GetValue(7).ToString());
                request9Body.FormPostParameters.Add("txtlistitems", reader.GetValue(8).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtHyperlinkText", reader.GetValue(9).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtHyperlinkPath", reader.GetValue(10).ToString());
                request9Body.FormPostParameters.Add("ctl00$contentmain$txtlistitem", "");
                request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
                request9Body.FormPostParameters.Add("ctl00$contentmain$chkspecific", "");   
                request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "27");
                request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "18");
                request9.Body = request9Body;
                yield return request9;
                request9 = null;               

                // Validate that the UDF has saved successfully
                WebTestRequest request11 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aeuserdefined.aspx");
                request11.QueryStringParameters.Add("userdefineid", udfID, false, false);
                if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                {
                    for (int x = 1; x < 9; x++)
                    {
                        if (x != 8)
                        {
                        ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                        validationRule2.FindText = reader.GetValue(x).ToString();
                        validationRule2.IgnoreCase = false;
                        validationRule2.UseRegularExpression = false;
                        validationRule2.PassIfTextFound = true;
                        request11.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                        }
                        else
                        {
                            // Different Validation for lists
                            if (reader.GetValue(8).ToString().Contains("Item 1-Item A"))
                            {
                                udfListItems.Add(1, "Item A</option>");
                                udfListItems.Add(2, "Item B</option>");
                            }
                            else if (reader.GetValue(8).ToString().Contains("Item 1-Item 1"))
                            {
                                udfListItems.Add(1, "Item 1</option>");
                                udfListItems.Add(2, "Item 2</option>");
                                udfListItems.Add(3, "Item 3 edited</option>");
                                udfListItems.Add(4, "Item 4</option>");
                            }

                            if (udfListItems.Count != 0)
                            {
                                for (int y = 1; y < udfListItems.Count + 1; y++)
                                {
                                    ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                                    validationRule2.FindText = udfListItems[y];
                                    validationRule2.IgnoreCase = false;
                                    validationRule2.UseRegularExpression = false;
                                    validationRule2.PassIfTextFound = true;
                                    request11.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                                }
                            }
                        }
                    }
                }
                yield return request11;
                request11 = null;

                // Clear udfID from context ready for next iteration
                this.Context.Remove("udfID");

                // Clear the udfListItems for the next iteration 
                udfListItems.Clear();
            }
            reader.Close();
        }
    }
}
