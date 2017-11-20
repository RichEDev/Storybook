using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.Script.Services;
using System.IO;
using SpendManagementLibrary;

namespace Spend_Management
{
	/// <summary>
	/// 
	/// </summary>
    public partial class FileUploadIFrame : System.Web.UI.Page
    {
        public string attachTitleID;
        public string attachDescID;

        private const string SCRIPT_TEMPLATE = "window.parent.UploadComplete('{0}', {1}, {2}, '{3}', '{4}', '{5}', '{6}');";

        /// <summary>
		/// Page_Load method
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["recordid"] != null)
            {
                ViewState["recordid"] = Convert.ToInt32(Request.QueryString["recordid"]);
            }

            if (GenIDVal.Value != "")
            {
                ViewState["recordid"] = Convert.ToInt32(GenIDVal.Value);
            }

            if (DocType.Value != "")
            {
                ViewState["attachdoctype"] = DocType.Value;
            }

            if (Request.QueryString["tablename"] != null)
            {
                ViewState["tablename"] = Request.QueryString["tablename"].ToString();
            }
            if (Request.QueryString["idfield"] != null)
            {
                ViewState["idfield"] = Request.QueryString["idfield"].ToString();
            }
            
            if (Request.QueryString["mdlcancel"] != null)
			{
				ViewState["modalcancelid"] = Request.QueryString["mdlcancel"];
				ClientScriptManager csm = this.ClientScript;
				Type cstype = this.GetType();
				csm.RegisterStartupScript(cstype, "modalID", "modalCancelID = '" + ViewState["modalcancelid"] + "';", true);
			}

            bool MultipleAttachments = true;

            litStyles.Text = cColours.customiseStyles(false);

            if (Request.QueryString["multipleAttachments"] != null)
            {
                MultipleAttachments = Convert.ToBoolean(int.Parse(Request.QueryString["multipleAttachments"]));
                ViewState["MultipleAttachments"] = MultipleAttachments;
            }

            
                if (MultipleAttachments)
                {
                    Label spanInput;
                    Label spanValidator;
                    Label spanIcon;
                    Label spanToolTip;

                    Panel pnlOneColumnSmall = new Panel();
                    pnlOneColumnSmall.CssClass = "onecolumnsmall";

                    Panel pnlOneColumn = new Panel();
                    pnlOneColumn.CssClass = "onecolumn";

                    #region Attachment Title

                    Label lblAttachTitle = new Label();
                    lblAttachTitle.Text = "Attachment title*";
                    lblAttachTitle.CssClass = "mandatory";

                    TextBox txtAttachTitle = new TextBox();
                    txtAttachTitle.CssClass = "fillspan";
                    txtAttachTitle.MaxLength = 50;
                    txtAttachTitle.ID = "txtTitle";

                    lblAttachTitle.AssociatedControlID = txtAttachTitle.ID;
                    attachTitleID = txtAttachTitle.ClientID;

                    spanInput = new Label();
                    spanInput.CssClass = "inputs";
                    spanInput.Controls.Add(txtAttachTitle);
                    spanValidator = new Label();
                    spanIcon = new Label();
                    spanToolTip = new Label();

                    spanValidator.CssClass = "inputvalidatorfield";
                    spanValidator.Text = "&nbsp;";

                var reqField = new RequiredFieldValidator();
                    reqField.ID = "reqTitle";
                reqField.ErrorMessage = "Please enter an Attachment title.";
                    reqField.ControlToValidate = "txtTitle";
                    reqField.Text = "*";
                reqField.ValidationGroup = "validUpload";

                    spanValidator.Controls.Add(reqField);

                    spanIcon.CssClass = "inputicon";
                    spanIcon.Text = "&nbsp;";

                    spanToolTip.CssClass = "inputtooltipfield";
                    spanToolTip.Text = "&nbsp;";

                    pnlOneColumnSmall.Controls.Add(lblAttachTitle);
                    pnlOneColumnSmall.Controls.Add(spanInput);
                    pnlOneColumnSmall.Controls.Add(spanIcon);
                    pnlOneColumnSmall.Controls.Add(spanToolTip);
                    pnlOneColumnSmall.Controls.Add(spanValidator);

                    pnlAttach.Controls.Add(pnlOneColumnSmall);

                    #endregion

                    #region Attachment Description

                    Label lblAttachDesc = new Label();
                    lblAttachDesc.Text = "Description";

                    TextBox txtAttachDesc = new TextBox();
                    txtAttachDesc.CssClass = "fillspan";
                    txtAttachDesc.TextMode = TextBoxMode.MultiLine;
                    txtAttachDesc.ID = "txtDesc";

                    lblAttachDesc.AssociatedControlID = txtAttachDesc.ID;
                    attachDescID = txtAttachDesc.ClientID;

                    spanInput = new Label();
                    spanInput.CssClass = "inputs";
                    spanInput.Controls.Add(txtAttachDesc);
                    spanValidator = new Label();
                    spanIcon = new Label();
                    spanToolTip = new Label();

                    spanValidator.CssClass = "inputvalidatorfield";
                    spanValidator.Text = "&nbsp;";

                    spanIcon.CssClass = "inputicon";
                    spanIcon.Text = "&nbsp;";

                    spanToolTip.CssClass = "inputtooltipfield";
                    spanToolTip.Text = "&nbsp;";

                    pnlOneColumn.Controls.Add(lblAttachDesc);
                    pnlOneColumn.Controls.Add(spanInput);
                    pnlOneColumn.Controls.Add(spanIcon);
                    pnlOneColumn.Controls.Add(spanToolTip);
                    pnlOneColumn.Controls.Add(spanValidator);

                    pnlAttach.Controls.Add(pnlOneColumn);

                    #endregion
                }
                else
                {
                    this.cmdCancelAttach.Src = "~/shared/images/buttons/btn_close.png";
                }
           
			
			//if (IsPostBack)
			//{
			//    UploadFile();
			//}
        }

	    /// <summary>
	    /// Perform the file upload
	    /// </summary>
	    protected void UploadFile()
	    {
	        string script = "";

	        if (fileUploadBox.HasFile)
	        {
	            CurrentUser user = cMisc.GetCurrentUser();
	            int? delegateId = null;
	            if (user.isDelegate)
	            {
	                delegateId = user.Delegate.EmployeeID;
	            }
	            string title = "";
	            string description = "";

	            if (ViewState["MultipleAttachments"] != null)
	            {
	                if ((bool)ViewState["MultipleAttachments"])
	                {
	                    TextBox txtTitle = (TextBox)pnlAttach.FindControl("txtTitle");
	                    title = txtTitle.Text;
	                    TextBox txtDesc = (TextBox)pnlAttach.FindControl("txtDesc");
	                    description = txtDesc.Text;
	                    description = description.Replace("\'", "\\\'");
	                }
	            }

	            using (Stream fileStream = fileUploadBox.PostedFile.InputStream)
	            {
	                if (fileStream != null)
	                {
	                    string uploadFile = fileUploadBox.PostedFile.FileName;              
	                    string[] path = uploadFile.Split('\\');
	                    string directory = "";
	                    string filename = path[path.Length - 1];
	                    string extension = filename.Substring(filename.LastIndexOf('.') + 1,
	                        filename.Length - filename.LastIndexOf('.') - 1);
	                    for (int x = 0; x < path.Length - 1; x++)
	                    {
	                        directory += path[x] + "\\";
	                    }

	                    cAttachments attachments = new cAttachments(user.AccountID, user.EmployeeID,
	                        user.CurrentSubAccountId,
	                        delegateId);

	                    cMimeType mimeType = attachments.checkMimeType(extension);

	                    if (mimeType != null)
	                    {
	                        byte[] bytes = attachments.getFileData(fileStream);	                        
	                        cTables tables = new cTables(user.AccountID);
	                        cTable attTable = tables.GetTableByName((string) ViewState["tablename"]);
	                        cAttachment attachment = new cAttachment(0, attTable.TableID, (int) ViewState["recordid"],
	                            title,
	                            description, filename, mimeType, DateTime.Now, user.EmployeeID, null, null, bytes);
	                        string tableid = "";
	                        if (attTable != null)
	                        {
	                            tableid = attTable.TableID.ToString();
	                        }
	                        int attid = attachments.saveAttachment((string) ViewState["tablename"],
	                            (string) ViewState["idfield"], (int) ViewState["recordid"], attachment, bytes);
	                        script = string.Format(SCRIPT_TEMPLATE, "File uploaded.", "false", attid, tableid, title,
	                            filename,
	                            description);

	                        AttachDocumentType attachDocType;

	                        if (ViewState["attachdoctype"] != null)
	                        {
	                            attachDocType = (AttachDocumentType) byte.Parse(ViewState["attachdoctype"].ToString());

	                            attachments.saveElementDocumentAttachment(attachDocType, (int) ViewState["recordid"],
	                                attid);
	                        }
	                    }
	                    else
	                    {
	                        script = "alert('Selected file type (." + extension + ") is not currently supported');" +
	                                 string.Format(SCRIPT_TEMPLATE,
	                                     "Selected file type (." + extension + ") is not currently supported", "true", 0,
	                                     "",
	                                     "", "", "");
	                    }
	                }

	                else
	                {
	                    script = "alert('Please specify a valid file');" +
	                             string.Format(SCRIPT_TEMPLATE, "Please specify a valid file.", "true", 0, "", "", "", "");
	                }
	                ClientScript.RegisterStartupScript(this.GetType(), "uploadNotify", script, true);
	            }
	        }
	    }



	    /// <summary>
        /// Upload button click event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected void btnUpload_Click(object sender, ImageClickEventArgs e)
		{
            this.UploadFile();
		}
        
    }
}
