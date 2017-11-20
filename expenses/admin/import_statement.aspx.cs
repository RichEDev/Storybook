using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using expenses.Old_App_Code.admin;

using SpendManagementLibrary;
using Spend_Management;
using System.Collections.Generic;

namespace expenses.admin
{
    using SpendManagementLibrary.Cards;

    public partial class import_statement : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Import Statement";
            Master.title = Title;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true, true);

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                cAccounts clsAccounts = new cAccounts();
                cAccount reqAccount = clsAccounts.GetAccountByID(user.AccountID);

                if (reqAccount.CorporateCardsEnabled == false)
                {
                    Response.Redirect("~/home.aspx", true);
                }

                CorporateCards clscards = new CorporateCards(user.AccountID);
                cmbprovider.Items.AddRange(clscards.CreateDropDown());

                if (cmbprovider.Items.Count == 0)
                {
                    litMessage.Text = "You currently have no card providers selected, you must add at least one to import a statement.";
                    pnlWizard.Visible = false;
                }
        
            }
        }

        protected void wizstatement_ActiveStepChanged(object sender, EventArgs e)
        {
        }

        protected void wizstatement_PreviousButtonClick(object sender, WizardNavigationEventArgs e)
        {
            if (e.NextStepIndex == 0)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Event handler for next button
        /// </summary>
        /// <param name="sender">Page from which the request came</param>
        /// <param name="e">Event arguments passed in</param>
        protected void wizstatement_NextButtonClick(object sender, WizardNavigationEventArgs e)
        {
            cCardStatements clsStatements = new cCardStatements((int)ViewState["accountid"]);

            switch (e.NextStepIndex)
            {
                case 1:
                    if (Page.IsValid)
                    {
                        string providername = cmbprovider.SelectedItem.Text;
                        ViewState["providerName"] = providername;
                        byte[] file = new byte[fuStatement.PostedFile.ContentLength];
                        fuStatement.PostedFile.InputStream.Read(file, 0, fuStatement.PostedFile.ContentLength);
                        ViewState["FileData"] = file;

                        cCardTemplates clstemplates = new cCardTemplates((int)ViewState["accountid"]);
                        cCardTemplate template = clstemplates.getTemplate(providername);
                        if (template == null)
                        {
                            litMessage.Text = "Unable to load the card template file for " + providername;
                            e.Cancel = true;
                            return;
                        }

                        litMessage.Text = "<font color=\"red\">";

                        //Check to see if ther are any card company record types
                        if (template.RecordTypes.ContainsKey(CardRecordType.CardCompany))
                        {
                            cCardRecordType recType = template.RecordTypes[CardRecordType.CardCompany];

                            if (recType != null)
                            {
                                cImport import = clsStatements.GetCardRecordTypeData(template, recType, file);

                                if (import == null)
                                {
                                    litMessage.Text += "The format of the selected file does not match that of a " + providername + " file.";
                                    litMessage.Text += "</font>";
                                    e.Cancel = true;
                                    return;
                                }

                                SortedList<string, string> lstCompanies = clsStatements.GetVCF4CompanyRecords(import, template.RecordTypes[CardRecordType.CardCompany]);

                                if (lstCompanies != null)
                                {
                                    //If multiple companies then give the user the option to select which ones to import
                                    if (lstCompanies.Count > 1)
                                    {
                                        DrawCardCompaniesGrid(lstCompanies);
                                    }
                                    else
                                    {
                                        //If only one company then need to save automatically and set as used for import if it doesn't already exist
                                        cCardCompanies clsCardComps = new cCardCompanies((int)ViewState["accountid"]);
                                        cCardCompany cardCompany = clsCardComps.GetCardCompanyByNumber(lstCompanies.Keys[0]);

                                        if (cardCompany == null)
                                        {
                                            svcCreditCards credCards = new svcCreditCards();
                                            List<cCardCompany> lstComps = new List<cCardCompany>();
                                            lstComps.Add(new cCardCompany(0, lstCompanies.Values[0], lstCompanies.Keys[0], true, null, null, null, null));
                                            credCards.SaveCardCompanies(lstComps);
                                        }

                                        importCardTransactions(ref e, ref clsStatements);
                                        wizstatement.MoveTo(this.WizardStep3);
                                    }
                                }
                            }
                        }
                        else
                        {
                            importCardTransactions(ref e, ref clsStatements);
                            wizstatement.MoveTo(this.WizardStep3);
                        }
                        
                    }
                    break;
                case 2:

                    importCardTransactions(ref e, ref clsStatements);

                    break;
                case 3:
                    string name = txtname.Text;
                    DateTime? statementdate = null;
                    if (txtstatementdate.Text.Trim() != "")
                    {
                        statementdate = Convert.ToDateTime(txtstatementdate.Text);
                    }

                    cCardStatement oldstatement = clsStatements.getStatementById((int)ViewState["statementid"]);
                    var accountId = (int)ViewState["accountid"];
                    var newstatement = new cCardStatement(accountId, oldstatement.CorporateCardId, oldstatement.statementid, name, statementdate, oldstatement.createdon, oldstatement.createdby, DateTime.Now, (int)ViewState["employeeid"]);
                    
                    //Check to see if user has changed the name and then if the new name exists already
                    if (string.Equals(newstatement.name, oldstatement.name, StringComparison.OrdinalIgnoreCase) == false && clsStatements.checkStatementNames(newstatement))
                    {
                        e.Cancel = true;
                        ClientScript.RegisterStartupScript(this.GetType(), "js", $"alert('{cCardStatements.CannotSaveExistingStatementNamesMessage}');", true);
                        break;
                    }

                    clsStatements.updateStatement(newstatement);
                    break;
            }
        }

        /// <summary>
        /// Draw the grid to show the credit card companies to use for the VCF4 import
        /// </summary>
        private void DrawCardCompaniesGrid(SortedList<string, string> lstCompanies)
        {
            cCardCompanies clsCardComps = new cCardCompanies((int)ViewState["accountid"]);
            cCardCompany cardCompany = null;
            TableRow row;
            TableCell cell;
            CheckBox chk;
            int i = 0;

            foreach (KeyValuePair<string, string> kvp in lstCompanies)
            {
                cardCompany = clsCardComps.GetCardCompanyByNumber(kvp.Key);
                row = new TableRow();
                cell = new TableCell();
                cell.Text = kvp.Value;

                if (i % 2 == 0)
                {
                    row.CssClass = "row1";
                }
                else
                {
                    row.CssClass = "row2";
                }

                row.Cells.Add(cell);

                cell = new TableCell();
                cell.Style.Add(HtmlTextWriterStyle.TextAlign, "center");
                chk = new CheckBox();
                chk.ID = kvp.Key;

                if (cardCompany != null)
                {
                    chk.Checked = cardCompany.usedForImport;
                    chk.InputAttributes.Add("value", cardCompany.cardCompanyID + "," + kvp.Value + "," + kvp.Key);
                }
                else
                {
                    chk.InputAttributes.Add("value", 0 + "," + kvp.Value + "," + kvp.Key);
                }
                
                cell.Controls.Add(chk);
                row.Cells.Add(cell);

                i++;
                tblCardCompanies.Rows.Add(row);
            }
        }

        /// <summary>
        /// Add the card statement and add the card transactions if the file format is valid 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="clsStatements"></param>
        private void importCardTransactions(ref WizardNavigationEventArgs e, ref cCardStatements clsStatements)
        {
            var accountId = (int)ViewState["accountid"];
            var employeeId = (int)ViewState["employeeid"];
            var clstemplates = new cCardTemplates(accountId);
            var clsProviders = new CardProviders();
            cCardProvider provider = clsProviders.getProviderByName(ViewState["providerName"].ToString());
            var clsCards = new CorporateCards(accountId);
            cCorporateCard corporatecard = clsCards.GetCorporateCardById(provider.cardproviderid);
            string name = provider.cardprovider + " statement imported " + DateTime.Now;
            var statement = new cCardStatement(accountId, corporatecard.cardprovider.cardproviderid, 0, name, null, DateTime.Now.ToUniversalTime(), employeeId, null, null);
            var fileData = (byte[])ViewState["FileData"];

            cCardTemplate template = clstemplates.getTemplate(ViewState["providerName"].ToString());
            if (template == null)
            {
                litMessage.Text = "Failed to load card template file for " + ViewState["providerName"].ToString();
                litMessage.Text += "</font>";
                e.Cancel = true;
                return;
            }
            cImport import = clsStatements.GetCardRecordTypeData(template, template.RecordTypes[CardRecordType.CardTransaction], fileData);

            if (import == null)
            {
                litMessage.Text += "The format of the selected file does not match that of a " + ViewState["providerName"].ToString() + " file.";
                litMessage.Text += "</font>";
                e.Cancel = true;
                return;
            }

            //Add the statement
            int statementid = clsStatements.addStatement(statement);

            //Add the card transactions
            clsStatements.ImportCardTransactions(template, statementid, statement.Corporatecard.cardprovider, fileData, import);
            clsStatements.SendEmployeeMail(statementid);

            SortedList<int, string> validateCountryLst = clsStatements.getInvalidCountries();

            if (validateCountryLst.Count > 0)
            {
                foreach (string val in validateCountryLst.Values)
                {
                    litMessage.Text += val + "<br />";
                }
            }

            SortedList<int, string> validateCurrencyLst = clsStatements.getInvalidCurrencies();

            if (validateCurrencyLst.Count > 0)
            {
                foreach (string val in validateCurrencyLst.Values)
                {
                    litMessage.Text += val + "<br />";
                }
            }
            litMessage.Text += "</font>";
            //lblmsg.Visible = true;

            ViewState["statementid"] = statementid;
            usrunallocatedcardnumbers.statementid = statementid;
            usrunallocatedcardnumbers.provider = ViewState["providerName"].ToString();
            usrunallocatedcardnumbers.accountid = (int)ViewState["accountid"];
            if (txtname.Text == "")
            {
                txtname.Text = name;
            }
        }
    }
}
