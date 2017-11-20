namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class NumberField : IntegerField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumberField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="NumberField"/> to decorate as a <see cref="IntegerField"/>.
        /// </param>
        public NumberField(IntegerField field) : base(field)
        {
        }
    }
}
