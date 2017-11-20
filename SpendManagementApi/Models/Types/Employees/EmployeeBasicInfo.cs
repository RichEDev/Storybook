using System.IO;
using SpendManagementApi.Utilities;

namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using Interfaces;


    /// <summary>
    /// Represents all of the personal information for a given user. This includes names, locale and enrcypted password data.
    /// </summary>
    public class OptionalGeneralDetails : IRequiresValidation, IEquatable<OptionalGeneralDetails>
    {
        /// <summary>
        /// The Gender of the employee.
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// The Date of birth of the employee.
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// The middle name(s), if any, of the employee.
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// The maiden name, if any, of the employee.
        /// </summary>
        public string MaidenName { get; set; }

        /// <summary>
        /// The preferred name of the employee.
        /// </summary>
        public string PreferredName { get; set; }

        /// <summary>
        /// The Id of the locale of the employee.
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// Whether the employee has any customised items present.
        /// </summary>
        internal bool HasCustomisedAddItems { get; set; }

        /// <summary>
        /// Whether this is the first logon of the employee.
        /// </summary>
        internal bool FirstLogon { get; set; }

        /// <summary>
        /// Whether this employee has an Admin Override applied.
        /// </summary>
        internal bool AdminOverride { get; set; }
        
        /// <summary>
        /// Validates the properties
        /// </summary>
        public void Validate(IActionContext actionContext)
        {
            if (!string.IsNullOrEmpty(Gender) && (Gender.ToLower() != "male" && Gender.ToLower() != "female"))
            {
                throw new InvalidDataException(ApiResources.ApiErrorGender);
            }

            if (LocaleId.HasValue && LocaleId > 0)
            {
                var locale = actionContext.Locales.getLocaleByID(LocaleId.Value);
                
                if (locale == null)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorLocaleInvalid);
                }
                
                if (!locale.Active)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorLocaleInactive);
                }
            }
        }

        internal static OptionalGeneralDetails Merge(OptionalGeneralDetails dataToUpdate, OptionalGeneralDetails existing)
        {
            if (dataToUpdate == null)
            {
                dataToUpdate = new OptionalGeneralDetails
                                   {
                                       AdminOverride = existing.AdminOverride,
                                       FirstLogon = existing.FirstLogon,
                                       HasCustomisedAddItems = existing.HasCustomisedAddItems,
                                       LocaleId = existing.LocaleId,
                                       MaidenName = existing.MaidenName,
                                       MiddleName = existing.MiddleName,
                                       PreferredName = existing.PreferredName,
                                       DateOfBirth = existing.DateOfBirth,
                                       Gender = existing.Gender,
                                   };
            }

            return dataToUpdate;
        }

        public bool Equals(OptionalGeneralDetails other)
        {
            if (other == null)
            {
                return false;
            }
            return DateOfBirth.Equals(other.DateOfBirth)
                    && FirstLogon.Equals(other.FirstLogon)
                    && Gender.Equals(other.Gender)
                    && HasCustomisedAddItems.Equals(other.HasCustomisedAddItems)
                    && LocaleId.Equals(other.LocaleId) && MaidenName.Equals(other.MaidenName)
                    && MiddleName.Equals(other.MiddleName);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as OptionalGeneralDetails);
        }
    }

    internal static class EmployeeOptionalBasicInfoConversion
    {
        internal static TResult Cast<TResult>(this SpendManagementLibrary.Employees.Employee employee)
            where TResult : OptionalGeneralDetails, new()
        {
            return new TResult
            {
                AdminOverride = employee.AdminOverride,
                FirstLogon = employee.FirstLogon,

                HasCustomisedAddItems = employee.HasCustomisedAddItems,
                LocaleId = employee.LocaleID,
                MaidenName = employee.MaidenName,
                MiddleName = employee.MiddleNames,
                PreferredName = employee.PreferredName,
                DateOfBirth = employee.DateOfBirth,
                Gender = employee.Gender,
            };
        }
    }
}