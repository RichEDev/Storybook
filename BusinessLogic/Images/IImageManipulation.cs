namespace BusinessLogic.Images
{
    using System.IO;

    /// <summary>
    /// Defines a <see cref="IImageManipulation"/> and it's members
    /// </summary>
    public interface IImageManipulation
    {
        /// <summary>
        /// Removes Exif data from a file.
        /// Initial code from http://www.techmikael.com/2009/07/removing-exif-data-continued.html
        /// </summary>
        /// <param name="inStream">
        /// The steam of data to process.
        /// </param>
        /// <returns>
        /// The file data of the jpg with the Exif data removed.
        /// </returns>
        string RemoveExifData(Stream inStream);
    }
}
