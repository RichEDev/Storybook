namespace Spend_Management
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    
    /// <summary>
    /// Generates a HTML list of receipt images (and other receipt attachment types)
    /// Before adding the control to a page DataSource should be set
    /// </summary>
    public partial class Receipts : System.Web.UI.UserControl
    {
        /// <summary>
        /// Gets or sets data for the asp:Repeater
        /// </summary>
        public List<AttachedReceipt> DataSource { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not Ajax Mode is enabled or not
        /// Used with (JavaScript) SEL.Receipts.BindAjax()
        /// When true the control will generate a HTML template instead
        /// </summary>
        public bool AjaxMode { get; set; }

        /// <summary>
        /// Gets or sets whether the current user should be able to delete the receipts in the page.
        /// </summary>
        public bool CanDeleteReceipts { get; set; }

        /// <summary>
        /// If the user cannot delete receipts, then there should be a reason why.
        /// </summary>
        public string ReceiptDeletionReason { get; set; }

        /// <summary>
        /// Page Load
        /// </summary>
        protected void Page_Load()
        {
            if (this.AjaxMode == true)
            {
                this.receiptsList.Attributes["class"] = "receiptsListTemplate";
                this.receiptsBackgroundList.Visible = true;
                this.DataSource = new List<AttachedReceipt>();

                // create template receipt items
                var dummyImageReceipt = new AttachedReceipt { expenseids = new List<int> { }, claimid = 0, extension = string.Empty, filename = string.Empty, validImageForBrowser = true };
                this.DataSource.Add(dummyImageReceipt);

                var dummyOtherReceipt = new AttachedReceipt { expenseids = new List<int> { }, claimid = 0, extension = string.Empty, filename = string.Empty };
                this.DataSource.Add(dummyOtherReceipt);
            }

            // bind the receipts list to the asp:repeater
            this.receiptsRepeater.DataSource = this.DataSource;
            this.receiptsRepeater.DataBind();
        }

        /// <summary>
        /// Called for each data item in the receipts data source, this method sets up the correct template for the repeater items.
        /// </summary>
        /// <param name="sender">The asp:Repeater</param>
        /// <param name="e">The event</param>
        protected void ReceiptsViewItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            var receipt = (AttachedReceipt)e.Item.DataItem;
            var multiView = (MultiView)e.Item.FindControl("receiptMultiView");

            // if the receipt can't be viewed in the browser render the item with the alternative ItemTemplate
            if (receipt.validImageForBrowser == false)
            {
                multiView.ActiveViewIndex = 1;
            }
        }
    }
}