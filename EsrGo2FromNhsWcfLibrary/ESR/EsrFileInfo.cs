namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.IO;
    using System.ServiceModel;

    [MessageContract]
    public class EsrFileRequestInfo : IDisposable
    {
        [MessageHeader(Namespace = "http://software-europe.com/API/2013/02")]
        private int FileId { get; set; }

        [MessageHeader(Namespace = "http://software-europe.com/API/2013/02")]
        private string FileName { get; set; }

        /// <summary>
        /// Gets or sets the file bytes in the transmitted file.
        /// </summary>
        [MessageHeader(Namespace = "http://software-europe.com/API/2013/02")]
        private long FileBytes { get; set; }

        [MessageBodyMember(Order = 1)]
        public Stream Stream { get; set; }

        public void Dispose()
        {
            if (this.Stream != null)
            {
                this.Stream.Dispose();
                this.Stream = null;    
            }
        }
    }

    [MessageContract]
    public class EsrFileResponseInfo
    {
        [MessageBodyMember(Order = 1)]
        public bool processed { get; set; }

        [MessageBodyMember(Order = 2)]
        public string message { get; set; }
    }
}
