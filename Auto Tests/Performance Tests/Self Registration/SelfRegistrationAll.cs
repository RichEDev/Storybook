
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class SelfRegistrationAll : WebTest
    {

        public SelfRegistrationAll()
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

            // Check for:
            // - Email Suffix
            // - Signoff Group - James signoff group
            // - Mileage Cat - Standard Auto mileage??

            string existingItems = string.Empty;

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT suffix FROM email_suffixes WHERE suffix = 'selauto.com'" +
                                            " UNION ALL SELECT groupname FROM groups WHERE groupname = '__Auto Signoff Group' UNION ALL SELECT carsize FROM" +
                                            " mileage_categories WHERE carsize = '__James Auto Test Standard'");
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    existingItems += " " + reader.GetValue(0).ToString();
                }                
            }
            reader.Close();
            
            if (!existingItems.Contains("selauto.com"))
            {
                // Add the e-mail suffix
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddEmailSuffix(), false)) { yield return r; }
            }

            if (!existingItems.Contains("__Auto Signoff Group"))
            {
                // Add Signoff group
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddSignoffGroup(), false)) { yield return r; }
            }

            if (!existingItems.Contains("__James Auto Test Standard"))
            {
                // Add mileage cat
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddJourneyCatStandard(true), false)) { yield return r; }
            }

            // Need department, costcode, project code, item role etc

            reader = database.GetReader("SELECT groupid FROM groups WHERE groupname = '__Auto Signoff Group' UNION ALL " +
                "SELECT mileageid FROM mileage_categories WHERE carsize = '__James Auto Test Standard' UNION ALL " +
                "SELECT employeeid FROM employees WHERE username = 'james' UNION ALL SELECT roleID FROM accessRoles WHERE " +
                "rolename = 'claimant' UNION ALL SELECT departmentid FROM departments WHERE department = 'technical' UNION ALL " +
                "SELECT costcodeid FROM costcodes WHERE costcode = 'expenses' UNION ALL SELECT projectcodeid FROM project_codes WHERE " +
                "description = 'product implementation'");

            reader.Read();
            string signoffID = reader.GetValue(0).ToString();
            reader.Read();
            string mileageRateID = reader.GetValue(0).ToString();
            reader.Read();
            string employeeID = reader.GetValue(0).ToString();
            reader.Read();
            string accessroleID = reader.GetValue(0).ToString();
            reader.Read();
            string departmentID = reader.GetValue(0).ToString();
            reader.Read();
            string costcodeID = reader.GetValue(0).ToString();
            reader.Read();
            string projectcodeID = reader.GetValue(0).ToString();                                                       
            reader.Close();
            
            WebTestRequest request2 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/logon.aspx"));
            request2.Encoding = System.Text.Encoding.GetEncoding("Windows-1252");
            request2.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/logon.aspx");
            ExtractHiddenFields extractionRule0 = new ExtractHiddenFields();
            extractionRule0.Required = true;
            extractionRule0.HtmlDecode = true;
            extractionRule0.ContextParameterName = "1";
            request2.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule0.Extract);
            yield return request2;
            request2 = null;                                

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request4.ThinkTime = 72;
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = true;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request4.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request5.ThinkTime = 11;
            request5.Method = "POST";
            FormPostHttpBody request5Body = new FormPostHttpBody();
            request5Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request5Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request5Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txttitle", "Mr");
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtfirstname", "Automated");
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtsurname", "Testuser");
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtemail", "testuser@selauto.com");
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtretypeemail", "testuser@selauto.com");
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtusername", "testuser1");
            request5Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request5Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StartNavigationTemplateContainerID$StartNextImageButton.x", "22");
            request5Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StartNavigationTemplateContainerID$StartNextImageButton.y", "8");
            request5.Body = request5Body;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request6.ThinkTime = 19;
            request6.Method = "POST";
            FormPostHttpBody request6Body = new FormPostHttpBody();
            request6Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request6Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request6Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request6Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtnew", "Password1");
            request6Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtrenew", "Password1");
            request6Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txthint", "Password1");
            request6Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request6Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request6Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "27");
            request6Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "19");
            request6.Body = request6Body;
            ExtractHiddenFields extractionRule3 = new ExtractHiddenFields();
            extractionRule3.Required = true;
            extractionRule3.HtmlDecode = true;
            extractionRule3.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request7.ThinkTime = 79;
            request7.Method = "POST";
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtextension", "123");
            request7Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtmobile", "456");
            request7Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtpager", "789");
            request7Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "32");
            request7Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "15");
            request7.Body = request7Body;
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "1";
            request7.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request8.ThinkTime = 45;
            request8.Method = "POST";
            FormPostHttpBody request8Body = new FormPostHttpBody();
            request8Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request8Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request8Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtaddress1", "autoAdd1");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtaddress2", "autoAdd2");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtcity", "autoCity");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtcounty", "autoCounty");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtpostcode", "autoPostcode");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txthomephone", "autoHomePhone");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txthomefax", "autoHomeFax");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txthomeemail", "autoHomeEmail");
            request8Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request8Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "17");
            request8Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "6");
            request8.Body = request8Body;
            ExtractHiddenFields extractionRule5 = new ExtractHiddenFields();
            extractionRule5.Required = true;
            extractionRule5.HtmlDecode = true;
            extractionRule5.ContextParameterName = "1";
            request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request9.ThinkTime = 9;
            request9.Method = "POST";
            FormPostHttpBody request9Body = new FormPostHttpBody();
            request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtpurchaseledger", "111");
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtposition", "Automated Test Dummy");
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtpayrollnumber", "222");
            // Need 'James' employeeID
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmblinemanager", employeeID);
            // Leaving countryID as 78 due to the need to inner join global_countries and countries
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmbcountry", "78");
            // Leaving currencyID as 2883 due to the need to inner join gloabl_currencies and currencies
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmbcurrency", "2883");
            request9Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "14");
            request9Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "14");
            request9.Body = request9Body;
            ExtractHiddenFields extractionRule6 = new ExtractHiddenFields();
            extractionRule6.Required = true;
            extractionRule6.HtmlDecode = true;
            extractionRule6.ContextParameterName = "1";
            request9.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule6.Extract);
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request10.ThinkTime = 100;
            request10.Method = "POST";
            FormPostHttpBody request10Body = new FormPostHttpBody();
            request10Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request10Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request10Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            // Need the Access Role ID
            request10Body.FormPostParameters.Add("role", accessroleID);
            request10Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request10Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request10Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "21");
            request10Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "8");
            request10.Body = request10Body;
            ExtractHiddenFields extractionRule7 = new ExtractHiddenFields();
            extractionRule7.Required = true;
            extractionRule7.HtmlDecode = true;
            extractionRule7.ContextParameterName = "1";
            request10.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule7.Extract);
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request11.ThinkTime = 5;
            request11.Method = "POST";
            FormPostHttpBody request11Body = new FormPostHttpBody();
            request11Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request11Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request11Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request11Body.FormPostParameters.Add("group", signoffID);
            request11Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request11Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request11Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "27");
            request11Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "14");
            request11.Body = request11Body;
            ExtractHiddenFields extractionRule8 = new ExtractHiddenFields();
            extractionRule8.Required = true;
            extractionRule8.HtmlDecode = true;
            extractionRule8.ContextParameterName = "1";
            request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule8.Extract);
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request12.ThinkTime = 7;
            request12.Method = "POST";
            FormPostHttpBody request12Body = new FormPostHttpBody();
            request12Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request12Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request12Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request12Body.FormPostParameters.Add("advancesgroup", signoffID);
            request12Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request12Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request12Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "32");
            request12Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "20");
            request12.Body = request12Body;
            ExtractHiddenFields extractionRule9 = new ExtractHiddenFields();
            extractionRule9.Required = true;
            extractionRule9.HtmlDecode = true;
            extractionRule9.ContextParameterName = "1";
            request12.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule9.Extract);
            yield return request12;
            request12 = null;

            WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request13.ThinkTime = 73;
            request13.Method = "POST";
            FormPostHttpBody request13Body = new FormPostHttpBody();
            request13Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request13Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request13Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            // Need deparmentID
            request13Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmbdepartment", departmentID);
            // Need costCodeID
            request13Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmbcostcode", costcodeID);
            // Need projectCodeID
            request13Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmbprojectcode", projectcodeID);
            request13Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request13Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request13Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "27");
            request13Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "18");
            request13.Body = request13Body;
            ExtractHiddenFields extractionRule10 = new ExtractHiddenFields();
            extractionRule10.Required = true;
            extractionRule10.HtmlDecode = true;
            extractionRule10.ContextParameterName = "1";
            request13.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule10.Extract);
            yield return request13;
            request13 = null;

            WebTestRequest request14 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request14.ThinkTime = 3;
            request14.Method = "POST";
            FormPostHttpBody request14Body = new FormPostHttpBody();
            request14Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request14Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request14Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request14Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtaccountholdername", "autoHolderName");
            request14Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtaccountholdernumber", "autoHolderNumber");
            request14Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtaccounttype", "autoAccountType");
            request14Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtsortcode", "autoSortCode");
            request14Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtaccountreference", "autoAccountRef");
            request14Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request14Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request14Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "34");
            request14Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "13");
            request14.Body = request14Body;
            ExtractHiddenFields extractionRule11 = new ExtractHiddenFields();
            extractionRule11.Required = true;
            extractionRule11.HtmlDecode = true;
            extractionRule11.ContextParameterName = "1";
            request14.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule11.Extract);
            yield return request14;
            request14 = null;

            WebTestRequest request15 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request15.ThinkTime = 25;
            request15.Method = "POST";
            FormPostHttpBody request15Body = new FormPostHttpBody();
            request15Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$pageContents$wizregister$chkusecar");
            request15Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request15Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request15Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request15Body.FormPostParameters.Add("ctl00$pageContents$wizregister$chkusecar", "on");
            request15Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtEngineSize", "");
            request15Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request15Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request15.Body = request15Body;
            ExtractHiddenFields extractionRule12 = new ExtractHiddenFields();
            extractionRule12.Required = true;
            extractionRule12.HtmlDecode = true;
            extractionRule12.ContextParameterName = "1";
            request15.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule12.Extract);
            yield return request15;
            request15 = null;

            WebTestRequest request16 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request16.ThinkTime = 25;
            request16.Method = "POST";
            FormPostHttpBody request16Body = new FormPostHttpBody();
            request16Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request16Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request16Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());
            request16Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$chkusecar", "on");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtmake", "Nissan");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtmodel", "350z");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtregno", "Z 350");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$txtEngineSize", "3500");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmbUom", "0");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$cmbcartype", "1");
            request16Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request16Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "31");
            request16Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "22");
            request16.Body = request16Body;
            ExtractHiddenFields extractionRule13 = new ExtractHiddenFields();
            extractionRule13.Required = true;
            extractionRule13.HtmlDecode = true;
            extractionRule13.ContextParameterName = "1";
            request16.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule13.Extract);
            yield return request16;
            request16 = null;

            WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request17.Method = "POST";
            FormPostHttpBody request17Body = new FormPostHttpBody();
            request17Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request17Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request17Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request17Body.FormPostParameters.Add("mileageid", mileageRateID);
            request17Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request17Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request17Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "25");
            request17Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "5");
            request17.Body = request17Body;
            ExtractHiddenFields extractionRule14 = new ExtractHiddenFields();
            extractionRule14.Required = true;
            extractionRule14.HtmlDecode = true;
            extractionRule14.ContextParameterName = "1";
            request17.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule14.Extract);
            yield return request17;
            request17 = null;

            WebTestRequest request18 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request18.Method = "POST";
            FormPostHttpBody request18Body = new FormPostHttpBody();
            request18Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request18Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request18Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            if (AutoTools.CheckAllUDFs() == true)
            {
                InsertUDFvalues.AddAllUDFs(request18Body, "AutoEmployees", false, true);
            }
            request18Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request18Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request18Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.x", "18");
            request18Body.FormPostParameters.Add("ctl00$pageContents$wizregister$StepNavigationTemplateContainerID$StepNextImageButton.y", "7");
            request18.Body = request18Body;
            ExtractHiddenFields extractionRule15 = new ExtractHiddenFields();
            extractionRule15.Required = true;
            extractionRule15.HtmlDecode = true;
            extractionRule15.ContextParameterName = "1";
            request18.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule15.Extract);
            yield return request18;
            request18 = null;

            WebTestRequest request19 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/register.aspx"));
            request19.Method = "POST";
            request19.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/register_success.aspx");
            FormPostHttpBody request19Body = new FormPostHttpBody();
            request19Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request19Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request19Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request19Body.FormPostParameters.Add("requiredStep", this.Context["$HIDDEN1.requiredStep"].ToString());
            request19Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request19Body.FormPostParameters.Add("ctl00$pageContents$wizregister$FinishNavigationTemplateContainerID$FinishImageButton.x", "22");
            request19Body.FormPostParameters.Add("ctl00$pageContents$wizregister$FinishNavigationTemplateContainerID$FinishImageButton.y", "6");
            request19.Body = request19Body;
            yield return request19;
            request19 = null;

            // Validation:
            reader = database.GetReader("SELECT employeeID FROM employees WHERE username = 'testuser1'");
            reader.Read();
            string testUserID = reader.GetValue(0).ToString();
            reader.Close();

            // Login as Admin and validate the user details
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request20 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/adminemployees.aspx");
            request20.QueryStringParameters.Add("username", "testuser");
            AutoTools.ValidateText("testuser1</td><td class=\"row([1-2])\">Mr</td><td class=\"row([1-2])\">Automated</td>" +
                                "<td class=\"row([1-2])\">Testuser</td><td class=\"row([1-2])\">__Auto Signoff Group</td>", request20, true, true);
            yield return request20;
            request20 = null;

            // Add validation for aeemployee.aspx

            



            // Activate the account and sign-in as the user
            // THIS SHOULD BE UPDATED TO ACTIVATE THE USER THROUGH THE APPLICATION
            reader = database.GetReader("UPDATE employees SET active = '1' WHERE username = 'testuser1'");

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Claimant), false)) { yield return r; }

            // Run other tests as the user??

            


        }
    }
}
