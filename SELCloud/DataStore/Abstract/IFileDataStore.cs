using System.IO;

namespace SELCloud.DataStore.Abstract
{
    /// <summary>
    /// The FileDataStore interface.
    /// </summary>
    public interface IFileDataStore
    {
        /// <summary>
        /// Creates a container if it does not already exist.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <returns>true if the container was created successfully, otherwise false.</returns>
        bool CreateContainer(string containerName);

        /// <summary>
        /// Creates an object using data from a file. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="sourceFilePath">The source file path. Example c:\folder1\folder2\image_name.jpeg</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        void CreateObject(string containerName, string sourceFilePath, string destinationFileName);

        /// <summary>
        /// Creates an object using data from a stream. If the destination file already exists, the contents are overwritten.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="destinationFileName">The destination object name. If null, the file name portion of filePath will be used.</param>
        /// <param name="fileData">The actualy file data in a memory stream</param>
        void CreateObject(string containerName, string destinationFileName, MemoryStream fileData);

        /// <summary>
        /// Gets an object, writing the data to the specified <see cref="MemoryStream"/> <paramref name="outputStream"/> .
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="outputStream">The output stream to populate.</param>
        /// <returns>A <see cref="MemoryStream"/> containing the object.</returns>
        MemoryStream GetObject(string containerName, string fileName, MemoryStream outputStream);

        /// <summary>
        /// Gets an object, writing the data to the specified <paramref name="destinationFilePath"/> and the file name set to the <paramref name="fileName"/>.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name to get. Example image_name.jpeg</param>
        /// <param name="destinationFilePath">The file path to create the file at.</param>
        void GetObject(string containerName, string fileName, string destinationFilePath);

        /// <summary>
        /// Deletes an object from a container.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        /// <param name="fileName">The file name. Example image_name.jpeg</param>
        void DeleteFile(string containerName, string fileName);

        /// <summary>
        /// Deletes a container and all contents.
        /// </summary>
        /// <param name="containerName">The container name.</param>
        void DeleteContainer(string containerName);
    }
}
