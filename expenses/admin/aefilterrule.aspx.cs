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

using System.Collections.Generic;
using AjaxControlToolkit;
using BusinessLogic;
using BusinessLogic.DataConnections;
using BusinessLogic.ProjectCodes;
using SpendManagementLibrary;
using expenses.Old_App_Code;
using Spend_Management;

namespace expenses.admin
{
    public partial class aefilterrule : System.Web.UI.Page
    {
        private cFilterRules _filterRules;

        private cCostcodes _costCodes;


        [Dependency]
        public IDataFactoryCustom<IProjectCodeWithUserDefinedFields, int> ProjectCodesRepository { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            Title = "Add/Edit Filter Rule";
            Master.title = Title;
            Master.showdummymenu = true;
            //Master.helpurl = "/help/AD_CAT_mileage.htm";

            if (IsPostBack == false)
            {
                Master.enablenavigation = false;
                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.FilterRules, true, true);

                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;
                ViewState["filterid"] = 0;
                ViewState["parentItemCount"] = 0;
                ViewState["childItemCount"] = 0;
                ViewState["action"] = Request.QueryString["action"];

                populateUserdefined();
                
                if (ViewState["action"].ToString() == "2")
                {
                    ViewState["filterid"] = int.Parse(Request.QueryString["filterid"]);
                    this.SetFilterRules((int)ViewState["accountid"]);
                    cFilterRule rule = this._filterRules.GetFilterRuleById((int)ViewState["filterid"]);
                    ViewState["parenttype"] = (int)rule.parent;
                    lstFilterRuleVals.Nodes.AddRange(this._filterRules.popFilterRuleValues(rule));
                    cUserdefinedFields clsudf = new cUserdefinedFields((int)ViewState["accountid"]);

                    if (rule.parent == FilterType.Userdefined)
                    {
                        cmbparent.SelectedValue = rule.paruserdefineid.ToString();
                    }
                    else
                    {
                        cmbparent.SelectedValue = rule.parent.ToString();
                    }

                    if (rule.child == FilterType.Userdefined)
                    {
                        cUserDefinedField field = clsudf.GetUserDefinedById(rule.childuserdefineid);

                        if (cmbchild.Items.FindByValue(field.userdefineid.ToString()) != null)
                        {
                            cmbchild.Items.FindByValue(field.userdefineid.ToString()).Selected = true;
                        }
                    }
                    else
                    {
                        cmbchild.SelectedValue = rule.child.ToString();
                    }
                    tblFilterRule.Enabled = false;
                    lblRuleVals.Visible = true;
                    pnlRuleVals.Visible = true;
                    generateRuleSearch(rule.parent, rule.child);
                }
                else
                {
                    ViewState["parenttype"] = Request.QueryString["FilterType"];

                    if (ViewState["parenttype"].ToString() != "0")
                    {
                        cmbparent.Enabled = false;
                        cmbparent.SelectedIndex = int.Parse(ViewState["parenttype"].ToString()) - 1;
                        
                        cmbchild.Items.Remove(cmbparent.SelectedItem);
                    }
                    populateChildDropdown();
                }

                if (cmbchild.Items.Count == 0)
                {
                    cmdAdd.Enabled = false;
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
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cmdok.Click += new ImageClickEventHandler(cmdok_Click);
            this.cmdcancel.Click += new ImageClickEventHandler(cmdcancel_Click);
            this.cmdAdd.Click += new ImageClickEventHandler(cmdAdd_Click);
            this.cmdAddVal.Click += new ImageClickEventHandler(cmdAddVal_Click);
        }

       

        #endregion

        protected override void OnInitComplete(EventArgs e)
        {
            base.OnInitComplete(e);
        }

        void cmdAdd_Click(object sender, ImageClickEventArgs e)
        {
            lblmsg.Visible = false;
            FilterType parent = FilterType.All;
            FilterType child = FilterType.All;
            int paruserdefineid = 0;
            int childuserdefineid = 0;
            bool enabled = true;

            if (cmbparent.SelectedValue == cmbparent.SelectedItem.Text)
            {
                switch (cmbparent.SelectedItem.Text)
                {
                    case "Costcode":
                        {
                            parent = FilterType.Costcode;
                            break;
                        }
                    case "Department":
                        {
                            parent = FilterType.Department;
                            break;
                        }
                    //case "Location":
                    //    {
                    //        parent = FilterType.Location;
                    //        break;
                    //    }
                    case "Projectcode":
                        {
                            parent = FilterType.Projectcode;
                            break;
                        }
                    case "Reason":
                        {
                            parent = FilterType.Reason;
                            break;
                        }
                }
                 
            }
            else
            {
                paruserdefineid = int.Parse(cmbparent.SelectedValue);
                parent = FilterType.Userdefined;
            }

            if (cmbchild.SelectedValue == cmbchild.SelectedItem.Text)
            {
                switch (cmbchild.SelectedItem.Text)
                {
                    case "Costcode":
                        {
                            child = FilterType.Costcode;
                            break;
                        }
                    case "Department":
                        {
                            child = FilterType.Department;
                            break;
                        }
                    //case "Location":
                    //    {
                    //        child = FilterType.Location;
                    //        break;
                    //    }
                    case "Projectcode":
                        {
                            child = FilterType.Projectcode;
                            break;
                        }
                    case "Reason":
                        {
                            child = FilterType.Reason;
                            break;
                        }
                }
            }
            else
            {
                childuserdefineid = int.Parse(cmbchild.SelectedValue);
                child = FilterType.Userdefined;
            }

            this.SetFilterRules((int)ViewState["accountid"]);
            sFilterRuleExistence ruleEx = this._filterRules.AddFilterRule(parent, child, paruserdefineid, childuserdefineid, enabled, (int)ViewState["employeeid"]);
            
            if (ruleEx.returncode == 1)
            {
                lblmsg.Text = ruleEx.message;
                lblmsg.Visible = true;
                return;
            }

            ViewState["filterid"] = ruleEx.returncode;
            tblFilterRule.Enabled = false;

            lblRuleVals.Visible = true;
            pnlRuleVals.Visible = true;
            generateRuleSearch(parent, child);
        }

        void cmdAddVal_Click(object sender, ImageClickEventArgs e)
        {
            cUserdefinedFields clsudf = new cUserdefinedFields((int)ViewState["accountid"]);
            this.SetFilterRules((int)ViewState["accountid"]);
            cFilterRule rule = this._filterRules.GetFilterRuleById((int)ViewState["filterid"]);
            string parVal = "";
            string childVal = "";
            int childid = 0;
            int parentid = 0;

            if ((int)ViewState["parentItemCount"] > 10)
            {
                parVal = txtParentSearch.Text;

                if (txtParentSearch.Text == "")
                {
                    return;
                }
            }
            else
            {
                parVal = cmbParVals.SelectedItem.Text;

                if (cmbParVals.Text == "")
                {
                    return;
                }
            }
            if (rule.parent != FilterType.Userdefined)
            {
                parentid = this._filterRules.getIdOfFilterType(rule.parent, parVal, this.ProjectCodesRepository);
            }
            else
            {
                cUserDefinedField field = clsudf.GetUserDefinedById(int.Parse(cmbparent.SelectedValue));
                //**todo**
                foreach (KeyValuePair<int, cListAttributeElement> kvp in field.items)
                {
                    if (kvp.Value.elementText == parVal)
                    {
                        parentid = kvp.Key;
                    }
                }
            }

            if ((int)ViewState["childItemCount"] > 10)
            {
                childVal = txtChildSearch.Text;

                if (txtChildSearch.Text == "")
                {
                    return;
                }
            }
            else
            {
                childVal = cmbChildVals.SelectedItem.Text;

                if (cmbChildVals.Text == "")
                {
                    return;
                }
            }
            

            if (rule.child != FilterType.Userdefined)
            {
                childid = this._filterRules.getIdOfFilterType(rule.child, childVal, this.ProjectCodesRepository);
            }
            else
            {
                cUserDefinedField field = clsudf.GetUserDefinedById(int.Parse(cmbchild.SelectedValue));
                //**todo**
                foreach (KeyValuePair<int, cListAttributeElement> kvp in field.items)
                {
                    if (kvp.Value.elementText == childVal)
                    {
                        childid = kvp.Key;
                    }
                }
            }

            cFilterRuleValue ruleval = new cFilterRuleValue(0, parentid, childid, (int)ViewState["filterid"], new DateTime(1900, 01, 01), 0);

            Infragistics.WebUI.UltraWebNavigator.Node node;
            
            foreach (Infragistics.WebUI.UltraWebNavigator.Node nde in lstFilterRuleVals.Nodes)
            {
                if (nde.Text == parVal)
                {
                    node = new Infragistics.WebUI.UltraWebNavigator.Node();
                    node.Text = childVal;
                    node.Tag = ruleval;
                    node.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
                    node.Images.DefaultImage.Url = "~//icons//delete2.gif";

                    foreach (Infragistics.WebUI.UltraWebNavigator.Node chnde in nde.Nodes)
                    {
                        if (chnde.Text == node.Text)
                        {
                            lblmsg.Text = "A parent item already has this child item.";
                            lblmsg.Visible = true;
                            return;
                        }
                    }
                    nde.Nodes.Add(node);
                    return;
                }

            }

            node = new Infragistics.WebUI.UltraWebNavigator.Node();
            node.Text = parVal;
            node.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
            node.Images.DefaultImage.Url = "~//icons//delete2.gif";
            node.Expanded = true;

            Infragistics.WebUI.UltraWebNavigator.Node chnode = new Infragistics.WebUI.UltraWebNavigator.Node();
            chnode.Text = childVal;
            chnode.Tag = ruleval;
            chnode.CheckBox = Infragistics.WebUI.UltraWebNavigator.CheckBoxes.False;
            chnode.Images.DefaultImage.Url = "~//icons//delete2.gif";
            node.Nodes.Add(chnode);

            lstFilterRuleVals.Nodes.Add(node);
            
            
        }

        void cmdok_Click(object sender, ImageClickEventArgs e)
        {
            this.SetFilterRules((int)ViewState["accountid"]);
            this._filterRules.DeleteFilterRuleValues((int)ViewState["filterid"]);

            var filterRules = new List<cFilterRuleValue>();

            foreach (Infragistics.WebUI.UltraWebNavigator.Node node in lstFilterRuleVals.Nodes)
            {
                foreach (Infragistics.WebUI.UltraWebNavigator.Node chnode in node.Nodes)
                {
                    filterRules.Add((cFilterRuleValue)chnode.Tag);          
                }
            }
            this._filterRules.AddFilterRuleValues(filterRules, (int) ViewState["employeeid"]);
            this._filterRules.ruleCheck();
        
            Response.Redirect("filterrules.aspx?FilterType=" + ViewState["parenttype"]);
        }

        void cmdcancel_Click(object sender, ImageClickEventArgs e)
        {
            if (ViewState["action"].ToString() != "2" && (int)ViewState["filterid"] != 0)
            {
                this.SetFilterRules((int)ViewState["accountid"]);
                this._filterRules.DeleteFilterRule((int)ViewState["filterid"]);
            }

            Response.Redirect("filterrules.aspx?FilterType=" + ViewState["parenttype"]);
        }

        protected void cmbparent_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateChildDropdown();
            cmbchild.Items.Remove(cmbparent.SelectedItem);
            //cmbchild.Items.Clear();
            //foreach (ListItem item in cmbparent.Items)
            //{
            //    if (item != cmbparent.SelectedItem)
            //    {
            //        cmbchild.Items.Add(item);
            //    }
            //}
        }

        private void generateRuleSearch(FilterType parent, FilterType child)
        {
            var clsudf = new cUserdefinedFields((int)ViewState["accountid"]);
            this.SetFilterRules((int)ViewState["accountid"]);

            //Parent values
            if (parent != FilterType.Userdefined)
            {
                sFilterRuleControlAttributes parentAtts = this._filterRules.getFilterValueAttributes(parent, this.ProjectCodesRepository);

                lblParentSearch.Text = parentAtts.labelText;

                ViewState["parentItemCount"] = parentAtts.itemCount;

                if (parentAtts.itemCount > 10)
                {
                    cellcmbpar.Visible = false;
                    celltxtpar.Visible = true;
                    autoCompPar.ServiceMethod = parentAtts.serviceMethod;
                }
                else
                {
                    cellcmbpar.Visible = true;
                    celltxtpar.Visible = false;
                    if (parentAtts.items != null)
                    {
                        cmbParVals.Items.AddRange(parentAtts.items.ToArray());
                    }
                }
            }
            else
            {
                cellcmbpar.Visible = true;
                celltxtpar.Visible = false;
                cUserDefinedField field = clsudf.GetUserDefinedById(int.Parse(cmbparent.SelectedValue));
                lblParentSearch.Text = field.label;
               
                
                foreach (KeyValuePair<int, cListAttributeElement> kp in field.items)
                {
                    cmbParVals.Items.Add(new ListItem(kp.Value.elementText, kp.Key.ToString()));
                }
                cmbParVals.SelectedIndex = 0;
            }

            //Child values

            if (child != FilterType.Userdefined)
            {
                sFilterRuleControlAttributes childAtts = this._filterRules.getFilterValueAttributes(child, this.ProjectCodesRepository);

                lblChildSearch.Text = childAtts.labelText;

                ViewState["childItemCount"] = childAtts.itemCount;

                if (childAtts.itemCount > 10)
                {
                    cellcmbchild.Visible = false;
                    celltxtchild.Visible = true;
                    autoCompChild.ServiceMethod = childAtts.serviceMethod;
                }
                else
                {
                    cellcmbchild.Visible = true;
                    celltxtchild.Visible = false;
                    cmbChildVals.Items.AddRange(childAtts.items.ToArray());
                }
            }
            else
            {
                cellcmbchild.Visible = true;
                celltxtchild.Visible = false;
                cUserDefinedField field = clsudf.GetUserDefinedById(int.Parse(cmbchild.SelectedValue));
                lblChildSearch.Text = field.label;

                foreach (KeyValuePair<int, cListAttributeElement> kp in field.items)
                {
                    cmbChildVals.Items.Add(new ListItem(kp.Value.elementText, kp.Key.ToString()));
                }
                cmbChildVals.SelectedIndex = 0;
            }
        }

        protected void lstFilterRuleVals_NodeClicked(object sender, Infragistics.WebUI.UltraWebNavigator.WebTreeNodeEventArgs e)
        {
            bool isChild = false;
            Infragistics.WebUI.UltraWebNavigator.Node delnode = null;

            foreach (Infragistics.WebUI.UltraWebNavigator.Node node in lstFilterRuleVals.Nodes)
            {
                foreach (Infragistics.WebUI.UltraWebNavigator.Node chnode in node.Nodes)
                {
                    if (chnode == e.Node)
                    {
                        isChild = true;
                        break;
                    }
                }

                if (isChild)
                {
                    delnode = node;
                    break;
                }
                else
                {
                    if (node.Text == e.Node.Text)
                    {
                        break;
                    }
                }
            }

            if (isChild)
            {
                delnode.Nodes.Remove(e.Node);
            }
            else
            {
                lstFilterRuleVals.Nodes.Remove(e.Node);
            }
        }

        private void populateChildDropdown()
        {
            cMisc clsmisc = new cMisc((int)ViewState["accountid"]);
            cGlobalProperties clsproperties = clsmisc.GetGlobalProperties((int)ViewState["accountid"]);
            cUserdefinedFields clsudf = new cUserdefinedFields((int)ViewState["accountid"]);
            this.SetFilterRules((int)ViewState["accountid"]);
            FilterType parent;
            int paruserdefineid = 0;

            if (cmbparent.SelectedValue == cmbparent.SelectedItem.Text)
            {
                parent = (FilterType)cmbparent.SelectedIndex + 1;
            }
            else
            {
                paruserdefineid = int.Parse(cmbparent.SelectedValue);
                parent = FilterType.Userdefined;
            }

            FilterArea area = this._filterRules.getArea(parent, true, paruserdefineid, 0);
            List<ListItem> lstfilterudfs = this._filterRules.getUserdefinedListItems();
            cmbchild.Items.Clear();

            switch (area)
            {
                case FilterArea.General:
                    {
                        cmbchild.Items.Add(new ListItem("Costcode"));
                        cmbchild.Items.Add(new ListItem("Department"));
                        cmbchild.Items.Add(new ListItem("Projectcode"));
                        cmbchild.Items.Add(new ListItem("Reason"));

                        foreach (ListItem item in lstfilterudfs)
                        {
                            cUserDefinedField udf = clsudf.GetUserDefinedById(int.Parse(item.Value));
                            if (paruserdefineid != udf.userdefineid)
                            {
                                cmbchild.Items.Add(item);
                            }
                        }
                        
                        break;
                    }
                case FilterArea.Breakdown:
                    {
                        cmbchild.Items.Add(new ListItem("Costcode"));
                        cmbchild.Items.Add(new ListItem("Department"));
                        cmbchild.Items.Add(new ListItem("Projectcode"));
                        break;
                    }
                case FilterArea.Individual:
                    {
                        cFieldToDisplay reason = clsmisc.GetGeneralFieldByCode("reason");
                        if (reason.individual)
                        {
                            cmbchild.Items.Add(new ListItem("Reason"));
                        }

                        
                        foreach (ListItem item in lstfilterudfs)
                        {
                            cUserDefinedField udf = clsudf.GetUserDefinedById(int.Parse(item.Value));
                            if (paruserdefineid != udf.userdefineid)
                            {
                                if (udf.itemspecific)
                                {
                                    cmbchild.Items.Add(item);
                                }
                            }
                        }
                        
                        break;
                    }
            }
            
        }

        private void populateUserdefined()
        {
            this.SetFilterRules((int)ViewState["accountid"]);
            List<ListItem> lstfilterudfs = this._filterRules.getUserdefinedListItems();

            foreach (ListItem item in lstfilterudfs)
            {
                //cmbparent.Items.Add(new ListItem(item.Text, item.Value));
                cmbchild.Items.Add(new ListItem(item.Text, item.Value));
            }
        }

        private void SetFilterRules(int accountId)
        {
            if (this._filterRules != null)
            {
                return;
            }

            if (this._costCodes == null)
            {
                this._costCodes = new cCostcodes(accountId);
            }

            this._filterRules = new cFilterRules(accountId, this._costCodes);
        }
    }
}
