
namespace Auto_Tests
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices;
    using System.IO;
    using System.Xml;
    using Microsoft.VisualStudio.TestTools.WebTesting;
    using Microsoft.VisualStudio.TestTools.WebTesting.Rules;
    using System.Text.RegularExpressions;

    public class MoveFiles : WebTest
    {

        public MoveFiles()
        {
            this.PreAuthenticate = true;
        }

        public override IEnumerator<WebTestRequest> GetRequestEnumerator()
        {
            #region Initialize validation
            // Initialize validation rules that apply to all requests in the WebTest
            if ((this.Context.ValidationLevel >= Microsoft.VisualStudio.TestTools.WebTesting.ValidationLevel.Low))
            {
                ValidateResponseUrl validationRule1 = new ValidateResponseUrl();
                this.ValidateResponse += new EventHandler<ValidationEventArgs>(validationRule1.Validate);
            }
            #endregion Initialize validation


            #region Non-required WebTestRequest

            // Non-required test - changed to www.sel-expenses.com to avoid potential errors with ServerToUse()
            WebTestRequest request1 = new WebTestRequest("https://www.sel-expenses.com/shared/logon.aspx");
            request1.ThinkTime = 8;
            request1.ParseDependentRequests = false;
            ExtractHiddenFields extractionRule1 = new ExtractHiddenFields();
            extractionRule1.Required = false;
            extractionRule1.HtmlDecode = true;
            extractionRule1.ContextParameterName = "1";
            request1.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule1.Extract);
            yield return request1;
            request1 = null;

            #region Extract FoD

            WebTestRequest request6 = new WebTestRequest("http://www.bbc.co.uk/");
            request6.ParseDependentRequests = false;            
            ExtractText extractionRule2 = new ExtractText();
            extractionRule2.StartsWith = "Fact of the day</a>";
            extractionRule2.EndsWith = "/p>";
            extractionRule2.Required = false;
            extractionRule2.HtmlDecode = false;
            extractionRule2.ContextParameterName = "";
            extractionRule2.ContextParameterName = "factOfDay";
            request6.ExtractValues += new EventHandler<ExtractionEventArgs>(extractionRule2.Extract);
            yield return request6;
            request6 = null;

            string factOfDay = string.Empty;

            try
            {
                factOfDay = AutoTools.GetID(this.Context["factOfDay"].ToString(), ">", "<", 1);
            }
            catch
            {
                factOfDay = "Could not get todays Fact of the day. Booooo!";
            }

            #endregion

            #endregion Non-required WebTestRequest

            // Moved
            #region Extract build information

            #region Get build settings from the log file

            DirectoryInfo builds = new DirectoryInfo(@"\\BUILDSERVER\Builds");

            DateTime latest = new DateTime();
            string buildName = string.Empty;

            foreach (DirectoryInfo directory in builds.GetDirectories())
            {
                DateTime dt = directory.LastWriteTime;
                if (dt > latest)
                {
                    latest = dt;
                    buildName = directory.Name;
                }
            }

            #endregion

            #region Get run settings from Lithium

            cDatabaseConnection database = new cDatabaseConnection(AutoTools.DatabaseConnectionString(true));
            System.Data.SqlClient.SqlDataReader reader = database.GetReader("SELECT * FROM RunTimeSettings WHERE BuildName = '" + buildName + "'");
            reader.Read();

            string buildMode = reader.GetValue(1).ToString();
            bool buildExpenses = reader.GetBoolean(2);
            bool buildFramework = reader.GetBoolean(3);
            string expensesWebAddress = reader.GetValue(4).ToString();
            string frameworkWebAddress = reader.GetValue(5).ToString();
            string expensesFolder = reader.GetValue(6).ToString();
            string frameworkFolder = reader.GetValue(7).ToString();
            string expensesCompanyID = reader.GetValue(8).ToString();
            string frameworkCompanyID = reader.GetValue(9).ToString();
            bool updateServices = reader.GetBoolean(10);
            string exReportsService = reader.GetValue(11).ToString();
            string exSchedulerService = reader.GetValue(12).ToString();
            string fwReportsService = reader.GetValue(13).ToString();
            string fwSchedulerService = reader.GetValue(14).ToString();
            string cDatabaseConnectionString = reader.GetValue(15).ToString();
            string resultsTable = reader.GetValue(16).ToString();
            string resultsRow = reader.GetValue(17).ToString();
            string tableInfo = string.Empty;
            reader.Close();

            #endregion

            #endregion

            // Moved
            #region Update Web.config file for unit tests

            tableInfo += resultsRow.Replace("taskreplace", "Update web.config file for unit tests");
            try
            {
                XmlDocument webConfig = new XmlDocument();
                webConfig.Load(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\_PublishedWebsites\expenses\Web.config");

                XmlNode node;
                node = webConfig.DocumentElement;

                foreach (XmlNode currentNode in node.ChildNodes)

                    if (currentNode.Name == "connectionStrings")
                    {
                        currentNode.InnerXml += "<add name=\"ESRFileTransfer\" connectionString=\"Data Source=testdb2;Initial Catalog=ESRFileTransfer;User ID=ESRUser;Password=53RL0g1n;Max Pool Size=10000;\" providerName=\"System.Data.SqlClient\" />";
                    }
                    else if (currentNode.Name == "appSettings")
                    {
                        currentNode.InnerXml += "<add key=\"inUnitTest\" value=\"256,517\" />";
                    }

                webConfig.Save(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\_PublishedWebsites\expenses\Web.config");

                tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
            }
            catch (Exception e)
            {
                tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
            }

            #endregion

            // Moved
            #region Send start email

            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient("oxygen");

            if (buildExpenses || buildFramework)
            {
                int time = DateTime.Now.TimeOfDay.Hours;
                string timeOfDay = string.Empty;
                if (time < 12)
                {
                    timeOfDay = "Morning";
                }
                else if (time > 11 && time < 17)
                {
                    timeOfDay = "Afternoon";
                }
                else if (time > 16)
                {
                    timeOfDay = "Evening";
                }
                System.Net.Mail.MailMessage startMessage = new System.Net.Mail.MailMessage();
                startMessage.IsBodyHtml = true;
                startMessage.To.Add("testers@software-europe.co.uk");
                startMessage.CC.Add("darren-newton@software-europe.co.uk, simon.davis@software-europe.co.uk, ben.jackson@software-europe.co.uk, martin.walker@software-europe.co.uk, paul.lancashire@software-europe.co.uk");
                startMessage.Subject = "New " + buildName + " release has been requested";
                startMessage.From = new System.Net.Mail.MailAddress("buildserver@sel-expenses.com");
                startMessage.Body = "<span style='font-size:10.0pt;font-family:\"Arial\",\"sans-serif\";color:black'>" +
                        timeOfDay + " all,<br><br>" +
                        "A new build " + " (" + buildName + ") has been requested." +
                        " I'm currently busy copying this across to the Test Server, " +
                        "so please allow a few minutes for this to complete before using the system.<br><br></span>" +
                        "<span style='font-size:9.0pt;font-family:\"Arial\",\"sans-serif\";color:gray'>Kind regards,<br></span><br>" +
                        "<b><span style='font-size:9.0pt;font-family:\"Arial\",\"sans-serif\";color:#404040'>Build Server</span></b><br>" +
                        "<span style='font-size:8.0pt;font-family:\"Arial\",\"sans-serif\";color:gray'>Continuous Integration Officer<br>" +
                        "Software Europe<br>Telephone: +44 (0)1522 881300 | Fax: +44 (0)1522 881355<br>testers@software-europe.co.uk</span>";
                smtp.Send(startMessage);
            }

            #endregion Send start email

            // Moved
            #region Set ServerToUse and copy files across

            #region Update Run-time settings

            if (buildExpenses || buildFramework)
            {
                tableInfo += resultsRow.Replace("taskreplace", "Update Runtime Settings on lithium");
                try
                {
                    database.ExecuteSQL("USE AutoTestingDataSources UPDATE ServerToUse SET Server = '" + expensesWebAddress + "', " +
                        "CompanyID = '" + expensesCompanyID + "', ExpensesServer = '" + expensesWebAddress + "', ExpensesCompanyID = '" + expensesCompanyID + "', " +
                        "FrameworkServer = '" + frameworkWebAddress + "', FrameworkCompanyID = '" + frameworkCompanyID + "'");

                    tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                }
                catch (Exception e)
                {
                    tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                }
            }

            #endregion


            #region Copy Expenses Files

            if (buildExpenses)
            {
                tableInfo += resultsRow.Replace("taskreplace", "Copy Expenses files across to Test Web 1");
                try
                {
                    CopyDir.CopyLatest("testweb1", expensesFolder, buildName, buildMode, "Expenses");
                    tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                }
                catch (Exception e)
                {
                    tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                }


                tableInfo += resultsRow.Replace("taskreplace", "Copy Expenses files across to Test Web 2");
                try
                {
                    CopyDir.CopyLatest("testweb2", expensesFolder, buildName, buildMode, "Expenses");
                    tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                }
                catch (Exception e)
                {
                    tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                }
            }

            #endregion


            #region Copy Framework Files

            if (buildFramework)
            {
                tableInfo += resultsRow.Replace("taskreplace", "Copy Framework files across to Test Web 1");
                try
                {
                    CopyDir.CopyLatest("testweb1", frameworkFolder, buildName, buildMode, "Framework");
                    tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                }
                catch (Exception e)
                {
                    tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                }


                tableInfo += resultsRow.Replace("taskreplace", "Copy Framework files across to Test Web 2");
                try
                {
                    CopyDir.CopyLatest("testweb2", frameworkFolder, buildName, buildMode, "Framework");
                    tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                }
                catch (Exception e)
                {
                    tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                }
            }

            #endregion


            #endregion Set ServerToUse and copy files accross

            // Moved
            #region Update any required services
            
            if (updateServices == true)
            {               
                AutoTools tools = new AutoTools();

                #region Stop the Expenses services and copy the files across

                if (buildExpenses)
                {
                    tableInfo += resultsRow.Replace("taskreplace", "Stop and update the required services for Expenses");

                    try
                    {
                        tools.StartStopService(exReportsService, "192.168.111.10", "stop");
                        tools.StartStopService(exSchedulerService, "192.168.111.10", "stop");
                        System.Threading.Thread.Sleep(5000);

                        #region Copy across the Reports files

                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Reports\Expenses_Reports.exe").Delete();
                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Reports\ExpensesLibrary.dll").Delete();
                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Reports\SpendManagementLibrary.dll").Delete();

                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\Expenses_Reports.exe").CopyTo(@"\\TESTWEB1\expenses2010_testing\Reports\Expenses_Reports.exe", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\ExpensesLibrary.dll").CopyTo(@"\\TESTWEB1\expenses2010_testing\Reports\ExpensesLibrary.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\SpendManagementLibrary.dll").CopyTo(@"\\TESTWEB1\expenses2010_testing\Reports\SpendManagementLibrary.dll", true);

                        #endregion


                        #region Copy across the Scheduler files

                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Scheduler\Expenses_Scheduler.exe").Delete();
                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Scheduler\Spend Management.dll").Delete();
                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Scheduler\ExpensesLibrary.dll").Delete();
                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Scheduler\SpendManagementHelpers.dll").Delete();
                        new FileInfo(@"\\TESTWEB1\expenses2010_testing\Scheduler\SpendManagementLibrary.dll").Delete();

                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\Expenses_Scheduler.exe").CopyTo(@"\\TESTWEB1\expenses2010_testing\Scheduler\Expenses_Scheduler.exe", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\Spend Management.dll").CopyTo(@"\\TESTWEB1\expenses2010_testing\Scheduler\Spend Management.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\ExpensesLibrary.dll").CopyTo(@"\\TESTWEB1\expenses2010_testing\Scheduler\ExpensesLibrary.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\SpendManagementHelpers.dll").CopyTo(@"\\TESTWEB1\expenses2010_testing\Scheduler\SpendManagementHelpers.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\SpendManagementLibrary.dll").CopyTo(@"\\TESTWEB1\expenses2010_testing\Scheduler\SpendManagementLibrary.dll", true);

                        #endregion

                        tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                    }
                    catch (Exception e)
                    {
                        tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                    }

                }

                #endregion


                #region Stop the Framework services and copy the files across

                if (buildFramework)
                {
                    tableInfo += resultsRow.Replace("taskreplace", "Stop and update the required services for Framework");

                    try
                    {
                        tools.StartStopService(fwReportsService, "192.168.111.10", "stop");
                        tools.StartStopService(fwSchedulerService, "192.168.111.10", "stop");
                        System.Threading.Thread.Sleep(5000);

                        #region Copy across the Reports files

                        new FileInfo(@"\\testweb1\SEL_ReportEngine\SEL_ReportEngine.exe").Delete();
                        new FileInfo(@"\\testweb1\SEL_ReportEngine\FWReportsLibrary.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_ReportEngine\FWClasses.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_ReportEngine\FWBase.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_ReportEngine\SpendManagementLibrary.dll").Delete();


                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\SEL_ReportEngine.exe").CopyTo(@"\\testweb1\SEL_ReportEngine\SEL_ReportEngine.exe", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\FWReportsLibrary.dll").CopyTo(@"\\testweb1\SEL_ReportEngine\FWReportsLibrary.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\FWClasses.dll").CopyTo(@"\\testweb1\SEL_ReportEngine\FWClasses.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\FWBase.dll").CopyTo(@"\\testweb1\SEL_ReportEngine\FWBase.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\SpendManagementLibrary.dll").CopyTo(@"\\testweb1\SEL_ReportEngine\SpendManagementLibrary.dll", true);

                        #endregion


                        #region Copy across the Scheduler files

                        new FileInfo(@"\\testweb1\SEL_Scheduler\csvParser.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_Scheduler\FWBase.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_Scheduler\FWClasses.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_Scheduler\FWCommon.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_Scheduler\FWReportsLibrary.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_Scheduler\SEL_Scheduler.exe").Delete();
                        new FileInfo(@"\\testweb1\SEL_Scheduler\SpendManagementLibrary.dll").Delete();
                        new FileInfo(@"\\testweb1\SEL_Scheduler\Spend Management.dll").Delete();

                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\csvParser.dll").CopyTo(@"\\testweb1\SEL_Scheduler\csvParser.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\FWBase.dll").CopyTo(@"\\testweb1\SEL_Scheduler\FWBase.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\FWClasses.dll").CopyTo(@"\\testweb1\SEL_Scheduler\FWClasses.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\FWCommon.dll").CopyTo(@"\\testweb1\SEL_Scheduler\FWCommon.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\FWReportsLibrary.dll").CopyTo(@"\\testweb1\SEL_Scheduler\FWReportsLibrary.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\SEL_Scheduler.exe").CopyTo(@"\\testweb1\SEL_Scheduler\SEL_Scheduler.exe", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\SpendManagementLibrary.dll").CopyTo(@"\\testweb1\SEL_Scheduler\SpendManagementLibrary.dll", true);
                        new FileInfo(@"\\BUILDSERVER\Continuous Integration\Source\Binaries\Spend Management.dll").CopyTo(@"\\testweb1\SEL_Scheduler\Spend Management.dll", true);

                        #endregion

                        tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                    }
                    catch (Exception e)
                    {
                        tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                    }

                }

                #endregion


                #region Start the Expenses Services

                if (buildExpenses)
                {
                    tableInfo += resultsRow.Replace("taskreplace", "Re-start the services for Expenses");

                    try
                    {
                        tools.StartStopService(exReportsService, "192.168.111.10", "start");
                        tools.StartStopService(exSchedulerService, "192.168.111.10", "start");
                        tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                    }
                    catch (Exception e)
                    {
                        tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                    }
                }

                #endregion


                #region Start the Framework Services

                if (buildFramework)
                {
                    tableInfo += resultsRow.Replace("taskreplace", "Re-start the services for Framework");

                    try
                    {
                        tools.StartStopService(fwReportsService, "192.168.111.10", "start");
                        tools.StartStopService(fwSchedulerService, "192.168.111.10", "start");
                        tableInfo = tableInfo.Replace("colourreplace", "00CC00").Replace("statusreplace", "Complete").Replace("detailsreplace", "-");
                    }
                    catch (Exception e)
                    {
                        tableInfo = tableInfo.Replace("colourreplace", "FF0000").Replace("statusreplace", "Error").Replace("detailsreplace", e.Message.ToString());
                    }
                }

                #endregion

            }

            #endregion Update any required services

            // Moved
            #region Send finish email

            if (buildExpenses || buildFramework)
            {
                #region Pick a fancy colour for the results table

                int random = int.Parse(System.DateTime.Now.Millisecond.ToString());

                random = random / 200;

                reader = database.GetReader("SELECT * FROM Colours WHERE ColourNumber = " + random);
                reader.Read();
                string headerColour = reader.GetValue(1).ToString();
                string rowColour = reader.GetValue(2).ToString();
                reader.Close();

                resultsTable = resultsTable.Replace("#666666", headerColour).Replace("#E8E9DF", rowColour);
                tableInfo = tableInfo.Replace("#666666", headerColour).Replace("#E8E9DF", rowColour);


                #endregion

                System.Net.Mail.MailMessage finishMessage = new System.Net.Mail.MailMessage();
                finishMessage.IsBodyHtml = true;
                finishMessage.To.Add("testers@software-europe.co.uk");
                finishMessage.CC.Add("darren-newton@software-europe.co.uk, simon.davis@software-europe.co.uk, ben.jackson@software-europe.co.uk, martin.walker@software-europe.co.uk, paul.lancashire@software-europe.co.uk");
                finishMessage.From = new System.Net.Mail.MailAddress("buildserver@sel-expenses.com");
                finishMessage.Subject = "Completed - New " + buildName + " release for testing";
                finishMessage.Body = "<span style='font-size:10.0pt;font-family:\"Arial\",\"sans-serif\";color:black'>" +
                    "A new build (" + buildName + ") has been placed onto the Test Server. <br><br>" +
                    "A breakdown of the build process is shown below:<br><br>" + resultsTable + tableInfo +
                    "</table><br><br>Please feel free to use the website while I continue to run automated tests.<br><br>" +
                    "For those who are interested, the new version can be found at <a href=\"" + expensesWebAddress + "\">the testing website for this branch.</a></span>" +
                    "<br><br><span style='font-size:9.0pt;font-family:\"Arial\",\"sans-serif\";color:gray'>Kind regards,<br></span><br>" +
                    "<b><span style='font-size:9.0pt;font-family:\"Arial\",\"sans-serif\";color:#404040'>Build Server</span></b><br>" +
                    "<span style='font-size:8.0pt;font-family:\"Arial\",\"sans-serif\";color:gray'>Continuous Integration Officer<br>" +
                    "Software Europe<br>Telephone: +44 (0)1522 881300 | Fax: +44 (0)1522 881355<br>testers@software-europe.co.uk</span>" +
                    "<br><br><span style='font-size:10.0pt;font-family:\"Arial\",\"sans-serif\";color:white'><b>Fact of the day:</b><br><br>" +
                    factOfDay + "</span>";
                smtp.Send(finishMessage);
            }
            #endregion Send finish email
        }
    }


    class CopyDir
    {
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            // Check if the target directory exists, if not, create it
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            // Copy each file into its new directory                        
            foreach (FileInfo fi in source.GetFiles())
            {
                if (fi.Name != "Web.config")
                {
                    // PUT THIS IN A TRY AND OUTPUT THE FILE THAT COULD NOT BE COPIED
                    fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);
                }
            }

            // Copy each subdirectory using recursion
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir = target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }


        public static void CopyLatest(string webserver, string folderName, string buildName, string buildMode, string buildProject)
        {
            #region Copy Expenses files across

            if (buildProject == "Expenses")
            {

                #region Set the directories to use

                DirectoryInfo diEXBuild = new DirectoryInfo(@"\\buildserver\" + buildName + @"\Source\Binaries\_PublishedWebsites\expenses");
                DirectoryInfo diEXTarget = new DirectoryInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses");

                DirectoryInfo diSMBuild = new DirectoryInfo(@"\\buildserver\" + buildName + @"\Source\Binaries\_PublishedWebsites\Spend Management");
                DirectoryInfo diSMTarget = new DirectoryInfo(@"\\" + webserver + @"\" + folderName + @"\Site\Spend Management");

                DirectoryInfo contImages = new DirectoryInfo(@"\\BUILDSERVER\Continuous Integration\Logon Images");
                DirectoryInfo testWebImages = new DirectoryInfo(@"\\" + webserver + @"\" + folderName + @"\Site\Spend Management\shared\images\logonimages");

                #endregion
                

                #region Delte files in target folder

                try
                {
                    new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\icons\48\Plain\trafficlight_green.gif").IsReadOnly = false;
                    new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\icons\48\Shadow\trafficlight_green.gif").IsReadOnly = false;
                }
                catch { }

                if (diEXTarget.Exists == true)
                {
                    try
                    {
                        diEXTarget.Delete(true);
                    }
                    catch { }
                }

                if (diSMTarget.Exists == true)
                {
                    try
                    {
                        diSMTarget.Delete(true);
                    }
                    catch { }
                }

                #endregion


                #region Copy across the latest files
                  
                CopyAll(diEXBuild, diEXTarget);

                CopyAll(diSMBuild, diSMTarget);

                CopyAll(contImages, testWebImages);

                #endregion


                #region Copy the correct .dll's

                try
                {
                   // new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\AjaxControlToolkit.dll").Delete();
                    //new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Base.dll").Delete();
                    //new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Web.dll").Delete();
                    //new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Tools.Web.dll").Delete();
                }
                catch { }

                try
                {
                    //new FileInfo(@"\\buildserver\Continuous Integration\Ajax\Syncfusion.Shared.Base.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Base.dll");
                    //new FileInfo(@"\\buildserver\Continuous Integration\Ajax\Syncfusion.Shared.Web.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Web.dll");
                    //new FileInfo(@"\\buildserver\Continuous Integration\Ajax\Syncfusion.Tools.Web.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Tools.Web.dll");
                  //  new FileInfo(@"\\buildserver\Continuous Integration\Ajax\AjaxControlToolkit.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\AjaxControlToolkit.dll");
                }
                catch { }

                #endregion


                #region Copy the correct web.config files

                new FileInfo(@"\\buildserver\Continuous Integration\Config File\Expenses\Web.config").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\Expenses\Web.config");
                //new FileInfo(@"\\buildserver\Continuous Integration\Config File\Spend Management\Web.config").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\Spend Management\Web.config");

                #endregion
                
            }

            #endregion


            #region Copy Framework files across

            else if (buildProject == "Framework")
            {
                #region Set the directories to use

                DirectoryInfo diFWBuild = new DirectoryInfo(@"\\buildserver\" + buildName + @"\Source\Binaries\_PublishedWebsites\Framework2006");

                DirectoryInfo diSMBuild = new DirectoryInfo(@"\\buildserver\" + buildName + @"\Source\Binaries\_PublishedWebsites\Spend Management");

                DirectoryInfo diFWTarget = new DirectoryInfo(@"\\" + webserver + @"\" + folderName);

                DirectoryInfo diSMTarget = new DirectoryInfo(@"\\" + webserver + @"\Spend Management");
                
                //DirectoryInfo contImages = new DirectoryInfo(@"\\BUILDSERVER\Continuous Integration\Logon Images");
                //DirectoryInfo testWebImages = new DirectoryInfo(@"\\" + webserver + @"\" + folderName + @"\Site\Spend Management\shared\images\logonimages");

                #endregion


                #region Delte files in target folder

                if (diFWTarget.Exists == true)
                {
                    try
                    {
                        diFWTarget.Delete(true);
                    }
                    catch { }
                }

                if (diSMTarget.Exists == true)
                {
                    try
                    {
                        diSMTarget.Delete(true);
                    }
                    catch { }
                }

                #endregion


                #region Copy across the latest files

                CopyAll(diFWBuild, diFWTarget);

                CopyAll(diSMBuild, diSMTarget);

                //CopyAll(contImages, testWebImages);

                #endregion


                #region Copy the correct .dll's

                //try
                //{
                //    new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\AjaxControlToolkit.dll").Delete();
                //    new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Base.dll").Delete();
                //    new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Web.dll").Delete();
                //    new FileInfo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Tools.Web.dll").Delete();
                //}
                //catch { }

                //try
                //{
                //    new FileInfo(@"\\buildserver\Continuous Integration\Ajax\Syncfusion.Shared.Base.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Base.dll");
                //    new FileInfo(@"\\buildserver\Continuous Integration\Ajax\Syncfusion.Shared.Web.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Shared.Web.dll");
                //    new FileInfo(@"\\buildserver\Continuous Integration\Ajax\Syncfusion.Tools.Web.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\Syncfusion.Tools.Web.dll");
                //    new FileInfo(@"\\buildserver\Continuous Integration\Ajax\AjaxControlToolkit.dll").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Site\expenses\bin\AjaxControlToolkit.dll");
                //}
                //catch { }

                #endregion


                #region Copy the correct web.config files

                new FileInfo(@"\\buildserver\Continuous Integration\Config File\Framework\Web.config").CopyTo(@"\\" + webserver + @"\" + folderName + @"\Web.config");                

                #endregion
            }

            #endregion
        }
    }
}
