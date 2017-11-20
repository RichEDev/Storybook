
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddUDF : WebTest
    {

        public AddUDF()
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = false;
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

            WebTestRequest request3 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeuserdefined.aspx"));
            request3.ThinkTime = 66;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request3.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request3;
            request3 = null;

            cDatabaseConnection dataSource = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
            System.Data.SqlClient.SqlDataReader reader = dataSource.GetReader("SELECT * FROM UserDefinedFields");

            while (reader.Read())
            {
                WebTestRequest request4 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/admin/aeuserdefined.aspx"));
                request4.Method = "POST";
                request4.ParseDependentRequests = false;
                request4.RecordResult = false;               
                request4.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/shared/admin/adminuserdefined.aspx");
                FormPostHttpBody request4Body = new FormPostHttpBody();                
                request4Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
                request4Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                request4Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtattributename", reader.GetValue(reader.GetOrdinal("UDF Name")).ToString());                
                request4Body.FormPostParameters.Add("ctl00$contentmain$vcattrributename_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$vcattrributename_ClientState"].ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtorder", "");
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmbappliesto", reader.GetValue(reader.GetOrdinal("Applied To")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$chkspecific", "");
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtattributedescription", reader.GetValue(reader.GetOrdinal("Description")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtattributetooltip", reader.GetValue(reader.GetOrdinal("Tool tip")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$ddlstgroup", "0");
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmbattributetype", reader.GetValue(reader.GetOrdinal("Attribute Type")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtmaxlength", reader.GetValue(reader.GetOrdinal("Text Max Length")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmbtextformat", reader.GetValue(reader.GetOrdinal("Text Format")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtmaxlengthlarge", reader.GetValue(reader.GetOrdinal("Text Max Length Large")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmbtextformatlarge", reader.GetValue(reader.GetOrdinal("Large Text Format")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtprecision", reader.GetValue(reader.GetOrdinal("Decimal Precision")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmbdateformat", reader.GetValue(reader.GetOrdinal("Date Format")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmbdefaultvalue", reader.GetValue(reader.GetOrdinal("Yes No Default")).ToString());
                request4Body.FormPostParameters.Add("txtlistitems", reader.GetValue(reader.GetOrdinal("List Items")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtlistitem", "");
                request4Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "39");
                request4Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "12");
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtHyperlinkText", reader.GetValue(reader.GetOrdinal("Hyperlink Text")).ToString());
                request4Body.FormPostParameters.Add("ctl00$contentmain$txtHyperlinkPath", reader.GetValue(reader.GetOrdinal("Hyperlink Path")).ToString());
                request4.Body = request4Body;
                yield return request4;
                request4 = null;
            }

            reader.Close();
        }
    }
}
