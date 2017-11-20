namespace SELCloud.DataStore
{
    using System.Configuration;
    using System.IO;
    using System.Threading.Tasks;
    using Abstract;
    
    /// <summary>
    /// Implements methods to interact with the Windows file system.
    /// </summary>
    public class WindowsFileSystemDataStore : FileDataStore
    {
        private readonly string rootPath;

        public WindowsFileSystemDataStore(DataStoreConfig config) : base(config)
        {
            rootPath = ConfigurationManager.AppSettings["DataStoreFileSystemModeRootPath"] ?? "";
        }

        /// <summary>
        /// Creates a directory if it does not already exist. 
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <returns>true if the container was created successfully or already exists, otherwise false.</returns>
        public override bool CreateContainer(string containerName)
        {
            ValidateContainerName(containerName);

            if (Directory.Exists(rootPath + containerName) == false)
            {
                Directory.CreateDirectory(rootPath + containerName);
            }

            return true;
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

            string targetPath = Path.Combine(rootPath, containerName, destinationFileName);
            string directory = Path.GetDirectoryName(targetPath);

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            DeleteFileIfExists(targetPath);

            File.WriteAllBytes(targetPath, fileData.ToArray());
        }

        /// <summary>
        /// Creates an object using data from a file. If the destination file already exists, the contents are overwritten.
        /// If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name (the full or relative path on the file system) to move the object to.</param>
        /// <param name="sourceFilePath">The source file path. Example c:\folder1\folder2\image_name.jpeg</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        public override void CreateObject(string containerName, string sourceFilePath, string destinationFileName = null)
        {
            ValidateContainerAndSourceFilePath(containerName, sourceFilePath);

            if (string.IsNullOrWhiteSpace(destinationFileName))
            {
                destinationFileName = Path.GetFileName(sourceFilePath);
            }

            string targetPath = Path.Combine(rootPath, containerName, destinationFileName);

            DeleteFileIfExists(targetPath);
            
            File.Move(sourceFilePath,  targetPath);
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

            string sourcePath = Path.Combine(rootPath, containerName, fileName);

            using (var file = new FileStream(sourcePath, FileMode.Open, FileAccess.Read))
            {
                var bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                outputStream.Write(bytes, 0, (int)file.Length);
            }

            return outputStream;
        }

        /// <summary>
        /// Gets an object, writing the data to the specified <paramref name="destinationFilePath"/> and the file name set to the <paramref name="fileName"/>.
        /// If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name (the full or relative path on the file system) to move the object to.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="destinationFilePath">The file path to create the file at.</param>
        public override void GetObject(string containerName, string fileName, string destinationFilePath)
        {
            ValidateContainerAndFileName(containerName, fileName);

            string sourcePath = Path.Combine(rootPath, containerName, fileName);
            string directory = Path.GetDirectoryName(sourcePath);

            // If the source and target paths are the same then return as no need to move file.
            if (sourcePath == destinationFilePath)
            {
                return;
            }

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            DeleteFileIfExists(destinationFilePath);

            File.Copy(sourcePath, destinationFilePath);
        }

        /// <summary>
        /// Deletes an object from a container.
        /// </summary>
        /// <param name="containerName">The container name (the full or relative path on the file system) to move the object to.</param>
        /// <param name="fileName">The file name. Example image_name.jpeg</param>
        public override void DeleteFile(string containerName, string fileName)
        {
            ValidateContainerAndFileName(containerName, fileName);

            string targetFile = Path.Combine(rootPath, containerName, fileName);

            DeleteFileIfExists(targetFile);
        }

        /// <summary>
        /// Deletes a container and all objects stored in the container.
        /// This is performed asynchronous so may not have completed when the method returns
        /// </summary>
        /// <param name="containerName">The container name (the full or relative path on the file system) to move the object to.</param>
        public override void DeleteContainer(string containerName)
        {
            ValidateContainerName(containerName);

            if (Directory.Exists(rootPath + containerName))
            {
                // Async delete of the directory
                Task.Factory.StartNew(() => Directory.Delete(rootPath + containerName));
            }       
        }


        /// <summary>
        /// Deletes a file if it exists.
        /// </summary>
        /// <param name="targetFile">The full or relative path to the file on the file system</param>
        private static void DeleteFileIfExists(string targetFile)
        {
            if (File.Exists(targetFile))
            {
                File.Delete(targetFile);
            }
        }
    }
}
