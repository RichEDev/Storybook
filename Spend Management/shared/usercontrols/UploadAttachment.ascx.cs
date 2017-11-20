using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.UI.WebControls;
using System.IO;
using System.Threading;

namespace Spend_Management
{
    public partial class UploadAttachment : System.Web.UI.UserControl
    {
        private string sTableName;
        private string sIdField;
        private int nRecordID;
		private string sModalCancelID;
        private string sIFrameName;
        private bool bMultipleAttachment;

        private int iHeight;

        #region properties

        public int accountid
        {
            get
            {
                if (ViewState["accountid"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["accountid"];
                }
            }
            set { ViewState["accountid"] = value; }
        }

        public int employeeid
        {
            get
            {
                if (ViewState["employeeid"] == null)
                {
                    return 0;
                }
                else
                {
                    return (int)ViewState["employeeid"];
                }
            }
            set { ViewState["employeeid"] = value; }
        }

        public string TableName
        {
            get
            {
                return sTableName; ;

            }

            set { sTableName = value; }
        }
        public string IDField
        {
            get { return sIdField; }
            set { sIdField = value; }
        }
        public int RecordID
        {
            get { return nRecordID; }
            set { nRecordID = value; }
        }

		public string modalCancelID
		{
			set { sModalCancelID = value; }
			get { return sModalCancelID; }
		}

        public string iFrameName
        {
            get { return sIFrameName; }
            set { sIFrameName = value; }
        }

        public bool MultipleAttachments
        {
            get { return bMultipleAttachment; }
            set { bMultipleAttachment = value; }
        }

        public int Height
        {
            get
            {
                return this.iHeight;
            }
            set
            {
                this.iHeight = value;
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            //int height = 500;
            //if (bMultipleAttachment == false)
            //{
            //    height = 200;
            //}

            string urlParams = "tablename=" + HttpUtility.UrlEncode(TableName) + "&idfield=" + IDField + "&recordid=" + RecordID + "&mdlcancel=" + sModalCancelID + "&multipleAttachments=" + Convert.ToInt32(MultipleAttachments);
            string filePath = ResolveUrl("~/shared/usercontrols/FileUploadIFrame.aspx?" + urlParams);
                        
            litAttachPage.Text = "<iframe id=\"" + iFrameName + "\" onload=\"initFileUpload('" + iFrameName + "'," + iHeight + ")\" scrolling=\"no\" frameborder=\"0\" hidefocus=\"true\" style=\"width: 100%; height: 100%;\" src=\"" + filePath + "\"></iframe>";
        }

        
    }
}