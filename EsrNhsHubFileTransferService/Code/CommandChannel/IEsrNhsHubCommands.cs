namespace EsrNhsHubFileTransferService.Code.CommandChannel
{
    using System.ServiceModel;

    using EsrGo2FromNhsWcfLibrary.Enum;

    [ServiceContract]
    interface IEsrNhsHubCommands
    {
        [OperationContract]
        bool FlagFileForNewTransfer(int fileId, string fileName);

        [OperationContract]
        bool SuccessfullyProcessedFile(int fileId, string fileName);

        [OperationContract]
        bool FailedToProcessFile(int fileId, string fileName, EsrHubStatus.EsrHubTransferStatus status);
    }
}
