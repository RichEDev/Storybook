namespace Spend_Management.shared.usercontrols
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using SpendManagementHelpers;
    using SpendManagementLibrary;
    using SpendManagementLibrary.BaseClasses;
    using SpendManagementLibrary.Employees;

    /// <summary>
    /// Class for BankAccounts
    /// </summary>
    public partial class bankAccounts : System.Web.UI.UserControl
    {

        #region User Control Properties
        /// <summary>
        /// EmployeeId having bank accounts.
        /// </summary>
        public int AccountsEmployeeId { get; set; }

        /// <summary>
        /// Defines if the bank account data should be displayed redacted in the grid
        /// </summary>
        public bool RedactGridData { get; set; }

        /// <summary>
        /// Defines if the user is allowed to edit bank accounts or not
        /// </summary>
        public bool AllowEdit { get; set; }

        /// <summary>
        /// Defines if the user is allowed to edit bank accounts or not
        /// </summary>
        public bool AllowDelete { get; set; }
        #endregion

        /// <summary>
        /// Page Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser reqCurrentUser = cMisc.GetCurrentUser();
            ViewState["accountid"] = reqCurrentUser.AccountID;
            ViewState["subAccountID"] = reqCurrentUser.CurrentSubAccountId;

            #region Modal Panel
            
            #region Buttons

            var reqModalSaveButton = new CSSButton { Text = @"save" };
            reqModalSaveButton.UseSubmitBehavior = false;
            reqModalSaveButton.Style.Add(HtmlTextWriterStyle.Display, "hidden");
            reqModalSaveButton.Attributes.Add("onclick", "SEL.BankAccounts.SaveBankAccount(); return false;");

            pnlAddEditAccountButtons.Controls.Add(reqModalSaveButton);

            var reqModalCancelButton = new CSSButton { Text = @"cancel" };
            reqModalCancelButton.Attributes.Add("onclick", "SEL.BankAccounts.CloseModal(); return false;");

            pnlAddEditAccountButtons.Controls.Add(reqModalCancelButton);

            var reqModalCloseButton = new CSSButton { Text = @"close" };
            reqModalCloseButton.UseSubmitBehavior = false;
            reqModalCloseButton.Attributes.Add("onclick", "SEL.BankAccounts.CloseInfoModal(); return false;");
            #endregion Buttons
            #endregion Modal Panel

            GenerateBankAccountGrid(reqModalSaveButton.ClientID);

            var employeePrimaryCurrency = 0;
            Employee employee = null;
            var employees = new cEmployees(reqCurrentUser.AccountID);

            // When you edit an employee from aeemployee.aspx
            if (AccountsEmployeeId > 0)
            {
                employee = employees.GetEmployeeById(AccountsEmployeeId);
                employeePrimaryCurrency = employee.PrimaryCurrency;
            }

            // When you access bank accounts from my details
            if (AccountsEmployeeId < 0)
            {
                employee = employees.GetEmployeeById(reqCurrentUser.EmployeeID);
                employeePrimaryCurrency = employee.PrimaryCurrency;
            }
            
            if (IsPostBack == false)
            {
                FillCurrency(employeePrimaryCurrency);
                FillCountry();
                BankAccountsBase.AddAccountTypesToDropDownList(ref this.cmbAccounttype);
            }
        }

        /// <summary>
        /// Provide proper name of the currency by currency id in the grid
        /// </summary>
        /// <param name="grid">The grid</param>
        public void AddCurrency(ref cGridNew grid)
        {
            var clscurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            List<ListItem> lstCurrency = clscurrencies.CreateDropDown();

            foreach (ListItem lstItem in lstCurrency)
            {
                ((cFieldColumn)grid.getColumnByName("CurrencyId")).addValueListItem(Convert.ToInt32(lstItem.Value), lstItem.Text );
            }
        }

        /// <summary>
        /// Fill currency list in dropdown
        /// </summary>
        /// <param name="primaryCurrencyId">The primaryCurrencyId of the employee</param>
        private void FillCurrency(int primaryCurrencyId)
        {
            var clscurrencies = new cCurrencies((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            List<ListItem> lstCurrency = clscurrencies.CreateDropDown();
            var itemNone = new ListItem("[None]", "0");
            lstCurrency.Insert(0,itemNone);
            ddlCurrency.Items.AddRange(lstCurrency.ToArray());

            // if the default currency is not 0 set it in the dropdown list
            if (primaryCurrencyId != 0)
            {
                ddlCurrency.SelectedValue = primaryCurrencyId.ToString();
            }
        }

        /// <summary>
        /// Generates the grid showing the users bank account associations.
        /// </summary>
        /// <param name="reqModalSaveButtonClientId">Save Button object client ID</param>
        private void GenerateBankAccountGrid(string reqModalSaveButtonClientId)
        {
            CurrentUser reqCurrentUser = cMisc.GetCurrentUser();
            var clsFields = new cFields(reqCurrentUser.AccountID);


            var gridBankAccounts = new cGridNew(reqCurrentUser.AccountID, reqCurrentUser.EmployeeID, "myBankAccounts", "SELECT dbo.getDecryptedValue(BankAccounts.AccountName),dbo.getDecryptedValue(BankAccounts.AccountNumber),dbo.getAccountType(BankAccounts.AccountType),dbo.getCurrencyLabel(BankAccounts.CurrencyId),dbo.getDecryptedValue(BankAccounts.SortCode),dbo.getDecryptedValue(BankAccounts.Reference),dbo.getCountryLabel(BankAccounts.CountryId), BankAccountId,EmployeeId,archived FROM BankAccounts");
            gridBankAccounts.addFilter(clsFields.GetFieldByID(new Guid("33873935-C9BC-4436-AD4C-3CF2120C7D4D")), ConditionType.Equals, new object[] { AccountsEmployeeId >= 0 ? AccountsEmployeeId : reqCurrentUser.EmployeeID }, null, ConditionJoiner.None);

            gridBankAccounts.KeyField = "EmployeeId";
            gridBankAccounts.getColumnByName("BankAccountId").hidden = true;
            gridBankAccounts.getColumnByName("EmployeeId").hidden = true;
            gridBankAccounts.getColumnByName("archived").hidden = true;
            gridBankAccounts.editlink = "javascript:SEL.BankAccounts.LoadBankAccountModal(SEL.BankAccounts.LoadType.Edit, {BankAccountId});";
            gridBankAccounts.deletelink = "javascript:SEL.BankAccounts.deleteBankAccount({BankAccountId},{EmployeeId});";
            gridBankAccounts.archivelink = "javascript:SEL.BankAccounts.changeArchiveStatus({BankAccountId},{EmployeeId});";
            gridBankAccounts.enableupdating = AllowEdit;
            gridBankAccounts.enabledeleting = AllowDelete; 
            gridBankAccounts.enablearchiving = AllowEdit;
            gridBankAccounts.ArchiveField = "archived";       
            gridBankAccounts.EmptyText = "There are currently no bank accounts defined.";
            gridBankAccounts.SortedColumn = gridBankAccounts.getColumnByName("dbo.getDecryptedValue(BankAccounts.AccountName)");

            if (this.RedactGridData)
            {
                gridBankAccounts.InitialiseRow += new cGridNew.InitialiseRowEvent(this.BankAccountGridOnInitialiseRow);
                gridBankAccounts.ServiceClassForInitialiseRowEvent = "Spend_Management.shared.usercontrols.bankAccounts";
                gridBankAccounts.ServiceClassMethodForInitialiseRowEvent = "BankAccountGridOnInitialiseRow";
            }

            string[] gridData = gridBankAccounts.generateGrid();
            this.litBankAccounts.Text = gridData[1];

            var sbJs = new StringBuilder();
            sbJs.Append("SEL.BankAccounts.ModalWindowDomID = \"" + this.modalAddEditAccount.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowAccountHeader = \"divBankAccountHeader\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowSpanAccountInfo = \"spanBankAccountInfo\";\n");

            sbJs.Append("SEL.BankAccounts.ModalWindowAccountName = \"" + this.txtAccountName.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowtxtAccountNumber = \"" + this.txtAccountNumber.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowcmbAccounttype = \"" + this.cmbAccounttype.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowtxtSortCode = \"" + this.txtSortCode.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowtxtReference = \"" + this.txtReference.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowddlCurrency = \"" + this.ddlCurrency.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowddlCountry = \"" + this.ddlCountry.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowSaveButtonDomID = \"" + reqModalSaveButtonClientId + "\";\n");
            sbJs.Append("SEL.BankAccounts.BankAccountGridDomID = \"myBankAccounts\";\n");

            sbJs.Append("SEL.BankAccounts.ModalAccountNameValidatorID = \"" + this.rfAccountName.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalAccountNumberValidatorID = \"" + this.rfAccountNumber.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalAccountTypeValidatorID = \"" + this.rvAccountType.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalCurrencyValidatorID = \"" + this.rvCurrency.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowIban = \"" + this.txtIban.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.ModalWindowSwiftCode = \"" + this.txtSwiftCode.ClientID + "\";\n");
            sbJs.Append("SEL.BankAccounts.CurrentEmployeeID = " + (this.AccountsEmployeeId >= 0 ? this.AccountsEmployeeId.ToString(CultureInfo.InvariantCulture) : reqCurrentUser.EmployeeID.ToString(CultureInfo.InvariantCulture)) + ";\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "BankAccounVars", sbJs + "\n" + cGridNew.generateJS_init("BankAccountGridVars", new List<string> { gridData[0] }, reqCurrentUser.CurrentActiveModule), true);
        
        }

        /// <summary>
        /// Fill country list in dropdown
        /// </summary>
        private void FillCountry()
        {
            var countriesByAccountId = new cCountries((int)ViewState["accountid"], (int)ViewState["subAccountID"]);
            var lstCountry = countriesByAccountId.CreateDropDown();
            lstCountry.RemoveAll(s => string.IsNullOrWhiteSpace(s.ToString()));
            ddlCountry.Items.AddRange(lstCountry.ToArray());
        }

        /// <summary>
        /// Handles the initialisation of the bank account grid and redacts the values of the sensitive data to stop administrators seeing them.
        /// </summary>
        /// <param name="row">The row of the grid</param>
        /// <param name="gridInfo"></param>
        private void BankAccountGridOnInitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            const int CharsToKeep = 2;

            var sortCode = row.getCellByID("dbo.getDecryptedValue(BankAccounts.SortCode)");
            sortCode.Value = string.IsNullOrEmpty(sortCode.Text) ? string.Empty : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(sortCode.Text, CharsToKeep);

            var accountNumber = row.getCellByID("dbo.getDecryptedValue(BankAccounts.AccountNumber)");
            accountNumber.Value = string.IsNullOrEmpty(accountNumber.Text) ? string.Empty : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(accountNumber.Text, CharsToKeep);

            var accountName = row.getCellByID("dbo.getDecryptedValue(BankAccounts.AccountName)");
            accountName.Value = string.IsNullOrEmpty(accountName.Text) ? string.Empty : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(accountName.Text, CharsToKeep);

            var reference = row.getCellByID("dbo.getDecryptedValue(BankAccounts.Reference)");
            reference.Value = string.IsNullOrEmpty(reference.Text) ? string.Empty : SpendManagementLibrary.Account.BankAccount.GetRedactedValues(reference.Text, CharsToKeep);
        }
    }
}