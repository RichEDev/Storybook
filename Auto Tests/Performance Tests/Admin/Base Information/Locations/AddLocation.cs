
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddLocation : WebTest
    {

        public AddLocation()
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
            request4.ThinkTime = 4;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx");
            request5.ThinkTime = 3;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx");
            request6.ThinkTime = 38;
            yield return request6;
            request6 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAutoComplete.asmx/getCountryList"));
            request11.ThinkTime = 8;
            request11.Method = "POST";
            StringHttpBody request11Body = new StringHttpBody();
            request11Body.ContentType = "application/json; charset=utf-8";
            request11Body.InsertByteOrderMark = false;
            request11Body.BodyString = "{\"prefixText\":\"united\",\"count\":10}";
            request11.Body = request11Body;
            yield return request11;
            request11 = null;

            WebTestRequest request7 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/webServices/svcAutoComplete.asmx/getAddress");
            request7.ThinkTime = 13;
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"country\":\"United Kingdom\",\"postcode\":\"LN6 8EB\"}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx/saveLocation");
            request8.Method = "POST";
            StringHttpBody request8Body = new StringHttpBody();
            request8Body.ContentType = "application/json; charset=utf-8";
            request8Body.InsertByteOrderMark = false;
            request8Body.BodyString = @"{""locationID"":0,""location"":""__James Auto Test"",""code"":""JAT"",""comment"":""Automatically generated location - do not edit"",""iscompany"":true,""showfrom"":true,""showto"":true,""parentcompany"":"""",""addressline1"":""Hutson Drive"",""addressline2"":""North Hykeham"",""city"":""Lincoln"",""county"":""Lincolnshire"",""postcode"":""LN6 8EB"",""country"":""United Kingdom"",""udfs"":[],""isPrivateAddress"":false}";
            request8.Body = request8Body;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/shared/admin/adminlocations.aspx?");
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
            {
                ValidationRuleFindText validationRule2 = new ValidationRuleFindText();
                validationRule2.FindText = "__James Auto Test</td><td class=\"row([1-2])\">Hutson Drive" +
                                            "</td><td class=\"row([1-2])\">LN6 8EB</td><td class=\"row([1-2])\">JAT</td>";
                validationRule2.IgnoreCase = false;
                validationRule2.UseRegularExpression = true;
                validationRule2.PassIfTextFound = true;
                request9.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
            }
            yield return request9;
            request9 = null;

            // Validation needed for the actual aelocation page
        }
    }
}
