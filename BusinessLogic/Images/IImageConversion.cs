namespace BusinessLogic.Images
{
    using SELCloud.Utilities;

    /// <summary>
    /// Defines a <see cref="IImageConversion"/> and it's members
    /// </summary>
    public interface IImageConversion
    {
        /// <summary>
        /// Convert the image to a different file type
        /// </summary>
        /// <param name="data">The data of the receipt</param>
        /// <returns>The information of the converted image</returns>
        ImageMemoryConversion AttemptConversion(byte[] data);
    }
}
