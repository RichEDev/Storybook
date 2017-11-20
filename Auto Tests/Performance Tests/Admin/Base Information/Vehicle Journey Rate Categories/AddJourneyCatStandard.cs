
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddJourneyCatStandard : WebTest
    {

        public AddJourneyCatStandard(bool MeasureInMiles)
        {
            this.Context.Add("WebServer1", AutoTools.ServerToUse());
            this.PreAuthenticate = true;
            string uom = "0";
            string catName = "__James Auto Test Standard";
            string sqlSelect = "";
            if (MeasureInMiles == false)
            {
                uom = "1";
                catName = "__James Auto Test Standard Kilometre";
                sqlSelect = " Kilometre";
            }
            this.Context.Add("uom", uom);
            this.Context.Add("CatName", catName);
            this.Context.Add("sqlSelect", sqlSelect);
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

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminmileage.aspx"));
            yield return request5;
            request5 = null;

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx"));
            request6.ThinkTime = 6;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "option selected=\"selected\"";
            extractionRule2.EndsWith = "Pound Sterling";
            extractionRule2.IgnoreCase = false;
            extractionRule2.UseRegularExpression = false;
            extractionRule2.Required = true;
            extractionRule2.ExtractRandomMatch = false;
            extractionRule2.Index = 0;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "CurrencyID";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            string currencyID = AutoTools.GetID(this.Context["CurrencyID"].ToString(), "=", "\"", 2);

            WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveVehicleJourneyRate"));
            request7.Method = "POST";
            StringHttpBody request7Body = new StringHttpBody();
            request7Body.ContentType = "application/json; charset=utf-8";
            request7Body.InsertByteOrderMark = false;
            request7Body.BodyString = "{\"mileageID\":0,\"carsize\":\"" + this.Context["CatName"].ToString() + "\",\"comment\":\"Automatically Generated V" +
                "ehicle Journey Rate - Do not edit\",\"thresholdtype\":0,\"calcmilestotal\":false,\"mil" +
                "eUom\":" + this.Context["uom"].ToString() + ",\"currencyid\":" + currencyID + "}";
            request7.Body = request7Body;
            yield return request7;
            request7 = null;

            // Get the ID of the VJR just created
            string sqlSelect = this.Context["sqlSelect"].ToString();
            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM mileage_categories WHERE carsize = '__James Auto Test Standard" + sqlSelect + "'");
            reader.Read();
            string journeyRateID = reader.GetValue(reader.GetOrdinal("mileageid")).ToString();
            reader.Close();

            WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveDateRange"));
            request10.Method = "POST";
            StringHttpBody request10Body = new StringHttpBody();
            request10Body.ContentType = "application/json; charset=utf-8";
            request10Body.InsertByteOrderMark = false;
            request10Body.BodyString = "{\"dateRangeID\":0,\"mileageID\":" + journeyRateID + ",\"daterangetype\":3,\"datevalue1\":null,\"datevalue2\":" +
                "null}";
            request10.Body = request10Body;
            yield return request10;
            request10 = null;

            // Get the ID of the Daterange just created
            cDatabaseConnection dbDateRange = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader readerDateRange = dbDateRange.GetReader("SELECT * FROM mileage_dateranges WHERE mileageid LIKE " + journeyRateID);
            readerDateRange.Read();
            int daterangeID = readerDateRange.GetInt32(readerDateRange.GetOrdinal("mileagedateid"));
            readerDateRange.Close();

            WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveThreshold"));
            request13.Method = "POST";
            StringHttpBody request13Body = new StringHttpBody();
            request13Body.ContentType = "application/json; charset=utf-8";
            request13Body.InsertByteOrderMark = false;
            request13Body.BodyString = "{\"thresholdID\":0,\"mileageID\":" + journeyRateID + ",\"dateRangeID\":" + daterangeID.ToString() + ",\"rangetype\":3,\"threshold1\":null," +
                "\"threshold2\":null,\"ppmPetrol\":0.4,\"ppmDiesel\":0.4,\"ppmLpg\":0.4,\"amountForVatP\":0" +
                ".1,\"amountForVatD\":0.1,\"amountForVatLpg\":0.1,\"passenger\":0,\"passengerx\":0,\"heavy" +
                "Bulky\":0}";
            request13.Body = request13Body;
            yield return request13;
            request13 = null;

            WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveDateRange"));
            request17.Method = "POST";
            StringHttpBody request17Body = new StringHttpBody();
            request17Body.ContentType = "application/json; charset=utf-8";
            request17Body.InsertByteOrderMark = false;
            request17Body.BodyString = "{\"dateRangeID\":" + daterangeID.ToString() + ",\"mileageID\":" + journeyRateID + ",\"daterangetype\":3,\"datevalue1\":null,\"datevalue2\"" +
                ":null}";
            request17.Body = request17Body;
            yield return request17;
            request17 = null;

            WebTestRequest request19 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveVehicleJourneyRate"));
            request19.Method = "POST";
            StringHttpBody request19Body = new StringHttpBody();
            request19Body.ContentType = "application/json; charset=utf-8";
            request19Body.InsertByteOrderMark = false;
            request19Body.BodyString = "{\"mileageID\":" + journeyRateID + ",\"carsize\":\"" + this.Context["CatName"].ToString() + "\",\"comment\":\"Automatically Generated " +
                "Vehicle Journey Rate - Do not edit\",\"thresholdtype\":0,\"calcmilestotal\":false,\"mileUom\":" + this.Context["uom"].ToString() + ",\"currencyid\":" + currencyID + "}";
            request19.Body = request19Body;
            yield return request19;
            request19 = null;

            WebTestRequest request20 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveThreshold"));
            request20.Method = "POST";
            StringHttpBody request20Body = new StringHttpBody();
            request20Body.ContentType = "application/json; charset=utf-8";
            request20Body.InsertByteOrderMark = false;
            request20Body.BodyString = "{\"thresholdID\":0,\"mileageID\":" + journeyRateID + ",\"dateRangeID\":" + daterangeID.ToString() + ",\"rangetype\":" +
                "3,\"threshold1\":null,\"threshold2\":null,\"ppmPetrol\":0.4,\"ppmDiesel\":0.4,\"ppmLpg\":0.4,\"amountForVatP\":0" +
                ".1,\"amountForVatD\":0.1,\"amountForVatLpg\":0.1,\"passenger\":0,\"passengerx\":0,\"heavy" +
                "Bulky\":0}";
            request20.Body = request13Body;
            yield return request20;
            request20 = null;
        }
    }
}
