
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;

    public class DeleteClaim : WebTest
    {
        public DeleteClaim()
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

            WebTestRequest request3 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/claimsmenu.aspx");
            ExtractText extractClaims = new ExtractText();
            extractClaims.StartsWith = "Current Claims (";
            extractClaims.EndsWith = ")";
            extractClaims.IgnoreCase = true;
            extractClaims.Required = true;
            extractClaims.Index = 0;
            extractClaims.HtmlDecode = true;
            extractClaims.ContextParameterName = "claims";
            request3.ExtractValues += new EventHandler<ExtractionEventArgs>(extractClaims.Extract);
            yield return request3;
            request3 = null;

            while (this.Context["claims"].ToString() != "0")
            {
                WebTestRequest request4 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/claimsummary.aspx");
                request4.ThinkTime = 6;
                request4.QueryStringParameters.Add("claimtype", "1", false, false);
                ExtractText extractcheck = new ExtractText();
                extractcheck.StartsWith = "claimid=";
                extractcheck.EndsWith = "JLloyd";
                extractcheck.IgnoreCase = true;
                extractcheck.UseRegularExpression = false;
                extractcheck.Required = false;
                extractcheck.ExtractRandomMatch = false;
                extractcheck.Index = 0;
                extractcheck.HtmlDecode = false;
                extractcheck.ContextParameterName = "extractCheck";
                request4.ExtractValues += new EventHandler<ExtractionEventArgs>(extractcheck.Extract);
                ExtractText extractAccount = new ExtractText();
                extractAccount.StartsWith = "accountid = ";
                extractAccount.EndsWith = ";";
                extractAccount.IgnoreCase = true;
                extractAccount.UseRegularExpression = false;
                extractAccount.Required = false;
                extractAccount.ExtractRandomMatch = false;
                extractAccount.Index = 0;
                extractAccount.HtmlDecode = false;
                extractAccount.ContextParameterName = "accountID";
                request4.ExtractValues += new EventHandler<ExtractionEventArgs>(extractAccount.Extract);
                yield return request4;
                request4 = null;

                string claimID = AutoTools.GetID(this.Context["extractCheck"].ToString());
                string accountID = this.Context["accountID"].ToString();

                WebTestRequest request5 = new WebTestRequest(this.Context["WebServer1"].ToString() + "/claimsummary.aspx/deleteClaim");
                request5.Method = "POST";
                StringHttpBody request5Body = new StringHttpBody();
                request5Body.ContentType = "application/json; charset=utf-8";
                request5Body.InsertByteOrderMark = false;
                request5Body.BodyString = "{\"accountid\":" + accountID + ",\"claimid\":" + claimID + "}";
                request5.Body = request5Body;
                yield return request5;
                request5 = null;
                
                this.Context.Remove("claims");

                WebTestRequest requestClaims = new WebTestRequest(this.Context["WebServer1"].ToString() + "/claimsmenu.aspx");
                ExtractText extractClaimsNo = new ExtractText();
                extractClaimsNo.StartsWith = "Current Claims (";
                extractClaimsNo.EndsWith = ")";
                extractClaimsNo.IgnoreCase = true;
                extractClaimsNo.Required = true;
                extractClaimsNo.Index = 0;
                extractClaimsNo.HtmlDecode = true;
                extractClaimsNo.ContextParameterName = "claims";
                requestClaims.ExtractValues += new EventHandler<ExtractionEventArgs>(extractClaimsNo.Extract);
                yield return requestClaims;
                requestClaims = null;
            }            
        }
    }
}
