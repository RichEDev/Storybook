
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class UpdatingMyDetailsInvalid : WebTest
    {

        public UpdatingMyDetailsInvalid()
        {            
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {

            #region Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }
            #endregion


            #region Update General Options - Claimants may edit their personal details to 'true'

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.GeneralOptionsEdit("tabsGeneralOptions$tabGeneral$chkeditmydetails", "1", "chk"), false)) { yield return r; }

            this.Context.Add("WebServer1", AutoTools.ServerToUse());

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));

            #endregion


            #region Validate that the breadcrumbs are present and correct. Check that the updated details are different.


            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/mydetailsmenu.aspx"));
            request5.ThinkTime = 1;
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/information/mydetails.aspx"));
            request6.ThinkTime = 57;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);            
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM MyDetails WHERE username = 'james'");
            int x = 1;
            reader.Read();
            while (x < reader.FieldCount)
            {
                AutoTools.ValidateText(reader.GetValue(x).ToString(), request6, false, false);
                x++;
            }
            reader.Close();
            yield return request6;
            request6 = null;

            #endregion


            #region Update My Details using the auto datasource

            reader = database.GetReader("SELECT * FROM MyDetails WHERE username = 'james'");
            reader.Read();

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/information/mydetails.aspx"));
            request7.ThinkTime = 5;
            request7.Method = "POST";
            request7.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/mydetailsmenu.aspx");
            FormPostHttpBody request7Body = new FormPostHttpBody();
            request7Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            request7Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            request7Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$lblusername", "james");
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtTitle", reader.GetValue(1).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtfirstname", reader.GetValue(2).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", reader.GetValue(3).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtextension", reader.GetValue(4).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtmobileno", reader.GetValue(5).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtpagerno", reader.GetValue(6).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtemail", reader.GetValue(7).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txttelno", reader.GetValue(8).ToString());
            request7Body.FormPostParameters.Add("ctl00$contentmain$txtemailhome", reader.GetValue(9).ToString());
            request7Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "36");
            request7Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "15");
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            #endregion 


            //#region Validate the changes have been made and that no other values can be updated

            //WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/information/mydetails.aspx"));
            //for (int y = 0; y < int.Parse(reader.FieldCount.ToString()); y ++)
            //{
            //    AutoTools.ValidateText(reader.GetValue(y).ToString(), request8);
            //}
            //AutoTools.ValidateText("ctl00_contentmain_txtpayroll\" disabled=\"disabled\"", request8);
            //AutoTools.ValidateText("ctl00_contentmain_txtposition\" disabled=\"disabled\"", request8);
            //AutoTools.ValidateText("ctl00_contentmain_txtcreditor\" disabled=\"disabled\"", request8);
            //AutoTools.ValidateText("ctl00_contentmain_txtmileage\" class=\"fillspan\" disabled=\"disabled\"", request8);
            //AutoTools.ValidateText("ctl00_contentmain_txtpersonalmiles\" class=\"fillspan\" disabled=\"disabled\"", request8);
            //AutoTools.ValidateText("ctl00_contentmain_ccb_ddlDepartments\" disabled=\"disabled\"", request8);
            //AutoTools.ValidateText("ctl00_contentmain_ccb_ddlCostCodes\" disabled=\"disabled\"", request8);
            //AutoTools.ValidateText("ctl00_contentmain_ccb_ddlProjectCodes\" disabled=\"disabled\"", request8);
            //yield return request8;
            //request8 = null;
            //reader.Close();

            //#endregion


            //#region Restore the values for ease-of-use


            //WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/information/mydetails.aspx"));
            //request9.ThinkTime = 5;
            //request9.Method = "POST";
            //request9.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/mydetailsmenu.aspx");
            //FormPostHttpBody request9Body = new FormPostHttpBody();
            //request9Body.FormPostParameters.Add("__EVENTTARGET", this.Context["$HIDDEN1.__EVENTTARGET"].ToString());
            //request9Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
            //request9Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
            //request9Body.FormPostParameters.Add("ctl00$contentmain$lblusername", "james");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtTitle", "Mr");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtfirstname", "James");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtsurname", "Lloyd");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtextension", "123");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtmobileno", "015687984");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtpagerno", "654");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtemail", "james.lloyd@software-europe.co.uk");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txttelno", "123654789");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$txtemailhome", "person@emailaddress.com");
            //request9Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "1");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.x", "36");
            //request9Body.FormPostParameters.Add("ctl00$contentmain$cmdok.y", "15");            
            //request9.Body = request9Body;
            //yield return request9;
            //request9 = null;

            //#endregion 

        }
    }
}
