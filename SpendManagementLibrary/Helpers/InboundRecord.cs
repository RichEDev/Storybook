namespace SpendManagementLibrary.Helpers
{
    using System.Collections.Generic;

    /// <summary>
    /// A class to hold data for a single ESR Inbound Record.
    /// </summary>
    public class InboundRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InboundRecord"/> class. 
        /// </summary>
        /// <param name="column">
        /// The string created by the export
        /// </param>
        public InboundRecord(string[] column)
        {
            this.ElementValues = new Dictionary<int, InboundRecordValue>();
            this.RecordType = column[0];
            this.EffectiveDate = column[1];
            this.OperationType = column[2];
            this.SourceSystemEmployeeReference = column[3];
            this.AssignmentNumber = column[4];
            this.EarnedDate = column[5];
            this.ElementName = column[6];
            var index = 1;
            for (int i = 7; i < 37; i = i + 2)
            {
                if (column.Length > i)
                {
                    this.ElementValues.Add(index, new InboundRecordValue(column[i], column[i + 1]));
                }
                
                index++;
            }

            if (column.Length >= 36)
            {
                this.AccountCodeLegalEntity = column[37];
                if (column.Length > 37)
                {
                    this.AccountCodeCharitableIndicator = column[38];
                    if (column.Length > 39)
                    {
                        this.AccountCodeCostCentre = column[39];
                        if (column.Length > 40)
                        {
                            this.AccountCodeSubjectiveCode = column[40];
                            if (column.Length > 41)
                            {
                                this.AccountCodeSubAnalysis = column[41];
                                if (column.Length > 42)
                                {
                                    this.AccountCodeElementNumber = column[42];
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InboundRecord"/> class.
        /// </summary>
        public InboundRecord()
        {
            this.ElementValues = new Dictionary<int, InboundRecordValue>();
        }

        /// <summary>
        /// Gets or sets the earned date.
        /// </summary>
        public string EarnedDate { get; set; }

        /// <summary>
        /// Gets or sets the source system employee reference.
        /// </summary>
        public string SourceSystemEmployeeReference { get; set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        public string EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the record type.
        /// </summary>
        public string RecordType { get; set; }

        /// <summary>
        /// Gets or sets the assignment number.
        /// </summary>
        public string AssignmentNumber { get; set; }

        /// <summary>
        /// Gets or sets the operation type.
        /// </summary>
        public string OperationType { get; set; }

        /// <summary>
        /// Gets or sets the element name.
        /// </summary>
        public string ElementName { get; set; }

        /// <summary>
        /// Gets or sets the account code legal entity.
        /// </summary>
        public string AccountCodeLegalEntity { get; set; }

        /// <summary>
        /// Gets or sets the account code charitable indicator.
        /// </summary>
        public string AccountCodeCharitableIndicator { get; set; }

        /// <summary>
        /// Gets or sets the account code cost centre.
        /// </summary>
        public string AccountCodeCostCentre { get; set; }

        /// <summary>
        /// Gets or sets the account code subjective code.
        /// </summary>
        public string AccountCodeSubjectiveCode { get; set; }

        /// <summary>
        /// Gets or sets the account code sub analysis.
        /// </summary>
        public string AccountCodeSubAnalysis { get; set; }

        /// <summary>
        /// Gets or sets the account code element number.
        /// </summary>
        public string AccountCodeElementNumber { get; set; }

        /// <summary>
        /// Gets the formatted element fields.
        /// </summary>
        public string FormattedElementFields
        {
            get
            {
                return string.Join(
                    ",",
                    this.ElementValues[1].ElementFieldName,
                    this.ElementValues[2].ElementFieldName,
                    this.ElementValues[3].ElementFieldName,
                    this.ElementValues[4].ElementFieldName,
                    this.ElementValues[5].ElementFieldName,
                    this.ElementValues[6].ElementFieldName,
                    this.ElementValues[7].ElementFieldName,
                    this.ElementValues[8].ElementFieldName,
                    this.ElementValues[9].ElementFieldName,
                    this.ElementValues[10].ElementFieldName,
                    this.ElementValues[11].ElementFieldName,
                    this.ElementValues[12].ElementFieldName,
                    this.ElementValues[13].ElementFieldName,
                    this.ElementValues[14].ElementFieldName,
                    this.ElementValues[15].ElementFieldName);
            }
        }

        /// <summary>
        /// Gets the element values for this record.
        /// </summary>
        private Dictionary<int, InboundRecordValue> ElementValues { get; }

        /// <summary>
        /// Get or set a specific value based on the given index..
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public InboundRecordValue this[int index]
        {
            get
            {
                return this.ElementValues.ContainsKey(index) ? this.ElementValues[index] : null;
            }

            set
            {
                this.ElementValues[index] = value;
            }
        }

        /// <summary>
        /// Output the current <see cref="InboundRecord"/> as a formatted string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return
                string.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27}", this.RecordType, this.EffectiveDate, this.OperationType, this.SourceSystemEmployeeReference, this.AssignmentNumber, this.EarnedDate, this.ElementName, this.ElementValues[1], this.ElementValues[2], this.ElementValues[3], this.ElementValues[4], this.ElementValues[5], this.ElementValues[6], this.ElementValues[7], this.ElementValues[8], this.ElementValues[9], this.ElementValues[10], this.ElementValues[11], this.ElementValues[12], this.ElementValues[13], this.ElementValues[14], this.ElementValues[15], this.AccountCodeLegalEntity, this.AccountCodeCharitableIndicator, this.AccountCodeCostCentre, this.AccountCodeSubjectiveCode, this.AccountCodeSubAnalysis, this.AccountCodeElementNumber);
        }
    }
}