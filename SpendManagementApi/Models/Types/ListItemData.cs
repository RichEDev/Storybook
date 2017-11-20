namespace SpendManagementApi.Models.Types
{
    /// <summary>
    /// Represents the data that makes up a list item
    /// </summary>
    public class ListItemData
    {
        /// <summary>
        /// The value of the list item
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// The text of a list item
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The constructor for <see cref="ListItemData">ListItemData</see>
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="text">The text</param>
        public ListItemData(int value, string text)
        {
            this.Value = value;
            this.Text = text;
        }
    }
}