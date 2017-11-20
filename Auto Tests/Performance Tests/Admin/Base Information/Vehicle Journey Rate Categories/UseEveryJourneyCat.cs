
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;


    public class UseEveryJourneyCat : WebTest
    {

        public UseEveryJourneyCat()
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

            #region Extract required information from the database

            #region Create cDatabaseConnection

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString());

            #endregion


            #region Extract employeeID workAddressID and workAddressName

            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT companyid, company, employeeWorkLocations.employeeID " +
                "FROM companies INNER JOIN employeeWorkLocations ON employeeWorkLocations.locationID = companies.companyid " +
                "WHERE employeeWorkLocations.employeeID = (SELECT employeeID FROM employees WHERE username = 'james')");
            reader.Read();
            string workAddressID = reader.GetValue(0).ToString();
            string workAddressName = reader.GetValue(1).ToString();
            string employeeID = reader.GetValue(2).ToString();
            reader.Close();

            #endregion


            #region Extract homeAddressID and homeAddressName

            reader = database.GetReader("SELECT companyid, company FROM companies INNER JOIN employeeHomeLocations ON " +
                "employeeHomeLocations.locationID = companies.companyid WHERE " +
                "employeeHomeLocations.employeeID = (SELECT employeeID FROM employees WHERE username = 'james')");
            reader.Read();
            string homeAddressID = reader.GetValue(0).ToString();
            string homeAddressName = reader.GetValue(1).ToString();
            reader.Close();

            #endregion


            #region Extract carID

            reader = database.GetReader("SELECT carid FROM cars INNER JOIN employees ON employees.employeeid = cars.employeeid " +
                "WHERE employees.username = 'james' AND cars.active = '1'");
            reader.Read();
            string carID = reader.GetValue(0).ToString();
            reader.Close();

            #endregion


            #region Extract mileageCatIDs and mileageCatNames into the SortedList mileageCatInfo

            reader = database.GetReader("SELECT mileageid, carsize FROM mileage_categories WHERE carsize like '__Auto%' AND catvalid = '1'");
            SortedList<string, string> mileageCatInfo = new SortedList<string, string>();
            while (reader.Read())
            {
                mileageCatInfo.Add(reader.GetValue(0).ToString(), reader.GetValue(1).ToString());
            }
            reader.Close();

            #endregion


            #region Extract subcatID for the expense item 'Mileage'

            reader = database.GetReader("SELECT subcatid FROM subcats WHERE subcat = 'mileage'");
            reader.Read();
            string subcatID = reader.GetValue(0).ToString();
            reader.Close();

            #endregion


            #region Delete any current claims and add a new claim to use

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.DeleteClaim(), false)) { yield return r; }

            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.AddFirstClaim(), false)) { yield return r; }

            reader = database.GetReader("SELECT claimid FROM claims_base WHERE employeeid = '" + employeeID + "' AND status = 0");
            reader.Read();
            string claimID = reader.GetValue(0).ToString();
            reader.Close();

            #endregion


            #endregion


            #region Check that the user has a valid car (start and end date valid for the past few years)

            #endregion


            #region Check that auto VJRs exist and are available for the user



            #endregion


            #region Make sure Flags and Limits are set to allow claims for the past 3 years

            #endregion


            #region Use the SelectSpecificItem test to select mileage

            #endregion


            #region Login and get to aeexpense.aspx


            foreach (WebTestRequest r in IncludeWebTest(new Auto_Tests.Logon(UserType.Admin), false)) { yield return r; }

            WebTestRequest request5 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx"));
            request5.ThinkTime = 2;
            ExtractHiddenFields extractionRule2 = new ExtractHiddenFields();
            extractionRule2.Required = true;
            extractionRule2.HtmlDecode = true;
            extractionRule2.ContextParameterName = "1";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            ExtractText extractionRule3 = new ExtractText();
            extractionRule3.StartsWith = "accountid = ";
            extractionRule3.EndsWith = ";";
            extractionRule3.IgnoreCase = false;
            extractionRule3.UseRegularExpression = false;
            extractionRule3.Required = true;
            extractionRule3.ExtractRandomMatch = false;
            extractionRule3.Index = 0;
            extractionRule3.HtmlDecode = false;
            extractionRule3.ContextParameterName = "";
            extractionRule3.ContextParameterName = "accountID";
            request5.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule3.Extract);
            yield return request5;
            request5 = null;

            string accountID = this.Context["accountID"].ToString();


            #endregion

            string mileageDistance = string.Empty;
            string dateToUse = string.Empty;
            string mileageDateID = string.Empty;
            decimal amountPayable = 0;

            for (int x = 0; x < mileageCatInfo.Count; x++)
            {

                for (int y = 0; y < 3; y++)
                {
                    #region Set dateToUse
                    switch (y)
                    {
                        case 0:
                            dateToUse = "05/05/2007";
                            break;
                        case 1:
                            dateToUse = "05/05/2008";
                            break;
                        case 2:
                            dateToUse = "05/05/2009";
                            break;
                    }
                    #endregion

                    for (int z = 0; z < 3; z++)
                    {
                        #region Set mileageDistance
                        switch (z)
                        {
                            case 0:
                                mileageDistance = "5";
                                break;
                            case 1:
                                mileageDistance = "55";
                                break;
                            case 2:
                                mileageDistance = "105";
                                break;
                        }
                        #endregion


                        #region The actual adding Mileage bit

                        WebTestRequest request6 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/usercontrols/FileUploadIFrame.aspx"));
                        request6.ThinkTime = 1;
                        request6.QueryStringParameters.Add("tablename", "cars_attachments", false, false);
                        request6.QueryStringParameters.Add("idfield", "id", false, false);
                        request6.QueryStringParameters.Add("recordid", "0", false, false);
                        request6.QueryStringParameters.Add("mdlcancel", "ctl00_contentmain_addCar_cmdCarDocAttachCancel", false, false);
                        request6.QueryStringParameters.Add("multipleAttachments", "0", false, false);
                        request6.ParseDependentRequests = false;
                        yield return request6;
                        request6 = null;

                        WebTestRequest request7 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAutocomplete.asmx/getMileageCategoriesByCar"));
                        request7.Method = "POST";
                        StringHttpBody request7Body = new StringHttpBody();
                        request7Body.ContentType = "application/json; charset=utf-8";
                        request7Body.InsertByteOrderMark = false;
                        request7Body.BodyString = "{\"knownCategoryValues\":\"undefined:" + carID + ";\",\"category\":\"cars\",\"contextKey\":\"" + accountID + "," + employeeID + "\"}";
                        request7.ParseDependentRequests = false;
                        request7.Body = request7Body;
                        yield return request7;
                        request7 = null;

                        WebTestRequest request8 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx/getMileageComment"));
                        request8.ThinkTime = 12;
                        request8.Method = "POST";
                        StringHttpBody request8Body = new StringHttpBody();
                        request8Body.ContentType = "application/json; charset=utf-8";
                        request8Body.InsertByteOrderMark = false;
                        request8Body.BodyString = "{\"id\":\"0\",\"accountid\":" + accountID + ",\"employeeid\":\"" + employeeID + "\",\"carid\":\"" + carID + "\",\"mileageid\":\"1\",\"" +
                            "date\":\"2010/06/10\",\"subcatid\":\"" + subcatID + "\"}";
                        request8.ParseDependentRequests = false;
                        request8.Body = request8Body;
                        yield return request8;
                        request8 = null;

                        WebTestRequest request9 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcLocationSearchModal.asmx/CheckAddressLocation"));
                        request9.ThinkTime = 3;
                        request9.Method = "POST";
                        StringHttpBody request9Body = new StringHttpBody();
                        request9Body.ContentType = "application/json; charset=utf-8";
                        request9Body.InsertByteOrderMark = false;
                        request9Body.BodyString = "{\"prefixText\":\"10/06/2010|home\",\"companyType\":1,\"activeTextbox\":\"ctl00_contentmai" +
                            "n_txtfromid0_0\",\"activeHiddenField\":\"ctl00_contentmain_txtfrom0_0\"}";
                        request9.ParseDependentRequests = false;
                        request9.Body = request9Body;
                        yield return request9;
                        request9 = null;

                        WebTestRequest request10 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcLocationSearchModal.asmx/FetchAddressByAddressID"));
                        request10.Method = "POST";
                        StringHttpBody request10Body = new StringHttpBody();
                        request10Body.ContentType = "application/json; charset=utf-8";
                        request10Body.InsertByteOrderMark = false;
                        request10Body.BodyString = "{\"addressID\":\"" + homeAddressID + "\"}";
                        request10.ParseDependentRequests = false;
                        request10.Body = request10Body;
                        yield return request10;
                        request10 = null;

                        WebTestRequest request12 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx/GetDistance"));
                        request12.ThinkTime = 5;
                        request12.Method = "POST";
                        StringHttpBody request12Body = new StringHttpBody();
                        request12Body.ContentType = "application/json; charset=utf-8";
                        request12Body.InsertByteOrderMark = false;
                        request12Body.BodyString = "{\"fromCompanyID\":\"" + homeAddressID + "\",\"toCompanyID\":\"" + workAddressID + "\",\"date\":\"2010/06/10\",\"carID\":\"" + carID + "" +
                            "\"}";
                        request12.Body = request12Body;
                        ExtractText extractionRule4 = new ExtractText();
                        extractionRule4.StartsWith = ",";
                        extractionRule4.EndsWith = "]";
                        extractionRule4.Required = true;
                        extractionRule4.Index = 0;
                        extractionRule4.HtmlDecode = false;
                        extractionRule4.ContextParameterName = "";
                        extractionRule4.ContextParameterName = "distance";
                        request12.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule4.Extract);
                        yield return request12;
                        request12 = null;

                        //string mileageDistance = this.Context["distance"].ToString();

                        WebTestRequest request13 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx/getMileageComment"));
                        request13.Method = "POST";
                        StringHttpBody request13Body = new StringHttpBody();
                        request13Body.ContentType = "application/json; charset=utf-8";
                        request13Body.InsertByteOrderMark = false;
                        request13Body.BodyString = "{\"id\":\"0\",\"accountid\":" + accountID + ",\"employeeid\":\"" + employeeID + "\",\"carid\":\"" + carID + "\",\"mileageid\":\"1\",\"" +
                            "date\":\"2010/06/10\",\"subcatid\":\"" + subcatID + "\"}";
                        request13.ParseDependentRequests = false;
                        request13.Body = request13Body;
                        yield return request13;
                        request13 = null;

                        WebTestRequest request14 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/shared/webServices/svcAutocomplete.asmx/getMileageCategoriesByCar"));
                        request14.Method = "POST";
                        StringHttpBody request14Body = new StringHttpBody();
                        request14Body.ContentType = "application/json; charset=utf-8";
                        request14Body.InsertByteOrderMark = false;
                        request14Body.BodyString = "{\"knownCategoryValues\":\"undefined:" + carID + ";\",\"category\":\"cars\",\"contextKey\":\"" + accountID + "," + employeeID + "\"}";
                        request14.ParseDependentRequests = false;
                        request14.Body = request14Body;
                        yield return request14;
                        request14 = null;

                        WebTestRequest request15 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx/getMileageComment"));
                        request15.ThinkTime = 11;
                        request15.Method = "POST";
                        StringHttpBody request15Body = new StringHttpBody();
                        request15Body.ContentType = "application/json; charset=utf-8";
                        request15Body.InsertByteOrderMark = false;
                        request15Body.BodyString = "{\"id\":\"0\",\"accountid\":" + accountID + ",\"employeeid\":\"" + employeeID + "\",\"carid\":\"" + carID + "\",\"mileageid\":\"" + mileageCatInfo.Keys[x] + "\"" +
                            ",\"date\":\"2010/06/10\",\"subcatid\":\"" + subcatID + "\"}";
                        request15.ParseDependentRequests = false;
                        request15.Body = request15Body;
                        yield return request15;
                        request15 = null;

                        WebTestRequest request16 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/aeexpense.aspx"));
                        request16.Method = "POST";
                        request16.ExpectedResponseUrl = (this.Context["WebServer1"].ToString() + "/expenses/claimViewer.aspx?claimid=" + claimID);
                        FormPostHttpBody request16Body = new FormPostHttpBody();
                        request16Body.FormPostParameters.Add("__EVENTTARGET", "ctl00$contentmain$cmdok");
                        request16Body.FormPostParameters.Add("__EVENTARGUMENT", this.Context["$HIDDEN1.__EVENTARGUMENT"].ToString());
                        request16Body.FormPostParameters.Add("employeeid", this.Context["$HIDDEN1.employeeid"].ToString());
                        request16Body.FormPostParameters.Add("accountid", this.Context["$HIDDEN1.accountid"].ToString());
                        request16Body.FormPostParameters.Add("__LASTFOCUS", this.Context["$HIDDEN1.__LASTFOCUS"].ToString());


                        // Need the countryID, currencyID, departmentID, costcodeID, projectcodeID

                        request16Body.FormPostParameters.Add("ctl00_contentmain_addCar_TabContainer1_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true]}");
                        request16Body.FormPostParameters.Add("ctl00_contentmain_addressSearch_tabContainerLocationSearch_ClientState", "{\"ActiveTabIndex\":0,\"TabState\":[true,true,true,true]}");
                        request16Body.FormPostParameters.Add("__VIEWSTATE", this.Context["$HIDDEN1.__VIEWSTATE"].ToString());
                        request16Body.FormPostParameters.Add("ctl00$contentleft$chkitems$41", "on");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$hdnExpDate", this.Context["$HIDDEN1.ctl00$contentmain$hdnExpDate"].ToString());
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtdate", dateToUse);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$_ClientState"].ToString());
                        request16Body.FormPostParameters.Add("ctl00$contentmain$valcalltxtdate_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$valcalltxtdate_ClientState"].ToString());
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbclaims", claimID);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbcountry", "78");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbcurrency", "2883");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtexchangerate", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtotherdetails", "Testing VJRs");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbdepartment0", "139");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbcostcode0", "245");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbprojectcode0", "26");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtpercentage0", "100");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtrows", "1");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtcostcodetotal", "100");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtnumitems", "1");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtsubcatid0", subcatID);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtexpenseid0", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtadvance0", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtfromidpreviousID_0_0", this.Context["$HIDDEN1.ctl00$contentmain$txtfromidpreviousID_0_0"].ToString());
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtfromid0_0", homeAddressID);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtfrom0_0", homeAddressName);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txttoidpreviousID_0_0", this.Context["$HIDDEN1.ctl00$contentmain$txttoidpreviousID_0_0"].ToString());
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txttoid0_0", workAddressID);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtto0_0", workAddressName);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtmileage0_0", mileageDistance);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtCalcMiles0_0", mileageDistance);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtpassengers0_0", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbcars0", carID);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$casmileage0_ClientState", mileageCatInfo.Keys[x] + ":::" + mileageCatInfo.Values[x]);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$cmbmileagecat0", mileageCatInfo.Keys[x]);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$txtmileagecat0", mileageCatInfo.Keys[x]);
                        request16Body.FormPostParameters.Add("ctl00$contentmain$vatreceipt0", "optvatreceiptyes0");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$TabContainer1$tabGenDets$txtmake", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$TabContainer1$tabGenDets$txtmodel", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$TabContainer1$tabGenDets$txtregno", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$TabContainer1$tabGenDets$cmbUom", "0");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$TabContainer1$tabGenDets$cmbcartype", "0");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$TabContainer1$tabGenDets$txtEngineSize", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$txtReadingDate", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$mskodoreadingdate_ClientState", this.Context["$HIDDEN1.ctl00$contentmain$addCar$mskodoreadingdate_ClientState"].ToString());
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$txtOldReading", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addCar$txtNewReading", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabFindAddress$txtFindAddressAddressName", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabFindAddress$txtFindAddressAddressLine1", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabFindAddress$txtFindAddressAddressLine2", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabFindAddress$txtFindAddressCity", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabFindAddress$txtFindAddressCounty", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabFindAddress$txtFindAddressPostCode", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabFindAddress$ddlFindAddressCountry", "78");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabAddNewAddress$txtAddNewAddressAddressName", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabAddNewAddress$txtAddNewAddressAddressLine1", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabAddNewAddress$txtAddNewAddressAddressLine2", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabAddNewAddress$txtAddNewAddressCity", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabAddNewAddress$txtAddNewAddressCounty", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabAddNewAddress$txtAddNewAddressPostCode", "");
                        request16Body.FormPostParameters.Add("ctl00$contentmain$addressSearch$tabContainerLocationSearch$tabAddNewAddress$ddlAddNewAddressCountry", "78");
                        request16Body.FormPostParameters.Add("hiddenInputToUpdateATBuffer_CommonToolkitScripts", "0");
                        request16.ParseDependentRequests = false;
                        request16.Body = request16Body;
                        yield return request16;
                        request16 = null;


                        #endregion


                        #region Validate the correct rate has been used


                        #region Calculate the correct Journey rate to use, based on the date of the expense

                        mileageDateID = string.Empty;

                        reader = database.GetReader("SELECT mileagedateid, datevalue1, datevalue2, daterangetype FROM mileage_dateranges " +
                                                                                    "WHERE mileageid =" + mileageCatInfo.Keys[x]);
                        while (reader.Read() && mileageDateID == string.Empty)
                        {
                            if (reader.GetByte(3) == 3)
                            {
                                // Date range of 'any' will be used for all dates
                                mileageDateID = reader.GetValue(0).ToString();
                            }
                            else if (reader.GetByte(3) == 0 && (DateTime.Parse(dateToUse) < reader.GetDateTime(1)))
                            {
                                // Date range of 'Before'
                                mileageDateID = reader.GetValue(0).ToString();
                            }
                            else if (reader.GetByte(3) == 1 && (DateTime.Parse(dateToUse) > reader.GetDateTime(1)))
                            {
                                // Date range of 'After'
                                mileageDateID = reader.GetValue(0).ToString();
                            }
                            else if (reader.GetByte(3) == 2 && (DateTime.Parse(dateToUse) > reader.GetDateTime(1)) && (DateTime.Parse(dateToUse) < reader.GetDateTime(2)))
                            {
                                // Date range of 'Between'
                                mileageDateID = reader.GetValue(0).ToString();
                            }
                        }
                        reader.Close();

                        #endregion


                        #region Calculate amount payable according to the correct threshold

                        amountPayable = 0;
                        reader = database.GetReader("SELECT rangetype, rangevalue1, rangevalue2, ppmpetrol FROM mileage_thresholds " +
                                                                                    "WHERE mileagedateid = " + mileageDateID);
                        while (reader.Read() && amountPayable == 0)
                        {
                            if (reader.GetByte(0) == 3)
                            {
                                // Threshold is 'any' and can be worked out for all of them
                                amountPayable = decimal.Round((decimal.Parse(mileageDistance) * reader.GetDecimal(3)),2);
                            }
                            else if (reader.GetByte(0) == 0 && (int.Parse(mileageDistance) > reader.GetInt32(1)))
                            {
                                // Threshold is 'greater than' and the distance being used is greater
                                amountPayable = decimal.Round((decimal.Parse(mileageDistance) * reader.GetDecimal(3)), 2);
                            }
                            else if (reader.GetByte(0) == 1 && (int.Parse(mileageDistance) > reader.GetInt32(1)) && (int.Parse(mileageDistance) < reader.GetInt32(2)))
                            {
                                // Threshold is 'between' and the distance being used is between
                                amountPayable = decimal.Round((decimal.Parse(mileageDistance) * reader.GetDecimal(3)), 2);
                            }
                            else if (reader.GetByte(0) == 2 && (int.Parse(mileageDistance) < reader.GetInt32(1)))
                            {
                                // Threshold is 'less than' and the distance being used is less than
                                amountPayable = decimal.Round((decimal.Parse(mileageDistance) * reader.GetDecimal(3)), 2);
                            }
                        }
                        reader.Close();

                        #endregion


                        #region Validate the amount payable

                        WebTestRequest request17 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/claimViewer.aspx?claimid=" + claimID));
                        AutoTools.ValidateText("£" + amountPayable, request17);
                        yield return request17;
                        request17 = null;

                        #endregion 


                        #endregion


                        #region Delete the expense item before continuing

                        WebTestRequest request18 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/claimViewer.aspx?claimid=" + claimID));
                        ExtractText extractionRule5 = new ExtractText();
                        extractionRule5.StartsWith = "(262,";
                        extractionRule5.EndsWith = ",";
                        extractionRule5.Required = true;
                        extractionRule5.Index = 0;
                        extractionRule5.HtmlDecode = false;
                        extractionRule5.ContextParameterName = "";
                        extractionRule5.ContextParameterName = "expenseItemID";
                        request18.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule5.Extract);
                        yield return request18;
                        request18 = null;

                        WebTestRequest request19 = new WebTestRequest((this.Context["WebServer1"].ToString() + "/expenses/claimViewer.aspx/deleteExpense"));
                        request19.Method = "POST";
                        StringHttpBody request19Body = new StringHttpBody();
                        request19Body.ContentType = "application/json; charset=utf-8";
                        request19Body.InsertByteOrderMark = false;
                        request19Body.BodyString = "{\"accountid\":" + accountID + ",\"expenseid\":" + this.Context["expenseItemID"].ToString() + ",\"claimid\":" + claimID + ",\"employeeid\":" + employeeID + "}";
                        request19.Body = request19Body;
                        yield return request19;
                        request19 = null;

                        #endregion

                        // Remove stuff from context before the next iteration
                        this.Context.Remove("expenseItemID");
                    }
                }
            }

        }
    }
}
