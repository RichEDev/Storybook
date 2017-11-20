using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using expenses.Old_App_Code.admin;

using expenses.Old_App_Code;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses.admin
{
    public partial class transaction_viewer : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Transaction Viewer";
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

                cCardStatements clsstatements = new cCardStatements(user.AccountID);
                cmbstatement.Items.AddRange(clsstatements.CreateDropDown());

                if (Request.QueryString["statementid"] != null)
                {

                    if (cmbstatement.Items.FindByValue(Request.QueryString["statementid"]) != null)
                    {
                        cmbstatement.Items.FindByValue(Request.QueryString["statementid"]).Selected = true;
                    }

                    
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gridtransactions.InitializeDataSource += new Infragistics.WebUI.UltraWebGrid.InitializeDataSourceEventHandler(gridtransactions_InitializeDataSource);
        }

        void gridtransactions_InitializeDataSource(object sender, Infragistics.WebUI.UltraWebGrid.UltraGridEventArgs e)
        {
            getTransactions();
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            getTransactions();
        }

        private void getTransactions()
        {
            cCardStatements clsstatements = new cCardStatements((int)ViewState["accountid"]);
            cEmployees clsemployees = new cEmployees((int)ViewState["accountid"]);
            int statementid = Convert.ToInt32(cmbstatement.SelectedValue);
            byte transactiontype = Convert.ToByte(cmbtransactiontype.SelectedValue);
            int employeeid;
            employeeid = clsemployees.getEmployeeidByUsername((int)ViewState["accountid"], txtemployee.Text);
            gridtransactions.DataSource = clsstatements.getTransactionGrid(statementid, transactiontype, employeeid);
            if (gridtransactions.DataSource == null)
            {
                gridtransactions.Visible = false;
                litMessage.Text = "<font color=\"red\">ERROR - Unable to load the card template for the provider. Please contact your administrator.</font>";
            }
            else
            {
                gridtransactions.DataBind();
            }
        }

        protected void cmbtransactiontype_SelectedIndexChanged(object sender, EventArgs e)
        {
            getTransactions();
        }
        
        
        protected void cmbstatement_SelectedIndexChanged(object sender, EventArgs e)
        {
            getTransactions();
            
        }

        protected void gridtransactions_InitializeLayout(object sender, Infragistics.WebUI.UltraWebGrid.LayoutEventArgs e)
        {
            if (cmbstatement.SelectedItem != null)
            {
                cCardTemplates clstemplates = new cCardTemplates((int)ViewState["accountid"]);
                cCardStatements clsstatements = new cCardStatements((int)ViewState["accountid"]);
                cCardStatement statement = clsstatements.getStatementById(Convert.ToInt32(cmbstatement.SelectedValue));
                cCardTemplate template = clstemplates.getTemplate(statement.Corporatecard.cardprovider.cardprovider);
                if (template != null)
                {
                    cCardRecordType recType = template.RecordTypes[CardRecordType.CardTransaction];

                    foreach (cCardTemplateField field in recType.Fields)
                    {
                        if (field.mappedtable != "card_transactions")
                        {
                            if (e.Layout.Bands[0].Columns.FromKey(field.mappedfield) != null)
                            {
                                e.Layout.Bands[0].Columns.FromKey(field.mappedfield).Header.Caption = field.label;
                            }
                        }
                    }
                }

                e.Layout.Bands[0].Columns.FromKey("transactionid").Hidden = true;
                if (e.Layout.Bands[0].Columns.FromKey("transactionid1") != null)
                {
                    e.Layout.Bands[0].Columns.FromKey("transactionid1").Hidden = true;
                }
                e.Layout.Bands[0].Columns.FromKey("corporatecardid").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("statementid").Hidden = true;
                e.Layout.Bands[0].Columns.FromKey("transaction_date").Header.Caption = "Transaction Date";
                e.Layout.Bands[0].Columns.FromKey("transaction_date").Format = "dd/MM/yyyy";
                e.Layout.Bands[0].Columns.FromKey("description").Header.Caption = "Description";
                e.Layout.Bands[0].Columns.FromKey("transaction_amount").Header.Caption = "Converted Amount";
                e.Layout.Bands[0].Columns.FromKey("original_amount").Header.Caption = "Original Amount";
                e.Layout.Bands[0].Columns.FromKey("label").Header.Caption = "Currency";
                e.Layout.Bands[0].Columns.FromKey("exchangerate").Header.Caption = "Exchange Rate";
                e.Layout.Bands[0].Columns.FromKey("allocated_amount").Header.Caption = "Allocated Amount";
                e.Layout.Bands[0].Columns.FromKey("unallocated_amount").Header.Caption = "Unallocated Amount";
                e.Layout.Bands[0].Columns.FromKey("allocated_original_amount").Header.Caption = "Original Allocated Amount";
                e.Layout.Bands[0].Columns.FromKey("unallocated_original_amount").Header.Caption = "Original Unallocated Amount";
                e.Layout.Bands[0].Columns.FromKey("card_number").Header.Caption = "Card Number";
                e.Layout.Bands[0].Columns.FromKey("employee").Header.Caption = "Employee";
            }
        }

        protected void lnkexporttoexcell_Click(object sender, EventArgs e)
        {
            gridtransactions.DisplayLayout.Pager.AllowPaging = false;
            getTransactions();
            
            export.Export(gridtransactions);
            
            gridtransactions.DisplayLayout.Pager.AllowPaging = true;
            getTransactions();
        }

        protected void gridtransactions_InitializeRow(object sender, Infragistics.WebUI.UltraWebGrid.RowEventArgs e)
        {
            
        }
    }
}
