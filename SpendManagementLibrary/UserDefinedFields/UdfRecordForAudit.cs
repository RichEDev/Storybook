using System;

namespace SpendManagementLibrary
{
    /// <summary>
    /// A class to hold the User Defined Field (UDF) audit details 
    /// </summary>
    public class UdfRecordForAudit
    {
        /// <summary>
        /// The fieldId of the UDF
        /// </summary>
        public Guid FieldId { get; set; }

        /// <summary>
        /// The previous UDF value
        /// </summary>     
        public string PreviousValue { get; set; }
       
        /// <summary>
        /// The new UDF value
        /// </summary>
        public string NewValue { get; set; }
      
        /// <summary>
        /// The id of the UDF
        /// </summary>
        public string UdfId { get; set; }

        public UdfRecordForAudit(Guid fieldId, string previousValue, string newValue, string udfId)
        {
            this.FieldId = fieldId;
            this.PreviousValue = previousValue;
            this.NewValue = newValue;
            this.UdfId = udfId;
        }
    }
}


