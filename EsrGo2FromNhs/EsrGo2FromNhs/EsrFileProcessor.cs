namespace EsrGo2FromNhs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;

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

                long expectedByteCount = OperationContext.Current.IncomingMessageHeaders.GetHeader<long>("FileBytes", "http://software-europe.com/API/2013/02");
                long incomingBytes = 0;

                var fileRows = new List<string>();

                using (var memoryStream = new MemoryStream())
                {
                    fileInfo.Stream.CopyTo(memoryStream);

                    memoryStream.Seek(0, SeekOrigin.Begin);

                    using (var reader = new StreamReader(memoryStream))
                    {
                        incomingBytes = memoryStream.Length;
                        string row;
                        while ((row = reader.ReadLine()) != null)
                        {
                            fileRows.Add(row);
                        }

                        reader.Close();
                    }

                    memoryStream.Close();
                }

                fileObject.FileRows = fileRows;

                fileInfo.Dispose();
                fileInfo = null;

                if (incomingBytes != expectedByteCount)
                {
                    var logger = new Log();
                    logger.Write(
                        "",
                        0,
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

                return new EsrFileResponseInfo() { processed = true, message = "File Transferred Successfully." };
            }
            catch (IOException ioex)
            {
                if (fileInfo != null)
                {
                    fileInfo.Dispose();
                }
                return new EsrFileResponseInfo() { processed = false, message = string.Format("File Transfer Failed.\n{0}\n{1}", ioex.Message, ioex.StackTrace) };
            }
            catch (Exception ex)
            {
                if (fileInfo != null)
                {
                    fileInfo.Dispose();
                }
                return new EsrFileResponseInfo() { processed = false, message = string.Format("File Transfer Failed.\n{0}\n{1}", ex.Message, ex.StackTrace) };
            }
        }
    }
}
