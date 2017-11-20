namespace SpendManagementApi.Models.Types.Expedite
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using Attributes.Validation;
    using Interfaces;
    using Utilities;

    /// <summary>
    /// Represents a single unit of validation for an ExpenseItem (claim line / savedexpense). 
    /// For an ExpenseItem, every applicable criterion must have a matching result, in order for
    /// validation to be complete. There must be a 1:1 relationship between Criteria and Results.
    /// </summary>
    public class ExpenseValidationResult : BaseExternalType, IApiFrontForDbObject<SpendManagementLibrary.Expedite.ExpenseValidationResult, ExpenseValidationResult>
    {
        /// <summary>
        /// The unique Id of this item in the database.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Id of the Expense Item (claim line) that this result pertains to. 
        /// Note that changing this whilst editing a result will move this Result to another ExpenseItem.
        /// </summary>
        public int ExpenseItemId { get; set; }

        /// <summary>
        /// The Id of the template (ExpenseSubCategory) of this Expense Item.
        /// that this result pertains to.
        /// Note that this is for reference only, it has no bearing other than for information.
        /// </summary>
        public int ExpenseSubCategoryId { get; set; }

        /// <summary>
        /// The rule that this is the result for. Note that changing this whilst editing a result
        /// will move this Result to another Criterion.
        /// </summary>
        public int CriterionId { get; set; }
        
        /// <summary>
        /// The result of matching the requirement to the Receipt.
        /// </summary>
        public int ReasonId { get; set; }

        /// <summary>
        /// The Business Reasons status of this ValidationResult.
        /// </summary>
        [ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public ExpenseValidationStatus BusinessStatus { get; set; }
        
        /// <summary>
        /// The VAT status of this ValidationResult.
        /// </summary>
        [ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public ExpenseValidationStatus VATStatus { get; set; }
        
        /// <summary>
        /// Whether the receipt / expense being validated is possibly fraudulent.
        /// </summary>
        public bool PossiblyFraudulent { get; set; }

        /// <summary>
        /// The DateTime at which this validation was performed.
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Any comments that the Expedite operator wishes to offer as 
        /// extra reasons for why the validation status is the way it is.
        /// </summary>
        [MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Comments { get; set; }
        
        /// <summary>
        /// Any other data, in XML format.
        /// </summary>
        public string Data { get; set; }

        /// <summary>
        /// The result of the expedite operator matching criteria to a receipt.
        /// Used to determine the actual status of this result.
        /// </summary>
        public ExpenseValidationMatchingResult MatchingResult { get; set; }

        /// <summary>
        /// Convert from a data access layer Type to an api Type.
        /// </summary>
        /// <param name="dbType">The instance of the data access layer Type to convert from.</param>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>An api Type</returns>
        public ExpenseValidationResult From(SpendManagementLibrary.Expedite.ExpenseValidationResult dbType, IActionContext actionContext)
        {
            if (dbType == null)
            {
                return null;
            }

            Id = dbType.Id;
            ExpenseItemId = dbType.ExpenseItemId;
            CriterionId = dbType.Criterion.Id;
            BusinessStatus = (ExpenseValidationStatus)dbType.BusinessStatus;
            VATStatus = (ExpenseValidationStatus)dbType.VATStatus;
            PossiblyFraudulent = dbType.PossiblyFraudulent;
            Timestamp = dbType.Timestamp;
            Comments = dbType.Comments;
            Data = dbType.Data;
            MatchingResult = (ExpenseValidationMatchingResult) dbType.MatchingResult;
            
            // Note: that whilst there is an ExpenseSubCategoryId in this class that should be populate, it is for the help of the API user.
            // It is set in the repository rather than here, as there is already plenty of validation. So no need to populate here.
            ExpenseSubCategoryId = ExpenseSubCategoryId;

            return this;
        }

        /// <summary>
        /// Converts to a data access layer Type from an api Type.
        /// </summary>
        /// <param name="actionContext">The actionContext which contains DAL classes.</param>
        /// <returns>A data access layer Type</returns>
        public SpendManagementLibrary.Expedite.ExpenseValidationResult To(IActionContext actionContext)
        {
            var criterion = actionContext.ExpenseValidation.GetCriterion(CriterionId);
            if (criterion == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorValidationCriterionDoesntExist);
            }

            return new SpendManagementLibrary.Expedite.ExpenseValidationResult
            {
                Id = Id,
                ExpenseItemId = ExpenseItemId,
                Criterion = criterion,
                BusinessStatus = (SpendManagementLibrary.Enumerators.Expedite.ExpenseValidationResultStatus)BusinessStatus,
                VATStatus = (SpendManagementLibrary.Enumerators.Expedite.ExpenseValidationResultStatus)VATStatus,
                PossiblyFraudulent = PossiblyFraudulent,
                Timestamp = Timestamp,
                Comments = Comments,
                Data = Data,
                MatchingResult = (SpendManagementLibrary.Enumerators.Expedite.ExpenseValidationMatchingResult)MatchingResult
            };
        }
    }
}
