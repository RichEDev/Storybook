namespace SELCloud.DataStore
{
    using System;
    using System.IO;
    using Abstract;
    using net.openstack.Core.Domain;
    using net.openstack.Providers.Rackspace;

    /// <summary>
    /// Implements methods to interact with Rackspace Cloud Files.
    /// <para><![CDATA[http://openstacknetsdk.org]]></para>
    /// <para><![CDATA[https://github.com/openstacknetsdk/openstack.net/wiki/Rackspace-Cloud-Files-Code-Samples]]></para>
    /// </summary>
    public class CloudDataStore : FileDataStore
    {
        #region Fields

        /// <summary>
        /// Stores an initialized instance of <see cref="CloudFilesProvider"/> for use throughout the class.
        /// </summary>
        private readonly CloudFilesProvider _cloudFilesProvider;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudDataStore"/> class.
        /// </summary>
        public CloudDataStore(DataStoreConfig config)
            : base(config)
        {
            if (string.IsNullOrWhiteSpace(config.UserName) || string.IsNullOrWhiteSpace(config.ApiKey))
            {
                throw new ArgumentException("Ensure Username and ApiKey are set correctly.");
            }

            var cloudIdentity = new CloudIdentity { Username = config.UserName, APIKey = config.ApiKey };
            this._cloudFilesProvider = new CloudFilesProvider(cloudIdentity);
        }

        #endregion Constructor

        #region Public Methods

        /// <summary>
        /// Creates a container if it does not already exist.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <returns>true if the container was created successfully, otherwise false.</returns>
        public override bool CreateContainer(string containerName)
        {
            ValidateContainerName(containerName);
            ObjectStore createContainerResponse = this._cloudFilesProvider.CreateContainer(containerName);
            return createContainerResponse == ObjectStore.ContainerCreated || createContainerResponse == ObjectStore.ContainerExists;
        }

        /// <summary>
        /// Creates an object using data from a file. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="sourceFilePath">The source file path. Example c:\folder1\folder2\image_name.jpeg</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        public override void CreateObject(string containerName, string sourceFilePath, string destinationFileName)
        {
            ValidateContainerAndSourceFilePath(containerName, sourceFilePath);
            this._cloudFilesProvider.CreateObjectFromFile(containerName, sourceFilePath, destinationFileName.Replace("\\", "/"));
        }

        /// <summary>
        /// Creates an object using data from a file. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        /// <param name="fileData">The actualy file data in a memory stream</param>
        public override void CreateObject(string containerName, string destinationFileName, MemoryStream fileData)
        {
            ValidateContainerName(containerName);
            this._cloudFilesProvider.CreateObject(containerName, fileData, destinationFileName.Replace("\\", "/"));
        }

        /// <summary>
        /// Gets an object, writing the data to the specified <see cref="MemoryStream"/> <paramref name="outputStream"/> .
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="outputStream">The output stream to populate.</param>
        /// <returns>A <see cref="MemoryStream"/> containing the object.</returns>
        public override MemoryStream GetObject(string containerName, string fileName, MemoryStream outputStream)
        {
            ValidateContainerAndFileName(containerName, fileName);
            try
            {
                this._cloudFilesProvider.GetObject(containerName, fileName.Replace("\\", "/"), outputStream);
                if (outputStream.Length == 0)
                {
                    throw new Exception("No item found");
                }
            }
            catch (Exception error)
            {
                // attempt uppercase extension
                var tempExt = Path.GetExtension(fileName);
                fileName = fileName.Replace(tempExt, tempExt.ToUpperInvariant());
                this._cloudFilesProvider.GetObject(containerName, fileName.Replace("\\", "/"), outputStream);
            }
            return outputStream;
        }

        /// <summary>
        /// Gets an object, writing the data to the specified <paramref name="destinationFilePath"/> and the file name set to <paramref name="fileName"/>.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="destinationFilePath">The file path to create the file at.</param>
        public override void GetObject(string containerName, string fileName, string destinationFilePath)
        {
            ValidateContainerAndFileName(containerName, fileName);

            using (Stream outputStream = new MemoryStream())
            {
                try
                {
                    this._cloudFilesProvider.GetObject(containerName, fileName.Replace("\\", "/"), outputStream);
                    if (outputStream.Length == 0)
                    {
                        throw new Exception("No item found");
                    }
                }
                catch (Exception error)
                {
                    // attempt uppercase extension
                    var tempExt = Path.GetExtension(fileName);
                    fileName = fileName.Replace(tempExt, tempExt.ToUpperInvariant());
                    this._cloudFilesProvider.GetObject(containerName, fileName.Replace("\\", "/"), outputStream);
                }

                var bytes = new byte[outputStream.Length];
                outputStream.Seek(0, SeekOrigin.Begin);

                int length = outputStream.Read(bytes, 0, bytes.Length);
                if (length < bytes.Length)
                {
                    Array.Resize(ref bytes, length);
                }

                File.WriteAllBytes(destinationFilePath, bytes);
            }
        }

        /// <summary>
        /// Deletes an object from a container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name. Example image_name.jpeg</param>
        public override void DeleteFile(string containerName, string fileName)
        {
            ValidateContainerAndFileName(containerName, fileName);

            this._cloudFilesProvider.DeleteObject(containerName, fileName.Replace("\\", "/"));
        }

        /// <summary>
        /// Deletes a container and all objects stored in the container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        public override void DeleteContainer(string containerName)
        {
            ValidateContainerName(containerName);

            this._cloudFilesProvider.DeleteContainer(containerName, true);
        }

        #endregion Public Methods

    }
}