
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class LogonTryAll : WebTest
    {

        public LogonTryAll()
        {            
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            #region Initialisation
            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }
            #endregion


            #region Ensure settings are correct for the test to complete

            // Update General Options - number of logon attempts to '4'
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit("txtattempts", "4", "txt"), false)) { yield return r; }

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT COUNT (*) FROM employees WHERE " +
                                                                        "email = 'james.lloyd@software-europe.co.uk'");
            reader.Read();
            int employeesWithFirstEmail = reader.GetInt32(0);
            reader.Close();

            if (employeesWithFirstEmail < 2)
            {
                // Set James and Testuser2 to have the same email address
                database.GetReader("UPDATE employees SET email = 'james.lloyd@software-europe.co.uk' WHERE (username = 'james' OR username = 'testuser2')");
            }                        

            SortedList<int, string> employeesWithSecondEmail = new SortedList<int, string>(); 

            reader = database.GetReader("SELECT employeeid, username FROM employees WHERE email = 'testuser1@software-europe.co.uk'");

            while (reader.Read())
            {
                employeesWithSecondEmail.Add(reader.GetInt32(0), reader.GetValue(1).ToString());
            }
            reader.Close();

            if (employeesWithSecondEmail.Count == 0)
            {
                // Update a standard user with the correct email address
                database.GetReader("UPDATE employees SET email = 'testuser1@software-europe.co.uk' WHERE username = 'testuser1'");
            }
            else if (employeesWithSecondEmail.Count > 1)
            {
                // Remove all but one of the users with the email address

                for (int x = 0; x < employeesWithSecondEmail.Count; x++)
                {
                    database.GetReader("UPDATE employees SET email = '' WHERE employeeid = " + employeesWithSecondEmail.Keys[x]);
                }

                database.GetReader("UPDATE employees SET email = 'testuser1@software-europe.co.uk' WHERE username = 'testuser1'");
            }

            // Add ServerToUse to context at this point to avoid cross-test issues
            this.Context.Add("WebServer1", AutoTools.ServerToUse()); 

            #endregion
                       

            #region Test 'Remember Details'


            #region Test 'Remember Details' stores the correct details

            WebTestRequest request2 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request2.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            request2.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request2.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request2;
            request2 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request4.ThinkTime = 5;
            request4.Method = "POST";
            request4.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/home.aspx");
            FormPostHttpBody request4Body = new FormPostHttpBody();
            request4Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request4Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request4Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request4Body.FormPostParameters.Add("txtCompanyID", AutoTools.UserLevel(UserType.Admin)[0]);
            request4Body.FormPostParameters.Add("txtUsername", AutoTools.UserLevel(UserType.Admin)[1]);
            request4Body.FormPostParameters.Add("txtPassword", AutoTools.UserLevel(UserType.Admin)[2]);
            request4Body.FormPostParameters.Add("chkRememberDetails", "on");
            request4Body.FormPostParameters.Add("btnLogon.x", "36");
            request4Body.FormPostParameters.Add("btnLogon.y", "6");
            request4.Body = request4Body;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/process.aspx"));
            request5.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            request5.QueryStringParameters.Add("process", "1", false, false);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            yield return request6;
            AutoTools.ValidateText(AutoTools.UserLevel(UserType.Admin)[0], request6);
            AutoTools.ValidateText(AutoTools.UserLevel(UserType.Admin)[1], request6);
            request6 = null;

            #endregion


            #region Test details are not saved if not using 'Remember Details'

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request7.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request8.ThinkTime = 5;
            request8.Method = "POST";
            request8.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/home.aspx");
            FormPostHttpBody request8Body = new FormPostHttpBody();
            request8Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request8Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request8Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request8Body.FormPostParameters.Add("txtCompanyID", AutoTools.UserLevel(UserType.Admin)[0]);
            request8Body.FormPostParameters.Add("txtUsername", AutoTools.UserLevel(UserType.Admin)[1]);
            request8Body.FormPostParameters.Add("txtPassword", AutoTools.UserLevel(UserType.Admin)[2]);
            request8Body.FormPostParameters.Add("btnLogon.x", "36");
            request8Body.FormPostParameters.Add("btnLogon.y", "6");
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/process.aspx"));
            request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            request9.QueryStringParameters.Add("process", "1", false, false);
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));            
            AutoTools.ValidateText(AutoTools.UserLevel(UserType.Admin)[0], request10, false, false);
            AutoTools.ValidateText(AutoTools.UserLevel(UserType.Admin)[1], request10, false, false);
            yield return request10;
            request10 = null;

            #endregion


            #endregion


            #region Test Forgotten Details


            #region Test Forgotten Details with invalid email address

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/forgottendetails.aspx"));
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/forgottendetails.aspx"));
            request12.Method = "POST";
            FormPostHttpBody request12Body = new FormPostHttpBody();
            request12Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request12Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request12Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request12Body.FormPostParameters.Add("ctl00$pageContents$txtEmailAddress", "thisisnotavalidemailaddress");
            request12Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request12Body.FormPostParameters.Add("ctl00$pageContents$btnOk.x", "25");
            request12Body.FormPostParameters.Add("ctl00$pageContents$btnOk.y", "12");
            request12.Body = request12Body;
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "1";
            request12.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            // Validate the message shown for the previous email address
            AutoTools.ValidateText("Sorry, the email address you have entered does not exist.  " +
                                    "Please call your administrator for assistance.", request12);
            yield return request12;
            request12 = null;

            #endregion


            #region Test Forgotten Details with a valid email address that does not exist

            WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/forgottendetails.aspx"));
            request13.Method = "POST";
            FormPostHttpBody request13Body = new FormPostHttpBody();
            request13Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request13Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request13Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request13Body.FormPostParameters.Add("ctl00$pageContents$txtEmailAddress", "notarealaddress@notreal.com");
            request13Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request13Body.FormPostParameters.Add("ctl00$pageContents$btnOk.x", "28");
            request13Body.FormPostParameters.Add("ctl00$pageContents$btnOk.y", "11");
            request13.Body = request13Body;

            // Validate the message shown for the previous email address
            AutoTools.ValidateText("Sorry, the email address you have entered does not exist.  " +
                                    "Please call your administrator for assistance.", request13);
            ExtractHiddenFields extractionRule5 = new ExtractHiddenFields();
            extractionRule5.Required = true;
            extractionRule5.HtmlDecode = true;
            extractionRule5.ContextParameterName = "1";
            request13.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
            yield return request13;
            request13 = null;

            #endregion


            #region Test Forgotten Details with duplicate email address

            WebTestRequest request14 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/forgottendetails.aspx"));
            request14.Method = "POST";
            FormPostHttpBody request14Body = new FormPostHttpBody();
            request14Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request14Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request14Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request14Body.FormPostParameters.Add("ctl00$pageContents$txtEmailAddress", "james.lloyd@software-europe.co.uk");
            request14Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request14Body.FormPostParameters.Add("ctl00$pageContents$btnOk.x", "26");
            request14Body.FormPostParameters.Add("ctl00$pageContents$btnOk.y", "13");
            request14.Body = request14Body;
            AutoTools.ValidateText("Sorry, the email address you have entered is not unique so your logon details cannot be sent there. " +
                                    "Please call your administrator for assistance.", request14);
            ExtractHiddenFields extractionRule6 = new ExtractHiddenFields();
            extractionRule6.Required = true;
            extractionRule6.HtmlDecode = true;
            extractionRule6.ContextParameterName = "1";
            request14.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule6.Extract);
            yield return request14;
            request14 = null;

            #endregion


            #region Test Forgotten Details with correct email address

            WebTestRequest request15 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/forgottendetails.aspx"));
            request15.Method = "POST";
            FormPostHttpBody request15Body = new FormPostHttpBody();
            request15Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request15Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request15Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request15Body.FormPostParameters.Add("ctl00$pageContents$txtEmailAddress", "testuser1@software-europe.co.uk");
            request15Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request15Body.FormPostParameters.Add("ctl00$pageContents$btnOk.x", "36");
            request15Body.FormPostParameters.Add("ctl00$pageContents$btnOk.y", "13");
            request15.Body = request15Body;
            AutoTools.ValidateText("Thank you, you will shortly receive an email with instructions on how to reset your password." +
                                    " <a href=\"logon.aspx\">Click here</a> to return to the logon page.", request15);
            yield return request15;
            request15 = null;
            
            WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/forgottendetails.aspx"));
            request17.Method = "POST";
            request17.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            FormPostHttpBody request17Body = new FormPostHttpBody();
            request17Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request17Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request17Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request17Body.FormPostParameters.Add("ctl00$pageContents$txtEmailAddress", "");
            request17Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request17Body.FormPostParameters.Add("ctl00$pageContents$btnCancel.x", "26");
            request17Body.FormPostParameters.Add("ctl00$pageContents$btnCancel.y", "14");
            request17.Body = request17Body;
            yield return request17;
            request17 = null;

            #endregion


            #endregion


            #region Test logging in with incorrect details


            #region Log in with incorrect Company ID

            WebTestRequest request18 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request18.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            request18.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            ExtractHiddenFields extractionRule7 = new ExtractHiddenFields();
            extractionRule7.Required = true;
            extractionRule7.HtmlDecode = true;
            extractionRule7.ContextParameterName = "1";
            request18.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule7.Extract);
            yield return request18;
            request18 = null;

            WebTestRequest request19 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request19.Method = "POST";
            request19.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            FormPostHttpBody request19Body = new FormPostHttpBody();
            request19Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request19Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request19Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request19Body.FormPostParameters.Add("txtCompanyID", "thisisnotarealcompanyid");
            request19Body.FormPostParameters.Add("txtUsername", AutoTools.UserLevel(UserType.Admin)[1]);
            request19Body.FormPostParameters.Add("txtPassword", AutoTools.UserLevel(UserType.Admin)[2]);
            request19Body.FormPostParameters.Add("btnLogon.x", "36");
            request19Body.FormPostParameters.Add("btnLogon.y", "6");
            request19.Body = request19Body;
            AutoTools.ValidateText("The details you have entered are incorrect.", request19);
            yield return request19;
            request19 = null;

            #endregion


            #region Log in with incorrect Username

            for (int x = 3; x > -2; x--)
            {
                WebTestRequest request20 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
                request20.Method = "POST";
                request20.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
                FormPostHttpBody request20Body = new FormPostHttpBody();
                request20Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                request20Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                request20Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                request20Body.FormPostParameters.Add("txtCompanyID", AutoTools.UserLevel(UserType.Admin)[0]);
                request20Body.FormPostParameters.Add("txtUsername", "thisisnotarealusername");
                request20Body.FormPostParameters.Add("txtPassword", AutoTools.UserLevel(UserType.Admin)[2]);
                request20Body.FormPostParameters.Add("btnLogon.x", "36");
                request20Body.FormPostParameters.Add("btnLogon.y", "6");

                // The viewstate must be extracted again to correctly get the number of attempts left
                this.Context.Remove("$HIDDEN1.__EVENTTARGET");
                this.Context.Remove("$HIDDEN1.__EVENTARGUMENT");
                this.Context.Remove("$HIDDEN1.__VIEWSTATE");
                ExtractHiddenFields extractionRule8 = new ExtractHiddenFields();
                extractionRule8.Required = true;
                extractionRule8.HtmlDecode = true;
                extractionRule8.ContextParameterName = "1";
                request20.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule8.Extract);
                request20.Body = request20Body;

                if (x == 0)
                {
                    AutoTools.ValidateText("The details you have entered are incorrect.", request20);
                }
                else if (x == -1)
                {
                    AutoTools.ValidateText("Too many attempts your account has been locked.", request20);
                }
                else
                {
                    AutoTools.ValidateText("The details you have entered are incorrect.  " + x + " attempts left.", request20);
                }
                yield return request20;
                request20 = null;
            }

            #endregion 


            #region Log in with incorrect Password

            // The viewstate must be extracted again to correctly get the number of attempts left
            this.Context.Remove("$HIDDEN1.__EVENTTARGET");
            this.Context.Remove("$HIDDEN1.__EVENTARGUMENT");
            this.Context.Remove("$HIDDEN1.__VIEWSTATE");

            WebTestRequest request21A = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request21A.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            request21A.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            ExtractHiddenFields extractionRule10 = new ExtractHiddenFields();
            extractionRule10.Required = true;
            extractionRule10.HtmlDecode = true;
            extractionRule10.ContextParameterName = "1";
            request21A.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule10.Extract);
            yield return request21A;
            request21A = null;

            for (int x = 3; x > -3; x--)
            {
                WebTestRequest request21 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
                request21.Method = "POST";
                request21.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
                FormPostHttpBody request21Body = new FormPostHttpBody();
                request21Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                request21Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                request21Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                request21Body.FormPostParameters.Add("txtCompanyID", AutoTools.UserLevel(UserType.Admin)[0]);
                request21Body.FormPostParameters.Add("txtUsername", AutoTools.UserLevel(UserType.Admin)[1]);
                if (x > -2)
                {
                    request21Body.FormPostParameters.Add("txtPassword", "thisisnotthecorrectpassword");
                }
                else
                {
                    request21Body.FormPostParameters.Add("txtPassword", AutoTools.UserLevel(UserType.Admin)[2]);
                }
                request21Body.FormPostParameters.Add("btnLogon.x", "36");
                request21Body.FormPostParameters.Add("btnLogon.y", "6");

                // The viewstate must be extracted again to correctly get the number of attempts left
                this.Context.Remove("$HIDDEN1.__EVENTTARGET");
                this.Context.Remove("$HIDDEN1.__EVENTARGUMENT");
                this.Context.Remove("$HIDDEN1.__VIEWSTATE");
                ExtractHiddenFields extractionRule9 = new ExtractHiddenFields();
                extractionRule9.Required = true;
                extractionRule9.HtmlDecode = true;
                extractionRule9.ContextParameterName = "1";
                request21.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule9.Extract);
                request21.Body = request21Body;

                if (x == 0)
                {
                    AutoTools.ValidateText("The details you have entered are incorrect.", request21);
                }
                else if (x < 0)
                {
                    AutoTools.ValidateText("Too many attempts your account has been locked.", request21);
                }
                else
                {
                    AutoTools.ValidateText("The details you have entered are incorrect.  " + x + " attempts left.", request21);
                }
                yield return request21;
                request21 = null;
            }

            // Un-archive the user, so that proceeding tests do not fail

            database.GetReader("UPDATE employees SET archived = 0 WHERE username = '" + AutoTools.UserLevel(UserType.Admin)[1] + "'");

            #endregion


            #endregion
        }
    }
}
