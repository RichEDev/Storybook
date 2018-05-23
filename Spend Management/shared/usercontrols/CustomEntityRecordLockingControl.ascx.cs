namespace Spend_Management.shared.usercontrols
{
    using System;
    using System.Text;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Modules;
    using BusinessLogic.ProductModules;

    using SpendManagementLibrary;

    /// <summary>
    /// A user control to lock and unlock custom entity records.
    /// </summary>
    public partial class CustomEntityRecordLockingControl : System.Web.UI.UserControl
    {
        /// <summary>
        /// The ID of the custom entity to lock / monitor.
        /// </summary>
        public int CustomEntityId { private get; set; }

        /// <summary>
        /// The record ID of the custom entity to lock / monitor.
        /// </summary>
        public int ItemId { private get; set; }

        private readonly IDataFactory<IProductModule, Modules> _productModuleFactory =
            FunkyInjector.Container.GetInstance<IDataFactory<IProductModule, Modules>>();

        /// <summary>
        /// The displayable Title for this record.
        /// </summary>
        public string Title
        {
            get { return this.elementLockingTitle.InnerText; }
            set
            {
                this.lockDialogTitle.InnerText = value + " - Locked";
                var module = this._productModuleFactory[cMisc.GetCurrentUser().CurrentActiveModule];
                this.lockDialogTitle.InnerText = "Message from " + module.BrandNameHtml;
                this.elementLockingTitle.InnerText = value;
                this.dialogLockingTitle.InnerText = value;
            }
        }

        /// <summary>
        /// True if the record is locked by another user.
        /// </summary>
        public bool Locked
        {
            get { return !string.IsNullOrEmpty(this.hiddenLocked.Value) && Convert.ToBoolean(this.hiddenLocked.Value); }
            set { this.hiddenLocked.Value = value.ToString(); }
        }

        /// <summary>
        /// The Full employee name of the user that has the record locked.
        /// </summary>
        public string LockedBy
        {
            get { return this.elementLockingUser.InnerText; }
            set
            {
                this.elementLockingUser.InnerText = value;
                this.dialogLockingUser.InnerText = value;
            }
       }

        /// <summary>
        /// Is locking active on thie custom entity?
        /// </summary>
        public bool Active 
        {
            get { return !string.IsNullOrEmpty(this.hiddenActive.Value) && Convert.ToBoolean(this.hiddenActive.Value); }
            set { this.hiddenActive.Value = value.ToString(); }
        }

        /// <summary>
        /// The page load event for this locking control.
        /// Generates Javascript in the startup script.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Active)
            {
                this.GenerateJavascript();
            }
        }

        private void GenerateJavascript()
        {
            var js = new StringBuilder();
            js.AppendFormat("SEL.CustomEntityRecordLocking.Ids.ElementId = {0};", this.CustomEntityId);
            js.AppendFormat("SEL.CustomEntityRecordLocking.Ids.Id = {0};", this.ItemId);
            if (this.Locked)
            {
                js.Append("SEL.CustomEntityRecordLocking.ShowDialog();");
            }
            else
            {
                js.Append("SEL.CustomEntityRecordLocking.StartLockTimer();");
            }

            Parent.Page.ClientScript.RegisterStartupScript(GetType(), this.ID + "JS", js.ToString(), true);
        }
    }
}