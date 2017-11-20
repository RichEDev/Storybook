namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class DateField : DateTimeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="DateField"/> to decorate as a <see cref="DateTimeField"/>.
        /// </param>
        public DateField(DateTimeField field) : base(field)
        {
        }
    }
}
