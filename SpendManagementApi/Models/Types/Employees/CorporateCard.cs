namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.IO;

    using SpendManagementLibrary;
    using Attributes.Validation;
    using Interfaces;
    using Utilities;
    using SpendManagementApi.Common.Enums;
    
    /// <summary>
    /// Represents a corporate card against which items can be paid for and expenses claimed.
    /// </summary>
    public class CorporateCard : BaseExternalType, IRequiresValidation
    {
        /// <summary>
        /// The Card Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The owning Employee.
        /// <strong>EmployeeID is essential to adding a CorporateCard. 
        /// A card cannot currently exist without a valid Employee to own it</strong>.
        /// </summary>
        public new int EmployeeId { get; set; }

        /// <summary>
        /// The type of card that this card is.
        /// Card Type - Purchase Card : 0, Credit Card : 1
        /// </summary>
        [ValidEnumValue(ErrorMessage = ApiResources.ApiErrorEnum)]
        public CardType CardType { get; set; }

        /// <summary>
        /// The Id of the Card Provider.
        /// </summary>
        public int CardProviderId { get; set; }

        /// <summary>
        /// The long Card Number.
        /// </summary>
        public string CardNumber { get; set; }

        /// <summary>
        /// Whether the card is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Validates this object.
        /// </summary>
        /// <param name="actionContext">The context for validation.</param>
        /// <exception cref="InvalidDataException">Errors in case the data provided is not valid.</exception>
        public void Validate(IActionContext actionContext)
        {
            if (actionContext.Employees.GetEmployeeById(EmployeeId) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (CardProviderId <= 0 || actionContext.CardProviders.getProviderByID(CardProviderId) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorValidCardProvider);
            }

            if (string.IsNullOrWhiteSpace(CardNumber))
            {
                throw new InvalidDataException(ApiResources.ApiErrorValidCardNumber);
            }
        }

        /// <summary>
        /// Determines whether this object is equal to another.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(CorporateCard other)
        {
            if (other == null)
            {
                return false;
            }

            return CardNumber.Equals(other.CardNumber) 
                    && CardProviderId.Equals(other.CardProviderId)
                    && CardType.Equals(other.CardType) 
                    && IsActive.Equals(other.IsActive) 
                    && EmployeeId.Equals(other.EmployeeId);
        }
    }

    internal static class CorporateCardConversion
    {
        internal static TResult Cast<TResult>(this cEmployeeCorporateCard corporateCard) where TResult : CorporateCard, new()
        {
            if (corporateCard == null)
            {
                return null;
            }

            return new TResult
                       {
                           Id = corporateCard.corporatecardid,
                           CardNumber = corporateCard.cardnumber,
                           CardProviderId = corporateCard.cardprovider.cardproviderid,
                           CardType = (CardType) corporateCard.cardprovider.cardtype,
                           CreatedById = corporateCard.CreatedBy.HasValue ? corporateCard.CreatedBy.Value : 0,
                           CreatedOn = corporateCard.CreatedOn.HasValue ? corporateCard.CreatedOn.Value : new DateTime(1900, 1, 1),
                           EmployeeId = corporateCard.employeeid,
                           IsActive = corporateCard.active,
                           ModifiedById = corporateCard.ModifiedBy,
                           ModifiedOn = corporateCard.ModifiedOn
                       };
        }

        internal static cEmployeeCorporateCard Cast<TResult>(this CorporateCard corporateCard, cEmployeeCorporateCard cCard = null) where TResult : cEmployeeCorporateCard
        {
            var cardProviders = new CardProviders();
            var cardProvider = cardProviders.getProviderByID(corporateCard.CardProviderId);
            if (cCard != null)
            {
                return new cEmployeeCorporateCard(corporateCard.Id, cCard.employeeid, cardProvider, corporateCard.CardNumber, corporateCard.IsActive, corporateCard.CreatedOn, corporateCard.CreatedById, DateTime.UtcNow, corporateCard.ModifiedById);
            }

            return new cEmployeeCorporateCard(corporateCard.Id, corporateCard.EmployeeId, cardProvider, corporateCard.CardNumber, corporateCard.IsActive, DateTime.UtcNow, corporateCard.CreatedById, DateTime.UtcNow, corporateCard.ModifiedById);
        }
    }
}