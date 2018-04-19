namespace BusinessLogic.GeneralOptions.Addresses
{
    /// <summary>
    /// The address options.
    /// </summary>
    public class AddressOptions : IAddressOptions
    {
        private string _retainLabelsTime;

        /// <summary>
        /// Gets or sets the retain labels time.
        /// </summary>
        public string RetainLabelsTime
        {
            get => string.IsNullOrEmpty(this._retainLabelsTime) ? "3" : this._retainLabelsTime;
            set => this._retainLabelsTime = value;
        }
    }
}
