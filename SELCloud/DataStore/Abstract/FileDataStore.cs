namespace SELCloud.DataStore.Abstract
{
    using System;
    using System.IO;

    /// <summary>
    /// FileDataStore is abstract. Use or create a concrete subclass by calling <see cref="Get"/>, 
    /// passing in a suitable <see cref="DataStoreConfig"/>.
    /// </summary>
    public abstract class FileDataStore : IFileDataStore
    {
        #region Fields

        public DataStoreConfig Config { get; protected set; }

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Pass the config through to this method from a concrete subclass in order to set the config.
        /// </summary>
        /// <param name="config">The <see cref="DataStoreConfig"/> instance that this Data Store is locked to.</param>
        protected FileDataStore(DataStoreConfig config)
        {
            Config = config;
        }

        #endregion Constructor
        
        #region Public Static Methods

        /// <summary>
        /// Gets an implementation of <see cref="IFileDataStore"/> depending on what the configuration file states.
        /// </summary>
        /// <returns>An implementation of <see cref="IFileDataStore"/>.</returns>
        public static FileDataStore Get(DataStoreConfig config)
        {
            switch (config.Type)
            {
                case DataStoreType.CloudFiles: return new CloudDataStore(config);
                case DataStoreType.WindowsFileSystem: return new WindowsFileSystemDataStore(config);
            }

            throw new ArgumentException("Ensure that the config.Type property (DataStoreType) is set to a valid value.");
        }
        
        #endregion Public Static Methods

        #region Public Abstract Methods To Override

        /// <summary>
        /// Creates a container if it does not already exist.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <returns>true if the container was created successfully, otherwise false.</returns>
        public abstract bool CreateContainer(string containerName);

        /// <summary>
        /// Creates an object using data from a stream. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        /// <param name="fileData">The actualy file data in a memory stream</param>
        public abstract void CreateObject(string containerName, string destinationFileName, MemoryStream fileData);

        /// <summary>
        /// Creates an object using data from a file. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="sourceFilePath">The source file path. Example c:\folder1\folder2\image_name.jpeg</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        public abstract void CreateObject(string containerName, string sourceFilePath, string destinationFileName);

        /// <summary>
        /// Gets an object, writing the data to the specified <see cref="MemoryStream"/>: <paramref name="outputStream"/> .
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="outputStream">The output stream to populate.</param>
        /// <returns>A <see cref="MemoryStream"/> containing the object.</returns>
        public abstract MemoryStream GetObject(string containerName, string fileName, MemoryStream outputStream);

        /// <summary>
        /// Gets an object, writing the data to the specified <paramref name="destinationFilePath"/> and the file name set to the <paramref name="fileName"/>.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="destinationFilePath">The file path to create the file at.</param>
        public abstract void GetObject(string containerName, string fileName, string destinationFilePath);

        /// <summary>
        /// Deletes an object from a container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name. Example image_name.jpeg</param>
        public abstract void DeleteFile(string containerName, string fileName);

        /// <summary>
        /// Deletes a container and all contents.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        public abstract void DeleteContainer(string containerName);

        #endregion Public Abstract Methods To Override

        #region Protected Methods

        /// <summary>
        /// Validates a containerName and fileName and sourceFilePath arguments. Here to adhere to DRY principles.
        /// </summary>
        /// <param name="containerName">The containerName property.</param>
        /// <param name="sourceFilePath">The sourceFilePath property.</param>
        protected void ValidateContainerAndSourceFilePath(string containerName, string sourceFilePath)
        {
            ValidateContainerName(containerName);
            ValidateSourceFilePath(sourceFilePath);
        }

        /// <summary>
        /// Validates a containerName and fileName arguments. Here to adhere to DRY principles.
        /// </summary>
        /// <param name="containerName">The containerName property.</param>
        /// <param name="fileName">The fileName property.</param>
        protected void ValidateContainerAndFileName(string containerName, string fileName)
        {
            ValidateContainerName(containerName);
            ValidateFileName(fileName);
        }

        /// <summary>
        /// Validates a fileName argument. Here to adhere to DRY principles.
        /// </summary>
        /// <param name="fileName">The fileName property.</param>
        protected void ValidateFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                throw new ArgumentException("fileName cannot be null or empty", "fileName");
            }
        }

        /// <summary>
        /// Validates a containerName argument. Here to adhere to DRY principles.
        /// </summary>
        /// <param name="containerName">The containerName property.</param>
        protected void ValidateContainerName(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
            {
                throw new ArgumentException("containerName cannot be null or empty", "containerName");
            }
        }

        /// <summary>
        /// Validates a sourceFilePath argument. Here to adhere to DRY principles.
        /// </summary>
        /// <param name="sourceFilePath">The sourceFilePath property.</param>
        protected void ValidateSourceFilePath(string sourceFilePath)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
            {
                throw new ArgumentException("sourceFilePath cannot be null or empty", "sourceFilePath");
            }
        }

        #endregion Protected Methods
    }
}
