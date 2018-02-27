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

namespace expenses.admin
{
    using SpendManagementLibrary.Cards;

    public partial class aecorporatecard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Add/Edit Corporate Card";
            Master.title = Title;
            Master.enablenavigation = false;

            if (IsPostBack == false)
            {
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.CorporateCards, true, true);

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                int cardproviderid = 0;
                if (Request.QueryString["cardproviderid"] != null)
                {
                    cardproviderid = Convert.ToInt32(Request.QueryString["cardproviderid"]);
                }
                ViewState["cardproviderid"] = cardproviderid;

                CardProviders clsproviders = new CardProviders();
                cSubcats clssubcats = new cSubcats(user.AccountID);

                cmbprovider.Items.AddRange(clsproviders.CreateList());
                cmbassignitem.Items.AddRange(clssubcats.CreateDropDownForCardTransactions());
                if (cardproviderid > 0)
                {
                    CorporateCards clscorporatecards = new CorporateCards(user.AccountID);
                    cCorporateCard card = clscorporatecards.GetCorporateCardById(cardproviderid);
                    if (cmbprovider.Items.FindByValue(cardproviderid.ToString()) != null)
                    {
                        cmbprovider.Items.FindByValue(cardproviderid.ToString()).Selected = true;
                    }
                    cmbprovider.Enabled = false;
                    chkclaimantsettlesbill.Checked = card.claimantsettlesbill;
                    if (cmbassignitem.Items.FindByValue(card.allocateditem.ToString()) != null)
                    {
                        cmbassignitem.Items.FindByValue(card.allocateditem.ToString()).Selected = true;
                    }
                    chkblockcash.Checked = card.blockcash;
                    //Option not implemented and needs thought on how to do
                    //chkreconciledbyadmin.Checked = card.reconciledbyadministrator;
                    chksingleclaim.Checked = card.singleclaim;
                    this.txtFileIdentifier.Text = card.FileIdentifier;
                }
            }
        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            int cardproviderid = Convert.ToInt32(cmbprovider.SelectedValue);
            int? allocateditem = null;
            bool claimantsettlesbill = chkclaimantsettlesbill.Checked;
            bool blockcash = chkblockcash.Checked;
            //Option not implemented and needs thought on how to do
            bool reconciledbyadministrator = false; //chkreconciledbyadmin.Checked;
            bool singleclaim = chksingleclaim.Checked;
            var fileIdentifier = this.txtFileIdentifier.Text;
            if (cmbassignitem.SelectedValue != "0")
            {
                allocateditem = Convert.ToInt32(cmbassignitem.SelectedValue);
            }
            CorporateCards clscorporatecards = new CorporateCards((int)ViewState["accountid"]);
            CardProviders clsproviders = new CardProviders();
            cCardProvider provider = clsproviders.getProviderByID(cardproviderid);
            cCorporateCard oldcard = clscorporatecards.GetCorporateCardById((int)ViewState["cardproviderid"]);
            cCorporateCard newcard;

            ICurrentUser user = cMisc.GetCurrentUser();
            if (oldcard == null)
            {
                newcard = new cCorporateCard(provider, claimantsettlesbill, DateTime.Now, (int)this.ViewState["employeeid"], null, null, allocateditem, blockcash, reconciledbyadministrator, singleclaim, fileIdentifier);
                clscorporatecards.AddCorporateCard(newcard, user);
            }
            else
            {
                newcard = new cCorporateCard(provider, claimantsettlesbill, oldcard.createdon, oldcard.createdby, DateTime.Now, (int)this.ViewState["employeeid"], allocateditem, blockcash, reconciledbyadministrator, singleclaim, fileIdentifier);
                clscorporatecards.UpdateCorporateCard(newcard, user);
            }

            this.Response.Redirect("corporate_cards.aspx", true);
        }
    }
}
