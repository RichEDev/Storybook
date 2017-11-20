namespace BusinessLogic.Fields.Type.Base
{
    /// <summary>
    /// an implementation of <see cref="IField"/> that is a Boolean.
    /// </summary>
    public class BooleanField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="BooleanField"/>.
        /// </param>
        public BooleanField(Field field) : base(field)
        {
        }
    }
}
