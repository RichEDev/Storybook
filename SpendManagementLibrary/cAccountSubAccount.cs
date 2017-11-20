using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    [Serializable]
    public class cAccountSubAccount
    {
        private readonly int _subAccountId;
        private readonly string _description;
        private readonly bool _archived;
        private readonly DateTime _createdOn;
        private readonly int _createdBy;
        private readonly DateTime? _modifiedOn;
        private readonly int? _modifiedBy;
        private readonly cAccountProperties _accountProperties;

        public cAccountSubAccount(int subAccountId, string description, bool archived, cAccountProperties accountProperties, DateTime createdOn, int createdBy, DateTime? modifiedOn, int? modifiedBy)
        {
            _subAccountId = subAccountId;
            _description = description;
            _archived = archived;
            _accountProperties = accountProperties;
            _createdOn = createdOn;
            _createdBy = createdBy;
            _modifiedOn = modifiedOn;
            _modifiedBy = modifiedBy;
        }

        /// <summary>
        /// SubAccountID, the unique ID assigned to this subAccountID;
        /// </summary>
        public int SubAccountID { get { return _subAccountId; } }

        /// <summary>
        /// Description for this subAccount
        /// </summary>
        public string Description { get { return _description; } }

        /// <summary>
        /// Gets the archived status of this sub account
        /// </summary>
        public bool IsArchived { get { return _archived; } }

        /// <summary>
        /// Gets the sub account properties related to this account
        /// </summary>
        public cAccountProperties SubAccountProperties { get { return _accountProperties; } }

        /// <summary>
        /// Created On Date
        /// </summary>
        public DateTime CreatedOn { get { return _createdOn; } }

        /// <summary>
        /// Created By employeeID
        /// </summary>
        public int CreatedBy { get { return _createdBy; } }

        /// <summary>
        /// ModifiedOn DateTime, null if not yet modified
        /// </summary>
        public DateTime? ModifiedOn { get { return _modifiedOn; } }

        /// <summary>
        /// ModifiedBY employeeID, null if not yet modified
        /// </summary>
        public int? ModifiedBy { get { return _modifiedBy; } }

        /// <summary>
        /// Gets the password policy text.
        /// </summary>
        public List<string> PasswordPolicyText
        {
            get
            {
                var policies = new List<string>();

                if (this.SubAccountProperties.PwdMustContainNumbers)
                {
                    policies.Add("Your new password must contain a number.");
                }

                if (this.SubAccountProperties.PwdMustContainUpperCase)
                {
                    policies.Add("Your new password must contain an upper case character.");
                }

                if (this.SubAccountProperties.PwdMustContainSymbol)
                {
                    policies.Add("Your new password must contain a symbol.");
                }

                switch (this.SubAccountProperties.PwdConstraint)
                {
                    case PasswordLength.EqualTo:
                        policies.Add(string.Format("Your new password must be {0} characters in length.", this.SubAccountProperties.PwdLength1));
                        break;
                    case PasswordLength.GreaterThan:
                        policies.Add(string.Format("Your new password must be greater than {0} characters in length.", this.SubAccountProperties.PwdLength1));
                        break;
                    case PasswordLength.LessThan:
                        policies.Add(string.Format("Your new password must be less than {0} characters in length.", this.SubAccountProperties.PwdLength1));
                        break;
                    case PasswordLength.Between:
                        policies.Add(string.Format("Your new password must be at least {0} and no greater than {1} characters in length.", this.SubAccountProperties.PwdLength1, this.SubAccountProperties.PwdLength2));
                        break;
                }

                if (this.SubAccountProperties.PwdHistoryNum > 0)
                {
                    policies.Add("Your new password cannot be any of the previous " + this.SubAccountProperties.PwdHistoryNum + " passwords you have used.");
                }

                return policies;
            }
        } 
    }
}
