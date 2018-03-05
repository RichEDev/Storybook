namespace BusinessLogic.Images
{
    using System.IO;

    using SELCloud.Utilities;

    /// <summary>
    /// Defines a <see cref="JpgImageConversion"/> and it's members
    /// </summary>
    public class JpgImageConversion : IImageConversion
    {
        /// <summary>
        /// Convert the image to a jpg
        /// </summary>
        /// <param name="data">The data of the receipt</param>
        /// <returns>The information of the converted image</returns>
        public ImageMemoryConversion AttemptConversion(byte[] data)
        {
            // create a memory stream of the data to push to the data store
            var memoryStream = new MemoryStream(data, 0, data.Length);

            // convert receipt to jpeg (if it is of an image type)
            var conversion = ImageConvertor.ImageToJpg(memoryStream, new MemoryStream(), 80);

            return conversion;
        }
    }
}
