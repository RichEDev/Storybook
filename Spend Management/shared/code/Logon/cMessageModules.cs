namespace Spend_Management.shared.code.Logon
{

    /// <summary>
    /// Marketing panel module class
    /// </summary>
    public class cMessageModules
    {
        /// <summary>
        /// Constructor for cMessageModules
        /// </summary>
        public cMessageModules(int moduleId, int messageId)
        {
            this.ModuleId = moduleId;
            this.MessageId = messageId;          
        }

       /// <summary>
        /// Gets or sets the moduleID
        /// </summary>
        public int ModuleId { get; }

        /// <summary>
        /// Gets or sets the MessageID
        /// </summary>
        public int MessageId { get; }
    }
}