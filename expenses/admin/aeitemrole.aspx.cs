using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;

using SpendManagementLibrary;
using Spend_Management;
public partial class admin_aeitemrole : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        // ADDED PAGE TITLE.
        Title = "Add / Edit Item Role";
        Master.title = Title;
        Master.showdummymenu = true;

        var updatePanel = false;

        if (IsPostBack == false)
        {
            Master.enablenavigation = false;

            CurrentUser user = cMisc.GetCurrentUser();
            user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ItemRoles, true, true);

            ViewState["accountid"] = user.AccountID;
            ViewState["employeeid"] = user.EmployeeID;
            int itemroleid = 0;

            if (Request.QueryString["itemroleid"] != null)
            {
                itemroleid = int.Parse(Request.QueryString["itemroleid"]);
            }
            ViewState["itemroleid"] = itemroleid;

            if (itemroleid > 0)
            {
                cItemRoles clsroles = new cItemRoles(user.AccountID);
                cItemRole reqrole = clsroles.getItemRoleById(itemroleid);
                txtrolename.Text = reqrole.rolename;
                txtdescription.Text = reqrole.description;
                List<cRoleSubcat> items = new List<cRoleSubcat>();
                foreach (cRoleSubcat rolesub in reqrole.items.Values)
                {
                    items.Add(rolesub);
                }
                ViewState["items"] = items;
            }
            else
            {
                ViewState["items"] = new List<cRoleSubcat>();
            }
            ClientScript.RegisterHiddenField("itemroleid", itemroleid.ToString());
            ClientScript.RegisterHiddenField("accountid", user.AccountID.ToString());
        }
        else
        {
            var controlEvent = this.Request["ctl00$scriptman"];
            if (controlEvent != null)
            {
                var controlName = controlEvent.Split('|').Last().Replace("$", "_");
                if (controlName.Contains("btnDel"))
                {
                    var buttonId = controlName.Split(' ').LastOrDefault();
                    if (buttonId != null)
                    {
                        int subcatId = int.Parse(buttonId);
                        var items = this.DeleteitemFromViewStateList(subcatId);
                        this.ViewState["items"] = items;
                        updatePanel = true;
                    }
                }

                if (controlName.Contains("lnksubcat"))
                {
                    var buttonId = controlName.Replace("lnksubcat", "#").Split('#').LastOrDefault();
                    if (buttonId != null)
                    {
                        int subcatId = int.Parse(buttonId);
                        cRoleSubcat rolesub;
                        var items = this.AddSubCatToTable(subcatId, out rolesub);
                        this.ViewState["items"] = items;
                        updatePanel = true;
                    }
                }
            }
        }

        var currentItems = (List<cRoleSubcat>)this.ViewState["items"];

        this.populateCheckList(currentItems);
        this.populateItems(currentItems);
        if (updatePanel)
        {
            this.UpdatePanel1.Update();
        }
    }

    public void populateCheckList(List<cRoleSubcat> items)
    {
        List<int> lstsubcats = new List<int>();
        foreach (cRoleSubcat rolesub in items)
        {
            lstsubcats.Add(rolesub.SubcatId);
        }
        pnlitems.Controls.Clear();
        var clssubcats = new cSubcats((int)ViewState["accountid"]);
        LinkButton lnksubcat;
        List<SubcatBasic> subcats = clssubcats.GetSortedList();

        foreach (SubcatBasic subcat in subcats)
        {
            if (!lstsubcats.Contains(subcat.SubcatId))
            {
                lnksubcat = new LinkButton();
                lnksubcat.ID = "lnksubcat" + subcat.SubcatId;
                lnksubcat.Click += this.lnksubcat_Click;
                lnksubcat.Text = subcat.Subcat;
                lnksubcat.CommandName = "subcat";
                lnksubcat.CssClass = "dropdownitem";
                lnksubcat.CommandArgument = subcat.SubcatId.ToString(CultureInfo.InvariantCulture);
                pnlitems.Controls.Add(lnksubcat);
            }
        }   
    }

    void lnksubcat_Click(object sender, EventArgs e)
    {
    }

    private List<cRoleSubcat> AddSubCatToTable(int subcatid, out cRoleSubcat rolesub)
    {
        if (ViewState["action"] == null && ViewState["items"] == null) //add the role
        {
            string rolename = txtrolename.Text;
            string description = txtdescription.Text;
            cItemRoles clsroles = new cItemRoles((int)ViewState["accountid"]);
            int roleid = clsroles.addRole(rolename, description, new List<cRoleSubcat>(), (int)ViewState["employeeid"]);
            ViewState["action"] = 2;
            ViewState["itemroleid"] = roleid;
            ViewState["items"] = new List<cRoleSubcat>();
        }

        rolesub = new cRoleSubcat(0, 0, subcatid, 0, 0, false);
        var items = (List<cRoleSubcat>)this.ViewState["items"];
        items.Add(rolesub);
        this.ViewState["items"] = items;
        return items;
    }

    private void populateItems(List<cRoleSubcat> items)
    {
        TableRow row;
        TableCell cell;
        TextBox txtbox;
        CheckBox chkbox;
        ImageButton cmddel;

        CompareValidator cellValidator;

        TableHeaderRow hrow = new TableHeaderRow();

        TableHeaderCell th;
        th = new TableHeaderCell();
        th.Text = "&nbsp;";
        th.Width = 25;
        hrow.Cells.Add(th);
        th = new TableHeaderCell();
        th.Text = "Expense Item";
        hrow.Cells.Add(th);
        th = new TableHeaderCell();
        th.Text = "Maximum Limit<br>Without Receipt";
        th.Width = 128;
        hrow.Cells.Add(th);
        th = new TableHeaderCell();
        th.Text = "Maximum Limit<br>With Receipt";
        th.Width = 128;
        hrow.Cells.Add(th);
        th = new TableHeaderCell();
        th.Text = "Add to Template";
        th.Width = 104;
        hrow.Cells.Add(th);
        tblitems.Rows.Add(hrow);

        foreach (cRoleSubcat rolesub in items)
        {
            row = new TableRow();
            row.ID = rolesub.SubcatId.ToString();
            cmddel = new ImageButton();
            cmddel.ImageUrl = "~/icons/delete2.gif";
            cmddel.Click += new ImageClickEventHandler(cmddel_Click);
            cmddel.ID = string.Format("btnDel {0}", rolesub.SubcatId);
            cmddel.CommandArgument = rolesub.SubcatId.ToString();
            
            cell = new TableCell();
            cell.Controls.Add(cmddel);
            
            var subcat = new cSubcats(cMisc.GetCurrentUser().AccountID).GetSubcatBasic(rolesub.SubcatId);
           
            row.Cells.Add(cell);
            cell = new TableCell();
            cell.Text = subcat.Subcat;
            row.Cells.Add(cell);
            cell = new TableCell();
            txtbox = new TextBox();
            txtbox.ID = "txtmaxwithout" + rolesub.SubcatId;
            txtbox.Text = rolesub.receiptmaximum.ToString("######0.00");
            txtbox.Style["text-align"] = "right";
            cell.Controls.Add(txtbox);
            cellValidator = new CompareValidator();
            cellValidator.ID = "cvmaxwithout" + rolesub.SubcatId;
            cellValidator.ControlToValidate = "txtmaxwithout" + rolesub.SubcatId;
            cellValidator.Type = ValidationDataType.Currency;
            cellValidator.Operator = ValidationCompareOperator.DataTypeCheck;
            cellValidator.ErrorMessage = "Max limit without a receipt must be a number.";
            cellValidator.Text = "*";
            cell.Controls.Add(cellValidator);
            row.Cells.Add(cell);
            cell = new TableCell();
            txtbox = new TextBox();
            txtbox.ID = "txtmaxwith" + rolesub.SubcatId;
            txtbox.Text = rolesub.maximum.ToString("#####0.00");
            txtbox.Style["text-align"] = "right";
            cell.Controls.Add(txtbox);
            cellValidator = new CompareValidator();
            cellValidator.ID = "cvmaxwith" + rolesub.SubcatId;
            cellValidator.ControlToValidate = "txtmaxwith" + rolesub.SubcatId;
            cellValidator.Type = ValidationDataType.Currency;
            cellValidator.Operator = ValidationCompareOperator.DataTypeCheck;
            cellValidator.ErrorMessage = "Max limit with a receipt must be a number.";
            cellValidator.Text = "*";
            cell.Controls.Add(cellValidator);
            row.Cells.Add(cell);

            cell = new TableCell();
            chkbox = new CheckBox();
            chkbox.ID = "chkadditem" + rolesub.SubcatId;
            chkbox.Checked = rolesub.isadditem;
            cell.Controls.Add(chkbox);
            row.Cells.Add(cell);
            
            
            tblitems.Rows.Add(row);
        }

        //UpdatePanel1.Update();
    }

    void cmddel_Click(object sender, ImageClickEventArgs e)
    {
    }

    private List<cRoleSubcat> DeleteitemFromViewStateList(int subcatid)
    {
        List<cRoleSubcat> items = (List<cRoleSubcat>)this.ViewState["items"];

        foreach (cRoleSubcat rolesub in items)
        {
            if (rolesub.SubcatId == subcatid)
            {
                items.Remove(rolesub);
                break;
            }
        }

        this.tblitems.Rows.Clear();
        return items;
    }

    [WebMethod(EnableSession = true)]
    public static bool alreadyExists(int accountid, int itemroleid, string name)
    {
        cItemRoles clsroles = new cItemRoles(accountid);
        return clsroles.alreadyExists(itemroleid, name);
    }

    protected void cmdok_Click(object sender, ImageClickEventArgs e)
    {
        string rolename = txtrolename.Text;
        string description = txtdescription.Text;

        cItemRoles clsroles = new cItemRoles((int)ViewState["accountid"]);
        List<cRoleSubcat> items = getItems();
        int itemroleid = (int)ViewState["itemroleid"];
        int returnvalue;
        if (itemroleid > 0)
        {
            returnvalue = clsroles.updateRole(itemroleid, rolename, description, items, (int)ViewState["employeeid"]);
        }
        else
        {
            returnvalue = clsroles.addRole(rolename, description, items,(int)ViewState["employeeid"]);
        }
        if (returnvalue == -1)
        {
            ClientScript.RegisterStartupScript(Page.GetType(), "alert", "alert('The item role name you have entered already exists')", true);
        }
        else
        {
            Response.Redirect("adminitemroles.aspx", true);
        }
    }

    private List<cRoleSubcat> getItems()
    {
        cRoleSubcat newrolesub;
        int subcatid;
        decimal receiptmaximum, maximum;
        bool isadditem;
        TextBox txtbox;
        CheckBox chkbox;
        List<cRoleSubcat> items = (List<cRoleSubcat>)ViewState["items"];
        List<cRoleSubcat> finalitems = new List<cRoleSubcat>();

        foreach (cRoleSubcat rolesub in items)
        {
            subcatid = rolesub.SubcatId;
            txtbox = (TextBox)pnlitems.FindControl("txtmaxwithout" + subcatid);
            if (txtbox.Text != "")
            {
                receiptmaximum = decimal.Parse(txtbox.Text);
            }
            else
            {
                receiptmaximum = 0;
            }
            txtbox = (TextBox)pnlitems.FindControl("txtmaxwith" + subcatid);
            if (txtbox.Text != "")
            {
                maximum = decimal.Parse(txtbox.Text);
            }
            else
            {
                maximum = 0;
            }
            chkbox = (CheckBox)pnlitems.FindControl("chkadditem" + subcatid);
            isadditem = chkbox.Checked;
            newrolesub = new cRoleSubcat(0, 0, rolesub.SubcatId, maximum, receiptmaximum, isadditem);
            finalitems.Add(newrolesub);
        }
        return finalitems;
    }
}
