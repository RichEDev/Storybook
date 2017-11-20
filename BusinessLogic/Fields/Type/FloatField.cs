namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class FloatField : DecimalField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FloatField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="FloatField"/> to decorate as a <see cref="DecimalField"/>.
        /// </param>
        public FloatField(DecimalField field) : base(field)
        {
        }
    }
}
