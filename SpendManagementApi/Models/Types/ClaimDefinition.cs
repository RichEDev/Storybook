    namespace SpendManagementApi.Models.Types
    {
        using System.Collections.Generic;
        using System.ComponentModel.DataAnnotations;
        using System.Linq;
        using SpendManagementApi.Common.Enums;
        using SpendManagementApi.Interfaces;
        using SpendManagementApi.Utilities;

        /// <summary>
        /// A basic definition of the claims object.
        /// </summary>
        public class ClaimDefinition : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.ClaimDefinition, ClaimDefinition>
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
            /// A list of <see cref="UserDefinedFieldValue">user defined fields</see>
            /// </summary>
            public List<UserDefinedFieldValue> UserDefinedFields { get; set; }

            /// <summary>
            /// The result of a <see cref="ClaimDefinition">ClaimDefinition</see> action
            /// </summary>
            public ClaimDefinitionOutcome ClaimDefinitionOutcome { get; set; }

            /// <summary>
            /// Convert from a data access layer Type to an api Type.
            /// </summary>
            /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
            /// <param name="actionContext">The actionContext which contains DAL classes.</param>
            /// <returns>An api Type</returns>
            public ClaimDefinition From(SpendManagementLibrary.ClaimDefinition dbType, IActionContext actionContext)
            {
                this.Id = dbType.ClaimId;
                this.Name = dbType.Name;
                this.Description = dbType.Description;
                this.UserDefinedFields = new List<UserDefinedFieldValue>();
                this.ClaimDefinitionOutcome = (ClaimDefinitionOutcome)dbType.ClaimDefinitionOutcome;

                if (dbType.UserDefinedFields != null)
                {

                    foreach (var userDefinedField in dbType.UserDefinedFields)
                    {
                        this.UserDefinedFields.Add(new UserDefinedFieldValue(userDefinedField.Id, userDefinedField.Value));
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
                    UserDefinedFields = new List<SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue>()
                };

                foreach (var userDefinedField in this.UserDefinedFields.Where(udf => udf.Id > 0))
                {
                    claimDefinition.UserDefinedFields.Add(new SpendManagementLibrary.UserDefinedFields.UserDefinedFieldValue(userDefinedField.Id, userDefinedField.Value));
                }

                return claimDefinition;
            }
        }
    }
