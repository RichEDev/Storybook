namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class LargeText : StringField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LargeText"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="LargeText"/> to decorate as a <see cref="StringField"/>.
        /// </param>
        public LargeText(StringField field) : base(field)
        {
        }
    }
}
