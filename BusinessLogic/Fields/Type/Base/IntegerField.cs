namespace BusinessLogic.Fields.Type.Base
{
    /// <summary>
    /// an instance of <see cref="IField"/> that is an integer.
    /// </summary>
    public class IntegerField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="IntegerField"/>.
        /// </param>
        public IntegerField(Field field) : base(field)
        {
        }
    }
}
