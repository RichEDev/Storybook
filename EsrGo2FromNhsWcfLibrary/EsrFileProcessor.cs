namespace EsrGo2FromNhsWcfLibrary
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;

    using Utilities.Cryptography;

    /// <summary>
    /// The ESR file processor service.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class EsrFileProcessor : IEsrFileProcessor
    {
        /// <summary>
        /// Process a streamed file.
        /// </summary>
        /// <param name="fileInfo">
        /// The file info.
        /// </param>
        /// <returns>
        /// The <see cref="EsrFileResponseInfo"/>.
        /// </returns>
        public EsrFileResponseInfo ProcessStreamedFile(EsrFileRequestInfo fileInfo)
        {
            try
            {
                var fileObject = new FileHeadersAndRows
                {
                    FileId = OperationContext.Current.IncomingMessageHeaders.GetHeader<int>("FileId", "http://software-europe.com/API/2013/02"),
                    FileName = OperationContext.Current.IncomingMessageHeaders.GetHeader<string>("FileName", "http://software-europe.com/API/2013/02")
                };

                var expectedByteCount = OperationContext.Current.IncomingMessageHeaders.GetHeader<long>("FileBytes", "http://software-europe.com/API/2013/02");
                long incomingBytes;

                var fileRows = new List<string>();
                
                var decryptedMemoryStream = new MemoryStream();

                using (var encryptedMemoryStream = new MemoryStream())
                {
                    fileInfo.Stream.CopyTo(encryptedMemoryStream);
                    incomingBytes = encryptedMemoryStream.Length;
                    var streamReader = new StreamReader(encryptedMemoryStream);
                    encryptedMemoryStream.Seek(0, SeekOrigin.Begin);
                    var encryptedFileString = streamReader.ReadToEnd();
                    var decrptedFileString = ExpensesCryptography.Decrypt(encryptedFileString);
                    var stringBytes = System.Text.Encoding.UTF8.GetBytes(decrptedFileString);
                    decryptedMemoryStream.Write(stringBytes, 0, stringBytes.Length);
                    encryptedMemoryStream.Close();
                }

                using (var reader = new EsrStreamReader(decryptedMemoryStream))
                    {
                        decryptedMemoryStream.Seek(0, SeekOrigin.Begin);

                        string row;
                        while ((row = reader.ReadLine()) != null)
                        {
                            fileRows.Add(row);
                        }

                        reader.Close();
                    }

                    decryptedMemoryStream.Close();
                    decryptedMemoryStream.Flush();
                    decryptedMemoryStream.Dispose();

                fileObject.FileRows = fileRows;

                fileInfo.Dispose();
                fileInfo = null;

                if (incomingBytes != expectedByteCount)
                {
                    var logger = new Log();
                    logger.Write(
                        "",
                        "0",
                        0,
                        LogRecord.LogItemTypes.OutboundFileValidationFailed,
                        LogRecord.TransferTypes.EsrOutbound,
                        0,
                        fileObject.FileName,
                        LogRecord.LogReasonType.Error,
                        string.Format("File Transfer Failed. - {0} - Expected Byte Count did not match the received file.", fileObject.FileName),
                        "EsrFileProcessor:ProcessStreamedFile()");
                    return new EsrFileResponseInfo
                    {
                        processed = false,
                        message = "File Transfer Failed. Expected Byte Count did not match received file."
                    };
                }

                Task.Run(() => new EsrRecordProcessor().ProcessEsrFile(fileObject));
                // todo: does fileObject need clearing up here, or does it affect the closure

                return new EsrFileResponseInfo { processed = true, message = "File Transferred Successfully." };
            }
            catch (IOException ioex)
            {
                if (fileInfo != null)
                {
                    fileInfo.Dispose();
                }
                return new EsrFileResponseInfo { processed = false, message = string.Format("File Transfer Failed.\n{0}\n{1}", ioex.Message, ioex.StackTrace) };
            }
            catch (Exception ex)
            {
                if (fileInfo != null)
                {
                    fileInfo.Dispose();
                }
                return new EsrFileResponseInfo { processed = false, message = string.Format("File Transfer Failed.\n{0}\n{1}", ex.Message, ex.StackTrace) };
            }
        }
    }
}
