namespace BusinessLogic.Announcements
{
    using BusinessLogic.Identity;

    /// <summary>
    /// A factory to generate <see cref="IReadReceipt"/> objects.
    /// </summary>
    public class ReadReceiptFactory
    {
        /// <summary>
        /// Create a new instance of <see cref="IReadReceipt"/>.
        /// </summary>
        /// <param name="identityProvider">
        /// An instance of <see cref="IIdentityProvider"/>.
        /// </param>
        /// <returns>
        /// The <see cref="ReadReceipt"/> a valid instance of <see cref="ReadReceipt"/>.
        /// </returns>
        public ReadReceipt Create(IIdentityProvider identityProvider)
        {
            var userIdentity = identityProvider.GetUserIdentity();
            return new ReadReceipt{ AccountId = userIdentity.AccountId, EmployeeId = userIdentity.EmployeeId };
        }
    }
}
