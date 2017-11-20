using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SpendManagementHelpers;
using SpendManagementLibrary;
using SpendManagementLibrary.Mobile;

namespace Spend_Management
{
    /// <summary>
    /// mobileDevices user control
    /// </summary>
    public partial class mobileDevices : System.Web.UI.UserControl
    {

        #region User Control Properties

        public int DevicesEmployeeID { get; set; }

        #endregion

        /// <summary>
        /// Page Load event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            CurrentUser reqCurrentUser = cMisc.GetCurrentUser();
            cMobileDevices clsMobileDevices = new cMobileDevices(reqCurrentUser.Account.accountid);

            #region Modal Panel
            if (IsPostBack == false)
            {
                clsMobileDevices.CreateDropDown(ref ddlDeviceType, true);
            }
            #region Buttons

            CSSButton reqModalSaveButton = new CSSButton { Text = "save" };
            reqModalSaveButton.Style.Add(HtmlTextWriterStyle.Display, "hidden");
            reqModalSaveButton.Attributes.Add("onclick", "SEL.MobileDevices.SaveMobileDevice(); return false;");

            pnlAddEditDeviceButtons.Controls.Add(reqModalSaveButton);

            CSSButton reqModalCancelButton = new CSSButton { Text = "cancel" };
            reqModalCancelButton.Attributes.Add("onclick", "SEL.MobileDevices.CloseModal(); return false;");

            pnlAddEditDeviceButtons.Controls.Add(reqModalCancelButton);

            CSSButton reqModalCloseButton = new CSSButton { Text = "close" };
            reqModalCloseButton.Attributes.Add("onclick", "SEL.MobileDevices.CloseInfoModal(); return false;");
            pnlPairingInfoButtons.Controls.Add(reqModalCloseButton);
            #endregion Buttons
            #endregion Modal Panel

            GenerateMobileDeviceGrid(reqCurrentUser, reqModalSaveButton.ClientID);
        }

        /// <summary>
        /// Generates the grid showing the users current mobile device associations.
        /// </summary>
        /// <param name="reqCurrentUser">Current User class object</param>
        /// <param name="reqModalSaveButtonClientID">Save Button object client ID</param>
        private void GenerateMobileDeviceGrid(CurrentUser reqCurrentUser, string reqModalSaveButtonClientID)
        {
            cFields clsFields = new cFields(reqCurrentUser.AccountID);
            
            cGridNew gridMobileDevices = new cGridNew(reqCurrentUser.AccountID, reqCurrentUser.EmployeeID, "myMobileDevices", "SELECT mobileDeviceID, deviceName, deviceTypeID, pairingKey, dbo.mobileDeviceIsPaired(mobileDeviceID) FROM dbo.mobileDevices");

            // Filter on the employeeID
            gridMobileDevices.addFilter(clsFields.GetFieldByID(new Guid("4761f874-8923-4a3c-9a7c-2679fcde87ab")), ConditionType.Equals, new object[] { DevicesEmployeeID >= 0 ? DevicesEmployeeID : reqCurrentUser.EmployeeID }, null, ConditionJoiner.None);

            gridMobileDevices.getColumnByName("mobileDeviceID").hidden = true;

            gridMobileDevices.editlink = "javascript:SEL.MobileDevices.LoadMobileDeviceModal(SEL.MobileDevices.LoadType.Edit, {mobileDeviceID});";
            gridMobileDevices.enableupdating = true;
            gridMobileDevices.enabledeleting = true;
            gridMobileDevices.EmptyText = "There are currently no mobile devices associated to this account.";
            gridMobileDevices.deletelink = "javascript:SEL.MobileDevices.DeleteMobileDevice({mobileDeviceID}, '{dbo.mobileDeviceIsPaired(mobileDeviceID)}');";

            // populate the mobile device types
            var devices = new cMobileDevices(reqCurrentUser.AccountID);

            cFieldColumn deviceIdCol = (cFieldColumn)gridMobileDevices.getColumnByName("deviceTypeID");

            foreach (MobileDeviceType mdType in devices.MobileDeviceTypes.Values)
            {
                deviceIdCol.addValueListItem(mdType.DeviceTypeId, mdType.DeviceTypeDescription);
            }

            string[] gridData = gridMobileDevices.generateGrid();
            this.litMobileDevices.Text = gridData[1];

            var sbJs = new StringBuilder();
            sbJs.Append("SEL.MobileDevices.ModalWindowDomID = \"" + this.modalAddEditDevice.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowDeviceHeader = \"divMobileDeviceHeader\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowSpanDeviceInfo = \"spanMobileDeviceInfo\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowSpanDeviceInfoDiv = \"divMobileDeviceInfo\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowSpanDeviceInfoImg = \"" + this.imgIPhoneEdit.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowSpanPairingInfo = \"spanPairingKeyInfo\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowImgDeviceInfoDomID = \"" + this.imgIPhoneNew.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowDeviceName = \"" + this.txtDeviceName.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowDeviceType = \"" + this.ddlDeviceType.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.ModalWindowSaveButtonDomID = \"" + reqModalSaveButtonClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.ModalInfoWindowDomID = \"" + this.modalPairingInfo.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.MobileDeviceGridDomID = \"myMobileDevices\";\n");
            sbJs.Append("SEL.MobileDevices.ModalDeviceTypeValidatorID = \"" + this.rfDeviceType.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.ModalDeviceNameValidatorID = \"" + this.rfDeviceName.ClientID + "\";\n");
            sbJs.Append("SEL.MobileDevices.CurrentEmployeeID = " + (this.DevicesEmployeeID >= 0 ? this.DevicesEmployeeID.ToString() : reqCurrentUser.EmployeeID.ToString()) + ";\n");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "MobileDeviceVars", sbJs.ToString() + "\n" + cGridNew.generateJS_init("MobileDeviceGridVars", new List<string> { gridData[0] }, reqCurrentUser.CurrentActiveModule), true);
        }
    }
}