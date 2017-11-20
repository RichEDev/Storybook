using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    [Serializable()]
    public class cGeneralOptions
    {      
        /// <summary>
        /// SubAccount ID
        /// </summary>
        public int nSubAccountID { get; set; }
        /// <summary>
        /// Key
        /// </summary>
        public string nKey { get; set; }
        /// <summary>
        /// Value
        /// </summary>
        public string nValue { get; set; }
        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime nCreatedOn { get; private set; }
        /// <summary>
        /// CreatedBy
        /// </summary>
        public int nCreatedBy { get; private set; }
        /// <summary>
        /// ModifiedOn
        /// </summary>
        public DateTime? nModifiedOn { get; private set; }
        /// <summary>
        /// ModifiedBy
        /// </summary>
        public int? nModifiedBy { get; private set; }
        /// <summary>
        /// PostKey
        /// </summary>
        public string nPostKey { get; set; }
        /// <summary>
        /// IsGlobal
        /// </summary>
        public bool nIsGlobal { get; set; }

        public cGeneralOptions(int subAccountID, string key, string value, DateTime createdOn, int createdBy, DateTime? ModifiedOn, int? ModifiedBy, string postKey, bool isGlobal)
        {
            this.nSubAccountID = subAccountID;
            this.nKey = key;
            this.nValue = value;
            this.nCreatedOn = createdOn;
            this.nCreatedBy = createdBy;
            this.nModifiedOn = ModifiedOn;
            this.nModifiedBy = ModifiedBy;
            this.nPostKey = postKey;
            this.nIsGlobal = isGlobal;
        }
    }
}
