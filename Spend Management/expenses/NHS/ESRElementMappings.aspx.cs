using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementLibrary;
using System.Text;

namespace Spend_Management
{
    public partial class ESRElementMappings : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int nTrustID;

            //currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ESRElements, true, true);           

            Master.title = "";
            Title = "ESR Element Mappings";
            Master.PageSubTitle = Title;

            int.TryParse(Request.QueryString["trustid"], out nTrustID);
            
            cESRElementMappings clsESRElementMappings = new cESRElementMappings(currentUser.AccountID, nTrustID);
            List<cSubcat> lstUnMappedExpenseItems = clsESRElementMappings.GetUnMappedExpenseItems();

            if (lstUnMappedExpenseItems.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                foreach (cSubcat subcat in lstUnMappedExpenseItems)
                {
                    sb.Append(subcat.subcat + "<br / >");
                }

                if (lstUnMappedExpenseItems.Count > 10)
                {
                    divUnMappedExpenseItems.Style.Add(HtmlTextWriterStyle.Height, "150px");
                    divUnMappedExpenseItems.Style.Add(HtmlTextWriterStyle.Overflow, "auto");
                    divUnMappedExpenseItemHolder.Style.Add(HtmlTextWriterStyle.MarginBottom, "30px");
                }


                litUnMappedExpenseItems.Text = sb.ToString();

            }
            else
            {
                divUnMappedExpenseItemHolder.Visible = false;
            }

            cFinancialExports clsExports = new cFinancialExports(currentUser.AccountID);

            cFinancialExport export = clsExports.getESRExport(nTrustID);

            if (export == null)
            {
                Literal litNoExport = new Literal();
                litNoExport.Text = "There is no Financial Export for the ESR Inbound interface set up for this trust.<br /><br />You need to create a financial export for the ESR Inbound interface for this trust before you can map the elements";
                pnlElementSummary.Controls.Add(litNoExport);
            }
            else
            {
                var lnkAddMapping = new HyperLink();
                lnkAddMapping.NavigateUrl = "aeESRElementMapping.aspx?trustid=" + nTrustID.ToString();
                lnkAddMapping.Text = "Add Mapping";
                lnkAddMapping.Attributes.Add("class", "submenuitem");
                pnlLinks.Controls.Add(lnkAddMapping);

                string strsql = "SELECT ESRElements.elementID, globalESRElements.ESRElementName, ESRElements.NHSTrustID FROM dbo.ESRElements";

                var clsFields = new cFields(currentUser.AccountID);
                var grid = new cGridNew(currentUser.AccountID, currentUser.EmployeeID, "gridESRElementSummary", strsql);

                var nhsId = clsFields.GetFieldByID(new Guid("B1FA6096-2A52-4A0C-B0C9-B3F0CFE3A376"));
                grid.addFilter(nhsId, ConditionType.Equals, new object[] { nTrustID }, null, ConditionJoiner.None);
                grid.enableupdating = true;
                grid.editlink = "aeESRElementMapping.aspx?trustid=" + nTrustID.ToString() + "&elementid={elementID}";
                grid.enabledeleting = true;
                grid.deletelink = "javascript:DeleteElementMapping({elementID}," + nTrustID.ToString() + ");";
                grid.getColumnByName("elementID").hidden = true;
                grid.getColumnByName("NHSTrustID").hidden = true;
                grid.KeyField = "ElementID";
                grid.EmptyText = "There are no mapped ESR Elements for this Trust";

                Literal str = new Literal();

                string[] gridData = grid.generateGrid();
                str.Text = gridData[1];

                pnlElementSummary.Controls.Add(str);

                // set the sel.grid javascript variables
                Page.ClientScript.RegisterStartupScript(this.GetType(), "ESRElementMappingGridVars", cGridNew.generateJS_init("ESRElementMappingGridVars", new List<string>() { gridData[0] }, currentUser.CurrentActiveModule), true);
            }
        }
    }
}
