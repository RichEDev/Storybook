namespace Spend_Management.shared.code
{
    using System;
    using System.Threading.Tasks;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    /// <summary>
    /// A class to encrypt existing data in a custom_XX table for a specific attribute.
    /// </summary>
    public class CustomEntityAttributeEncryptor
    {
        /// <summary>
        /// The current user.
        /// </summary>
        private readonly ICurrentUser _currentUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEntityAttributeEncryptor"/> class.
        /// </summary>
        /// <param name="currentUser">
        /// The current user.
        /// </param>
        public CustomEntityAttributeEncryptor(ICurrentUser currentUser)
        {
            this._currentUser = currentUser;
        }

        /// <summary>
        /// Encrypt a specific column.  This method runs a threaded <seealso cref="Task"/>
        /// </summary>
        /// <param name="attribute">An instance of <see cref="cAttribute"/>identifying a specific column</param>
        /// <param name="tableId">The <see cref="Guid"/>ID of the table that is is attached to.</param>
        public void Encrypt(cAttribute attribute, Guid tableId)
        {
            Task.Run(() => this.EncryptColumn(attribute, tableId));
        }

        /// <summary>
        /// Encrypt a specific column.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to encrypt.
        /// </param>
        /// <param name="tableId">
        /// The table id (currently not used).
        /// </param>
        private void EncryptColumn(cAttribute attribute, Guid tableId)
        {
            using (var expdata = new DatabaseConnection(cAccounts.getConnectionString(this._currentUser.AccountID)))
            {
                expdata.AddWithValue("@attributeId", attribute.attributeid);
                expdata.AddWithValue("@tableId", tableId);
                expdata.AddWithValue("@salt", "2FD583C9-BF7E-4B4E-B6E6-5FC9375AD069");
                expdata.ExecuteProc("dbo.EncryptCustomEntityAttributeData");
            }
        }
    }
}