namespace SpendManagementLibrary.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using BusinessLogic.GeneralOptions.ESR;

    /// <summary>
    /// Maintain the exported file and summarise if necessary.
    /// </summary>
    public class EsrInboundBuilder : IDisposable
    {
        /// <summary>
        /// True if the Esr is to be summarised..
        /// </summary>
        private readonly bool _summaryOutput;

        /// <summary>
        /// An instance of <see cref="InboundValueTypes"/> which holds the "type" of each column based on it's content.
        /// </summary>
        private readonly InboundValueTypes _inboundValueTypes;

        /// <summary>
        /// An instance of <see cref="InboundFactory"/> used to summarise and format the output for the summary.
        /// </summary>
        private readonly InboundFactory _inboundFactory;

        /// <summary>
        /// An instance of <see cref="StringBuilder"/> used to collate the output.
        /// </summary>
        private StringBuilder _stringBuilder;

        /// <summary>
        /// An array of the lines used to process the file when summarising..
        /// </summary>
        private string[] _lines;

        /// <summary>
        /// A list of ESR Elements, one for each line output.
        /// </summary>
        private List<cESRElement> _elements;

        /// <summary>
        /// The _global elements, used in this export.
        /// </summary>
        private List<cGlobalESRElement> _globalElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="EsrInboundBuilder"/> class. 
        /// </summary>
        /// <param name="summaryOutput">
        /// True if this is a summary output
        /// </param>
        /// <param name="esrInboundFormatting">
        /// An instance i=of <see cref="IEsrInboundFormatting"/> class
        /// </param>
        /// <param name="esrRoundingType">
        /// The type of rounding to apply.
        /// </param>
        public EsrInboundBuilder(bool summaryOutput, IEsrInboundFormatting esrInboundFormatting, EsrRoundingType esrRoundingType)
        {
            this._summaryOutput = summaryOutput;
            this._stringBuilder = new StringBuilder();
            this._inboundValueTypes = new InboundValueTypes();
            this._inboundFactory = new InboundFactory(esrRoundingType, esrInboundFormatting);
            this._globalElements = new List<cGlobalESRElement>();
            this._elements = new List<cESRElement>();
        }

        /// <summary>
        /// Append the given value to the export file.
        /// </summary>
        /// <param name="value">
        /// The value to append
        /// </param>
        /// <param name="globalElement">
        /// The global element for this line of the export file.
        /// </param>
        public void Append(StringBuilder value, cGlobalESRElement globalElement)
        {
            this._stringBuilder.Append(value);

            if (!this._globalElements.Contains(globalElement))
            {
                this._globalElements.Add(globalElement);
            }

        }

        /// <summary>
        /// Append the given value to the export file.
        /// </summary>
        /// <param name="value">The value to append</param>
        public void Append(string value)
        {
            this._stringBuilder.Append(value);
        }

        /// <summary>
        /// Append the given value to the export file.
        /// </summary>
        /// <param name="value">The value to append (as String)</param>
        public void Append(int value)
        {
            this._stringBuilder.Append(value);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
        }

        /// <summary>
        /// Output the content of the EsrInboundBuilder.
        /// </summary>
        /// <returns>
        /// Either the detail or summary ESR Inbound file.
        /// </returns>
        public override string ToString()
        {
            var recordCount = 0;
            if (this._summaryOutput)
            {
                var recordList = this.PopulateData();
                this.SetValueTypes(recordList);
                this._stringBuilder = new StringBuilder(this._lines[0]);
                this._stringBuilder.Append("\r");

                var groupedRecords =
                    recordList.GroupBy(
                        item =>
                        new
                        {
                            item.AssignmentNumber,
                            item.ElementName,
                            item.FormattedElementFields,
                            values = this.ValueFilters(item),
                            AccountCode_SubjectiveCode = item.AccountCodeSubjectiveCode,
                            AccountCode_CostCentre = item.AccountCodeCostCentre,
                        });

                foreach (var grouping in groupedRecords)
                {
                    InboundRecord grouped = this._inboundFactory.Consolidate(new List<InboundRecord>(grouping), this._globalElements);
                    this._stringBuilder.Append(grouped);
                    this._stringBuilder.Append("\r");
                    recordCount++;
                }

                this._stringBuilder.Append("FTR,");
                this._stringBuilder.Append(recordCount);
                this._stringBuilder.Append(",0");

                return this._stringBuilder.ToString();
            }
            else
            {
                return this._stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Iterate through every item and for each value, if NOT numeric, set type to groupBy
        /// </summary>
        /// <param name="recordList">A <see>
        ///         <cref>List</cref>
        ///     </see>
        /// of <see cref="InboundRecord"/> instances to process.
        /// </param>
        private void SetValueTypes(List<InboundRecord> recordList)
        {
            foreach (InboundRecord inboundRecord in recordList)
            {
                for (int i = 0; i < 15; i++)
                {
                    double testValue;
                    var elementField = inboundRecord[i + 1];
                    if (elementField != null && !double.TryParse(elementField.Value, out testValue) && !string.IsNullOrEmpty(elementField.Value))
                    {
                        this._inboundValueTypes[i] = ValueTypes.GroupBy;
                    }
                }
            }
        }

        /// <summary>
        /// Set to columns used to summarise the input data.
        /// </summary>
        /// <param name="item">
        /// The <see cref="InboundRecord"/> to process / filter.
        /// </param>
        /// <returns>
        /// A <see cref="string"/> containing the concatenated data from each columns marked as Group By.
        /// </returns>
        private string ValueFilters(InboundRecord item)
        {
            var filterString = new StringBuilder();
            for (int i = 0; i < 15; i++)
            {
                var itemValue = item[i + 1];
                decimal valueDecimal;
                if (!decimal.TryParse(itemValue.Value, out valueDecimal))
                {
                    filterString.Append(itemValue.Value);
                    filterString.Append(",");
                }
            }

            return filterString.ToString();
        }

        /// <summary>
        /// Convert the current <see cref="StringBuilder"/> into a List of <see cref="InboundRecord"/>.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .Of <see cref="InboundRecord"/> based on the current <see cref="StringBuilder"/>
        /// </returns>
        private List<InboundRecord> PopulateData()
        {
            this._lines = this._stringBuilder.ToString().Split('\r');

            return (from line in this._lines where !string.IsNullOrEmpty(line) && line.StartsWith("ATT") select new InboundRecord(line.Split(','))).ToList();
        }
    }
}
