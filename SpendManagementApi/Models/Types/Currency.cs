namespace SpendManagementApi.Models.Types
{
    using Spend_Management;
    using SpendManagementLibrary;
    using System;
    using System.ComponentModel.DataAnnotations;
    using CurrencyType = SpendManagementApi.Common.Enums.CurrencyType;
    using Common;
    
    /// <summary>
    /// Represents a monetary currency. There are a finite amount of Currencies in the world, so SpendManagement
    /// represents these with a Global set. Each account can then choose how many from the global set they would like
    /// including in their custom configuration of Expenses. That is what this Currency object is for.
    /// The GlobalCurrencyId propery can be used with the Globals resource to find more information on currencies.
    /// </summary>
    public class Currency : ArchivableBaseExternalType, IEquatable<Currency>
    {
        /// <summary>
        /// The unique Id of the currency.
        /// </summary>
        [Required]
        public int CurrencyId { get; set; }

        /// <summary>
        /// The currency name
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// The Global Currency Id.<br/>
        /// Use the Globals resource to find GlobalCurrencies.
        /// </summary>
        [Required]
        public int GlobalCurrencyId { get; set; }

        /// <summary>
        /// Gets or sets the alpha code of the currency
        /// </summary>
        public string AlphaCode { get; set; }

        /// <summary>
        /// The Currency Type enumeration - Static(0), Monthly(1), Range(2)
        /// </summary>
        internal CurrencyType? CurrencyType { get; set; }

        public bool Equals(Currency other)
        {
            if (other == null)
            {
                return false;
            }
            return this.Archived.Equals(other.Archived) 
                   && this.GlobalCurrencyId.Equals(other.GlobalCurrencyId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Currency);
        }
    }

    /// <summary>
    /// Conversion from external to internal type and vice and versa
    /// </summary>
    internal static class CurrencyTypeConversion
    {
        internal static SpendManagementLibrary.CurrencyType Cast<TRes>(this CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case CurrencyType.Monthly:
                    return SpendManagementLibrary.CurrencyType.Monthly;
                case CurrencyType.Range:
                    return SpendManagementLibrary.CurrencyType.Range;
                case CurrencyType.Static:
                    return SpendManagementLibrary.CurrencyType.Static;
                default:
                    throw new ApiException("Invalid Currency Type", "Invalid currency type");
            }
        }

        internal static CurrencyType Cast<TRes>(this SpendManagementLibrary.CurrencyType currencyType)
        {
            switch (currencyType)
            {
                case SpendManagementLibrary.CurrencyType.Monthly:
                    return CurrencyType.Monthly;
                case SpendManagementLibrary.CurrencyType.Range:
                    return CurrencyType.Range;
                case SpendManagementLibrary.CurrencyType.Static:
                    return CurrencyType.Static;
                default:
                    throw new ApiException("Invalid Currency Type", "Invalid currency type");
            }
        }

    }

    internal static class CurrencyConversion
    {
        internal static TResult Cast<TResult>(this cCurrency currency, cGlobalCurrencies clsGlobalCurrencies)
            where TResult : Currency, new()
        {
            if (currency == null)
                return null;
            return new TResult
            {
                AccountId = currency.accountid,
                Archived = currency.archived,
                CreatedById = currency.createdby ?? 0,
                CreatedOn = currency.createdon,
                CurrencyId = currency.currencyid,
                GlobalCurrencyId = currency.globalcurrencyid,
                ModifiedById = currency.modifiedby,
                ModifiedOn = currency.modifiedon
            };
        }

        internal static TRes Cast<TRes>(this Currency currency) where TRes : cCurrency, new()
        {
            return new TRes
            {
                accountid = (currency.AccountId??-1),
                createdby = currency.CreatedById, 
                createdon = currency.CreatedOn,
                currencyid = currency.CurrencyId,
                globalcurrencyid = currency.GlobalCurrencyId
            };
        }

        internal static TResult Cast<TResult>(this cGlobalCurrency globalCurrency, bool toCurrency) where TResult : Currency, new()
        {
            if (globalCurrency != null)
                return new TResult
                {
                    GlobalCurrencyId = globalCurrency.globalcurrencyid,
                    Archived = true
                };
            return null;
        }
    }

}