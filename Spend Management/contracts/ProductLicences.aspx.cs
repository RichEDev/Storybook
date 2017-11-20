using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Spend_Management;
using SpendManagementLibrary;
using System.Web.Services;

namespace Spend_Management
{
    public partial class ProductLicences : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            curUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.ProductLicences, false, true);

            cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
            cTables tables = new cTables(curUser.AccountID);
            cTable ptable = tables.GetTableByName("productLicences");
            StringBuilder sb = new StringBuilder();

            if (!this.IsPostBack)
            {
                Title = "Product Licences";

                Master.PageSubTitle = Title;

                lnkAdd.Visible = curUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.ProductLicences, false);

                panelEdit.Style.Add(HtmlTextWriterStyle.Display, "none");

                currentProductId.Value = Request.QueryString["pid"];

                cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
                txtProductName.Text = products.GetProductById(int.Parse(currentProductId.Value)).ProductName;
            }

            ufields.createFieldPanel(ref phPLUFields, ptable.GetUserdefinedTable(), "plicences", out sb);
            ScriptManager.RegisterStartupScript(this, Page.GetType(), "udfScript", sb.ToString(), true);
            cmdUpdate.Attributes.Add("onclick", "javascript:if(validateform('plicences') == false) { return false; }");
            DisplayLicences(int.Parse(currentProductId.Value));
        }

        private void DisplayLicences(int productid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
            cProductLicences licences = new cProductLicences(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, productid, cAccounts.getConnectionString(curUser.AccountID));
            Table licTab = new Table();
            licTab.CssClass = "datatbl";

            TableHeaderRow licTHRow = new TableHeaderRow();
            TableHeaderCell licTHCell;

            string editimgpath = string.Empty;

            if (curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ProductLicences, true))
            {
                licTHCell = new TableHeaderCell();
                Image editimg = new Image();
                editimgpath = "~/icons/edit.gif";
                editimg.ImageUrl = editimgpath;
                licTHCell.Controls.Add(editimg);
                licTHCell.Width = Unit.Pixel(25);
                licTHRow.Cells.Add(licTHCell);
              
            }
            string delimgpath = string.Empty;

            if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ProductLicences, true))
            {
                licTHCell = new TableHeaderCell();
                Image delimg = new Image();
                delimgpath = "~/icons/delete2.gif";
                delimg.ImageUrl = delimgpath;
                licTHCell.Controls.Add(delimg);
                licTHCell.Width = Unit.Pixel(25);
                licTHRow.Cells.Add(licTHCell);
             
            }

            licTHCell = new TableHeaderCell();
            licTHCell.Text = "Licence Key";
            licTHRow.Cells.Add(licTHCell);

            licTHCell = new TableHeaderCell();
            licTHCell.Text = "Licence Expires";
            licTHRow.Cells.Add(licTHCell);

            licTHCell = new TableHeaderCell();
            licTHCell.Text = "Licence Location";
            licTHRow.Cells.Add(licTHCell);

            licTHCell = new TableHeaderCell();
            licTHCell.Text = "Number Copies Held";
            licTHRow.Cells.Add(licTHCell);

            licTab.Rows.Add(licTHRow);

            string rowClass = "row1";
            bool rowalt = false;

            TableRow licRow;
            TableCell licCell;

            if (licences.Count > 0)
            {
                foreach (KeyValuePair<int, cProductLicence> i in licences.GetCollection)
                {
                    cProductLicence curLic = (cProductLicence)i.Value;
                    rowalt = (rowalt ^ true);
                    if (rowalt)
                    {
                        rowClass = "row1";
                    }
                    else
                    {
                        rowClass = "row2";
                    }

                    licRow = new TableRow();

                    if (curUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.ProductLicences, true))
                    {
                        licCell = new TableCell();
                        licCell.CssClass = rowClass;
                        ImageButton edit = new ImageButton();
                        edit.ImageUrl = editimgpath;
                        edit.ID = "edit" + curLic.LicenceId.ToString();
                        edit.Click += new ImageClickEventHandler(edit_Click);
                        edit.CommandName = "edit";
                        edit.CommandArgument = curLic.LicenceId.ToString();
                        edit.ToolTip = "Edit the licence details";
                        edit.Attributes.Add("onmouseover", "window.status='Edit the licence details';return true;");
                        edit.Attributes.Add("onmouseout", "window.status='Done';");
                        licCell.Controls.Add(edit);                       
                        licRow.Cells.Add(licCell);
                    }

                    if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ProductLicences, true))
                    {
                        licCell = new TableCell();
                        licCell.CssClass = rowClass;
                        Image delete = new Image();
                        delete.ID = "delete" + curLic.LicenceId.ToString();
                        delete.ImageUrl = delimgpath;
                        delete.ToolTip = "Delete the licence details";
                        delete.Attributes.Add("onclick", "DeleteLicence(" + currentProductId.Value + ", " + curLic.LicenceId + "); return false;");
                        delete.Attributes.Add("onmouseover", "window.status='Delete the licence details';return true;");
                        delete.Attributes.Add("onmouseout", "window.status='Done';");
                        delete.CssClass = "btn";                   
                        //delete.CommandArgument = curLic.LicenceId.ToString();
                        //delete.CausesValidation = false;
                        licCell.Controls.Add(delete);
                        licRow.Cells.Add(licCell);
                    }
                    licCell = new TableCell();
                    licCell.CssClass = rowClass;
                    licCell.Text = curLic.LicenceKey;
                    licRow.Cells.Add(licCell);

                    licCell = new TableCell();
                    licCell.CssClass = rowClass;
                    if (curLic.LicenceExpiry == DateTime.MinValue)
                    {
                        licCell.Text = "";
                    }
                    else
                    {
                        licCell.Text = curLic.LicenceExpiry.ToShortDateString();
                    }
                    licRow.Cells.Add(licCell);

                    licCell = new TableCell();
                    licCell.CssClass = rowClass;
                    licCell.Text = curLic.LicenceLocation;
                    licRow.Cells.Add(licCell);

                    licCell = new TableCell();
                    licCell.CssClass = rowClass;
                    licCell.Text = curLic.NumberCopiesHeld.ToString();
                    licRow.Cells.Add(licCell);

                    licTab.Rows.Add(licRow);
                }
            }
            else
            {
                licRow = new TableRow();
                licCell = new TableCell();
                licCell.CssClass = "row1";
                licCell.ColumnSpan = 6;
                licCell.HorizontalAlign = HorizontalAlign.Center;
                licCell.Text = "No licences currently defined";
                licRow.Cells.Add(licCell);
                licTab.Rows.Add(licRow);
            }

            panelLicenceList.Controls.Add(licTab);
        }

        void edit_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
            ImageButton button = (ImageButton)sender;
            if (button.CommandName == "edit")
            {
                panelEdit.Style.Add(HtmlTextWriterStyle.Display, "");
                panelLicenceList.Visible = false;
                cmdClose.Visible = false;

                Master.enablenavigation = false;
                //Master.useCloseNavigationMsg = false;
                //Master.RefreshBreadcrumbInfo();

                hiddenLicenceId.Value = button.CommandArgument;

                if (hiddenLicenceId.Value != "" && hiddenLicenceId.Value != "0")
                {
                    cProductLicences licences = new cProductLicences(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, int.Parse(currentProductId.Value), cAccounts.getConnectionString(curUser.AccountID));
                    cProductLicence curlicence = licences.GetLicenceById(int.Parse(hiddenLicenceId.Value));

                    if (curlicence != null)
                    {
                        txtLocation.Text = curlicence.LicenceLocation;
                        txtKey.Text = curlicence.LicenceKey;

                        lstProdLicenceType.Items.Clear();
                        cBaseDefinitions clsBaseDefinitions = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductLicenceTypes);

                        int curLTid = curlicence.LicenceType;

                        lstProdLicenceType.Items.AddRange(clsBaseDefinitions.CreateDropDown(true, curLTid));

                        //if (curLTid > 0 && lstProdLicenceType.Items.FindByValue(curLTid.ToString()) != null)
                        //{
                        //    lstProdLicenceType.Items.FindByValue(curLTid.ToString()).Selected = true;
                        //}

                        lstRenewalType.Items.Clear();
                        clsBaseDefinitions = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.LicenceRenewalTypes);

                        if (curlicence.LicenceRenewalType != null)
                        {
                            int curLRid = curlicence.LicenceRenewalType.ID;

                            lstRenewalType.Items.AddRange(clsBaseDefinitions.CreateDropDown(true, curLRid));

                            if (curLRid > 0 && lstRenewalType.Items.FindByValue(curLRid.ToString()) != null)
                            {
                                lstRenewalType.Items.FindByValue(curLRid.ToString()).Selected = true;
                            }
                        }
                        else
                        {
                            lstRenewalType.Items.AddRange(clsBaseDefinitions.CreateDropDown(true, 0));
                        }

                        chkHardCopy.Checked = curlicence.IsHardCopyHeld;
                        chkSoftCopy.Checked = curlicence.IsElectronicCopyHeld;
                        chkUnlimited.Checked = curlicence.IsUnlimitedLicence;

                        if (curlicence.LicenceExpiry != DateTime.MinValue)
                        {
                            dateExpiry.Text = curlicence.LicenceExpiry.ToShortDateString();
                        }
                        else
                        {
                            dateExpiry.Text = "";
                        }
                        txtNotifyDays.Text = curlicence.NotifyDays.ToString();

                        cTeams teams = new cTeams(curUser.AccountID, curUser.CurrentSubAccountId);
                        lstNotify.Items.Clear();
                        lstNotify.Items.AddRange(teams.GetCombinedEmployeeListItems(true));
                        if (curlicence.NotifyType == AudienceType.Individual)
                        {
                            lstNotify.Items.FindByValue(curlicence.NotifyId.ToString()).Selected = true;
                        }
                        else
                        {
                            lstNotify.Items.FindByValue("TEAM_" + curlicence.NotifyId.ToString()).Selected = true;
                        }
                        txtNumberHeld.Text = curlicence.NumberCopiesHeld.ToString();

                        cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
                        cTables tables = new cTables(curUser.AccountID);
                        cFields fields = new cFields(curUser.AccountID);
                        cTable pltable = tables.GetTableByName("productLicences");
                        SortedList<int, object> udfs;
                        if (curlicence.LicenceId > 0)
                        {
                            udfs = ufields.GetRecord(pltable.GetUserdefinedTable(), curlicence.LicenceId, tables, fields);
                            ufields.populateRecordDetails(ref phPLUFields, pltable.GetUserdefinedTable(), udfs);
                        }
                        else
                        {
                            udfs = new SortedList<int, object>();
                        }
                        ViewState["record"] = udfs;
                    }
                }
            }
        }

        protected void lnkAdd_Click(object sender, EventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            panelEdit.Style.Add(HtmlTextWriterStyle.Display, "");
            panelLicenceList.Visible = false;
            cmdClose.Visible = false;

            Master.enablenavigation = false;
            //Master.useCloseNavigationMsg = false;
            //Master.RefreshBreadcrumbInfo();

            hiddenLicenceId.Value = "0";

            txtLocation.Text = "";
            txtKey.Text = "";
            lstProdLicenceType.Items.Clear();

            cBaseDefinitions clsBaseDefinitions = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.ProductLicenceTypes);
            lstProdLicenceType.Items.AddRange(clsBaseDefinitions.CreateDropDown(true, 0));

            lstRenewalType.Items.Clear();

            clsBaseDefinitions = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.LicenceRenewalTypes);
            lstRenewalType.Items.AddRange(clsBaseDefinitions.CreateDropDown(true, 0));

            chkHardCopy.Checked = false;
            chkSoftCopy.Checked = false;
            chkUnlimited.Checked = false;
            dateExpiry.Text = "";
            txtNotifyDays.Text = "0";
            txtNumberHeld.Text = "0";
            cTeams teams = new cTeams(curUser.AccountID, curUser.CurrentSubAccountId);

            lstNotify.Items.AddRange(teams.GetCombinedEmployeeListItems(true));

            cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
            cTables tables = new cTables(curUser.AccountID);
            cFields fields = new cFields(curUser.AccountID);
            cTable pltable = tables.GetTableByName("productLicences");
            SortedList<int, object> udfs = ufields.GetRecord(pltable.GetUserdefinedTable(), 0, tables, fields);
            ufields.populateRecordDetails(ref phPLUFields, pltable.GetUserdefinedTable(), udfs);
            ViewState["record"] = udfs;
        }

        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            string url = "~/ProductDetails.aspx?action=edit&id=" + currentProductId.Value + "&item=" + txtProductName.Text;
            Response.Redirect(url, true);
        }

        protected void cmdCancel_Click(object sender, ImageClickEventArgs e)
        {
            panelEdit.Style.Add(HtmlTextWriterStyle.Display, "none");
            panelLicenceList.Visible = true;

            cmdClose.Visible = true;
            Master.enablenavigation = true;
            //Master.RefreshBreadcrumbInfo();
        }

        protected void cmdUpdate_Click(object sender, ImageClickEventArgs e)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();

            cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
            cBaseDefinitions clsBaseDefinitions = new cBaseDefinitions(curUser.AccountID, curUser.CurrentSubAccountId, SpendManagementElement.LicenceRenewalTypes);
            cLicenceRenewalType rentype = (cLicenceRenewalType)clsBaseDefinitions.GetDefinitionByID((int.Parse(lstRenewalType.SelectedItem.Value)));
            cProductLicences licences = new cProductLicences(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, int.Parse(currentProductId.Value), cAccounts.getConnectionString(curUser.AccountID));

            string notifystr = lstNotify.SelectedItem.Value;
            int notifyid = 0;
            DateTime expiry = DateTime.MinValue;
            if (dateExpiry.Text.Trim() != null && dateExpiry.Text.Trim() != "")
            {
                expiry = DateTime.Parse(dateExpiry.Text.Trim());
            }

            AudienceType notify_type;
            if (notifystr.Substring(0, 1) == "T")
            {
                notifyid = int.Parse(notifystr.Replace("TEAM_", ""));
                notify_type = AudienceType.Team;
            }
            else
            {
                notifyid = int.Parse(notifystr);
                notify_type = AudienceType.Individual;
            }
            int numcopies = int.Parse(txtNumberHeld.Text);

            if (hiddenLicenceId.Value == "0")
            {
                // adding a new licence
                cProductLicence newlicence = new cProductLicence(0, int.Parse(currentProductId.Value), txtKey.Text, int.Parse(lstProdLicenceType.SelectedValue), expiry, rentype, notifyid, notify_type, int.Parse(txtNotifyDays.Text), txtLocation.Text, chkHardCopy.Checked, chkSoftCopy.Checked, chkUnlimited.Checked, numcopies, "", "", "", null, DateTime.Now, curUser.EmployeeID, null, null);
                hiddenLicenceId.Value = licences.UpdateLicence(newlicence).ToString();
            }
            else
            {
                // must be amending an existing licence
                cProductLicence curlicence = licences.GetLicenceById(int.Parse(hiddenLicenceId.Value));
                cProductLicence newlicence = new cProductLicence(int.Parse(hiddenLicenceId.Value), int.Parse(currentProductId.Value), txtKey.Text, int.Parse(lstProdLicenceType.SelectedValue), expiry, rentype, notifyid, notify_type, int.Parse(txtNotifyDays.Text), txtLocation.Text, chkHardCopy.Checked, chkSoftCopy.Checked, chkUnlimited.Checked, numcopies, "", "", "", null, curlicence.CreatedOn, curlicence.CreatedBy, DateTime.Now, curUser.EmployeeID);
                hiddenLicenceId.Value = licences.UpdateLicence(newlicence).ToString();
            }

            cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
            cTables tables = new cTables(curUser.AccountID);
            cFields fields = new cFields(curUser.AccountID);
            cTable plTable = tables.GetTableByName("productLicences");
            SortedList<int, object> newrec = ufields.getItemsFromPanel(ref phPLUFields, plTable.GetUserdefinedTable());

            var record = txtProductName.Text + " - " + int.Parse(hiddenLicenceId.Value);
            ufields.SaveValues(plTable.GetUserdefinedTable(), int.Parse(hiddenLicenceId.Value), newrec, tables, fields, curUser,elementId:(int)SpendManagementElement.ProductLicences, record: record);

            Response.Redirect("ProductLicences.aspx?pid=" + currentProductId.Value, true);
        }

        /// <summary>
        /// Called via Ajax to delete the licence
        /// </summary>
        /// <param name="productid"></param>
        /// <param name="licenceid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public static int DeleteLicence(int productid, int licenceid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.ProductLicences, true))
            {
                cProducts products = new cProducts(curUser.AccountID, curUser.CurrentSubAccountId);
                cProductLicences licences = new cProductLicences(curUser.AccountID, curUser.CurrentSubAccountId, curUser.EmployeeID, productid, cAccounts.getConnectionString(curUser.AccountID));
                licences.DeleteLicence(licenceid, curUser.EmployeeID);

                return 0;
            }
            else
            {
                return -1;
            }
        }
    }
}
