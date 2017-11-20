
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class AddJourneyCatAll : WebTest
    {

        public AddJourneyCatAll()
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

            #region If Auto VJRs already exist, delete them before continuing with the test

            cDatabaseConnection checkAddedDatabase = new cDatabaseConnection(AutoTools.DatabaseConnectionString());
            System.Data.SqlClient.SqlDataReader checkAddedReader = checkAddedDatabase.GetReader("SELECT COUNT(*) AS 'Total' FROM " +
                                                                                "mileage_categories WHERE carsize LIKE '__Auto%'");
            checkAddedReader.Read();
            if (checkAddedReader.GetInt32(0) > 10)
            {
                foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.DeleteJourneyCatAll(), false)) { yield return r; }
            }
            checkAddedReader.Close();

            #endregion

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx"));
            request6.ThinkTime = 6;
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "option";
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
            string dateRangeType = "";
            string dateRange1 = null;
            string dateRange2 = null;
            string thresholdType = "";
            string thresholdFirstVal = "";
            string thresholdSecondVal = "";
            string shouldBeValid = "";
            string journeyRateID = "";
            string journeyCatName = "";
            string journeyDesc = "";
            string ratePerMile = "";
            string vatRate = "";
            string passengerRate = "";
            int daterangeID = 0;
            bool valid = false;

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM JourneyCatRates");            

            while (reader.Read())
            {
                // Only create a new VJR if the datasource is not null
                if (reader.GetValue(reader.GetOrdinal("VJR Cat Name")).ToString() != "")
                {                    
                    journeyCatName = reader.GetValue(reader.GetOrdinal("VJR Cat Name")).ToString();
                    journeyDesc = reader.GetValue(reader.GetOrdinal("VJR Desc")).ToString();

                    WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveVehicleJourneyRate"));
                    request7.Method = "POST";
                    StringHttpBody request7Body = new StringHttpBody();
                    request7Body.ContentType = "application/json; charset=utf-8";
                    request7Body.InsertByteOrderMark = false;
                    request7Body.BodyString = "{\"mileageID\":0,\"carsize\":\"" + journeyCatName + "\",\"comment\":\"" +
                         journeyDesc + "\",\"thresholdtype\":1,\"calcmilestotal\":false,\"mil" +
                        "eUom\":0,\"currencyid\":" + currencyID + "}";
                    request7.Body = request7Body;
                    yield return request7;
                    request7 = null;                    

                    // Get the ID of the VJR just created

                    WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminmileage.aspx"));
                    ExtractText extractionRule3 = new ExtractText();
                    extractionRule3.StartsWith = "javascript";
                    extractionRule3.EndsWith = journeyCatName;
                    extractionRule3.IgnoreCase = false;
                    extractionRule3.UseRegularExpression = false;
                    extractionRule3.Required = true;
                    extractionRule3.ExtractRandomMatch = false;
                    extractionRule3.Index = 0;
                    extractionRule3.HtmlDecode = false;
                    extractionRule3.ContextParameterName = "";
                    extractionRule3.ContextParameterName = "journeyRateID";
                    request8.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
                    yield return request8;
                    request8 = null;

                    journeyRateID = AutoTools.GetID(this.Context["journeyRateID"].ToString());
                    this.Context.Remove("journeyRateID");                                                
                }

                // Only create a new date range if the datasource is not null
                if (reader.GetValue(reader.GetOrdinal("Date Range Type")).ToString() != "")
                {
                    dateRangeType = reader.GetValue(reader.GetOrdinal("Date Range Type")).ToString();
                    dateRange1 = reader.GetValue(reader.GetOrdinal("Date Range 1")).ToString();
                    dateRange2 = reader.GetValue(reader.GetOrdinal("Date Range 2")).ToString();

                    if (dateRange1 == "")
                    {
                        dateRange1 = "null";
                    }
                    else
                    {
                        dateRange1 = "\"" + dateRange1.Substring(0, 10) + "\"";
                    }

                    if (dateRange2 == "")
                    {
                        dateRange2 = "null";
                    }
                    else
                    {
                        dateRange2 = "\"" + dateRange2.Substring(0, 10) + "\"";
                    }

                    daterangeID = 0;

                    // Need to get the correct date range ID after it is created
                    if (reader.GetValue(reader.GetOrdinal("VJR Cat Name")).ToString() == "")
                    {
                        // Get the value of the last date range ID and increment
                        WebTestRequest request11 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx"));
                        request11.QueryStringParameters.Add("mileageid", journeyRateID, false, false);
                        ExtractText extractionRule4 = new ExtractText();
                        extractionRule4.StartsWith = "Date Value 2</a></th></tr>";
                        extractionRule4.EndsWith = "gridDateRanges_footer";
                        extractionRule4.IgnoreCase = false;
                        extractionRule4.UseRegularExpression = false;
                        extractionRule4.Required = true;
                        extractionRule4.ExtractRandomMatch = false;
                        extractionRule4.Index = 0;
                        extractionRule4.HtmlDecode = false;
                        extractionRule4.ContextParameterName = "";
                        extractionRule4.ContextParameterName = "daterangeID";
                        request11.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
                        yield return request11;
                        request11 = null;

                        daterangeID = int.Parse(AutoTools.GetID(this.Context["daterangeID"].ToString())) + 1;
                        this.Context.Remove("daterangeID"); 
                    }

                    WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveDateRange"));
                    request10.Method = "POST";
                    StringHttpBody request10Body = new StringHttpBody();
                    request10Body.ContentType = "application/json; charset=utf-8";
                    request10Body.InsertByteOrderMark = false;
                    request10Body.BodyString = "{\"dateRangeID\":0,\"mileageID\":" + journeyRateID + ",\"daterangetype\":" + dateRangeType + ",\"datevalue1\":" + dateRange1 + ",\"datevalue2\":" +
                        dateRange2 + "}";
                    request10.Body = request10Body;
                    yield return request10;
                    request10 = null;
                   
                    // Get the ID of the new date range if 0 was previously used as the ID
                    if (daterangeID == 0)
                    {
                        WebTestRequest request14 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx"));
                        request14.QueryStringParameters.Add("mileageid", journeyRateID, false, false);
                        ExtractText extractionRule5 = new ExtractText();
                        extractionRule5.StartsWith = "Date Value 2</a></th></tr>";
                        extractionRule5.EndsWith = ";\"><img alt=\"Delete\"";
                        extractionRule5.IgnoreCase = false;
                        extractionRule5.UseRegularExpression = false;
                        extractionRule5.Required = true;
                        extractionRule5.ExtractRandomMatch = false;
                        extractionRule5.Index = 0;
                        extractionRule5.HtmlDecode = false;
                        extractionRule5.ContextParameterName = "";
                        extractionRule5.ContextParameterName = "daterangeID";
                        request14.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
                        yield return request14;
                        request14 = null;

                        daterangeID = int.Parse(AutoTools.GetID(this.Context["daterangeID"].ToString()));
                        this.Context.Remove("daterangeID");
                    }
                }

                // Save threshold information       
                thresholdType = reader.GetValue(reader.GetOrdinal("Threshold Type")).ToString();
                thresholdFirstVal = reader.GetValue(reader.GetOrdinal("Threshold Val 1")).ToString();
                thresholdSecondVal = reader.GetValue(reader.GetOrdinal("Threshold Val 2")).ToString();
                ratePerMile = reader.GetValue(reader.GetOrdinal("Rate Per Mile")).ToString();
                vatRate = reader.GetValue(reader.GetOrdinal("VAT Amount")).ToString();
                passengerRate = reader.GetValue(reader.GetOrdinal("Passenger Rate")).ToString();

                if (thresholdFirstVal == "") { thresholdFirstVal = "null";  }

                if (thresholdSecondVal == "") { thresholdSecondVal = "null"; }

                shouldBeValid = reader.GetValue(reader.GetOrdinal("Should be valid")).ToString();

                WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/aemileage.aspx/saveThreshold"));
                request13.Method = "POST";
                StringHttpBody request13Body = new StringHttpBody();
                request13Body.ContentType = "application/json; charset=utf-8";
                request13Body.InsertByteOrderMark = false;
                request13Body.BodyString = "{\"thresholdID\":0,\"mileageID\":" + journeyRateID + ",\"dateRangeID\":" + daterangeID.ToString() + ",\"rangetype\":" +
                    thresholdType + ",\"threshold1\":" + thresholdFirstVal + ",\"threshold2\":" + thresholdSecondVal + ",\"ppmPetrol\":" + ratePerMile + ",\"ppmDiesel\":" +
                    ratePerMile + ",\"ppmLpg\":" + ratePerMile + ",\"amountForVatP\":" + vatRate + ",\"amountForVatD\":" + vatRate + ",\"amountForVatLpg\":" + vatRate +
                    ",\"passenger\":" + passengerRate + ",\"passengerx\":" + passengerRate + ",\"heavyBulky\":" + passengerRate + "}";
                request13.Body = request13Body;
                yield return request13;
                request13 = null;

                // Check if the VJR should be valid

                // Check page for the tick box of the VJR just created
                WebTestRequest request15 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/admin/adminmileage.aspx"));
                ExtractText extractionRule6 = new ExtractText();
                extractionRule6.StartsWith = "Currency</a></th>";
                extractionRule6.EndsWith = journeyCatName;
                extractionRule6.IgnoreCase = false;
                extractionRule6.UseRegularExpression = false;
                extractionRule6.Required = true;
                extractionRule6.ExtractRandomMatch = false;
                extractionRule6.Index = 0;
                extractionRule6.HtmlDecode = false;
                extractionRule6.ContextParameterName = "";
                extractionRule6.ContextParameterName = "tickboxCheck";                
                request15.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule6.Extract);
                yield return request15;
                request15 = null;                                           

                if (reader.GetValue(reader.GetOrdinal("Should be valid")).ToString() == "True")
                {
                    if (this.Context["tickboxCheck"].ToString().Contains("input type=\"checkbox\" checked=\"checked\"") == true)
                    {
                        valid = true;
                    }
                    else
                    {
                        valid = false;
                    }
                }
                else
                {
                    if (this.Context["tickboxCheck"].ToString().Contains("input type=\"checkbox\" checked=\"checked\"") == true)
                    {
                        valid = false;
                    }
                    else
                    {
                        valid = true;
                    }
                }

                // Clear tickboxCheck from context for the next pass
                this.Context.Remove("tickboxCheck");

                // Validate the VJR

                WebTestRequest requestValidate = new WebTestRequest(this.Context["WebServer1"].ToString() + "/expenses/admin/adminmileage.aspx");
                if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.High))
                {
                    ValidationRuleFindText validationRule2 = new ValidationRuleFindText();

                    if (valid == true)
                    {
                        validationRule2.FindText = journeyCatName;
                    }
                    else
                    {
                        validationRule2.FindText = "jamesAutoTestValidationFail";
                    }

                    validationRule2.IgnoreCase = false;
                    validationRule2.UseRegularExpression = false;
                    validationRule2.PassIfTextFound = true;
                    requestValidate.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule2.Validate);
                }
                
                yield return requestValidate;
                requestValidate = null;
                //catNo = reader.GetFloat(0);
            }
            reader.Close();
        }
    }
}
