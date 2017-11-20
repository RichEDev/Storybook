namespace EsrGo2FromNhs.ESR
{
    using System;
    using System.Configuration;
    using System.IO;

    /// <summary>
    /// The ESR file.
    /// </summary>
    public class EsrFile
    {
        /// <summary>
        /// The record delimiter.
        /// </summary>
        public const char RecordDelimiter = '~';

        /// <summary>
        /// The record deletion column ref.
        /// </summary>
        public const int RecordDeletionColumnRef = 1;

        /// <summary>
        /// The record delete length.
        /// </summary>
        public const int RecordDeleteLength = 2;

        #region Private Variables

        /// <summary>
        /// The _trust vpd.
        /// </summary>
        private readonly string trustVpd;

        /// <summary>
        /// The _file name.
        /// </summary>
        private readonly string fileName;

        /// <summary>
        /// The _ESR file.
        /// </summary>
        private readonly Stream esrFile;

        #endregion Private Variables

        /// <summary>
        /// Initialises static members of the <see cref="EsrFile"/> class.
        /// </summary>
        static EsrFile()
        {
            try
            {
                int batch;
                RecordBatchSize = int.TryParse(ConfigurationManager.AppSettings["apiBatchSize"], out batch) ? batch : 50;
            }
            catch (Exception)
            {
                RecordBatchSize = 50;
            }

            try
            {
                int timeout;
                CacheExpiryMinutes = int.TryParse(ConfigurationManager.AppSettings["apiCacheTime"], out timeout)
                                         ? timeout
                                         : 5;
            }
            catch (Exception)
            {
                CacheExpiryMinutes = 5;
            }
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrFile"/> class.
        /// </summary>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="esrFileBinaryStream">
        /// The ESR file binary stream.
        /// </param>
        public EsrFile(string trustVpd, string fileName, Stream esrFileBinaryStream)
        {
            this.trustVpd = trustVpd;
            this.fileName = fileName;
            this.esrFile = esrFileBinaryStream;
        }

        #region Properties

        /// <summary>
        /// Gets the record batch size.
        /// </summary>
        public static int RecordBatchSize { get; private set; }

        /// <summary>
        /// Gets the cache expiry minutes.
        /// </summary>
        public static int CacheExpiryMinutes { get; private set; }

        /// <summary>
        /// Gets the trust VPD.
        /// </summary>
        public string TrustVpd
        {
            get
            {
                return this.trustVpd;
            }
        }

        /// <summary>
        /// Gets the ESR file stream.
        /// </summary>
        public Stream EsrFileStream
        {
            get
            {
                return this.esrFile;
            }
        }

        /// <summary>
        /// Gets the file name.
        /// </summary>
        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        #endregion Properties
    }
}
