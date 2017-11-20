namespace Auto_Tests.UIMaps.SharedMethodsUIMapClasses
{
    using Microsoft.VisualStudio.TestTools.UITesting.WinControls;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Windows.Input;
    using System.CodeDom.Compiler;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UITest.Extension;
    using Microsoft.VisualStudio.TestTools.UITesting;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;
    using Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse;
    using MouseButtons = System.Windows.Forms.MouseButtons;
    using System.Configuration;
    using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Web;
    using System.Threading;
    using System.Net;
    using System.Net.Sockets;

    public partial class SharedMethodsUIMap
    {
        public BrowserWindow browserWindow { get; private set; }

        /// <summary>
        /// This method will start a new instance of IE if it is not open. Alternatively
        /// it will use any exsisting instances. Pass in the product type to go to the desired logon page.
        /// </summary>
        /// <param name="productType"></param>
        public void StartIE(ProductType productType)
        {
  
            string sWebAddress = string.Empty;

            switch (productType)
            {
                case ProductType.expenses:
                    sWebAddress = cGlobalVariables.ExpensesAddress;
                    break;
                case ProductType.framework:
                    sWebAddress = cGlobalVariables.FrameworkAddress;
                    break;
                default:
                    sWebAddress = cGlobalVariables.ExpensesAddress;
                    break;
            }


            bool bHasOpenIE = false;
            Process[] lstProcesses = Process.GetProcesses();

            for (int i = 0; i < 3; i++)
            {
                #region Go through each process

                lstProcesses = Process.GetProcesses();

                foreach (Process clsProcess in lstProcesses)
                {
                    if (clsProcess.ProcessName.Equals("iexplore"))
                    {

                        bHasOpenIE = true;

                        #region Clear search parameters and wait for IE to become ready

                        GetBrowserWindow(sWebAddress);
                        #endregion

                        SharedNavigateToExpensesLogon();

                        break;
                    }
                }

                #endregion

                #region Break if IE found

                if (bHasOpenIE)
                {
                    break;
                }
                else
                {
                    Playback.Wait(1000);
                }

                #endregion
            }

            if (bHasOpenIE == false)
            {
                SharedLaunchIEParams.Url = sWebAddress + "/shared/logon.aspx";

                SharedLaunchIE();
            }
        }

        private void GetBrowserWindow(string sWebAddress)
        {
            UIExpenseslogonWindowsWindow.SearchProperties.Remove(UITestControl.PropertyNames.Name);
            UIExpenseslogonWindowsWindow.WindowTitles.Clear();
            UIFrameworklogonWindowWindow.SearchProperties.Remove(UITestControl.PropertyNames.Name);
            UIFrameworklogonWindowWindow.WindowTitles.Clear();

            SharedNavigateToExpensesLogonParams.UIExpenseslogonWindowsWindowUrl = sWebAddress + "/shared/logon.aspx";
            browserWindow = UIExpenseslogonWindowsWindow;
        }

        public void CloseBrowserWindow()
        {
            Process[] lstProcesses = Process.GetProcesses();
            foreach (Process clsProcess in lstProcesses)
            {
                if (clsProcess.ProcessName.Equals("iexplore"))
                {
                    clsProcess.Kill();
                }
            }
        }

        /// <summary>
        /// This method will start a new instance of IE if it is not open. Alternatively
        /// it will use any exsisting instances. Pass in the product type to go to the desired logon page.
        /// </summary>
        /// <param name="productType"></param>
        public void StartIE(ProductType productType, string pageExtension)
        {
            string sWebAddress = string.Empty;

            switch (productType)
            {
                case ProductType.expenses:
                    sWebAddress = cGlobalVariables.ExpensesAddress;
                    break;
                case ProductType.framework:
                    sWebAddress = cGlobalVariables.FrameworkAddress;
                    break;
                default:
                    sWebAddress = cGlobalVariables.ExpensesAddress;
                    break;
            }

            bool bHasOpenIE = false;
            Process[] lstProcesses = Process.GetProcesses();

            for (int i = 0; i < 3; i++)
            {
                #region Go through each process

                lstProcesses = Process.GetProcesses();

                foreach (Process clsProcess in lstProcesses)
                {
                    if (clsProcess.MainWindowTitle.ToLower().Contains("windows internet explorer"))
                    {
                        bHasOpenIE = true;

                        #region Clear search parameters and wait for IE to become ready

                        UIExpenseslogonWindowsWindow.SearchProperties.Remove(UITestControl.PropertyNames.Name);
                        UIExpenseslogonWindowsWindow.WindowTitles.Clear();
                        UIFrameworklogonWindowWindow.SearchProperties.Remove(UITestControl.PropertyNames.Name);
                        UIFrameworklogonWindowWindow.WindowTitles.Clear();

                        SharedNavigateToExpensesLogonParams.UIExpenseslogonWindowsWindowUrl = sWebAddress + pageExtension;

                        #endregion

                        SharedNavigateToExpensesLogon();

                        break;
                    }
                }

                #endregion

                #region Break if IE found

                if (bHasOpenIE)
                {
                    break;
                }
                else
                {
                    Playback.Wait(1000);
                }

                #endregion
            }

            if (bHasOpenIE == false)
            {
                SharedLaunchIEParams.Url = sWebAddress + "/shared/logon.aspx";

                SharedLaunchIE();
            }
        }

        /// <summary>
        /// Navigate to a specifc page within the product. The TLDR is specified by the product and should be used like such: NavigateToPage(ProductType.expenses, "/shared/admin/accountOptions.aspx"); 
        /// </summary>
        /// <param name="productType">The product this test is being executed on</param>
        /// <param name="pageExtension">The page address after the TLDR, prefixed with a slash</param>
        public void NavigateToPage(ProductType productType, string pageExtension)
        {
            //Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.AllThreads;
            string sNavigatePage = string.Empty;// = cGlobalVariables.ExpensesAddress + "/home.aspx";

            switch (productType)
            {
                case ProductType.expenses:
                    sNavigatePage = cGlobalVariables.ExpensesAddress;
                    break;
                case ProductType.framework:
                    sNavigatePage = cGlobalVariables.FrameworkAddress;
                    break;
                default:
                    sNavigatePage = cGlobalVariables.ExpensesAddress;
                    break;
            }

            sNavigatePage = sNavigatePage + pageExtension;
            SharedNavigateToExpensesLogonParams.UIExpenseslogonWindowsWindowUrl = sNavigatePage;

            #region Clear search parameters and ensure browser is ready

            UIExpenseslogonWindowsWindow.SearchProperties.Remove(UITestControl.PropertyNames.Name);
            UIExpenseslogonWindowsWindow.WindowTitles.Clear();
            UIFrameworklogonWindowWindow.SearchProperties.Remove(UITestControl.PropertyNames.Name);
            UIFrameworklogonWindowWindow.WindowTitles.Clear();
            //DateTime timeStarted = DateTime.Now;
            //TimeSpan timePassed = new TimeSpan();
            UIExpenseslogonWindowsWindow.WaitForControlReady();
            /*
            while (UIFrameworklogonWindowWindow.UIItemWindow.UIDoneText.DisplayText != "Done")
            {
                Playback.Wait(1000);
                timePassed = (DateTime.Now - timeStarted);
                if (timePassed.Seconds > 10) { break; }
            }
             * 
            */

            #endregion 
            SharedNavigateToExpensesLogon();         
        }


        /// <summary>
        /// Use this method to logon to the desired product. Passing in a password is optional and will override
        /// both the default password and the password within the app.config file
        /// </summary>
        public void Logon(ProductType product, LogonType logonType, string password = "")
        {

            #region Start a new instance of IE and navigate to the desired product's logon page

            StartIE(product);

            #endregion


            #region Set the Company ID to use

            SharedLogonEnterLogonDetailsParams.UICompanyIDEditText = cGlobalVariables.CompanyID(product);
            SharedFrameworkLogonEnterLogonDetailsParams.UICompanyIDEditText = cGlobalVariables.CompanyID(product);

            #endregion


            #region Set the Username and Password to use

            if (logonType == LogonType.administrator)
            {
                SharedLogonEnterLogonDetailsParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(product);
                SharedFrameworkLogonEnterLogonDetailsParams.UIUsernameEditText = cGlobalVariables.AdministratorUserName(product);

                if (password == string.Empty)
                {
                    SharedLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(cGlobalVariables.AdministratorPassword(product));
                    SharedFrameworkLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(cGlobalVariables.AdministratorPassword(product));
                }
                else
                {
                    SharedLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(password);
                    SharedFrameworkLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(password);
                }

            }
            else if (logonType == LogonType.claimant)
            {
                SharedLogonEnterLogonDetailsParams.UIUsernameEditText = cGlobalVariables.ClaimantUserName(product);
                SharedFrameworkLogonEnterLogonDetailsParams.UIUsernameEditText = cGlobalVariables.ClaimantUserName(product);

                if (password == string.Empty)
                {
                    SharedLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(cGlobalVariables.ClaimantPassword(product));
                    SharedFrameworkLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(cGlobalVariables.ClaimantPassword(product));
                }
                else
                {
                    SharedLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(password);
                    SharedFrameworkLogonEnterLogonDetailsParams.UIPasswordEditPassword = Playback.EncryptText(password);
                }

            }

            #endregion


            #region Logon to the desired product
            PerformLogon(product, true);

            #endregion

            var LogonPage = SharedNavigateToExpensesLogonParams.UIExpenseslogonWindowsWindowUrl.Split('/');
            if(browserWindow.Uri.ToString().Contains(LogonPage[LogonPage.Length -1]))
            {
                PerformLogon(product, false);
            }

        }

        private void PerformLogon(ProductType product, bool setWindowsProperties)
        {
            if (product == ProductType.expenses)
            {
                if (setWindowsProperties)
                {
                    UIExpenseslogonWindowsWindow.WindowTitles.Add("logon to expenses2010");
                }

                SharedLogonEnterLogonDetails();
            }
            else
            {
                if (setWindowsProperties)
                {
                    UIFramework2010logonWiWindow.SearchProperties[UITestControl.PropertyNames.Name] = "framework logon";
                    UIFramework2010logonWiWindow.WindowTitles.Add("framework logon");
                }

                SharedFrameworkLogonEnterLogonDetails();
            }
        }

        /// <summary>
        /// Navigates to the Home Page for Expensess
        /// </summary>
        public void NavigateToExpensesHomePage()
        {
            string sNavigatePage = cGlobalVariables.ExpensesAddress + "/home.aspx";

            SharedNavigateToExpensesLogonParams.UIExpenseslogonWindowsWindowUrl = sNavigatePage;

            SharedNavigateToExpensesLogon();
        }


        /// <summary>
        /// Navigates to the Home Page for Framework
        /// </summary>
        public void NavigateToFrameworkHomePage()
        {
            string sNavigatePage = cGlobalVariables.FrameworkAddress + "/home.aspx";

            SharedNavigateToExpensesLogonParams.UIExpenseslogonWindowsWindowUrl = sNavigatePage;

            SharedNavigateToExpensesLogon();
        }


        /// <summary>
        /// Use this method to input a date into a date field
        /// </summary>
        /// <param name="date">Specify the date in the format "DD/MM/YYYY". No other format is accepted</param>
        public void TypeInDate(string date)
        {
            try
            {
                string day = date.Substring(0, 2);
                string month = date.Substring(3, 2);
                string firstYear = date.Substring(6, 2);
                string endYear = date.Substring(8, 2);

                Keyboard.SendKeys("/" + day + "/" + endYear + "//" + month + firstYear);
            }
            catch { }
        }

        public string GetIPAddressOfLocalMachine()
        {
            string localIPAddress = "Unknown";
            IPHostEntry host;
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIPAddress = ip.ToString();
                }
            }
            return localIPAddress;
        }

        public void VerifyPageLayout(string pageTitle, string breadcrumbs, string menubar, string userinfo, string menuoptions)
        {
            Thread.Sleep(1000);
            string htmlMarkUp = ExtractHtmlMarkUpFromPage();
            Assert.IsTrue(htmlMarkUp.Trim().Replace("\r", "").Replace("\n", "").Replace("\t", "").ToLower().Contains(("<TITLE>" + pageTitle + "</TITLE>").ToLower()), "Failed to Validate the Page Title"); //The browser needs to be in compatibility mode for this to work
            Assert.AreEqual(breadcrumbs, new ExtractBreadCrumbs(htmlMarkUp).ExtractFromHtml());
            Assert.AreEqual(menubar, new ExtractMenuBar(htmlMarkUp).ExtractFromHtml());
            Assert.AreEqual(userinfo, new ExtractLoggedInUserAndCurrentDate(htmlMarkUp).ExtractFromHtml());
            //Assert.AreEqual(menuoptions, new ExtractPageOptions(htmlMarkUp).ExtractFromHtml().Replace("\t", ""));
        }

        /// <summary>
        /// Used to set focus on a control and press it
        ///</summary>
        public void SetFocusOnControlAndPressEnter(UITestControl control)
        {
            if (control == null)
            {
                throw new NullReferenceException("Control cannot be null!!");
            }
            control.SetFocus();
            Keyboard.SendKeys("{Enter}");
        }

        /// <summary>
        /// Restores the Default Sorting Order for a Grid by specifying the grid and the product the grid exists
        ///</summary>
        public void RestoreDefaultSortingOrder(string grid, ProductType executingProduct)
        {
            int employeeid = AutoTools.GetEmployeeIDByUsername(executingProduct);
            cDatabaseConnection dbex_CodedUI = new cDatabaseConnection(cGlobalVariables.dbConnectionString(executingProduct));
            //Ensure employee is recaching
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@currentDate", DateTime.Now);
            dbex_CodedUI.ExecuteSQL("UPDATE employees SET CacheExpiry = @currentDate WHERE employeeID = @employeeID");
            dbex_CodedUI.sqlexecute.Parameters.Clear();

            //Ensure grid always uses default sorting order
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@employeeID", employeeid);
            dbex_CodedUI.sqlexecute.Parameters.AddWithValue("@grid", grid);
            dbex_CodedUI.ExecuteSQL("DELETE FROM employeeGridSortOrders WHERE employeeID = @employeeID AND gridID = @grid");
            dbex_CodedUI.sqlexecute.Parameters.Clear();
        }

        //public string ExtractHtmlMarkUpFromPage()
        //{
        //    if (control == null)
        //    {
        //        throw new NullReferenceException("Control cannot be null!!");
        //    }
        //    control.SetFocus();
        //    Keyboard.SendKeys("{Enter}");
        //}

        public string ExtractHtmlMarkUpFromPage()
        {
            Thread.Sleep(1000);
            if (browserWindow == null)
            {
                GetBrowserWindow(cGlobalVariables.ExpensesAddress);
            }
           UITestControl browserWindowControl = browserWindow.CurrentDocumentWindow;
           PageLayoutVerification pageVerification = new PageLayoutVerification(browserWindow);
           UITestControl control = pageVerification.Findcontrol("ctl00_lblpageoptions", "Page Options", "IP Address Filtering");

            object htmlDocument = ((mshtml.HTMLSpanElementClass)(control.NativeElement)).document;
            mshtml.HTMLDocumentClass document = (mshtml.HTMLDocumentClass)htmlDocument;

            mshtml.IHTMLElement elementTag = document.IHTMLDocument3_documentElement;
            return elementTag.innerHTML;
        }


        internal class ExtractBreadCrumbs : ExtractInformationFromHtml
        {
            internal ExtractBreadCrumbs(string htmlText)
                : base(htmlText)
            {
                successRegex = "id=\"?ctl00_sitemap\"?.*?</div>";
                textBetweenHtmlRegex = "(?<=^|>)[^><]+?(?=<|$)";
            }

            internal override string ExtractFromHtml()
            {
                string FoundValue = base.ExtractFromHtml();
                if (string.IsNullOrEmpty(FoundValue)) 
                {
                    if (pageSource.Contains("Before you can continue, please confirm the action required at the bottom of your screen."))
                    {
                        FoundValue = "Before you can continue, please confirm the action required at the bottom of your screen.";
                    }
                }
                return FoundValue;
            }
        }

        internal class ExtractMenuBar : ExtractInformationFromHtml
        {
            internal ExtractMenuBar(string htmlText)
                : base(htmlText)
            {
                successRegex = "<div class=\"?linkbar\"?.*?</div>";
                textBetweenHtmlRegex = "(?<=^|>)[^><]+?(?=<|$)";
            }
        }

        internal class ExtractPageOptions : ExtractInformationFromHtml
        {
            internal ExtractPageOptions(string htmlText)
                : base(htmlText)
            {
                successRegex = "div id=\"?submenu\"?.*?</div></div>";
                //successRegex = "div class=\"?submenuholder\"?.*?<div id=\"?header\"?>";
                textBetweenHtmlRegex = "(?<=^|>)[^><]+?(?=<|$)";
            }
        }

        internal class ExtractLoggedInUserAndCurrentDate : ExtractInformationFromHtml
        {
            internal ExtractLoggedInUserAndCurrentDate(string htmlText)
                : base(htmlText)
            {
                successRegex = "<div class=\"?userinfo\"?.*?</div>";
                textBetweenHtmlRegex = "(?<=^|>)[^><]+?(?=<|$)";
            }
        }

        internal class ExtractInformationFromHtml
        {
            protected string successRegex = "";
            protected string textBetweenHtmlRegex = "";
            protected string pageSource;

            internal ExtractInformationFromHtml(string htmlPageSource)
            {
                pageSource = htmlPageSource;
              
            }

            /// <summary>
            /// Extracts required values based upon provided regex 
            /// </summary>
            /// <returns></returns>
            internal virtual string ExtractFromHtml()
            {
                StringBuilder result = new StringBuilder();

                Match match = Regex.Match(pageSource, successRegex, RegexOptions.IgnoreCase | RegexOptions.Singleline);

                if (match.Success)
                {
                    string key = match.Groups[0].Value;
                    MatchCollection collection = Regex.Matches(RemoveComments(key), textBetweenHtmlRegex);
                    foreach (Match iterator in collection)
                    {
                        result.Append(iterator.Value);
                    }
                }
                return HttpUtility.HtmlDecode(result.ToString().Replace("\r", "").Replace("\n", "").Trim());
            }

            /// <summary>
            /// strips any found comments from page source
            /// </summary>
            private string RemoveComments(string stringtoMatchAgainst)
            {
                string commentRegex = "(<!--.*?-->)";
                Match match = Regex.Match(stringtoMatchAgainst, commentRegex, RegexOptions.None);
                if (match.Success)
                {
                    //Remove line from source
                    foreach (Group matchedGroups in match.Groups)
                    {
                        stringtoMatchAgainst = stringtoMatchAgainst.Replace(matchedGroups.Value, "");
                    } 
                }
                return stringtoMatchAgainst;
            }
        }


        private class PageLayoutVerification
        {
            private UITestControl control;
            private HtmlSpan mUIPageOptionsPane;

            private string controlId;
            private string labelName;
            private string windowTitle;

            public PageLayoutVerification(UITestControl control)
            {
                this.control = control;
            }

            public UITestControl Findcontrol(string controlId, string labelName, string windowTitle)
            {
                this.controlId = controlId;
                this.labelName = labelName;
                this.windowTitle = windowTitle;
                return UIPageOptionsPane;
            }

            private HtmlSpan UIPageOptionsPane
            {
                get
                {
                    if ((this.mUIPageOptionsPane == null))
                    {
                        this.mUIPageOptionsPane = new HtmlSpan(control);
                        #region Search Criteria
                        this.mUIPageOptionsPane.SearchProperties[HtmlDiv.PropertyNames.Id] = controlId;
                        this.mUIPageOptionsPane.SearchProperties[HtmlDiv.PropertyNames.Name] = null;
                        this.mUIPageOptionsPane.FilterProperties[HtmlDiv.PropertyNames.InnerText] = labelName;
                        this.mUIPageOptionsPane.FilterProperties[HtmlDiv.PropertyNames.Title] = null;
                        this.mUIPageOptionsPane.FilterProperties[HtmlDiv.PropertyNames.Class] = null;
                        //this.mUIPageOptionsPane.FilterProperties[HtmlDiv.PropertyNames.ControlDefinition] = "id=ctl00_lblpageoptions";
                        //this.mUIPageOptionsPane.FilterProperties[HtmlDiv.PropertyNames.TagInstance] = "1";
                        //this.mUIPageOptionsPane.WindowTitles.Add(windowTitle);
                        #endregion
                    }
                    return this.mUIPageOptionsPane;
                }
            }
        }

        /// <summary>
        /// Paste text to control
        /// </summary>
        public void PasteText(HtmlControl control)
        {
            if (control.ControlType == ControlType.Edit)
            {
                #region Variable Declarations
                //HtmlEdit uIEntitynameEdit = this.UICustomEntityabcabcabWindow.UICustomEntityabcabcabDocument.UIEntitynameEdit;
                //    WinMenuItem uIPasteMenuItem = this.UIItemWindow.UIContextMenu.UIPasteMenuItem;
                #endregion

                // Right-Click 'Entity name*' text box
                //    Mouse.Click(control, MouseButtons.Right, ModifierKeys.None, new Point(30, 9));

                // Click 'Paste' menu item
                //    Mouse.Click(uIPasteMenuItem, new Point(38, 11));

                Keyboard.PressModifierKeys(control, ModifierKeys.Control);
                Keyboard.SendKeys("v");
                Keyboard.ReleaseModifierKeys(ModifierKeys.Control);
            }
            else
            {
                throw new ArgumentException("Invalid HtmlControl! I only support edit boxes!!");
            }
        }
    }
}
