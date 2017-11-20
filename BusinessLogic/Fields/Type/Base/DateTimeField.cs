namespace BusinessLogic.Fields.Type.Base
{
    /// <summary>
    /// An instance of <see cref="IField"/> that is a DateTime type.
    /// </summary>
    public class DateTimeField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="DateTimeField"/>.
        /// </param>
        public DateTimeField(Field field) : base(field)
        {
        }
    }
}
