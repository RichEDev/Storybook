namespace BusinessLogic.Databases
{
    using System;

    /// <summary>
    /// <see cref="DatabaseServer">DatabaseServer</see> defines a database server
    /// </summary>
    [Serializable]
    public class DatabaseServer : IDatabaseServer
    {
        /// <summary>
        /// Initialises an instance of <see cref="DatabaseServer">DatabaseServer</see>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="hostname"></param>
        public DatabaseServer(int id, string hostname)
        {
            this.Id = id;
            this.Hostname = hostname;
        }
        /// <summary>
        /// The server host name
        /// </summary>
        public string Hostname { get; }
    
        /// <summary>
        /// The server Id
        /// </summary>
        public int Id { get; set; }
    }
}