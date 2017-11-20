using SpendManagementLibrary;

namespace Spend_Management
{
    public interface IVat
    {
        int accountid { get; }

        void calculateVAT();
        void calculateVehicleJourneyVAT(cSubcat reqsubcat, ref decimal vat, decimal costperunit, decimal fuelcost, decimal total);
    }
}