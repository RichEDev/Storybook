namespace BusinessLogic.ISELCloud
{
    using System;
    using System.IO;

    using SELCloud.Utilities;

    /// <summary>
    /// Defines a <see cref="CloudWalletReceipt"/> and all it's members
    /// </summary>
    public class CloudWalletReceipt : ICloudReceipt
    {
        /// <summary>
        /// Gets a receipt from cloud
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="containerName">Folder the file is in</param>
        /// <returns>The data of the receipt</returns>
        public string Get(string fileName, string containerName)
        {
            using (var stream = new MemoryStream())
            {
                SELCloud.SELCloud.Instance.GetObject(containerName, $@"Receipts\{fileName}", stream);
                return Convert.ToBase64String(stream.GetBuffer(), Base64FormattingOptions.None);
            }
        }

        /// <summary>
        /// Delete a receipt from the cloud
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="containerName">Folder the file is in</param>
        public void Delete(string fileName, string containerName)
        {
            SELCloud.SELCloud.Instance.DeleteFile(containerName, @"Receipts\" + fileName);
        }

        /// <summary>
        /// Save a receipt to the cloud
        /// </summary>
        /// <param name="fileName">Name of the file</param>
        /// <param name="conversion">Converted file</param>
        /// <param name="containerName">Folder the file is in</param>
        public void Save(string fileName, ImageMemoryConversion conversion, string containerName)
        {
            try
            {
                // generate the container (if not exists)
                SELCloud.SELCloud.Instance.CreateContainer(containerName);

                // attempt to save the file in the data store
                SELCloud.SELCloud.Instance.CreateObject(containerName, $@"Receipts\{fileName}", conversion.ConvertedFile);
            }
            catch (IOException)
            {
                // roll back receipt attachment
                this.Delete(fileName, containerName);
            }
        }
    }
}
