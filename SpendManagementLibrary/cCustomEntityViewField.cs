namespace SpendManagementLibrary
{
    using SpendManagementLibrary.Definitions.JoinVia;

    /// <summary>
    /// View field with unique guid to use when a custom join is needed and joinVia is populated
    /// </summary>
    public class cCustomEntityViewField
    {
        /// <summary>
        /// The field to display in the view
        /// </summary>
        public cField Field { get; set; }
        /// <summary>
        /// A list of join table/field guids that the field was picked via
        /// </summary>
        public JoinVia JoinVia { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="field">The field to display on the view</param>
        /// <param name="joinVia">Populated if the field is from a joined table (from MtO relationship(s))</param>
        public cCustomEntityViewField(cField field, JoinVia joinVia = null)
        {
            this.Field = field;
            this.JoinVia = joinVia;
        }
    }
}