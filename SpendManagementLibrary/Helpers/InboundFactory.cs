namespace SpendManagementLibrary.Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    using SpendManagementLibrary.Enumerators;

    /// <summary>
    /// Helpers to generate ESR Inbound files.
    /// </summary>
    public class InboundFactory
    {
        /// <summary>
        /// The ESR rounding type.
        /// </summary>
        private readonly EsrRoundingType _esrRoundingType;

        /// <summary>
        /// An instance of <see cref="IEsrInboundFormatting"/> used to format the output.
        /// </summary>
        private readonly IEsrInboundFormatting _esrInboundFormatting;

        /// <summary>
        /// Initializes a new instance of the <see cref="InboundFactory"/> class. 
        /// </summary>
        /// <param name="esrRoundingType">
        /// The <see cref="EsrRoundingType"/> used for this instance
        /// </param>
        /// <param name="esrInboundFormatting">
        /// The <see cref="IEsrInboundFormatting"/> used for this instance
        /// </param>
        public InboundFactory(EsrRoundingType esrRoundingType, IEsrInboundFormatting esrInboundFormatting)
        {
            this._esrRoundingType = esrRoundingType;
            this._esrInboundFormatting = esrInboundFormatting;
        }

        /// <summary>
        /// ESR Inbound record consolidation.
        /// </summary>
        /// <param name="inboundRecords">
        /// A list of InboundRecords to be consolidated
        /// </param>
        /// <param name="globalElements">A list of <see cref="cGlobalESRElement"/> used in this export</param>
        /// <returns>
        /// A <see cref="InboundRecord"/> with all the <see cref="ValueTypes"/> of "value" consolidated.
        /// </returns>
        public InboundRecord Consolidate(List<InboundRecord> inboundRecords, List<cGlobalESRElement> globalElements)
        {
            var result = inboundRecords.FirstOrDefault();
            if (result == null)
            {
                return null;
            }

            var sumValue = this.InitialiseSumValue();

            this.ConsolidateValuesFromInboundRecords(inboundRecords, sumValue, globalElements);

            this.GenerateConsolidatedInboundRecord(result, sumValue, globalElements);

            return result;
        }

        /// <summary>
        /// Generate a consolidated inbound record based on the sumValue array.
        /// </summary>
        /// <param name="result">
        ///  The instance of <see cref="InboundRecord"/> to update.
        /// </param>
        /// <param name="sumValue">
        ///  The sum values of each column.
        /// </param>
        /// <param name="globalElements">A list of <see cref="cGlobalESRElement"/> used in the export</param>
        private void GenerateConsolidatedInboundRecord(InboundRecord result, double[] sumValue, List<cGlobalESRElement> globalElements)
        {
            for (var i = 0; i < 15; i++)
            {
                var item = result[i + 1];
                var currentElement = this._esrInboundFormatting.GetElementField(globalElements, result.ElementName,
                    item.ElementFieldName);
                if (currentElement == null || !currentElement.SummaryColumn)
                {
                    continue;
                }

                if (currentElement.Rounded)
                {
                    var num = (decimal)sumValue[i];
                    item.Value = num > 0 ? num.EsrRounding(this._esrRoundingType).ToString("##########") : "0";

                }
                else
                {
                    item.Value = sumValue[i] > 0 ? sumValue[i].ToString("F") : string.Empty;
                }

                result[i + 1] = item;
            }
        }

        /// <summary>
        /// Consolodate the values from the inbound records.
        /// </summary>
        /// <param name="inboundRecords">
        /// The inbound records to consolidate.
        /// </param>
        /// <param name="sumValue">
        /// An array of <see cref="double"/> representing the consolidated value of each value column.
        /// </param>
        /// <param name="globalElements"></param>
        private void ConsolidateValuesFromInboundRecords(List<InboundRecord> inboundRecords, double[] sumValue, List<cGlobalESRElement> globalElements)
        {
            foreach (var inboundRecord in inboundRecords)
            {
                for (var i = 0; i < 15; i++)
                {
                    var currentElement = this._esrInboundFormatting.GetElementField(globalElements, inboundRecord.ElementName,
                    inboundRecord[i + 1].ElementFieldName);
                    double numericValue;
                    if (currentElement != null && currentElement.SummaryColumn && double.TryParse(inboundRecord[i + 1].Value, out numericValue))
                    {
                        sumValue[i] = sumValue[i] + numericValue;
                    }
                }
            }
        }

        /// <summary>
        /// Create and initilise the sumValue array.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>double[]</cref>
        ///     </see>
        ///     of values all set to zero.
        /// </returns>
        private double[] InitialiseSumValue()
        {
            var sumValue = new double[15];
            for (var i = 0; i < 15; i++)
            {
                sumValue[i] = 0;
            }

            return sumValue;
        }
    }
}