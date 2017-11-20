namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System.IO;
    using System.Text;

    /// <summary>
    /// An enhanced version of stream reader, targetting esr outbound files
    /// </summary>
    class EsrStreamReader : StreamReader
    {
        public EsrStreamReader(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Reads to a line feed 0A character rather than a carriage return. 
        /// </summary>
        /// <returns>A string representing a file row.</returns>
        public override string ReadLine()
        {
            int characterNumber = this.Read();

            if (characterNumber == -1)
            {
                return null;
            }

            var lineBuilder = new StringBuilder();

            do
            {
                var character = (char)characterNumber;

                if (character == '\n')
                {
                    return lineBuilder.ToString();
                }

                lineBuilder.Append(character);
            }
            while ((characterNumber = this.Read()) != -1);

            return lineBuilder.ToString();
        }
    }

}
