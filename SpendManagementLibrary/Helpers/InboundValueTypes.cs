namespace SpendManagementLibrary.Helpers
{
    /// <summary>
    /// Maintain the column types for the InboundRecord.
    /// </summary>
    public class InboundValueTypes
    {
        /// <summary>
        /// A local array containing the types for each column.
        /// </summary>
        private ValueTypes[] _types;

        /// <summary>
        /// Initializes a new instance of the <see cref="InboundValueTypes"/> class.
        /// </summary>
        public InboundValueTypes()
        {
            this._types = new ValueTypes[15];
            for (int i = 0; i < 15; i++)
            {
                this._types[i] = ValueTypes.Value;
            }
        }

        /// <summary>
        /// Get and set the value for the columns 0 to 14.
        /// </summary>
        /// <param name="index">The index of the column <see cref="ValueTypes"/> to return</param>
        /// <returns>The value of the indexed column</returns>
        public ValueTypes this[int index]
        {
            get
            {
                if (index > -1 && index < 15)
                {
                    return this._types[index];
                }

                return ValueTypes.Null;
            }

            set
            {
                if (index > -1 && index < 15)
                {
                    this._types[index] = value;
                }
            }
        }
    }
}