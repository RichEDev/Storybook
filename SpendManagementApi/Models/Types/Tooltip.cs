namespace SpendManagementApi.Models.Types
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data;

    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Helpers;

    /// <summary>
	/// A Tooltip provides help text to clarify elements of the system.<br/>
	/// </summary>
	public class Tooltip : BaseExternalType
	{
		/// <summary>
		/// The Id of this object.
		/// </summary>
		[Required]
		public Guid Id { get; set; }

		/// <summary>
		/// The name / label for this Tooltip object.
		/// </summary>
		[Required, MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
		public string HelpText { get; set; }


        /// <summary>
        /// Constructor for the <see cref="Tooltip"> object</see>
        /// </summary>
        /// <param name="id">The guid identifier for the tooltip</param>
        /// <param name="accountId">The current user of the system</param>
        public Tooltip(Guid id, int accountId)
        {
            this.Id = id;

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.ClearParameters();
                connection.AddWithValue("@tooltipId", id);

                
                using (var reader = connection.GetReader("dbo.GetTooltipByTooltipId", CommandType.StoredProcedure))
                {
                    int helpTextOrdinal = reader.GetOrdinal("helptext");

                    while (reader.Read())
                    {
                        this.HelpText = reader.GetString(helpTextOrdinal);
                    }
                }

                if (string.IsNullOrEmpty(this.HelpText))
                {
                    throw new ApiException(string.Format(ApiResources.ApiErrorANonExistentX, "tooltip"), string.Format(ApiResources.ApiErrorXUnsuccessfulMessage, "Get tooltip"));
                }
            }
        }
	}
}