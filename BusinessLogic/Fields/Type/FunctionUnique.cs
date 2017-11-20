namespace BusinessLogic.Fields.Type
{
    using BusinessLogic.Fields.Type.Base;

    public class FunctionUnique : GuidField
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionUnique"/> class. 
        /// </summary>
        /// <param name="field">
        /// The base <see cref="FunctionUnique"/> to decorate as a <see cref="GuidField"/>.
        /// </param>
        public FunctionUnique(GuidField field) : base(field)
        {
        }
    }
}
