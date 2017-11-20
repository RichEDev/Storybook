namespace ApiClientHelper.Models
{
    /// <summary>
    /// A class that defines the content of a file.
    /// </summary>
    public class FileDataResponse
    {
        /// <summary>
        /// A 64bit encoded string of the file content
        /// </summary>
        public string FileContent { get; }

        /// <summary>
        /// Create a new instance of <see cref="FileDataResponse"/>
        /// </summary>
        /// <param name="fileContent"></param>
        public FileDataResponse(string fileContent)
        {
            this.FileContent = fileContent;
        }
    }
}