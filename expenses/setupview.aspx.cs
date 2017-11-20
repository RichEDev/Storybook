using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using expenses.Old_App_Code;
using Spend_Management;
using SpendManagementLibrary;

namespace expenses
{
    /// <summary>
    /// Summary description for setupview.
    /// </summary>
    public partial class setupview : Page
    {
        cViews clsviews;

        int viewid = 0;
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Set-Up View";
            Master.title = Title;
            Master.helpid = 1170;

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                clsviews = new cViews(user.AccountID, user.EmployeeID);
                viewid = int.Parse(Request.QueryString["viewid"]);
                int viewforemployeeid;
                int.TryParse(Request.QueryString["viewowner"], out viewforemployeeid);
                ViewState["viewforemployeeid"] = viewforemployeeid;
                txtviewid.Text = viewid.ToString();
                lstselected.Items.AddRange(clsviews.getSelectedItems(viewid));
                CreateAvailableTreeStructure();

                if (Request.QueryString["claimid"] != null)
                {
                    ViewState["claimid"] = Request.QueryString["claimid"];
                }
                if (Request.QueryString["stage"] != null)
                {
                    ViewState["stage"] = Request.QueryString["stage"];
                }

                if (txtviewid.Text == "-2")
                {
                    user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.DefaultPrintView, true, true);
                }
            }
        }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            string fromClaimSelector = Request["claimSelector"] == null ? "false" : "true";
            this.cmdok.Attributes.Add("claimselector", fromClaimSelector);
            this.cmdcancel.Attributes.Add("claimselector", fromClaimSelector);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdup.Click += new System.Web.UI.ImageClickEventHandler(this.cmdup_Click);
            this.cmdright.Click += new System.Web.UI.ImageClickEventHandler(this.cmdright_Click);
            this.cmddown.Click += new System.Web.UI.ImageClickEventHandler(this.cmddown_Click);
            this.cmdleft.Click += new System.Web.UI.ImageClickEventHandler(this.cmdleft_Click);
            this.cmddleft.Click += new System.Web.UI.ImageClickEventHandler(this.cmddleft_Click);
            this.cmdok.Click += new System.Web.UI.ImageClickEventHandler(this.cmdok_Click);
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);
        }
        #endregion

        private void CreateAvailableTreeStructure()
        {
            cFields clsfields = new cFields((int)ViewState["accountid"]);
            cViewGroups clsviewgroups = new cViewGroups();
            List<cViewGroup> groups = clsviewgroups.getParentGroups();

            SortedList<string, cViewGroup> sortedGroups = new SortedList<string, cViewGroup>();
            foreach (cViewGroup group in groups)
            {
                sortedGroups.Add(group.groupname, group);
            }

            Infragistics.WebUI.UltraWebNavigator.Node node;

            foreach (cViewGroup group in sortedGroups.Values)
            {
                node = new Infragistics.WebUI.UltraWebNavigator.Node();
                node.Text = group.groupname;
                node.Tag = group.viewgroupid;
                node.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
                if (group.children.Count > 0 || clsfields.getFieldsByViewGroup(group.viewgroupid).Count > 0)
                {
                    node.ShowExpand = true;
                }
                lstavailable.Nodes.Add(node);
            }

        }
        protected void cmdright_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            cFields clsfields = new cFields((int)ViewState["accountid"]);
            cField newField, curField;

            System.Web.UI.WebControls.ListItem item;
            Infragistics.WebUI.UltraWebNavigator.Node curnode;

            System.Collections.IEnumerator enumer = lstavailable.CheckedNodes.GetEnumerator();
            int i = 0;

            while (enumer.MoveNext())
            {
                curnode = (Infragistics.WebUI.UltraWebNavigator.Node)enumer.Current;
                item = new System.Web.UI.WebControls.ListItem();
                curnode = (Infragistics.WebUI.UltraWebNavigator.Node)lstavailable.CheckedNodes[i];
                item.Text = curnode.Text;
                item.Value = curnode.Tag.ToString();

                newField = clsfields.GetFieldByID(new Guid(item.Value));

                bool exists = false;
                foreach (ListItem tmp in lstselected.Items)
                {
                    curField = clsfields.GetFieldByID(new Guid(tmp.Value));

                    if (tmp.Text == item.Text)
                    {
                        // check that field isn't a udf with same description as main field
                        if (newField.FieldName.StartsWith("udf") && curField.FieldName != newField.FieldName)
                        {
                            continue;
                        }

                        // if field is from savedexpenses, it will have been substituted for _current or _previous so don't add - otherwise allow
                        if (newField.GetParentTable().TableName == "savedexpenses" || newField.GetParentTable().TableID == curField.TableID)
                        {
                            exists = true;
                            break;
                        }
                    }
                }
                if (!exists)
                {
                    lstselected.Items.Add(item);
                }
                curnode.Hidden = true;
                curnode.Checked = false;
            }

        }

        protected void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            int i;

            string claimid = string.Empty;
            if (this.ViewState["claimid"] != null)
            {
                claimid = ViewState["claimid"].ToString();
            }

            var fields = new ArrayList();
            this.clsviews = new cViews((int)ViewState["accountid"], (int)ViewState["employeeid"]);

            int viewId = int.Parse(this.txtviewid.Text);
            for (i = 0; i < this.lstselected.Items.Count; i++)
            {
                var fieldid = new Guid(this.lstselected.Items[i].Value);
                fields.Add(fieldid);
            }

            if (viewId == -1)
            {
                this.clsviews.updateDefaultView(fields);
            }
            else if (viewId == -2)
            {
                this.clsviews.updatePrintView(fields);
            }
            else
            {
                this.clsviews.updateUserView(viewId, fields, false);
            }

            string viewOwnerEmployeeId = string.Empty;
            if (this.ViewState["viewforemployeeid"] != null && (int)ViewState["viewforemployeeid"] > 0)
            {
                viewOwnerEmployeeId = string.Format("employeeid={0}&", (int)ViewState["viewforemployeeid"]);
            }

            string claimSelectorUrlPart = ((ImageButton)sender).Attributes["claimselector"] == "false" ? string.Empty : "&claimSelector=true";
            switch (viewId)
            {
                case 1:
                case 2:
                case 3:
                    Response.Redirect(string.Format("expenses/claimViewer.aspx?{0}claimid={1}{2}", viewOwnerEmployeeId, claimid, claimSelectorUrlPart), true);
                    break;
                case 4:
                    Response.Redirect("expenses/checkexpenselist.aspx?claimid=" + claimid, true);
                    break;
                case -1:
                case -2:
                    this.lblmsg.Text = "The default view has been updated successfully.";
                    this.lblmsg.Visible = true;
                    Response.Redirect("tailoringmenu.aspx", true);
                    break;
            }
        }

        protected void cmddright_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {




        }

        protected void cmdleft_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstselected.SelectedValue != string.Empty)
            {
                Guid fieldid = Guid.Empty;
                fieldid = new Guid(lstselected.SelectedValue);

                if (lstavailable.Find(fieldid) != null)
                {
                    lstavailable.Find(fieldid).Hidden = false;
                }
                else
                {
                    lstavailable.Nodes.Add(lstselected.Items[lstselected.SelectedIndex].Text, fieldid);
                }
                lstselected.Items.RemoveAt(lstselected.SelectedIndex);
            }
        }

        protected void cmddleft_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            int i = 0;
            Guid fieldid = Guid.Empty;
            for (i = lstselected.Items.Count - 1; i >= 0; i--)
            {
                fieldid = new Guid(lstselected.Items[i].Value);
                if (lstavailable.Find(fieldid) != null)
                {
                    lstavailable.Find(fieldid).Hidden = false;
                }
                else
                {
                    lstavailable.Nodes.Add(lstselected.Items[i].Text, fieldid);
                }
                lstselected.Items.RemoveAt(i);
            }
        }

        protected void cmdup_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstselected.SelectedIndex != -1)
            {
                System.Web.UI.WebControls.ListItem temp = new ListItem();
                int selectedindex = lstselected.SelectedIndex;
                string text = string.Empty;
                string strvalue = string.Empty;


                if (lstselected.SelectedIndex != 0)
                {
                    text = lstselected.Items[selectedindex - 1].Text;
                    strvalue = lstselected.Items[selectedindex - 1].Value;
                    lstselected.Items[selectedindex - 1].Text = lstselected.Items[selectedindex].Text;
                    lstselected.Items[selectedindex - 1].Value = lstselected.Items[selectedindex].Value;
                    lstselected.Items[selectedindex].Selected = false;
                    lstselected.Items[selectedindex - 1].Selected = true;

                    lstselected.Items[selectedindex].Text = text;
                    lstselected.Items[selectedindex].Value = strvalue;

                }
            }
        }

        protected void cmddown_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (lstselected.SelectedIndex != -1)
            {
                int selectedindex = lstselected.SelectedIndex;
                string text = string.Empty;
                string strvalue = string.Empty;

                if (lstselected.SelectedIndex != (lstselected.Items.Count - 1))
                {
                    text = lstselected.Items[selectedindex + 1].Text;
                    strvalue = lstselected.Items[selectedindex + 1].Value;
                    lstselected.Items[selectedindex + 1].Text = lstselected.Items[selectedindex].Text;
                    lstselected.Items[selectedindex + 1].Value = lstselected.Items[selectedindex].Value;
                    lstselected.Items[selectedindex].Selected = false;
                    lstselected.Items[selectedindex + 1].Selected = true;

                    lstselected.Items[selectedindex].Text = text;
                    lstselected.Items[selectedindex].Value = strvalue;

                }
            }
        }

        protected void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            string claimid = string.Empty;
            if (this.ViewState["claimid"] != null)
            {
                claimid = ViewState["claimid"].ToString();
            }

            int viewId = int.Parse(this.txtviewid.Text);

            string viewOwnerEmployeeId = string.Empty;
            if (this.ViewState["viewforemployeeid"] != null && (int)ViewState["viewforemployeeid"] > 0)
            {
                viewOwnerEmployeeId = string.Format("employeeid={0}&", (int)ViewState["viewforemployeeid"]);
            }

            string claimSelectorUrlPart = ((ImageButton)sender).Attributes["claimselector"] == "false" ? string.Empty : "&claimSelector=true";

            switch (viewId)
            {
                case 1:
                case 2:
                case 3:
                    Response.Redirect(string.Format("expenses/claimViewer.aspx?{0}claimid={1}{2}", viewOwnerEmployeeId, claimid, claimSelectorUrlPart), true);
                    break;
                case 4:
                    Response.Redirect("expenses/checkexpenselist.aspx?claimid=" + claimid, true);
                    break;
                case -1:
                case -2:
                    Response.Redirect("tailoringmenu.aspx", true);
                    break;
            }
        }

        protected void lstavailable_DemandLoad(object sender, Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
        {

            List<Guid> selecteditems = new List<Guid>();
            foreach (ListItem i in lstselected.Items)
            {
                selecteditems.Add(new Guid(i.Value));
            }
            Guid viewgroupid = (Guid)e.Node.Tag;
            cViewGroups clsviewgroups = new cViewGroups();
            cFields clsfields = new cFields((int)ViewState["accountid"]);
            cViewGroup currentgroup = clsviewgroups.getViewGroupById(viewgroupid);
            SortedList<string, cViewGroup> sortedGroups = new SortedList<string, cViewGroup>();
            foreach (cViewGroup group in currentgroup.children.Values)
            {
                sortedGroups.Add(group.groupname, group);
            }

            Infragistics.WebUI.UltraWebNavigator.Node node;

            foreach (cViewGroup group in sortedGroups.Values)
            {
                node = new Infragistics.WebUI.UltraWebNavigator.Node();
                node.Text = group.groupname;
                node.Tag = group.viewgroupid;
                node.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
                if (group.children.Count > 0 || clsfields.getFieldsByViewGroup(group.viewgroupid).Count > 0)
                {
                    node.ShowExpand = true;
                }
                e.Node.Nodes.Add(node);
            }

            SortedList<Guid, cField> fields = clsfields.getAllFieldsByViewGroup(viewgroupid);
            foreach (cField field in fields.Values)
            {
                if (field.NormalView && !field.IDField && !selecteditems.Contains(field.FieldID))
                {
                    node = new Infragistics.WebUI.UltraWebNavigator.Node();
                    node.Text = field.Description;
                    node.Tag = field.FieldID;
                    e.Node.Nodes.Add(node);
                }
            }
        }
    }
}
