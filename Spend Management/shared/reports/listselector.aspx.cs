using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using expenses;

using SpendManagementLibrary;
using Spend_Management;
using System.Collections.Generic;
public partial class reports_listselector : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack == false)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;

            

            Guid fieldid = new Guid(Request.QueryString["fieldid"]);

            cFields clsfields = new cFields(user.AccountID);
            cField reqfield = clsfields.GetFieldByID(fieldid);
            int key;

            if (reqfield.ValueList)
            {
                foreach (KeyValuePair<object, string> kvp in reqfield.ListItems)
                {
                    key = int.Parse(kvp.Key.ToString());
                    treeitems.Nodes.Add(kvp.Value, key);
                }
            }
            else
            {
                cReports clsreports = new cReports(user.AccountID, user.CurrentSubAccountId);
                clsreports.getAvailableListItems(reqfield, 0, ref treeitems);
            }

            if (Request.QueryString["id"] != null)
            {
                string[] items = Request.QueryString["id"].Split(',');
                Infragistics.WebUI.UltraWebNavigator.Node node;
                for (int i = 0; i < items.Length; i++)
                {
                    node = treeitems.Find(int.Parse(items[i]));
                    if (node != null)
                    {
                        node.Checked = true;
                    }
                }
            }

            cColours clscolours = new cColours(user.AccountID, user.CurrentSubAccountId, user.CurrentActiveModule);
            System.Text.StringBuilder output = new System.Text.StringBuilder();
            output.Append("<style type=\"text/css\">\n");
            if (clscolours.headerBGColour != clscolours.defaultHeaderBGColour)
            {
                output.Append(".infobar\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");
                output.Append(".datatbl th\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");
                output.Append(".inputpaneltitle\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");
                output.Append(".paneltitle\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");
                output.Append(".homepaneltitle\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.headerBGColour + ";\n");
                output.Append("border-color: " + clscolours.headerBGColour + ";\n");
                output.Append("}\n");
            }
            if (clscolours.rowBGColour != clscolours.defaultRowBGColour)
            {
                output.Append(".datatbl .row1\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.rowBGColour + ";\n");
                output.Append("}\n");
            }
            if (clscolours.rowTxtColour != clscolours.defaultRowTxtColour)
            {
                output.Append(".datatbl .row1\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.rowTxtColour + ";\n");
                output.Append("}\n");
            }
            if (clscolours.altRowBGColour != clscolours.defaultAltRowBGColour)
            {
                output.Append(".datatbl .row2\n");
                output.Append("{\n");
                output.Append("background-color: " + clscolours.defaultAltRowBGColour + ";\n");
                output.Append("}\n");
            }
            if (clscolours.altRowTxtColour != clscolours.defaultAltRowTxtColour)
            {
                output.Append(".datatbl .row2\n");
                output.Append("{\n");
                output.Append("color: " + clscolours.altRowTxtColour + ";\n");
                output.Append("}\n");
            }
            output.Append("</style>");
            litstyles.Text = output.ToString();
        }
    }
    
}
