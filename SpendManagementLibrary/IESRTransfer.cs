using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary.ESRTransferServiceClasses;

namespace SpendManagementLibrary
{
    public interface IESRTransfer
    {
        cESRFileInfo[] getOutboundData(int DataID);
    }
}
