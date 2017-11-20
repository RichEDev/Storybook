using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using System.Reflection;
using Spend_Management;

namespace tempMobileUnitTests
{
    internal class cExpenseItemObject
    {
        public static cExpenseItem New(cExpenseItem item)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            cExpenseItems expItems = new cExpenseItems(currentUser.AccountID, currentUser.EmployeeID);
            int newExpenseId = expItems.addItem(item, currentUser.EmployeeID);

            cClaims clsClaims = new cClaims(currentUser.AccountID);
            cClaim curClaim = clsClaims.getClaimById(item.claimid);
            return curClaim.getExpenseItemById(newExpenseId);
        }

        public static cExpenseItem Template(int subcatid, int claimid, int expenseid = 0, ItemType itemtype = default(ItemType), decimal bmiles = 0, decimal pmiles = 0, string reason = default(string), bool receipt = false, decimal net = 0, decimal vat = 0, decimal total = 0, DateTime date = default(DateTime), byte staff = 0, byte others = 0, int companyid = 0, bool returned = false, bool home = false, string refnum = default(string), int plitres = 0, int blitres = 0, int currencyid = 0, string attendees = default(string), decimal tip = 0, int countryid = 0, decimal foreignvat = 0, decimal convertedtotal = 0, double exchangerate = 1, bool tempallow = false, int reasonid = 0, bool normalreceipt = false, DateTime allowancestartdate = default(DateTime), DateTime allowancesenddate = default(DateTime), int carid = 0, decimal allowancededuct = 0, int allowanceid = 0, byte nonights = 0, double quantity = 0, byte directors = 0, decimal amountpayable = 0, int hotelid = 0, bool primaryitem = true, byte norooms = 0, string vatnumber = default(string), byte personalguests = 0, byte remoteworkers = 0, string accountcode = default(string), int basecurrency = 0, int globalbasecurrency = 0, double globalexchangerate = 0, decimal globaltotal = 0, SortedList<int, object> userdefined = null, int floatid = 0, bool corrected = false, bool receiptattached = false, int transactionid = 0, string connstring = default(string), int mileageid = 0, SortedList<int, cJourneyStep> journeysteps = null, MileageUOM journeyunit = default(MileageUOM), Dictionary<FlagType, cFlaggedItem> flags = null, List<cDepCostItem> costcodebreakdown = null, int assignmentnum = 0, HomeToLocationType hometoofficedeductionmethod = default(HomeToLocationType), bool ismobileitem = false, int mobiledevicetypeid = 0)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            itemtype = (itemtype == default(ItemType) ? ItemType.Cash : itemtype);
            hometoofficedeductionmethod = (hometoofficedeductionmethod == default(HomeToLocationType) ? HomeToLocationType.None : hometoofficedeductionmethod);
            journeyunit = (journeyunit == default(MileageUOM) ? MileageUOM.Mile : journeyunit);
            reason = (reason == default(string) ? "" : reason);
            allowancestartdate = (allowancestartdate == default(DateTime) ? DateTime.Parse("01/01/1900") : allowancestartdate);
            allowancesenddate = (allowancesenddate == default(DateTime) ? DateTime.Parse("01/01/1900") : allowancesenddate);
            date = (date == default(DateTime) ? DateTime.UtcNow : date);
            costcodebreakdown = (costcodebreakdown ?? new List<cDepCostItem>());

            cExpenseItem item = new cExpenseItem(currentUser.AccountID, expenseid, itemtype, bmiles, pmiles, reason, receipt, net, vat, total, subcatid, date, staff, others, companyid, returned, home, refnum, claimid, plitres, blitres, currencyid, attendees, tip, countryid, foreignvat, convertedtotal, exchangerate, tempallow, reasonid, normalreceipt, allowancestartdate, allowancesenddate, carid, allowancededuct, allowanceid, nonights, quantity, directors, amountpayable, hotelid, primaryitem, norooms, vatnumber, personalguests, remoteworkers, accountcode, basecurrency, globalbasecurrency, globalexchangerate, globaltotal, userdefined, floatid, corrected, receiptattached, transactionid, DateTime.UtcNow, currentUser.EmployeeID, DateTime.UtcNow, currentUser.EmployeeID, connstring, mileageid, journeysteps, journeyunit, flags, costcodebreakdown, assignmentnum, hometoofficedeductionmethod, ismobileitem, mobiledevicetypeid);

            return item;
        }

        public static void TearDown(int claimid, int expenseid)
        {
            ICurrentUser currentUser = Moqs.CurrentUser();

            cClaims claims = new cClaims(currentUser.AccountID);
            cClaim claim = claims.getClaimById(claimid);
            cExpenseItem item = claim.getExpenseItemById(expenseid);
            if(item != null)
            {
                claims.deleteExpense(claim, item, false);
            }
        }
    }
}