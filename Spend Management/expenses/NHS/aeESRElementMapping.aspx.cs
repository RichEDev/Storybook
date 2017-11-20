using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;

namespace Spend_Management
{
    using System.Globalization;

    public partial class aeESRElementMapping : System.Web.UI.Page
    {        
        System.Web.Caching.Cache cache = new System.Web.Caching.Cache();

        int nTrustID;
        int nElementID;

        /// <summary>
        /// TrustID passed to page
        /// </summary>
        public int TrustID
        {
            get { return nTrustID; }
        }
        /// <summary>
        /// ElementID passed to page
        /// </summary>
        public int ElementID
        {
            get { return nElementID; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            //currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRElements, true, true);

            #region query string checks
            int.TryParse(Request.QueryString["trustid"], out nTrustID);
            if(nTrustID <= 0)
            {
                Response.Redirect(ErrorHandlerWeb.MissingRecordUrl, true);
            }
            int.TryParse(Request.QueryString["elementid"], out nElementID);
            #endregion query string checks

            cESRElementMappings clsElementMappings = new cESRElementMappings(currentUser.AccountID, nTrustID);

            if (!IsPostBack && !IsCallback && !IsAsync)
            {
                Master.title = "";
                Master.PageSubTitle = "ESR Element Mapping";
                Master.enablenavigation = false;

                cESRElement reqElement = null;
                if (nElementID > 0)
                {
                    reqElement = clsElementMappings.getESRElementByID(nElementID);
                }

                #region elements tab
                ddlESRElements.Items.AddRange(clsElementMappings.CreateGlobalElementDropDown().ToArray()); // Populate Element Dropdown
                if (reqElement != null)
                {
                    if (ddlESRElements.Items.FindByValue(reqElement.GlobalElementID.ToString()) != null)
                    {
                        ddlESRElements.SelectedValue = reqElement.GlobalElementID.ToString();
                        ddlESRElements.Enabled = false;
                    }
                }
                else
                {
                    SortedList<int, cESRElement> list = clsElementMappings.listElements();
                    foreach (cESRElement val in list.Values)
                    {
                        if (ddlESRElements.Items.FindByValue(val.GlobalElementID.ToString()) != null)
                        {
                            ddlESRElements.Items.Remove(ddlESRElements.Items.FindByValue(val.GlobalElementID.ToString()));
                        }
                    }
                    if (ddlESRElements.Items.Count == 0)
                    {
                        ddlESRElements.Items.Add(new ListItem("There are no Elements remaining to map"));
                        ddlESRElements.Enabled = false;
                        btnSubmit.Style.Add(HtmlTextWriterStyle.Display, "none");
                        btnCancel.ImageUrl = "~/shared/images/buttons/btn_close.png";
                        return;
                    }
                }
                cGlobalESRElement clsGlobalElement = clsElementMappings.GetGlobalESRElementByID(Convert.ToInt32(ddlESRElements.SelectedValue));
                List<Panel> pnlTwoColumns = clsElementMappings.CreateESRElementFieldPanels(clsGlobalElement, clsElementMappings, reqElement);
                foreach (Panel pnl in pnlTwoColumns)
                {
                    pnlESRElementFields.Controls.Add(pnl);
                }
                ViewState["totalRows"] = pnlTwoColumns.Count();
                #endregion elements tab

                #region SubCats tab
                var subcats = new cSubcats(currentUser.AccountID);
                List<SubcatBasic> subcatBasics = subcats.GetSortedList();
                
                Label lblSubCatName;
                HiddenField hfSubCatID;
                CheckBox chkSubCatSelected;
                Label spanValidator;
                Label spanIcon;
                Label spanToolTip;

                int inputCount = 0;
                Panel pnlTwoColumn = new Panel();
                pnlTwoColumn.CssClass = "twocolumn";

                foreach (SubcatBasic subcat in subcatBasics)
                {
                    // if it's the third input since a div creation, add the current div to the page then create a new div to hold it
                    if (inputCount == 2)
                    {
                        pnlSubCats.Controls.Add(pnlTwoColumn);
                        pnlTwoColumn = new Panel();
                        pnlTwoColumn.CssClass = "twocolumn";
                        inputCount = 0;
                    }

                    lblSubCatName = new Label();
                    hfSubCatID = new HiddenField();
                    chkSubCatSelected = new CheckBox();
                    spanValidator = new Label();
                    spanIcon = new Label();
                    spanToolTip = new Label();

                    lblSubCatName.ID = string.Format("subcat_{0}", subcat.SubcatId);
                    lblSubCatName.Text = subcat.Subcat;
                    chkSubCatSelected.ID = string.Format("subcat_selected_{0}", subcat.SubcatId);
                    chkSubCatSelected.CssClass = "inputs";
                    if (reqElement != null)
                    {
                        if (reqElement.Subcats.Contains(subcat.SubcatId))
                        {
                            chkSubCatSelected.Checked = true;
                        }
                    }

                    lblSubCatName.AssociatedControlID = chkSubCatSelected.ID;

                    spanValidator.CssClass = "inputvalidatorfield";
                    spanValidator.Text = "&nbsp;";

                    spanIcon.CssClass = "inputicon";
                    spanIcon.Text = "&nbsp;";

                    spanToolTip.CssClass = "inputtooltipfield";
                    spanToolTip.Text = "&nbsp;";

                    hfSubCatID.ID = "subcatid_" + subcat.SubcatId;
                    hfSubCatID.Value = subcat.SubcatId.ToString(CultureInfo.InvariantCulture);

                    pnlTwoColumn.Controls.Add(lblSubCatName);
                    pnlTwoColumn.Controls.Add(chkSubCatSelected);
                    pnlTwoColumn.Controls.Add(spanValidator);
                    pnlTwoColumn.Controls.Add(spanIcon);
                    pnlTwoColumn.Controls.Add(spanToolTip);
                    pnlTwoColumn.Controls.Add(hfSubCatID);

                    inputCount++;
                }

                // add the last div to the page if there is one to add
                if (inputCount > 0)
                {
                    pnlSubCats.Controls.Add(pnlTwoColumn);
                }
                #endregion SubCats tab
            }
        }

        protected void ddlESRElements_SelectedIndexChanged(object sender, EventArgs e)
        {
            #region ESR Fields update panel
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cESRElementMappings clsElementMappings = new cESRElementMappings(currentUser.AccountID, nTrustID);
            cGlobalESRElement clsGlobalElement = clsElementMappings.GetGlobalESRElementByID(Convert.ToInt32(ddlESRElements.SelectedValue));
            List<Panel> pnlTwoColumns = clsElementMappings.CreateESRElementFieldPanels(clsGlobalElement, clsElementMappings, null);
            foreach (Panel pnl in pnlTwoColumns)
            {
                pnlESRElementFields.Controls.Add(pnl);
            }
            ViewState["totalRows"] = pnlTwoColumns.Count();
            #endregion ESR Fields update panel
        }
    }
}
