
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class EditLocation : WebTest
    {

        public EditLocation()
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

            WebTestRequest request3 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/adminmenu.aspx");
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/categorymenu.aspx");
            request4.ThinkTime = 7;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx");
            request5.ThinkTime = 9;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx");
            request6.ThinkTime = 6;
            request6.Method = "POST";
            request6.ExpectedResponseUrl = this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx?";
            FormPostHttpBody request6Body = new FormPostHttpBody();
            request6Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request6Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request6Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtname", "__James Auto Test");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcode", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmbshowfrom", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmbshowto", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmbiscompany", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtparentcompany", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline1", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline2", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcity", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcounty", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtpostcode", "");
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtcountry", "");
            request6Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "0");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "0");
            request6.Body = request6Body;
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "javascript";
            extractionRule3.EndsWith = "James Auto Test</td>";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "NewID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request6;
            request6 = null;

            string extracted, id;
            extracted = this.Context["NewID"].ToString();
            id = AutoTools.GetID(extracted);

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx");
            request8.ThinkTime = 11;
            request8.QueryStringParameters.Add("companyid", id, false, false);
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/webServices/svcAutoComplete.asmx/getAddress");
            request9.ThinkTime = 6;
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"country\":\"United Kingdom\",\"postcode\":\"LN6 8LF\"}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx/saveLocation");
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = @"{""locationID"":" + id + @",""location"":""__James Auto Test EDITED"",""code"":""JAT"",""comment"":""Automatically generated location - EDITED"",""iscompany"":false,""showfrom"":true,""showto"":true,""parentcompany"":"""",""addressline1"":""Dore Avenue"",""addressline2"":""North Hykeham"",""city"":""Lincoln"",""county"":""Lincolnshire"",""postcode"":""LN6 8LF"",""country"":""United Kingdom"",""udfs"":[],""isPrivateAddress"":false}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx?");
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test EDITED</td><td class=\"row([1-2])\">Dore Avenue" +
                                            "</td><td class=\"row([1-2])\">LN6 8LF</td><td class=\"row([1-2])\">JAT</td>";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = true;
                validationRule2.PassIfTextFound = true;
                request11.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return request11;
            request11 = null;

            // Validation needed for the actual aelocation page
        }
    }
}
