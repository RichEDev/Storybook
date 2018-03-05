namespace BusinessLogic.Images
{
    using System;
    using System.IO;

    /// <summary>
    /// Defines a <see cref="JpgImageManipulation"/> and it's members
    /// </summary>
    public class JpgImageManipulation : IImageManipulation
    {
        /// <summary>
        /// Removes Exif data from a jpg file.
        /// Initial code from http://www.techmikael.com/2009/07/removing-exif-data-continued.html
        /// </summary>
        /// <param name="inStream">
        /// The steam of data to process.
        /// </param>
        /// <returns>
        /// The file data of the jpg with the Exif data removed.
        /// </returns>
        public string RemoveExifData(Stream inStream)
        {
            using (inStream)
            {
                Stream outStream = new MemoryStream();

                byte[] jpegHeader = new byte[2];
                jpegHeader[0] = (byte)inStream.ReadByte();
                jpegHeader[1] = (byte)inStream.ReadByte();

                //check if it's a jpg file
                if (jpegHeader[0] == 0xff && jpegHeader[1] == 0xd8)
                {
                    this.SkipAppHeaderSection(inStream);
                }

                outStream.WriteByte(0xff);
                outStream.WriteByte(0xd8);

                int readCount;
                byte[] readBuffer = new byte[4096];

                while ((readCount = inStream.Read(readBuffer, 0, readBuffer.Length)) > 0)
                    outStream.Write(readBuffer, 0, readCount);

                using (MemoryStream ms = new MemoryStream())
                {
                    outStream.Position = 0;
                    outStream.CopyTo(ms);
                    var receiptData = Convert.ToBase64String(ms.ToArray());
                    outStream.Dispose();

                    return receiptData;
                }
            }
        }

        /// <summary>
        /// The skip app header section.
        /// </summary>
        /// <param name="inStream">
        /// The in stream.
        /// </param>
        private void SkipAppHeaderSection(Stream inStream)
        {
            byte[] header = new byte[2];
            header[0] = (byte)inStream.ReadByte();
            header[1] = (byte)inStream.ReadByte();

            while (header[0] == 0xff && (header[1] >= 0xe0 && header[1] <= 0xef))
            {
                int exifLength = inStream.ReadByte();
                exifLength = exifLength << 8;
                exifLength |= inStream.ReadByte();

                for (int i = 0; i < exifLength - 2; i++)
                {
                    inStream.ReadByte();
                }
                header[0] = (byte)inStream.ReadByte();
                header[1] = (byte)inStream.ReadByte();
            }
            inStream.Position -= 2; //skip back two bytes
        }
    }
}
