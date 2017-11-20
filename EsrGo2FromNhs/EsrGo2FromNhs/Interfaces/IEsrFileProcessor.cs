namespace EsrGo2FromNhs.Interfaces
{
    using System.ServiceModel;

    using EsrGo2FromNhs.ESR;

    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract(Namespace="http://localhost/EsrFileProcessingService")]
    public interface IEsrFileProcessor
    {
        [OperationContract]
        EsrFileResponseInfo ProcessStreamedFile(EsrFileRequestInfo stream);
    }
}
