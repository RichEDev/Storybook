namespace ApiClientHelper.Models
{
    /// <summary>
    /// A class to define file information to conpare the current and previous file size.
    /// </summary>
    public class FileInformation
    {
        /// <summary>
        /// Create a new instance of <see cref="FileInformation"/>
        /// </summary>
        /// <param name="fileName">The file name of the file</param>
        /// <param name="previousFileSize">The previous size (in bytes) of the file</param>
        /// <param name="fileSize">The current size (in bytes) of the file</param>
        public FileInformation(string fileName, int previousFileSize, int fileSize)
        {
            this.PreviousFileSize = previousFileSize;
            this.FileSize = fileSize;
            this.FileName = fileName;
        }

        /// <summary>
        /// Gets the filename of this file
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the current file size (in bytes) of the file
        /// </summary>
        public int FileSize { get; private set; }

        /// <summary>
        /// Gets the previous file size (in bytes)
        /// </summary>
        public int PreviousFileSize { get; private set; }
    }
}