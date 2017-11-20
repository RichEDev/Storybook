
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddMultipleLocations : WebTest
    {

        public AddMultipleLocations()
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

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/adminmenu.aspx"));
            request3.ThinkTime = 3;
            yield return request3;
            request3 = null;

            WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/categorymenu.aspx"));
            request4.ThinkTime = 1;
            yield return request4;
            request4 = null;

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx"));
            request5.ThinkTime = 2;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx"));
            request6.ThinkTime = 51;
            yield return request6;
            request6 = null;

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx/saveLocation"));
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = @"{""locationID"":0,""location"":""__Auto Recommended Test 1"",""code"":""AutoRec1"",""comment"":""Automatically generated Recommended Distances test - do not edit"",""iscompany"":false,""showfrom"":true,""showto"":true,""parentcompany"":"""",""addressline1"":""Dore Avenue"",""addressline2"":""North Hykeham"",""city"":""Lincoln"",""county"":""Lincolnshire"",""postcode"":""LN6 8LF"",""country"":""United Kingdom"",""udfs"":[],""isPrivateAddress"":false}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx"));
            request8.ThinkTime = 38;
            yield return request8;
            request8 = null;

            WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx"));
            request9.ThinkTime = 56;
            yield return request9;
            request9 = null;

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aelocation.aspx/saveLocation"));
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = @"{""locationID"":0,""location"":""__Auto Recommended Test 2"",""code"":""AutoRec2"",""comment"":""Automatically generated Recommended Distances test - do not edit"",""iscompany"":false,""showfrom"":true,""showto"":true,""parentcompany"":"""",""addressline1"":""Edgefield"",""addressline2"":""Weston"",""city"":""Spalding"",""county"":""Lincolnshire"",""postcode"":""PE12 6RQ"",""country"":""United Kingdom"",""udfs"":[],""isPrivateAddress"":false}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/locationsearch.aspx"));
            yield return request11;
            request11 = null;

            // ADD VALIDATION
        }
    }
}
