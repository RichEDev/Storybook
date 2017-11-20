namespace BusinessLogic.Fields.Type.Base
{
    /// <summary>
    /// An instance if <see cref="IField"/> that is a string.
    /// </summary>
    public class StringField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="StringField"/>.
        /// </param>
        public StringField(Field field) : base(field)
        {
        }
    }
}
