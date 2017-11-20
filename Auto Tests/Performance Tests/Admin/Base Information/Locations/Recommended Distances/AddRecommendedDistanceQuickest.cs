
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddRecommendedDistanceQuickest : WebTest
    {

        public AddRecommendedDistanceQuickest()
        {
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            string serverToUse = AutoTools.ServerToUse();

            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }


            // Make sure postcode calculation type is set to 'quickest'
            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit("tabsNewExpenses$tabmileage$cmbmileagecalculationtype", "2", "list"), false)) { yield return r; }


            WebTestRequest request3 = new WebTestRequest((serverToUse + "/adminmenu.aspx"));
            request3.ThinkTime = 2;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((serverToUse + "/categorymenu.aspx"));
            request4.ThinkTime = 3;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((serverToUse + "/shared/admin/locationsearch.aspx"));
            request5.ThinkTime = 3;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((serverToUse + "/shared/admin/locationsearch.aspx"));
            request6.ThinkTime = 6;
            request6.Method = "POST";
            request6.ExpectedResponseUrl = (serverToUse + "/shared/admin/adminlocations.aspx?");
            FormPostHttpBody request6Body = new FormPostHttpBody();
            request6Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request6Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request6Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request6Body.FormPostParameters.Add("ctl00$contentmain$txtname", "");
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
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "19");
            request6Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "20");
            request6.Body = request6Body;
            ExtractText extractFirstLocation = new ExtractText();
            extractFirstLocation.StartsWith = "javascript:changeArchiveStatus";
            extractFirstLocation.EndsWith = "__Auto Recommended Test 1";
            extractFirstLocation.IgnoreCase = false;
            extractFirstLocation.UseRegularExpression = false;
            extractFirstLocation.Required = true;
            extractFirstLocation.ExtractRandomMatch = false;
            extractFirstLocation.Index = 0;
            extractFirstLocation.HtmlDecode = false;
            extractFirstLocation.ContextParameterName = "";
            extractFirstLocation.ContextParameterName = "firstLocationID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractFirstLocation.Extract);
            ExtractText extractSecondLocation = new ExtractText();
            extractSecondLocation.StartsWith = "javascript:changeArchiveStatus";
            extractSecondLocation.EndsWith = "__Auto Recommended Test 2";
            extractSecondLocation.IgnoreCase = false;
            extractSecondLocation.UseRegularExpression = false;
            extractSecondLocation.Required = true;
            extractSecondLocation.ExtractRandomMatch = false;
            extractSecondLocation.Index = 0;
            extractSecondLocation.HtmlDecode = false;
            extractSecondLocation.ContextParameterName = "";
            extractSecondLocation.ContextParameterName = "secondLocationID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractSecondLocation.Extract);
            yield return request6;
            request6 = null;


            string firstLocationID = AutoTools.GetID(this.Context["firstLocationID"].ToString());
            string secondLocationID = AutoTools.GetID(this.Context["secondLocationID"].ToString());


            WebTestRequest request7 = new WebTestRequest((serverToUse + "/shared/admin/aelocation.aspx"));
            request7.ThinkTime = 5;
            request7.QueryStringParameters.Add("companyid", firstLocationID, false, false);
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((serverToUse + "/shared/admin/aelocation.aspx/saveLocation"));
            request8.ThinkTime = 8;
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = @"{""locationID"":" + firstLocationID + @",""location"":""__Auto Recommended Test 1"",""code"":""AutoRec1"",""comment"":""Automatically generated Recommended Distances test - do not edit"",""iscompany"":false,""showfrom"":true,""showto"":true,""parentcompany"":"""",""addressline1"":""Dore Avenue"",""addressline2"":""North Hykeham"",""city"":""Lincoln"",""county"":""Lincolnshire"",""postcode"":""LN6 8LF"",""country"":""United Kingdom"",""udfs"":[],""isPrivateAddress"":false}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((serverToUse + "/shared/admin/aelocation.aspx/saveDistance"));
            request9.Method = "POST";
            StringHttpBody request9Body = new StringHttpBody();
            request9Body.ContentType = "application/json; charset=utf-8";
            request9Body.InsertByteOrderMark = false;
            request9Body.BodyString = "{\"locationID\":" + firstLocationID + ",\"location\":" + secondLocationID + ",\"distance\":0}";
            request9.Body = request9Body;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((serverToUse + "/shared/admin/aelocation.aspx/createDistanceGrid"));
            request10.ThinkTime = 3;
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "{\"contextKey\":" + firstLocationID + "}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((serverToUse + "/shared/admin/aelocation.aspx/saveLocation"));
            request11.Method = "POST";
            StringHttpBody request11Body = new StringHttpBody();
            request11Body.ContentType = "application/json; charset=utf-8";
            request11Body.InsertByteOrderMark = false;
            request11Body.BodyString = @"{""locationID"":" + firstLocationID + @",""location"":""__Auto Recommended Test 1"",""code"":""AutoRec1"",""comment"":""Automatically generated Recommended Distances test - do not edit"",""iscompany"":false,""showfrom"":true,""showto"":true,""parentcompany"":"""",""addressline1"":""Dore Avenue"",""addressline2"":""North Hykeham"",""city"":""Lincoln"",""county"":""Lincolnshire"",""postcode"":""LN6 8LF"",""country"":""United Kingdom"",""udfs"":[],""isPrivateAddress"":false}";
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            WebTestRequest request12 = new WebTestRequest((serverToUse + "/shared/admin/locationsearch.aspx"));
            request12.ThinkTime = 5;
            ExtractHiddenFields extractionRule4 = new ExtractHiddenFields();
            extractionRule4.Required = true;
            extractionRule4.HtmlDecode = true;
            extractionRule4.ContextParameterName = "1";
            request12.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
            yield return request12;
            request12 = null;

            WebTestRequest request13 = new WebTestRequest((serverToUse + "/shared/admin/locationsearch.aspx"));
            request13.ThinkTime = 3;
            request13.Method = "POST";
            request13.ExpectedResponseUrl = (serverToUse + "/shared/admin/adminlocations.aspx?");
            FormPostHttpBody request13Body = new FormPostHttpBody();
            request13Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request13Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request13Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtname", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtcode", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmbshowfrom", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmbshowto", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmbiscompany", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtparentcompany", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline1", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtaddressline2", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtcity", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtcounty", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtpostcode", "");
            request13Body.FormPostParameters.Add("ctl00$contentmain$txtcountry", "");
            request13Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "44");
            request13Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "19");
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            WebTestRequest request14 = new WebTestRequest((serverToUse + "/shared/admin/aelocation.aspx"));
            request14.QueryStringParameters.Add("companyid", firstLocationID, false, false);
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__Auto Recommended Test 2</td><td class=\"row([1-2])\">0.00</td><td class=\"row([1-2])\">45.20";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = true;
                validationRule2.PassIfTextFound = true;
                request14.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return request14;
            request14 = null;
        }
    }
}
