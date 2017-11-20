using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using SpendManagementLibrary;


namespace ExpensesLibrary
{
    public interface IOffline
    {
        sOnlineAddscreenItemsInfo getAddscreenItems(int accountid, DateTime offGlobaldate);
        sOnlineAllowInfo getAllowances(int accountid, DateTime offGlobalDate);
        sOnlineBroadcastInfo getBroadcastMessages(int accountid, int employeeid, DateTime offGlobalDate);
        sCarInfo getCars(int accountid, int employeeid, DateTime offGlobalDate);
        Dictionary<int, int> getPoolCars(int accountid, int employeeid, DateTime offGlobalDate);
        sOnlineCatInfo getCategories(int accountid, DateTime offGlobalDate);
        Dictionary<int, cClaimHistory> getClaimHistory(int accountid, int employeeid, DateTime offGlobalDate);
        sOnlineCompInfo getCompanies(int accountid, DateTime offGlobalDate);
        Dictionary<int[], cCompanyDistance> getCompanyDistances(int accountid, DateTime offGlobalDate);
        ArrayList getCostcodes(int accountid, DateTime offGlobalDate);
        sOnlineCountryInfo getCountries(int accountid, int employeeid, DateTime offGlobalDate);
        sOnlineGlobalCountryInfo getGlobalCountries(int accountid, DateTime offGlobalDate);
        sOnlineCurrencyInfo getCurrencies(int accountid, DateTime offGlobalDate);
        sOnlineGlobalCurrencyInfo getGlobalCurrencies(int accountid, DateTime offGlobalDate);
        ArrayList getDepartments(int accountid, DateTime offGlobalDate);
        sViewInfo getViews(int accountid, int employeeid, DateTime offGlobalDate);
        sOnlineFieldInfo getFields(int accountid, DateTime offGlobalDate);
        sFilterRuleInfo getFilterRules(int accountid, DateTime offGlobalDate);
        sFloatInfo getFloats(int accountid, DateTime offGlobalDate);
        Dictionary<int, cHotel> getHotels(int accountid, DateTime offGlobalDate);
        sOnlineJoinInfo getJoins(int accountid, DateTime offGlobalDate);
        cGlobalProperties getProperties(int accountid, DateTime offGlobalDate);
        sOnlineSubcatInfo getSubcats(int accountid, DateTime offGlobalDate);
        sOnlineItemRoleInfo getItemRoles(int accountid, DateTime offGlobalDate);
        sMileageInfo getMileageInfo(int accountid, DateTime offGlobalDate);
        ArrayList getProjectCodes(int accountid, DateTime offGlobalDate);
        ArrayList getReasons(int accountid, DateTime offGlobalDate);
        sOnlineRoleInfo getRoles(int accountid, DateTime offGlobalDate);
        sOnlineTableInfo getTables(int accountid, DateTime offGlobalDate);
        sOnlineUserdefinedInfo getUserdefined(int accountid, DateTime offGlobalDate);
        sOnlineViewgroupInfo getViewgroups(int accountid, DateTime offGlobalDate);
        sOnlineSignoffGroupInfo getSignoffgroups(int accountid, DateTime offGlobalDate);
        sOnlineClaimInfo getOnlineClaimInfo(int accountid, int employeeid, DateTime offGlobalDate, bool prevItemsSynched);
        sOnlineItemInfo getOnlineItemInfo(int accountid, int employeeid, DateTime offGlobalDate, bool prevItemsSynched);
        SortedList<int, sReceiptFileInfo> getOnlineReceipts(int accountid, int employeeid, DateTime offGlobalDate);
        List<int> getOnlineDelReceipts(List<int> offlineRecIds, int accountid, int employeeid);
        string sayHello(string name);
        int costcodeCount(int accountid);
        int departmentCount(int accountid);
        int projectcodeCount(int accountid);
        int reasonCount(int accountid);
        ArrayList getCostcodeIds(int accountid);
        ArrayList getDepartmentIds(int accountid);
        ArrayList getProjectcodeIds(int accountid);
        ArrayList getReasonIds(int accountid);

        
        //---------------

        sCompanyDetails getCompanyDetails(int accountid);
        int[] getOnlineClaimableItems(int accountid, int employeeid);
        void updateClaimableItems(int[] subcatids, int accountid, int employeeid);
        void changeUserPassword(string password, int accountid, int employeeid, byte checkpwd);
        void updateEmployeeStatus(int accountid, int employeeid);
        SortedList<int, int> addOfflineHotels(Dictionary<int, cHotel> addedHotels, int accountid, int employeeid);
        SortedList<int, int> addOfflineCars(Dictionary<int, cCar> addedCars, int accountid, int employeeid);
        SortedList<int, int> addOfflineCompanies(Dictionary<int, cCompany> addedCompanies, int accountid, int employeeid);
        SortedList<int, int> addOfflineClaims(Dictionary<int, cClaim> addedClaims, int accountid);
        SortedList<int, int> addOfflineOdometerReadings(Dictionary<int, cOdometerReading> addedOdometerReadings, int accountid, int employeeid);
        void updateOfflineClaims(Dictionary<int, cClaim> updatedClaims, int accountid);
        void deleteOfflineClaims(List<int> deletedClaims, int accountid);
        SortedList<int, int> addOfflineExpenseItems(Dictionary<int, cExpenseItem> addedExpItems, int accountid, int employeeid);
        SortedList<int, int> updateExpenseItems(Dictionary<int, cExpenseItem> updatedExpItems, int accountid, int employeeid);
        void deleteExpenseItems(SortedList<int, int> deletedExpItems, int accountid, int employeeid);
        void submitClaimToOnline(cClaim claim, bool cash, bool credit, bool purchase, int approver, int accountid);
        void unsubmitClaimOnOnline(cClaim claim, bool approver, int accountid, int employeeid);
        void addOfflineReceipts(SortedList<int, sReceiptFileInfo> lstReceipts, List<int> lstClaimids, int accountid, int employeeid);
        void deleteReceiptsFromOffline(SortedList<int, sReceiptFileInfo> lstexpids, int accountid, int emloyeeid);
        byte[] getOnlinePolicyFile(int accountid, DateTime offGlobalDate);
        cAccount getRegisteredUser(string companyid);
        cEmployee getEmployeeFromOnline(string username, int accountid);
        Dictionary<int, cEmployee> getOnlineModifiedEmployees(List<int> lstempids, DateTime offDate, int accountid);
        string isUpdateAvailable(int accountid);
        SortedList<string, byte[]> getUpdateFiles(int accountid);
        void sendOfflineErrors(List<string> lstErrMsgs, int accountid);
        decimal getOnlineDistance(int locationa, int locationb, int accountid, int employeeid, DateTime date);
        void addOfflineViews(SortedList<int, ArrayList> addedViews, int accountid, int employeeid);
        sCardInfo getCardInfo(int accountid, int employeeid, DateTime offGlobalDate);
        Dictionary<int, string> getESRAssignmentNums(int accountid, int employeeid, DateTime offGlobalDate);
        DateTime getServerDate();
        string getExpensesConnectVersion(int accountid);
        void addAuditLogEntry(string category, string value, int accountid, int employeeid);
    }
}
