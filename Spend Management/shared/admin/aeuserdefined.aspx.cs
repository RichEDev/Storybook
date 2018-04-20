namespace Spend_Management
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Web.Script.Serialization;
    using System.Web.UI;
    using System.Web.UI.WebControls;

    using SpendManagementLibrary;

    /// <summary>
    /// Summary description for aeuserdefined.
    /// </summary>
    public partial class aeuserdefined : Page
    {
        protected ImageButton cmdhelp;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Title = "Add / Edit User Defined Field";
            Master.title = Title;
            Master.PageSubTitle = "User Defined Field Details";
            Master.UseDynamicCSS = true;

            if (IsPostBack == false)
            {
                cmdok.Attributes.Add("onclick", "populateListItems();if (validateform('vgAttribute') == false) {return;}");
                cmbattributetype.Attributes.Add("onchange", "ClearRelatedTable(); showFurtherAttributeOptions(); FilterRelatedTables();");
                ddlstgroup.Attributes.Add("onchange", "ddlstgroup_OnChange();");
                Master.enablenavigation = false;

                CurrentUser user = cMisc.GetCurrentUser();
                user.CheckAccessRole(AccessRoleType.View, SpendManagementElement.UserDefinedFields, true, true);
                ViewState["accountid"] = user.AccountID;
                ViewState["employeeid"] = user.EmployeeID;

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                        Master.helpid = 1148;
                        break;
                    default:
                        Master.helpid = 1007;
                        break;
                }

                //cEmployees clsemployees = new cEmployees(user.accountid);
                //cEmployee reqemp;
                //reqemp = clsemployees.GetEmployeeById(user.employeeid);
                //ViewState["accountid"] = reqemp.accountid;

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                        phAllowSearch.Visible = true;
                        break;
                    default:
                        phAllowSearch.Visible = false;
                        break;
                }

                //cMisc clsmisc = new cMisc(reqemp.accountid);

                int userdefineid = 0;
                if (Request.QueryString["userdefineid"] != null)
                {
                    userdefineid = Convert.ToInt32(Request.QueryString["userdefineid"]);
                }
                ViewState["userdefineid"] = userdefineid;

                cTables clstables = new cTables(user.AccountID);
                cmbappliesto.Items.AddRange(clstables.CreateUserDefinedDropDown(user.CurrentActiveModule).ToArray());
                cmbappliesto.Attributes.Add("onchange", "FilterFieldTypes(); showFurtherAttributeOptions(); setItemSpecificState(); getAvailableGroupings(); FilterRelatedTables(); SetEmployeePopulationState();");
                ddlRelatedTable.Items.AddRange(clstables.CreateEntityRelationshipDropDown(filterModule: user.CurrentActiveModule).ToArray());

                cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings(user.AccountID);

                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                        // hide the item specific checkbox if we're in framework
                        pnlItemSpecific.Style.Remove(HtmlTextWriterStyle.Display);
                        pnlItemSpecific.Style.Add(HtmlTextWriterStyle.Display, "none");
                        break;
                    default:
                        break;
                }

                if (userdefineid > 0) //edit
                {
                    user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.UserDefinedFields, true, true);
                    cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]);
                    cUserDefinedField reqfield = clsuserdefined.GetUserDefinedById(userdefineid);
                    txtattributename.Text = reqfield.label;
                    txtattributedescription.Text = reqfield.description;
                    txtorder.Text = reqfield.order.ToString();
                    if (cmbattributetype.Items.FindByValue(((byte)reqfield.fieldtype).ToString()) != null)
                    {
                        cmbattributetype.SelectedValue = ((byte)reqfield.fieldtype).ToString();
                    }

                    cTable masterTbl = clstables.GetTableByUserdefineTableID(reqfield.table.TableID);

                    if (masterTbl != null)
                    {
                        if (cmbappliesto.Items.FindByValue(masterTbl.TableID.ToString()) != null)
                        {
                            cmbappliesto.SelectedValue = masterTbl.TableID.ToString();
                        }
                    }
                    chkattributemandatory.Checked = reqfield.mandatory;

                    switch (user.CurrentActiveModule)
                    {
                        case Modules.contracts:
                        case Modules.SpendManagement:
                        case Modules.SmartDiligence:
                            chkallowsearch.Checked = reqfield.AllowSearch;
                            break;
                        default:
                            break;
                    }

                    cmbattributetype.Enabled = false;
                    cmbappliesto.Enabled = false;
                    cmbdateformat.Enabled = false;
                    txtprecision.Enabled = false;
                    txtattributetooltip.Text = reqfield.tooltip;
                    ddlstgroup.Items.AddRange(groupings.CreateDropDown(masterTbl.TableID).ToArray());

                    if (reqfield.Grouping != null)
                    {
                        if (ddlstgroup.Items.FindByValue(reqfield.Grouping.UserdefinedGroupID.ToString()) != null)
                        {
                            ddlstgroup.SelectedValue = reqfield.Grouping.UserdefinedGroupID.ToString();
                        }
                        txtgroupid.Value = reqfield.Grouping.UserdefinedGroupID.ToString();
                    }
                    if (masterTbl.TableID.ToString() == "d70d9e5f-37e2-4025-9492-3bcf6aa746a8")
                    {
                        // expense items field, so enable the Item Specific checkbox
                        chkspecific.Enabled = true;
                    }
                    else
                    {
                        chkspecific.Enabled = false;
                    }
                    chkspecific.Checked = reqfield.Specific;
                    chkallowclaimantpopulation.Checked = reqfield.AllowEmployeeToPopulate;

                    cTextAttribute txtatt;
                    switch (reqfield.fieldtype)
                    {
                        case FieldType.LargeText:
                            txtatt = (cTextAttribute)reqfield.attribute;
                            if (txtatt.maxlength != null)
                            {
                                txtmaxlengthlarge.Text = txtatt.maxlength.ToString();
                            }
                            if (cmbtextformatlarge.Items.FindByValue(((byte)txtatt.format).ToString()) != null)
                            {
                                cmbtextformatlarge.SelectedValue = ((byte)txtatt.format).ToString();
                            }
                            break;
                        case FieldType.Text:
                            txtatt = (cTextAttribute)reqfield.attribute;
                            if (txtatt.maxlength != null)
                            {
                                txtmaxlength.Text = txtatt.maxlength.ToString();
                            }
                            if (cmbtextformat.Items.FindByValue(((byte)txtatt.format).ToString()) != null)
                            {
                                cmbtextformat.SelectedValue = ((byte)txtatt.format).ToString();
                            }
                            break;
                        case FieldType.TickBox:
                            if (((cTickboxAttribute)reqfield.attribute).defaultvalue != "")
                            {
                                if (cmbdefaultvalue.Items.FindByText(((cTickboxAttribute)reqfield.attribute).defaultvalue) != null)
                                {
                                    cmbdefaultvalue.SelectedValue = cmbdefaultvalue.Items.FindByText(((cTickboxAttribute)reqfield.attribute).defaultvalue).Value;
                                }
                            }
                            break;
                        case FieldType.List:
                            cListAttribute lstatt = (cListAttribute)reqfield.attribute;
                            List<cListAttributeElement> lstEleLi = new List<cListAttributeElement>();

                            foreach (KeyValuePair<int, cListAttributeElement> item in lstatt.items)
                            {
                                cListAttributeElement element = (cListAttributeElement)item.Value;
                                lstEleLi.Add(element);
                            }

                            // sort the list by element order
                            lstEleLi.Sort(delegate(cListAttributeElement liAttEle1, cListAttributeElement liAttEle2) { return liAttEle1.elementOrder.CompareTo(liAttEle2.elementOrder); });

                            var jss = new JavaScriptSerializer();

                            foreach (cListAttributeElement lae in lstEleLi)
                            {
                                var listItemText = lae.elementText;
                                if (lae.Archived)
                                {
                                    listItemText = listItemText + " (Archived)";
                                }

                                this.lstitems.Items.Add(new ListItem(listItemText, jss.Serialize(lae)));
                            }

                            break;
                        case FieldType.Number:
                            cNumberAttribute numberatt = (cNumberAttribute)reqfield.attribute;
                            txtprecision.Text = numberatt.precision.ToString();
                            break;
                        case FieldType.Hyperlink:
                            cHyperlinkAttribute hyperlinkatt = (cHyperlinkAttribute)reqfield.attribute;
                            txtHyperlinkText.Text = hyperlinkatt.hyperlinkText;
                            txtHyperlinkPath.Text = hyperlinkatt.hyperlinkPath;
                            break;
                        case FieldType.DateTime:
                            cDateTimeAttribute datelinkatt = (cDateTimeAttribute)reqfield.attribute;
                            if (cmbdateformat.Items.FindByValue(((byte)datelinkatt.format).ToString()) != null)
                            {
                                cmbdateformat.SelectedValue = ((byte)datelinkatt.format).ToString();
                            }
                            break;
                        case FieldType.Relationship:
                            cManyToOneRelationship relatedTableAtt = (cManyToOneRelationship)reqfield.attribute;
                            if (relatedTableAtt.relatedtable != null)
                            {
                                if (ddlRelatedTable.Items.FindByValue(relatedTableAtt.relatedtable.TableID.ToString()) != null)
                                {
                                    ddlRelatedTable.SelectedValue = relatedTableAtt.relatedtable.TableID.ToString();
                                    ddlRelatedTable.Enabled = false;

                                    svcAutoComplete svc = new svcAutoComplete();
                                    cmbDisplayField.Items.AddRange(svc.getRelationshipLookupOptions(relatedTableAtt.relatedtable.TableID.ToString(), new List<string>().ToArray()).ToArray());
                                }

                                if (cmbDisplayField.Items.FindByValue(relatedTableAtt.AutoCompleteDisplayField.ToString()) != null)
                                {
                                    cmbDisplayField.SelectedValue = relatedTableAtt.AutoCompleteDisplayField.ToString();
                                }

                                if (relatedTableAtt.AutoCompleteMatchFieldIDList.Count > 0)
                                {
                                    cFields clsFields = new cFields(user.AccountID);

                                    foreach (Guid g in relatedTableAtt.AutoCompleteMatchFieldIDList)
                                    {
                                        cField curField = clsFields.GetFieldByID(g);
                                        if (curField != null)
                                        {
                                            lstmatchfields.Items.Add(new ListItem(curField.Description, curField.FieldID.ToString()));
                                        }
                                    }
                                }

                                if (relatedTableAtt.AutoCompleteMatchRows > 0)
                                {
                                    txtmaxrows.Text = relatedTableAtt.AutoCompleteMatchRows.ToString();
                                }
                                else
                                {
                                    txtmaxrows.Text = "15";
                                }
                            }

                            break;
                    }

                    this.chkEncrypt.Checked = reqfield.Encrypted;
                    if (reqfield.Encrypted)
                    {
                        this.chkEncrypt.Enabled = false;
                    }

                    Master.title = "Userdefined Field: " + reqfield.label;

                    StringBuilder sbJS = new StringBuilder();
                    sbJS.Append("function pcEditUDF()\n{\n");
                    sbJS.Append("\tshowFurtherAttributeOptions();\n");
                    sbJS.Append("}\n");
                    sbJS.Append("if (window.addEventListener) // W3C standard\n");
                    sbJS.Append("{\n");
                    sbJS.Append("\twindow.addEventListener('load', pcEditUDF, false);\n");
                    sbJS.Append("}\n");
                    sbJS.Append("else if (window.attachEvent) // Microsoft\n");
                    sbJS.Append("{\n");
                    sbJS.Append("\twindow.attachEvent('onload', pcEditUDF);\n");
                    sbJS.Append("}\n");

                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "jsUDFEdit", sbJS.ToString(), true);
                }
                else
                {
                    user.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.UserDefinedFields, true, true);

                    if (cmbappliesto.Items.Count > 0)
                    {
                        ddlstgroup.Items.AddRange(groupings.CreateDropDown(new Guid(cmbappliesto.SelectedValue)).ToArray());
                    }
                    else
                    {
                        cmbappliesto.Items.Add(new ListItem("[None]", Guid.Empty.ToString()));
                        ddlstgroup.Items.AddRange(groupings.CreateDropDown(Guid.Empty).ToArray());
                    }
                    chkspecific.Enabled = false;
                    Master.title = "Userdefined Field: New";
                }

                var userdefinedEncrypted = this.chkEncrypt.Checked ? "true" : "false";
                this.Page.ClientScript.RegisterClientScriptBlock(
                    this.GetType(),
                    "vars",
                    $"curUserdefinedID = {userdefineid}; curUserdefinedEncrypted = {userdefinedEncrypted};",
                    true);

                // currently only Framework uses relationshiptextbox udfs so if it's something else, hide that from the type dropdowns
                // currently only Framework uses DynamicHyperlink udfs so if it's something else, hide that from the type dropdowns
                switch (user.CurrentActiveModule)
                {
                    case Modules.contracts:
                    case Modules.SpendManagement:
                    case Modules.SmartDiligence:
                        cmbattributetype.Items.FindByValue("16").Enabled = true;
                        break;
                    default:
                        break;
                }
                // comment to force merge of source as FindByValue("17") should not be below this line!!

                revHyperlinkPath.ValidationExpression = @"^(https?|ftps?)://(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?.*|^mailto\:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?";
            }
            else
            {
                string items = Request.Form["txtlistitems"];
                string[] arritems = items.Split(new string[1] {"^#}^"}, StringSplitOptions.RemoveEmptyEntries);
                string[] arrVal;

                for (int i = 0; i < arritems.GetLength(0); i++)
                {
                    arrVal = arritems[i].Split(new string[1] { "^{#^" }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrVal[0] != "" && (arrVal.Length > 1 && !string.IsNullOrWhiteSpace(arrVal[1])))
                    {
                        lstitems.Items.Add(new ListItem(arrVal[0], arrVal[1]));
                    }
                }
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "initialActions", "SetEmployeePopulationState();", true);
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
            this.cmdcancel.Click += new System.Web.UI.ImageClickEventHandler(this.cmdcancel_Click);

        }
        #endregion

        private void cmdcancel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            Response.Redirect("adminuserdefined.aspx", true);
        }

        //protected void cmdok_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //{
        //    CurrentUser curUser = cMisc.GetCurrentUser();

        //    cTables clstables = new cTables((int)ViewState["accountid"]);
        //    cUserdefinedFields clsuserdefined = new cUserdefinedFields((int)ViewState["accountid"]); ;
        //    cUserdefinedFieldGrouping group = null;
        //    cUserdefinedFieldGroupings groupings = new cUserdefinedFieldGroupings((int)ViewState["accountid"]);
        //    cFilterRules clsfilterrules = new cFilterRules((int)ViewState["accountid"]);
        //    string description;
        //    FieldType fieldtype;
        //    bool mandatory = chkattributemandatory.Checked;
        //    int order;
        //    DateTime createdon;
        //    int createdby;
        //    DateTime? modifiedon = null; ;
        //    int? modifiedby = null;
        //    Guid fieldid;
        //    int? maxlength = null;
        //    cAttribute attribute = null;
        //    int userdefineid = (int)ViewState["userdefineid"];

        //    Guid tableid;
        //    AttributeFormat format;
        //    bool archived = false;
        //    bool specific = chkspecific.Checked;
        //    string tooltip = txtattributetooltip.Text;
        //    string attributename = txtattributename.Text;
        //    string displayname = txtattributename.Text;
        //    bool allowsearch = false;

        //    string hyperlinkText = txtHyperlinkText.Text;
        //    string hyperlinkPath = txtHyperlinkPath.Text;

        //    Guid relatedTableID;
        //    cTable relatedTable = null;
        //    if (Guid.TryParse(ddlRelatedTable.SelectedValue, out relatedTableID))
        //    {
        //        relatedTable = clstables.GetTableByID(relatedTableID);
        //    }
            
        //    description = txtattributedescription.Text;
        //    fieldtype = (FieldType)byte.Parse(cmbattributetype.SelectedValue);
        //    tableid = new Guid(cmbappliesto.SelectedValue);
        //    switch (curUser.CurrentActiveModule)
        //    {
        //        case Modules.contracts:
        //        case Modules.SpendManagement:
        //        case Modules.SmartDiligence:
        //            allowsearch = chkallowsearch.Checked;
        //            break;
        //        default:
        //            break;
        //    }

        //    if (txtgroupid.Value != "")
        //    {
        //        group = groupings.GetGroupingByID(Convert.ToInt32(txtgroupid.Value));
        //    }

        //    if (userdefineid > 0)
        //    {
        //        cUserDefinedField oldfield = clsuserdefined.GetUserDefinedById(userdefineid);
        //        createdon = oldfield.createdon;
        //        createdby = oldfield.createdby;
        //        modifiedby = (int)ViewState["employeeid"];
        //        modifiedon = DateTime.Now;
        //        attributename = oldfield.attribute.attributename;
        //        fieldid = oldfield.attribute.fieldid;
        //        archived = oldfield.Archived;
        //    }
        //    else
        //    {
        //        createdon = DateTime.Now;
        //        createdby = (int)ViewState["employeeid"];
        //        fieldid = Guid.Empty;
        //    }

        //    if (txtorder.Text != "")
        //    {
        //        order = int.Parse(txtorder.Text);
        //    }
        //    else
        //    {
        //        order = clsuserdefined.GetNextOrder();
        //    }

        //    switch (fieldtype)
        //    {
        //        case FieldType.Currency:
        //            attribute = new cNumberAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, 2, fieldid, false, false, false, false, false);
        //            break;
        //        case FieldType.DateTime:
        //            attribute = new cDateTimeAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, (AttributeFormat)Convert.ToByte(cmbdateformat.SelectedValue), fieldid, false, false, false, false);
        //            break;
        //        case FieldType.Integer:
        //            attribute = new cNumberAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, 0, fieldid, false, false, false, false, false);
        //            break;
        //        case FieldType.LargeText:

        //            if (txtmaxlengthlarge.Text != "")
        //            {
        //                maxlength = Convert.ToInt32(txtmaxlengthlarge.Text);
        //            }
        //            format = (AttributeFormat)Convert.ToByte(cmbtextformatlarge.SelectedValue);
        //            attribute = new cTextAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, maxlength, format, fieldid, false, false, false, false);
        //            break;
        //        case FieldType.Number:
        //            byte precision = 0;
        //            if (txtprecision.Text.Trim() != "")
        //            {
        //                precision = Convert.ToByte(txtprecision.Text);
        //            }
        //            attribute = new cNumberAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, precision, fieldid, false, false, false, false, false);
        //            break;
        //        case FieldType.Text:

        //            if (txtmaxlength.Text != "")
        //            {
        //                maxlength = Convert.ToInt32(txtmaxlength.Text);
        //            }
        //            format = (AttributeFormat)Convert.ToByte(cmbtextformat.SelectedValue);
        //            attribute = new cTextAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, maxlength, format, fieldid, false, false, false, false);

        //            break;
        //        case FieldType.TickBox:
        //            attribute = new cTickboxAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, fieldtype, createdon, createdby, modifiedon, modifiedby, cmbdefaultvalue.SelectedItem.Text, fieldid, false, false, false, false);
        //            break;
        //        case FieldType.List:
        //            format = (AttributeFormat)Convert.ToByte(cmbtextformat.SelectedValue);
        //            SortedList<int, cListAttributeElement> list = new SortedList<int, cListAttributeElement>();
        //            string items = Request.Form["txtlistitems"];
        //            ListItemCollection liarr = lstitems.Items;
        //            string[] arritems = items.Split(new string[1] { "^#}^" }, StringSplitOptions.RemoveEmptyEntries); //items.Split(',');
        //            string[] arrVal;
        //            int valID = 0;

        //            for (int i = 0; i < arritems.GetLength(0); i++)
        //            {
        //                arrVal = arritems[i].Split(new string[1] { "^{#^" }, StringSplitOptions.RemoveEmptyEntries); //'-');

        //                if (arrVal.Length != 2)
        //                {
        //                    arrVal = new string[2];
        //                }

        //                if (int.TryParse(arrVal[1], out valID))
        //                {
        //                    if (valID > 0)
        //                    {
        //                        list.Add(i, new cListAttributeElement(valID, arrVal[0], i));
        //                    }
        //                    else
        //                    {
        //                        list.Add(i, new cListAttributeElement(0, arrVal[0], i));
        //                    }
        //                }
        //                else
        //                {
        //                    list.Add(i, new cListAttributeElement(0, arrVal[0], i));
        //                }
        //            }
        //            attribute = new cListAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, FieldType.List, createdon, createdby, modifiedon, modifiedby, list, fieldid, false, false, format, false, false);
        //            break;
        //        case FieldType.Hyperlink:
        //            attribute = new cHyperlinkAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, FieldType.Hyperlink, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, hyperlinkText, hyperlinkPath, false, false);
        //            break;
        //        case FieldType.DynamicHyperlink:
        //            attribute = new cHyperlinkAttribute(userdefineid, attributename, displayname, description, tooltip, mandatory, FieldType.DynamicHyperlink, createdon, createdby, modifiedon, modifiedby, fieldid, false, false, string.Empty, string.Empty, false, false);
        //            break;
        //        case FieldType.Relationship:
        //            Guid acDisplayField = Guid.Empty;
        //            Guid.TryParse(cmbDisplayField.SelectedValue, out acDisplayField);

        //            List<Guid> acMatchFields = null;
        //            if (lstmatchfields.Items.Count > 0)
        //            {
        //                acMatchFields = new List<Guid>();
        //                foreach (ListItem x in lstmatchfields.Items)
        //                {
        //                    acMatchFields.Add(new Guid(x.Value));
        //                }
        //            }

        //            attribute = new cManyToOneRelationship(userdefineid, attributename, displayname, description, tooltip, mandatory, createdon, createdby, modifiedon, modifiedby, relatedTable, fieldid, false, false, false, null, acDisplayField, acMatchFields);
        //            break;
        //    }

        //    cTable tbl = clstables.GetTableByID(tableid);
        //    cTable udftbl = clstables.GetTableByID(tbl.UserDefinedTableID);
        //    cUserDefinedField userdefined = new cUserDefinedField(userdefineid, udftbl, order, null, createdon, createdby, modifiedon, modifiedby, attribute, group, archived, specific, allowsearch);

        //    if (clsuserdefined.AlreadyExists(userdefined) == false)
        //    {
        //        clsuserdefined.SaveUserDefinedField(userdefined);
        //        Response.Redirect("adminuserdefined.aspx", true);
        //    }
        //    else
        //    {
        //        ClientScript.RegisterStartupScript(this.GetType(), "alreadyExists", "alert('Userdefined field already exists');", true);
        //        ClientScript.RegisterStartupScript(this.GetType(), "moreFields", "showFurtherAttributeOptions();", true);

        //        //string items = Request.Form["txtlistitems"];
        //        //string[] arritems = items.Split(',');

        //        //StringBuilder javascript = new StringBuilder();

        //        //javascript.Append("listItems = new Array();\n");

        //        //for (int i = 0; i < arritems.GetLength(0); i++)
        //        //{
        //        //    javascript.Append("listItems.push(" + arritems[i] + ");\n");
        //        //}

        //        //ClientScript.RegisterStartupScript(this.GetType(), "listItems", javascript.ToString(), true);

        //        //if (fieldtype == FieldType.List)
        //        //{
        //        //    ViewState["lstItemAtt"] = attribute;
        //        //}
        //    }

        //    //clsfilterrules.ruleCheck();

        //}
    }
}
