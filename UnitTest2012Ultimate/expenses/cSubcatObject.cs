namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Spend_Management;
    using SpendManagementLibrary;

    /// <summary>
    /// The subcat object.
    /// </summary>
    internal class SubcatObject
    {
        /// <summary>
        /// The template.
        /// </summary>
        /// <param name="categoryId">
        /// The category id.
        /// </param>
        /// <param name="subcatid">
        /// The subcatid.
        /// </param>
        /// <param name="subcat">
        /// The subcat.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="mileageapp">
        /// The mileageapp.
        /// </param>
        /// <param name="staffapp">
        /// The staffapp.
        /// </param>
        /// <param name="othersapp">
        /// The othersapp.
        /// </param>
        /// <param name="tipapp">
        /// The tipapp.
        /// </param>
        /// <param name="pmilesapp">
        /// The pmilesapp.
        /// </param>
        /// <param name="bmilesapp">
        /// The bmilesapp.
        /// </param>
        /// <param name="allowanceamount">
        /// The allowanceamount.
        /// </param>
        /// <param name="accountcode">
        /// The accountcode.
        /// </param>
        /// <param name="attendeesapp">
        /// The attendeesapp.
        /// </param>
        /// <param name="addasnet">
        /// The addasnet.
        /// </param>
        /// <param name="pdcatid">
        /// The pdcatid.
        /// </param>
        /// <param name="eventinhomeapp">
        /// The eventinhomeapp.
        /// </param>
        /// <param name="receiptapp">
        /// The receiptapp.
        /// </param>
        /// <param name="calculation">
        /// The calculation.
        /// </param>
        /// <param name="passengersapp">
        /// The passengersapp.
        /// </param>
        /// <param name="nopassengersapp">
        /// The nopassengersapp.
        /// </param>
        /// <param name="comment">
        /// The comment.
        /// </param>
        /// <param name="splitentertainment">
        /// The splitentertainment.
        /// </param>
        /// <param name="entertainmentid">
        /// The entertainmentid.
        /// </param>
        /// <param name="reimbursable">
        /// The reimbursable.
        /// </param>
        /// <param name="nonightsapp">
        /// The nonightsapp.
        /// </param>
        /// <param name="attendeesmand">
        /// The attendeesmand.
        /// </param>
        /// <param name="nodirectorsapp">
        /// The nodirectorsapp.
        /// </param>
        /// <param name="hotelapp">
        /// The hotelapp.
        /// </param>
        /// <param name="noroomsapp">
        /// The noroomsapp.
        /// </param>
        /// <param name="hotelmand">
        /// The hotelmand.
        /// </param>
        /// <param name="vatnumberapp">
        /// The vatnumberapp.
        /// </param>
        /// <param name="vatnumbermand">
        /// The vatnumbermand.
        /// </param>
        /// <param name="nopersonalguestsapp">
        /// The nopersonalguestsapp.
        /// </param>
        /// <param name="noremoteworkersapp">
        /// The noremoteworkersapp.
        /// </param>
        /// <param name="alternateaccountcode">
        /// The alternateaccountcode.
        /// </param>
        /// <param name="splitpersonal">
        /// The splitpersonal.
        /// </param>
        /// <param name="splitremote">
        /// The splitremote.
        /// </param>
        /// <param name="personalid">
        /// The personalid.
        /// </param>
        /// <param name="remoteid">
        /// The remoteid.
        /// </param>
        /// <param name="reasonapp">
        /// The reasonapp.
        /// </param>
        /// <param name="otherdetailsapp">
        /// The otherdetailsapp.
        /// </param>
        /// <param name="userdefined">
        /// The userdefined.
        /// </param>
        /// <param name="shortsubcat">
        /// The shortsubcat.
        /// </param>
        /// <param name="fromapp">
        /// The fromapp.
        /// </param>
        /// <param name="toapp">
        /// The toapp.
        /// </param>
        /// <param name="countries">
        /// The countries.
        /// </param>
        /// <param name="allowances">
        /// The allowances.
        /// </param>
        /// <param name="associatedudfs">
        /// The associatedudfs.
        /// </param>
        /// <param name="split">
        /// The split.
        /// </param>
        /// <param name="companyapp">
        /// The companyapp.
        /// </param>
        /// <param name="vatrates">
        /// The vatrates.
        /// </param>
        /// <param name="enablehometolocationmileage">
        /// The enablehometolocationmileage.
        /// </param>
        /// <param name="hometolocationtype">
        /// The hometolocationtype.
        /// </param>
        /// <param name="mileagecategory">
        /// The mileagecategory.
        /// </param>
        /// <param name="isrelocationmileage">
        /// The isrelocationmileage.
        /// </param>
        /// <param name="reimbursablesubcatid">
        /// The reimbursablesubcatid.
        /// </param>
        /// <param name="allowHeavyBulkyMileage">
        /// The allow heavy bulky mileage.
        /// </param>
        /// <param name="homeToOfficeAsZero">
        /// The home To Office As Zero.
        /// </param>
        /// <returns>
        /// The <see cref="cSubcat"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Category ID exception.
        /// </exception>
        public static cSubcat Template(int categoryId, int subcatid = 0, string subcat = default(string), string description = default(string), bool mileageapp = false, bool staffapp = false, bool othersapp = false, bool tipapp = false, bool pmilesapp = false, bool bmilesapp = false, decimal allowanceamount = 0, string accountcode = default(string), bool attendeesapp = false, bool addasnet = false, int pdcatid = 0, bool eventinhomeapp = false, bool receiptapp = false, CalculationType calculation = default(CalculationType), bool passengersapp = false, bool nopassengersapp = false, bool passengernamesapp = false, string comment = default(string), bool splitentertainment = false, int entertainmentid = 0, bool reimbursable = true, bool nonightsapp = false, bool attendeesmand = false, bool nodirectorsapp = false, bool hotelapp = false, bool noroomsapp = false, bool hotelmand = false, bool vatnumberapp = false, bool vatnumbermand = false, bool nopersonalguestsapp = false, bool noremoteworkersapp = false, string alternateaccountcode = default(string), bool splitpersonal = false, bool splitremote = false, int personalid = 0, int remoteid = 0, bool reasonapp = false, bool otherdetailsapp = false, SortedList<int, object> userdefined = null, string shortsubcat = default(string), bool fromapp = false, bool toapp = false, List<cCountrySubcat> countries = null, List<int> allowances = null, List<int> associatedudfs = null, List<int> split = null, bool companyapp = false, List<cSubcatVatRate> vatrates = null, bool enablehometolocationmileage = false, HomeToLocationType hometolocationtype = default(HomeToLocationType), int? mileagecategory = null, bool isrelocationmileage = false, int? reimbursablesubcatid = null, bool allowHeavyBulkyMileage = false, bool homeToOfficeAsZero = false, float? homeToOfficeFixedMiles = null, int? publicTransportRate = null)
        {
            if (categoryId == 0)
            {
                throw new Exception("CategoryId must have a valid ID");
            }

            ICurrentUser currentUser = Moqs.CurrentUser();

            subcat = subcat == default(string) ? "Unit Test dummy Item" : subcat;
            description = description == default(string) ? "Unit Test dummy Item" : description;
            accountcode = accountcode == default(string) ? "UnitTest01" : accountcode;
            calculation = calculation == default(CalculationType) ? CalculationType.NormalItem : calculation;
            comment = comment == default(string) ? "Used for Unit Tests" : comment;
            shortsubcat = shortsubcat == default(string) ? "Unit Test Normal" : shortsubcat;
            countries = countries ?? new List<cCountrySubcat>();
            allowances = allowances ?? new List<int>();
            associatedudfs = associatedudfs ?? new List<int>();
            split = split ?? new List<int>();
            vatrates = vatrates ?? new List<cSubcatVatRate>();
            hometolocationtype = hometolocationtype == default(HomeToLocationType) ? HomeToLocationType.None : hometolocationtype;
            alternateaccountcode = alternateaccountcode == default(string) ? string.Empty : alternateaccountcode;

            var retSubcat = new cSubcat(subcatid, categoryId, subcat, description, mileageapp, staffapp, othersapp, tipapp, pmilesapp, bmilesapp, allowanceamount, accountcode, attendeesapp, addasnet, pdcatid, eventinhomeapp, receiptapp, calculation, passengersapp, nopassengersapp, passengernamesapp, comment, splitentertainment, entertainmentid, reimbursable, nonightsapp, attendeesmand, nodirectorsapp, hotelapp, noroomsapp, hotelmand, vatnumberapp, vatnumbermand, nopersonalguestsapp, noremoteworkersapp, alternateaccountcode, splitpersonal, splitremote, personalid, remoteid, reasonapp, otherdetailsapp, userdefined, DateTime.UtcNow, currentUser.EmployeeID, null, null, shortsubcat, fromapp, toapp, countries, allowances, associatedudfs, split, companyapp, vatrates, enablehometolocationmileage, hometolocationtype, mileagecategory, isrelocationmileage, reimbursablesubcatid, allowHeavyBulkyMileage, homeToOfficeAsZero, homeToOfficeFixedMiles, publicTransportRate);

            return retSubcat;
        }

        /// <summary>
        /// The new.
        /// </summary>
        /// <param name="subcat">
        /// The subcat.
        /// </param>
        /// <returns>
        /// The <see cref="cSubcat"/>.
        /// </returns>
        public static cSubcat New(cSubcat subcat)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            var clsSubcats = new cSubcats(currentUser.AccountID);

            int tempSubcatId = clsSubcats.SaveSubcat(subcat);
            Assert.IsTrue(tempSubcatId > 0, "cSubcatObject.New failure : Subcat creation returned error code " + tempSubcatId);

            clsSubcats = new cSubcats(currentUser.AccountID);
            cSubcat newSubcat = clsSubcats.GetSubcatById(tempSubcatId);

            return newSubcat;
        }

        /// <summary>
        /// The tear down.
        /// </summary>
        /// <param name="subcatId">
        /// The subcat id.
        /// </param>
        public static void TearDown(int subcatId)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();
            var clsSubcats = new cSubcats(currentUser.AccountID);

            int retCode = clsSubcats.DeleteSubcat(subcatId);
            switch (retCode)
            {
                case 1:
                    Assert.Fail("cSubcatObject.TearDown failure : Expense Items assigned to the subcat.");
                    break;
                case 2:
                    Assert.Fail("cSubcatObject.TearDown failure : Mobile Expense Items assigned to the subcat.");
                    break;
                case 3:
                    Assert.Fail("cSubcatObject.TearDown failure : Flag rules assigned to the subcat.");
                    break;
                case 4:
                    Assert.Fail("cSubcatObject.TearDown failure : Mobile Journeys assigned to the subcat.");
                    break;
                case -10:
                    Assert.Fail("cSubcatObject.TearDown failure : checkReferencedBy stored proc preventing TearDown");
                    break;
            }

        }
    }
}
