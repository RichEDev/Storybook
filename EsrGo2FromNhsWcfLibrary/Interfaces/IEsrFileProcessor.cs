namespace EsrGo2FromNhsWcfLibrary.Interfaces
{
    using System.ServiceModel;

    using EsrGo2FromNhsWcfLibrary.ESR;

    [ServiceContract]
    public interface IEsrFileProcessor
    {
        [OperationContract]
        EsrFileResponseInfo ProcessStreamedFile(EsrFileRequestInfo stream);
    }
}
