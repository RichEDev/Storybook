namespace BusinessLogic.Fields.Type.Base
{
    public class DecimalField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="DecimalField"/>.
        /// </param>
        public DecimalField(Field field) :base(field)
        {
        }
    }
}
