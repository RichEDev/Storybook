namespace BusinessLogic.Databases
{
    using BusinessLogic.Interfaces;

    public interface IDatabaseServer : IIdentifier<int>
    {
        /// <summary>
        /// The server host name
        /// </summary>
        string Hostname { get; }
    }
}