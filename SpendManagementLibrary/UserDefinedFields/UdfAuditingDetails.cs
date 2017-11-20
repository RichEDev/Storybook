namespace SpendManagementLibrary.UserDefinedFields
{
    /// <summary>
    /// A class to hold Udf values and field details 
    /// </summary>
    public class UdfAuditingDetails
    {
        /// <summary>
        /// The value of the UDF
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// The Id of the UDF
        /// </summary>
        public int UdfId { get; set; }

        /// <summary>
        /// The field
        /// </summary>
        public cField Field { get; set; }

        public UdfAuditingDetails(int udfId, object value, cField field)
        {         
            this.Value = value;
            this.UdfId = udfId;     
            this.Field = field;
        }
    }
}





