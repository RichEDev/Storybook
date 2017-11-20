namespace BusinessLogic.Fields.Type.Base
{
    /// <summary>
    /// an instance of <see cref="IField"/> that is a long.
    /// </summary>
    public class LongField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LongField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="LongField"/>.
        /// </param>
        public LongField(Field field) : base(field)
        {
        }
    }
}
