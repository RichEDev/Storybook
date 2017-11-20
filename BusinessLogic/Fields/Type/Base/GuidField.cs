namespace BusinessLogic.Fields.Type.Base
{
    /// <summary>
    /// an instance of <see cref="IField"/> that is a Guid.
    /// </summary>
    public class GuidField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GuidField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="GuidField"/>.
        /// </param>
        public GuidField(Field field) :base (field)
        {
        }
    }
}
