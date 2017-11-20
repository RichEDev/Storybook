namespace BusinessLogic.Fields.Type.Base
{
    public class VarBinaryField : Field
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VarBinaryField"/> class. 
        /// </summary>
        /// <param name="field">The base <see cref="Field"/> to decorate as a <see cref="VarBinaryField"/>.
        /// </param>
        public VarBinaryField(Field field)
            : base(field)
        {
        }
    }
}
