namespace SELCloud.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Contains image converstion utilities.
    /// </summary>
    public static class ImageConvertor
    {
        private static readonly Dictionary<string, string> MimeTypes = new Dictionary<string, string> { { ".png", "image/png" }, { ".gif", "image/gif" }, { ".bmp", "image/bmp" } };

        /// <summary>
        /// Converts a file to a JPG, using MemoryStreams, if the type of the file is an image type. 
        /// Otherwise, the file is unaltered. Use the <see cref="ImageMemoryConversion.ConvertedFile"/> 
        /// property regardless of whether the conversion happened.
        /// </summary>
        /// <param name="inputStream">The stream to do the conversion on.</param>
        /// <param name="outputStream">The output stream.</param>
        /// <param name="quality">The qualitiy parameter to pass to the conversion.</param>
        /// <returns>An <see cref="ImageFileConversion"/> contianing the necessary information.</returns>
        public static ImageMemoryConversion ImageToJpg(MemoryStream inputStream, MemoryStream outputStream, long quality)
        {
            // attempt to read the content type (this closes the stream)
            var copy = new MemoryStream();
            inputStream.CopyTo(copy);
            copy.Position = inputStream.Position = 0;
            ImageFormat contentType = GetContentType(copy);

            if (contentType == null)
            {
                outputStream = inputStream;
            }
            else
            {
                var codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == "image/jpeg");

                if (codec == null)
                {
                    throw new InvalidOperationException("Unable to find the mime type to perform this operation.");
                }

                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                var sourceImage = new Bitmap(inputStream);
                sourceImage.Save(outputStream, codec, encoderParams);
            }

            inputStream.Position = outputStream.Position = 0;
            return new ImageMemoryConversion(inputStream, outputStream);
        }

        /// <summary>
        /// Converts an image to a JPG, using Files on disk.
        /// </summary>
        /// <param name="sourceImagePath">The path of the source file.</param>
        /// <param name="saveToPath">The path to create the new image.</param>
        /// <param name="quality">The qualitiy parameter to pass to the conversion.</param>
        /// <returns>An <see cref="ImageFileConversion"/> contianing the necessary information.</returns>
        public static ImageFileConversion ImageToJpg(string sourceImagePath, string saveToPath, long quality)
        {
            string mimeType = null;

            string fileExtension = Path.GetExtension(sourceImagePath);

            if (string.IsNullOrWhiteSpace(fileExtension) == false)
            {
                MimeTypes.TryGetValue(fileExtension, out mimeType);
            }

            if (string.IsNullOrWhiteSpace(mimeType) == false)
            {

                ImageCodecInfo codec = ImageCodecInfo.GetImageEncoders().FirstOrDefault(t => t.MimeType == "image/jpeg");

                if (codec == null)
                {
                    throw new InvalidOperationException("Unable to find the mime type to perform this operation.");
                }

                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                var sourceImage = new Bitmap(sourceImagePath);

                string directoryPath = Path.GetDirectoryName(saveToPath);

                Directory.CreateDirectory(directoryPath);

                sourceImage.Save(saveToPath, codec, encoderParams);

                return new ImageFileConversion(new FileInfo(sourceImagePath), new FileInfo(saveToPath));
            }

            return null;
        }

        private static ImageFormat GetContentType(MemoryStream stream)
        {
            using (var binaryReader = new BinaryReader(stream))
            {
                int maxMagicBytesLength = ImageFormatDecoders.Keys.OrderByDescending(x => x.Length).First().Length;

                var magicBytes = new byte[maxMagicBytesLength];

                for (int i = 0; i < maxMagicBytesLength; i += 1)
                {
                    magicBytes[i] = binaryReader.ReadByte();

                    foreach (var kvPair in ImageFormatDecoders)
                    {
                        if (magicBytes.StartsWith(kvPair.Key))
                        {
                            return kvPair.Value;
                        }
                    }
                }

                return null;
            }
        }

        private static bool StartsWith(this byte[] thisBytes, byte[] thatBytes)
        {
            for (int i = 0; i < thatBytes.Length; i += 1)
            {
                if (thisBytes[i] != thatBytes[i])
                {
                    return false;
                }
            }
            return true;
        }

        private static readonly Dictionary<byte[], ImageFormat> ImageFormatDecoders = new Dictionary<byte[], ImageFormat>()
        {
            { new byte[]{ 0x42, 0x4D }, ImageFormat.Bmp},
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x37, 0x61 }, ImageFormat.Gif },
            { new byte[]{ 0x47, 0x49, 0x46, 0x38, 0x39, 0x61 }, ImageFormat.Gif },
            { new byte[]{ 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }, ImageFormat.Png },
            { new byte[]{ 0xff, 0xd8 }, ImageFormat.Jpeg },
        };
    }

    /// <summary>
    /// Represents the information from a file conversion for memory streams.
    /// </summary>
    public class ImageMemoryConversion : BaseFileConversion
    {
        /// <summary>
        /// The source MemoryStream.
        /// </summary>
        public MemoryStream SourceFile { get; private set; }

        /// <summary>
        /// The converted MemoryStream. 
        /// </summary>
        public MemoryStream ConvertedFile { get; private set; }

        /// <summary>
        /// Creates a new ImageMemoryConversion.
        /// </summary>
        /// <param name="sourceImage">The source MemoryStream.</param>
        /// <param name="convertedFile">The converted MemoryStream. </param>
        public ImageMemoryConversion(MemoryStream sourceImage, MemoryStream convertedFile)
        {
            this.SourceFile = sourceImage;
            this.ConvertedFile = convertedFile;
            this.ConvertedDifference = convertedFile.Length - sourceImage.Length;
            this.ConvertedDifferenceHuman = Size(sourceImage.Length - convertedFile.Length);
        }
    }

    /// <summary>
    /// Represents the information from a file conversion for files on disk.
    /// </summary>
    public class ImageFileConversion : BaseFileConversion
    {
        /// <summary>
        /// The source image FileInfo.
        /// </summary>
        public FileInfo SourceFile { get; private set; }

        /// <summary>
        /// The converted image ImageFileConversion.
        /// </summary>
        public FileInfo ConvertedFile { get; private set; }

        /// <summary>
        /// Creates a new ImageMemoryConversion.
        /// </summary>
        /// <param name="sourceImage">The source FileInfo.</param>
        /// <param name="convertedImage">The converted FileInfo. </param>
        public ImageFileConversion(FileInfo sourceImage, FileInfo convertedImage)
        {
            this.SourceFile = sourceImage;
            this.ConvertedFile = convertedImage;
            this.ConvertedDifference = convertedImage.Length - sourceImage.Length;
            this.ConvertedDifferenceHuman = Size(sourceImage.Length - convertedImage.Length);
        }
    }

    /// <summary>
    /// Represents the base Conversion information for an image conversion.
    /// </summary>
    public abstract class BaseFileConversion
    {
        /// <summary>
        /// The difference in sizes between the compressed and the original.
        /// </summary>
        public long ConvertedDifference { get; internal set; }

        /// <summary>
        /// The difference in sizes between the compressed and the original, in a human.
        /// </summary>
        public string ConvertedDifferenceHuman { get; internal set; }

        /// <summary>
        /// Gets the size in human readable format.
        /// </summary>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string Size(double len)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return string.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}