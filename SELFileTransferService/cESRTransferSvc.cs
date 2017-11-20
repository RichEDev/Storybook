namespace SELFileTransferService
{
    using System;
    using SpendManagementLibrary;
    using SpendManagementLibrary.ESRTransferServiceClasses;
    using Common.Logging;

    public class cESRTransferSvc : MarshalByRefObject , IESRTransfer
    {
        /// <summary>
        /// An instance of <see cref="ILog"/> for logging <see cref="ICache{T,TK}"/> diagnostics and information.
        /// </summary>
        private static readonly ILog Log = new LogFactory<cESRTransferSvc>().GetLogger();

        public cESRFileInfo[] getOutboundData(int DataID)
        {
            if (Log.IsDebugEnabled)
            {
                Log.Debug($"getOutboundData for Data Id {DataID}");
            }
            cOutboundTransfers clsTransfers = new cOutboundTransfers(null);
            return clsTransfers.getOutboundFiles(DataID).ToArray();
        }

        
    }
}
