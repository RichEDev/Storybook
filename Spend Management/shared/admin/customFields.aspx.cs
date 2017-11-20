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
using SpendManagementLibrary;

namespace Spend_Management
{
    public partial class customFields : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                Master.title = "Custom Fields";
                Master.PageSubTitle = "Add/Edit Custom Fields";
                CurrentUser user = cMisc.GetCurrentUser();
                cTables clsTables = new cTables(user.AccountID);
                cCustomFields clsCustFields = new cCustomFields(user.AccountID);
                ddlCustFieldType.Attributes.Add("onchange", "showCustFieldOptions();");
                string[] gridData = clsCustFields.CreateCustomFieldGrid();
                litFieldGrid.Text = gridData[1];

                ddlTables.Items.Insert(0, new ListItem("[None]", "0"));
                ddlTables.Items.AddRange(clsTables.CreateTablesDropDown().ToArray());
                ddlFields.Items.Insert(0, new ListItem("[None]", "0"));

                ClientScriptManager scrmgr = this.ClientScript;
                scrmgr.RegisterStartupScript(this.GetType(), "custFieldGridVars", cGridNew.generateJS_init("custFieldGridVars", new System.Collections.Generic.List<string>() { gridData[0]}, user.CurrentActiveModule), true);
            }

        }
    }
}
