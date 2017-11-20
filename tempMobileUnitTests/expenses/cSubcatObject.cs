using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace tempMobileUnitTests
{
    internal class cSubcatObject
    {
        public static cSubcat Template(int categoryId, int subcatid = 0, string subcat = default(string), string description = default(string), bool mileageapp = false, bool staffapp = false, bool othersapp = false, bool tipapp = false, bool pmilesapp = false, bool bmilesapp = false, decimal allowanceamount = 0, string accountcode = default(string), bool attendeesapp = false, bool addasnet = false, int pdcatid = 0, bool eventinhomeapp = false, bool receiptapp = false, CalculationType calculation = default(CalculationType), bool passengersapp = false, bool nopassengersapp = false, string comment = default(string), bool splitentertainment = false, int entertainmentid = 0, bool reimbursable = true, bool nonightsapp = false, bool attendeesmand = false, bool nodirectorsapp = false, bool hotelapp = false, bool noroomsapp = false, bool hotelmand = false, bool vatnumberapp = false, bool vatnumbermand = false, bool nopersonalguestsapp = false, bool noremoteworkersapp = false, string alternateaccountcode = default(string), bool splitpersonal = false, bool splitremote = false, int personalid = 0, int remoteid = 0, bool reasonapp = false, bool otherdetailsapp = false, SortedList<int, object> userdefined = null, string shortsubcat = default(string), bool fromapp = false, bool toapp = false, List<cCountrySubcat> countries = null, List<int> allowances = null, List<int> associatedudfs = null, List<int> split = null, bool companyapp = false, List<cSubcatVatRate> vatrates = null, bool enablehometolocationmileage = false, HomeToLocationType hometolocationtype = default(HomeToLocationType), int? mileagecategory = null, bool isrelocationmileage = false, int? reimbursablesubcatid = null, bool allowHeavyBulkyMileage = false)
        {
            if(categoryId == 0)
                throw new Exception("CategoryId must have a valid ID");

            ICurrentUser currentUser = Moqs.CurrentUser();

            subcat = (subcat == default(string) ? "Unit Test dummy Item" : subcat);
            description = (description == default(string) ? "Unit Test dummy Item" : description);
            accountcode = (accountcode == default(string) ? "UnitTest01" : accountcode);
            calculation = (calculation == default(CalculationType) ? CalculationType.NormalItem : calculation);
            comment = (comment == default(string) ? "Used for Unit Tests" : comment);
            shortsubcat = (shortsubcat == default(string) ? "Unit Test Normal" : shortsubcat);
            countries = countries ?? new List<cCountrySubcat>();
            allowances = allowances ?? new List<int>();
            associatedudfs = associatedudfs ?? new List<int>();
            split = split ?? new List<int>();
            vatrates = vatrates ?? new List<cSubcatVatRate>();
            hometolocationtype = (hometolocationtype == default(HomeToLocationType) ? HomeToLocationType.None : hometolocationtype);
            alternateaccountcode = (alternateaccountcode == default(string) ? "" : alternateaccountcode);

            cSubcat retSubcat = new cSubcat(subcatid, categoryId, subcat, description, mileageapp, staffapp, othersapp, tipapp, pmilesapp, bmilesapp, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, eventinhomeapp, receiptapp, calculation, passengersapp, nopassengersapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, reasonapp, otherdetailsapp, userdefined, DateTime.UtcNow, currentUser.EmployeeID, null, null, shortsubcat, fromapp, toapp, countries, allowances, associatedudfs, split, companyapp, vatrates, enablehometolocationmileage, hometolocationtype, mileagecategory, isrelocationmileage, reimbursablesubcatid, allowHeavyBulkyMileage);

            return retSubcat;
        }

        public static cSubcat New(cSubcat subcat)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cSubcats clsSubcats = new cSubcats(currentUser.AccountID);

            int tempSubcatID = clsSubcats.saveSubcat(subcat);
            
            clsSubcats = new cSubcats(currentUser.AccountID);
            cSubcat newSubcat = clsSubcats.getSubcatById(tempSubcatID);

            return newSubcat;
        }

        public static void TearDown(int subcatId)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            cSubcats clsSubcats = new cSubcats(currentUser.AccountID);

            clsSubcats.deleteSubcat(subcatId);
        }
    }
}
