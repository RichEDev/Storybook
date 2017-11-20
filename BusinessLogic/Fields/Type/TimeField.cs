namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class TimeField : DateTimeField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimeField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="TimeField"/> to decorate as a <see cref="DateTimeField"/>.
        /// </param>
        public TimeField(DateTimeField field) : base(field)
        {
        }
    }
}
