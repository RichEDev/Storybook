namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Utilities;
    using SpendManagementLibrary;

    /// <summary>
    /// A basic definition of the claims object.
    /// </summary>
    public class ClaimDefinitionResponse : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.ClaimDefinition, ClaimDefinitionResponse>
    {
        /// <summary>
        /// The id number of the claim.
        /// </summary>
        [Required]
        [Range(0, int.MaxValue)]
        public int Id { get; set; }

        /// <summary>
        /// The name of the claim.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Name { get; set; }

        /// <summary>
        /// A description of the claim.
        /// </summary>
        [MaxLength(2000, ErrorMessage = ApiResources.ErrorMaxLength + @"2000")]
        public string Description { get; set; }

        /// <summary>
        /// A list of <see cref="UserDefinedFieldValue">user defined fields</see>./>
        /// </summary>
        public List<UserDefinedFieldType> UserDefinedFields { get; set; }

        /// <summary>
        /// The result of a <see cref="ClaimDefinitionResponse">ClaimDefinition</see> action
        /// </summary>
        public ClaimDefinitionOutcome ClaimDefinitionOutcome { get;  set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public ClaimDefinitionResponse From(SpendManagementLibrary.ClaimDefinition dbType, IActionContext actionContext)
        {
            this.Id = dbType.ClaimId;
            this.Name = dbType.Name;
            this.Description = dbType.Description;
            this.UserDefinedFields = new List<UserDefinedFieldType>();
            this.ClaimDefinitionOutcome = (ClaimDefinitionOutcome)dbType.ClaimDefinitionOutcome;

            if (dbType.UserDefinedFields != null)
            {

                //Convert the SML userDefinedField to a the UserDefinedFieldType API type
                foreach (SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue userDefinedField in dbType.UserDefinedFields)
                {
                    var userDefinedFiledValue = (cUserDefinedField)userDefinedField.Value;
                    var userDefinedFieldType = new UserDefinedFieldType().From(userDefinedFiledValue, actionContext);
                    this.UserDefinedFields.Add(userDefinedFieldType);
                }
            }

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.ClaimDefinition To(IActionContext actionContext)
        {
            var claimDefinition = new SpendManagementLibrary.ClaimDefinition
                                  {
                                      ClaimId = this.Id,
                                      Name = this.Name,
                                      Description = this.Description,
                                      UserDefinedFields =
                                          new List<SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue>()
                                  };

            foreach (var userDefinedField in this.UserDefinedFields.Where(udf => udf.UserDefinedFieldId > 0))
            {
                claimDefinition.UserDefinedFields.Add(new SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue(userDefinedField.UserDefinedFieldId, userDefinedField.Value));
            }

            return claimDefinition;
        }
    }
}

