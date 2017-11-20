

namespace SpendManagementLibrary.Interfaces.Expedite
{
    using System.Collections.Generic;
    using SpendManagementLibrary.Expedite;
    public interface IManageFunds
    {
        Fund GetFundAvailable(int accountId);
        Fund GetFundLimit(int accountId);
        Fund UpdateFundLimit(int accountId,decimal amount);
        Fund AddFundTransaction(Fund fund);
    }
}
