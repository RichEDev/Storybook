namespace BusinessLogic.ISELCloud
{
    using SELCloud.Utilities;

    /// <summary>
    /// Defines a <see cref="ICloudReceipt"/> and all it's members
    /// </summary>
    public interface ICloudReceipt
    {
        /// <summary>
        /// Gets a receipt from cloud
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="containerName">Folder the file is in</param>
        /// <returns>The data of te receipt</returns>
        string Get(string fileName, string containerName);

        /// <summary>
        /// Delete a receipt from the cloud
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="containerName">Folder the file is in</param>
        void Delete(string fileName, string containerName);

        /// <summary>
        /// Save a receipt to the cloud 
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="conversion">Converted file</param>
        /// <param name="containerName">Folder the file is in</param>
        void Save(string fileName, ImageMemoryConversion conversion, string containerName);
    }
}
