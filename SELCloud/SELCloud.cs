using System.Configuration;
using System.IO;
using SELCloud.DataStore;
using SELCloud.DataStore.Abstract;

namespace SELCloud
{
    /// <summary>
    /// The Singleton SELCloud is for use all over the solution. Multiple versions cannot be created.
    /// Mananges operations on Cloud files or the windows file system, depending on the configuration.
    /// </summary>
    public sealed class SELCloud : IFileDataStore
    {
        /// <summary>
        /// Gets the data store for the application. Initialised on constructor, pointing to cloud files.
        /// </summary>
        private static FileDataStore DataStore { get; set; }

        /// <summary>
        /// Private static instance reference.
        /// </summary>
        private static SELCloud _instance;

        /// <summary>
        /// Private static instance reference.
        /// </summary>
        public static SELCloud Instance
        {
            get { return _instance ?? (_instance = new SELCloud()); }
        }


        /// <summary>
        /// Private constructor - this is a singleton.
        /// </summary>
        private SELCloud()
        {
            // configure the data store storage settings
            var isCloud = ConfigurationManager.AppSettings["DataStoreMode"] == "cloudfiles";
            var storageConfig = new DataStoreConfig
            {
                Type = isCloud ? DataStoreType.CloudFiles : DataStoreType.WindowsFileSystem,
                UserName = isCloud ? ConfigurationManager.AppSettings["RackSpaceCloudUsername"] : null,
                ApiKey = isCloud ? ConfigurationManager.AppSettings["RackSpaceCloudApiKey"] : null,
            };

            // create the data store
            DataStore = FileDataStore.Get(storageConfig);
        }

        /// <summary>
        /// Creates a container if it does not already exist.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <returns>true if the container was created successfully, otherwise false.</returns>
        public bool CreateContainer(string containerName)
        {
            return DataStore.CreateContainer(containerName);
        }

        /// <summary>
        /// Creates an object using data from a file. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="sourceFilePath">The source file path. Example c:\folder1\folder2\image_name.jpeg</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        public void CreateObject(string containerName, string sourceFilePath, string destinationFileName)
        {
            DataStore.CreateObject(containerName, sourceFilePath, destinationFileName);
        }

        /// <summary>
        /// Creates an object using data from a stream. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        /// <param name="fileData">The actualy file data in a memory stream</param>
        public void CreateObject(string containerName, string destinationFileName, MemoryStream fileData)
        {
            DataStore.CreateObject(containerName, destinationFileName, fileData);
        }

        /// <summary>
        /// Gets an object, writing the data to the specified <see cref="MemoryStream"/> <paramref name="outputStream"/> .
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="outputStream">The output stream to populate.</param>
        /// <returns>A <see cref="MemoryStream"/> containing the object.</returns>
        public MemoryStream GetObject(string containerName, string fileName, MemoryStream outputStream)
        {
            return DataStore.GetObject(containerName, fileName, outputStream);
        }

        /// <summary>
        /// Gets an object, writing the data to the specified <paramref name="destinationFilePath"/> and the file name set to the <paramref name="fileName"/>.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="destinationFilePath">The file path to create the file at.</param>
        public void GetObject(string containerName, string fileName, string destinationFilePath)
        {
            DataStore.GetObject(containerName, fileName, destinationFilePath);
        }

        /// <summary>
        /// Deletes an object from a container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name. Example image_name.jpeg</param>
        public void DeleteFile(string containerName, string fileName)
        {
            DataStore.DeleteFile(containerName, fileName);
        }

        /// <summary>
        /// Deletes a container and all contents.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        public void DeleteContainer(string containerName)
        {
            DataStore.DeleteContainer(containerName);
        }
    }

}
