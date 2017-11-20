namespace BusinessLogic.Fields.Type.Attributes
{
    using BusinessLogic.Fields.Type.Base;

    /// <summary>
    /// The associated <see cref="IField"/> has been relabeled.
    /// </summary>
    public class RelabelAttribute : IFieldAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RelabelAttribute"/> class.
        /// </summary>
        /// <param name="relabelParam">
        /// New name (label).
        /// </param>
        public RelabelAttribute(string relabelParam)
        {
            this.RelabelParam = relabelParam;
        }

        /// <summary>
        /// Gets the alternate name.
        /// </summary>
        public string RelabelParam { get; }
    }
}
