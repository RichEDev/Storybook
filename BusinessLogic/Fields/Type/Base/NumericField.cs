namespace BusinessLogic.Fields.Type.Base
{
    /// <summary>
    /// an instance of <see cref="IField"/> that is numeric (money).
    /// </summary>
    public class NumericField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="NumericField"/>.
        /// </param>
        public NumericField(Field field) : base(field)
        {
        }
    }
}
