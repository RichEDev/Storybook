using SELCloud.DataStore.Abstract;

namespace SELCloud.DataStore
{
    /// <summary>
    /// Defines the configuration properties necessary to instantiate a concrete instance of <see cref="IFileDataStore"/>.
    /// </summary>
    public class DataStoreConfig
    {
        /// <summary>
        /// The type of <see cref="IFileDataStore"/> to instantiate.
        /// </summary>
        public DataStoreType Type { get; set; }

        /// <summary>
        /// The Username if applicable. 
        /// This is usually used in logging into cloud files or a different filesystem.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The API Key to use if applicable.
        /// This is usually used in logging into cloud files or a different filesystem.
        /// </summary>
        public string ApiKey { get; set; }
        
        /// <summary>
        /// The Employee Id of the user upon who's behalf the data store is being accessed.
        /// </summary>
        public int EmployeeId { get; set; }
    }

    /// <summary>
    /// Defines the type of IFileDataStore to create.
    /// </summary>
    public enum DataStoreType
    {
        /// <summary>
        /// The Cloud Files Data Store, using rackspace APIs.
        /// Represents <see cref="CloudDataStore"/>
        /// </summary>
        CloudFiles = 0,

        /// <summary>
        /// The Windows File System store.
        /// Represents <see cref="WindowsFileSystemDataStore"/>
        /// </summary>
        WindowsFileSystem = 1
    }
}
