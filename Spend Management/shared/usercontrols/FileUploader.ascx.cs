using System;
using System.Collections.Generic;
using System.Web;

using SpendManagementLibrary.GreenLight;
using SpendManagementLibrary.Helpers;

namespace Spend_Management.shared.usercontrols
{
    /// <summary>
    /// The FileUploader control is currently used to display greenlight attachment attributes within client forms.
    /// </summary>
    public partial class FileUploader : System.Web.UI.UserControl
    {
        private Guid fileGuid;

        /// <summary>
        /// Gets whether an attachment has been selected
        /// </summary>
        public bool HasFile { get { return this.UploadType != string.Empty; } }

        /// <summary>
        /// Gets or sets whether the browser is ie9 or less
        /// </summary>
        public bool Ie9OrLess { get; set; }

        /// <summary>
        /// Gets whether the attachment has changed
        /// </summary>
        /// <summary>
        /// Sets whether the attachment has changed
        /// </summary>
        public bool Changed
        {
            get
            {
                return this.txtChanged.Text != string.Empty;
            }
            set
            {
                this.txtChanged.Text = string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the attribute id
        /// </summary>
        public string AttributeId { get; set; }

        /// <summary>
        /// Gets the posted file property of the file upload control
        /// </summary>
        public HttpPostedFile PostedFile
        {
            get { return this.fileUpload.PostedFile; }
        }

        /// <summary>
        /// Gets or sets whether this attribute is mandatory
        /// </summary>
        public bool Mandatory { get; set; }

        /// <summary>
        /// Gets or sets the validation group to be used
        /// </summary>
        public string ValidationControlGroup { get; set; }

        /// <summary>
        /// Gets or sets the name of the attachment
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the attachment e.g. add.png
        /// </summary>
        public string AttachmentName { get; set; }

        /// <summary>
        /// Gets or sets the view ID of the current record
        /// </summary>
        public int ViewId { get; set; }

        /// <summary>
        /// Gets or sets the entity ID of the current record
        /// </summary>
        public int EntityId { get; set; }

        /// <summary>
        /// Gets or sets the record ID of the current record
        /// </summary>
        public int RecordId { get; set; }

        /// <summary>
        /// Gets or sets the html input control ID
        /// </summary>
        public string ControlId { get; set; }

        /// <summary>
        /// Gets or sets the guid for an existing attribute
        /// </summary>
        public Guid FileGuid
        {
            set
            {
                this.fileGuid = value;
                this.txtGuid.Text = value.ToString();
                this.UploadType = this.fileGuid == Guid.Empty ? "" : "FileBrowser";
                this.SetFileName();
            }
            get
            {
                Guid.TryParse(this.txtGuid.Text, out this.fileGuid);
                return this.fileGuid;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            string deleteIconId = "img" + AttributeId;

            if (IncludeImageLibrary)
            {
                string browserInit =
                    string.Format(
                        "javascript:ShowImageBrowserPopup('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                        this.txtImageLibraryFilePath.ClientID,
                        this.txtImageLibraryFileName.ClientID,
                        this.fileUpload.ClientID,
                        this.txtUploadType.ClientID,
                        this.attachmentLink.ClientID,
                        this.replacementText.ClientID,
                        this.txtChanged.ClientID,
                        this.reqFileUploader.ClientID,
                        this.AttachmentName,
                        this.ImageLibraryModalPopupExtenderId,
                        deleteIconId,
                        this.Mandatory);
                this.UploadButton.Attributes.Add("onclick", browserInit);
                this.ImageLibraryButton.Attributes.Add("onclick", browserInit);
            }
            else
            {
                string fileUploadClick = string.Format(
                    "document.getElementById('{0}').click();",
                    this.fileUpload.ClientID);
                this.UploadButton.Attributes.Add("onclick", fileUploadClick);
                this.ImageLibraryButton.Visible = false;
            }

            var inc = IncludeImageLibrary ? "true" : "false";
            string fileUploadChange =
                string.Format(
                    "javascript:SetFileUploadReplacementFileName(this.value, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}');",
                    this.attachmentLink.ClientID,
                    this.replacementText.ClientID,
                    this.txtUploadType.ClientID,
                    this.fileUpload.ClientID,
                    this.txtImageLibraryFilePath.ClientID,
                    this.txtImageLibraryFileName.ClientID,
                    inc,
                    this.reqFileUploader.ClientID,
                    this.txtChanged.ClientID,
                    deleteIconId,
                    this.Mandatory);
            this.fileUpload.Attributes.Add("onchange", fileUploadChange);
        }

        protected override void OnInit(EventArgs e)
        {
            reqFileUploader.ValidationGroup = ValidationControlGroup;
            reqFileUploader.ErrorMessage = reqFileUploader.ErrorMessage.Replace("{0}", Name);
            if (Mandatory)
            {
                reqFileUploader.Enabled = true;
            }

            if (this.Request.Browser.Browser == "IE" && this.Request.UserAgent != null && (this.Request.UserAgent.Contains("MSIE 10") || this.Request.UserAgent.Contains("MSIE 9") || this.Request.UserAgent.Contains("MSIE 8") || this.Request.UserAgent.Contains("MSIE 7")))
            {
                this.Ie9OrLess = true;
            }
            else
            {
                this.Ie9OrLess = false;
            }
        }

        private void SetFileName()
        {
            reqFileUploader.ValidationGroup = "NotMandatory";
            if (this.fileGuid != Guid.Empty)
            {
                HtmlImageData imageData = CustomEntityImageData.GetData(cMisc.GetCurrentUser().AccountID, this.fileGuid.ToString());
                if (imageData != null)
                {
                    string selected = imageData.FileName;
                    string dotonated = selected;
                    if (selected.Length > 6)
                    {
                        dotonated = cMisc.DotonateString(selected, 6);
                    }
                    this.attachmentLink.Text = dotonated;
                    this.attachmentLink.ToolTip = selected;
                    this.attachmentLink.Attributes.Add("referenceValue", imageData.FileId);
                    this.AttachmentName = selected;
                    this.attachmentLink.NavigateUrl = string.Format("javascript:viewFieldLevelAttachment('{0}', {1}, {2}, {3}, '{4}');", imageData.FileId, this.EntityId, this.ViewId, this.RecordId, this.ControlId);
                }
            }
            else
            {
                this.attachmentLink.Text = "";
                this.attachmentLink.ToolTip = "";
                this.AttachmentName = "";
                this.attachmentLink.NavigateUrl = "";
            }
        }

        /// <summary>
        /// Gets or sets whether the control should include the image library functionality
        /// </summary>
        public bool IncludeImageLibrary { get; set; }

        /// <summary>
        /// The modal popup extender id to be used for the image library browser
        /// </summary>
        public string ImageLibraryModalPopupExtenderId { get; set; }

        /// <summary>
        /// The full file path (including image name) of the selected item from the image library
        /// </summary>
        public string ImageLibrarySelectedFilePath
        {
            get
            {
                return this.txtImageLibraryFilePath.Text;
            }
        }

        /// <summary>
        /// The file name (including extension) of the selected item from the image library
        /// </summary>
        public string ImageLibrarySelectedFileName
        {
            get
            {
                return this.txtImageLibraryFileName.Text;
            }
        }

        /// <summary>
        /// Gets or sets the following values depending on what type of attachment has been selected
        /// FileBrowser
        /// ImageLibrary
        /// Empty String if nothing selected.
        /// </summary>
        public string UploadType
        {
            get
            {
                return this.txtUploadType.Text;
            }
            set
            {
                this.txtUploadType.Text = value;
            }
        }
    }
}