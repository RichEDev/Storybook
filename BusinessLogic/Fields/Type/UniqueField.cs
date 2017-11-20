namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class UniqueField : GuidField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniqueField"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="UniqueField"/> to decorate as a <see cref="GuidField"/>.
        /// </param>
        public UniqueField(GuidField field) : base(field)
        {
        }
    }
}
