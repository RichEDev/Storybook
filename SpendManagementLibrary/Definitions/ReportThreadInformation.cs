namespace SpendManagementLibrary.Definitions
{
    using System;
    using System.Threading;

    /// <summary>
    /// Simplified thread information
    /// </summary>
    [Serializable]
    public class ReportThreadInformation
    {
        public string ThreadKey { get; set; }

        public string Name { get; set; }

        public ThreadState State { get; set; }

        public ThreadPriority Priority { get; set; }

        public int ManagedId { get; set; }
    }
}
