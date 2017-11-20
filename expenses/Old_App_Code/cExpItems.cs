using System;
using System.Collections.Generic;
using ExpensesLibrary;
using expenses.Old_App_Code;
using expenses.Old_App_Code.admin;
using SpendManagementLibrary;
using Spend_Management;

namespace expenses
{
    /// <summary>
    /// Summary description for items.
    /// </summary>


    public class cExpenseItems
    {
        private string auditcat = "Expense Items";

        private int nAccountid;
        
        string strsql;
        cMisc clsmisc;
        cGlobalProperties clsproperties;
        cEmployee reqemp;
        #region properties
        public int accountid
        {
            get { return nAccountid; }
        }
        #endregion

        public cExpenseItems(int accountid, int employeeid)
        {
            nAccountid = accountid;
            //expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            clsmisc = new cMisc(accountid);
            clsproperties = clsmisc.getGlobalProperties(accountid);
            cEmployees clsemployees = new cEmployees(accountid);
            reqemp = clsemployees.GetEmployeeById(employeeid);
        }

        public int addItem(cExpenseItem expitem, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cMisc clsmisc = new cMisc(accountid);
            cClaims clsclaims = new cClaims(accountid);
            cClaim reqclaim = clsclaims.getClaimById(expitem.claimid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            bool amountpayableCalculated = false;
            decimal total = 0;
            decimal amountpay = 0;

            if (expitem.transactionid > 0)
            {
                cCardStatements clsstatements = new cCardStatements(accountid);
                cCardTransaction transaction = clsstatements.getTransactionById(expitem.transactionid);
                cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
                if (statement.corporatecard.singleclaim)
                {
                    if (clsstatements.canReconcileItem(expitem.claimid, transaction.statementid, reqclaim.employeeid) == false)
                    {
                        return -1;

                    }
                }
            }
            
            int expenseid;
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            cSubcats clssubcats = new cSubcats(accountid);
            cSubcat subcat = clssubcats.getSubcatById(expitem.subcatid);
            if (subcat == null)
            {
                return 0;
            }

            cMileagecats clsmileagecats = new cMileagecats(accountid);
            cCompanies clscomps = new cCompanies(accountid);
            cCar car = reqemp.getCarById(expitem.carid);

            cVat clsvat = new cVat(accountid, ref expitem, reqemp, clsmisc, clsproperties, null);

            cMileageCat reqmileage = clsmileagecats.GetMileageCatById(expitem.mileageid);

            if (reqmileage != null)
            {
                if (reqmileage.thresholdType == ThresholdType.Journey) //Journey calculation
                {
                    total = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Journey);
                }
                else //Annual calculation
                {
                    total = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Annual);
                }

                if (clsproperties.autoCalcHomeToLocation)
                {
                    if (expitem.journeysteps.Count > 0)
                    {
                        decimal diff = 0;
                        bool homeDiffDeducted = false;
                        int stepIndex = 0;

                        foreach (cJourneyStep step in expitem.journeysteps.Values)
                        {
                            if (step.startlocation != null && step.endlocation != null)
                            {
                                if (step.startlocation.companyid == reqemp.homelocationid)
                                {
                                    decimal hometolocationdist = step.nummiles;
                                    decimal officetolocationdist = clscomps.getDistance(reqemp.officelocationid, step.endlocation.companyid, employeeid);

                                    if (hometolocationdist <= 0 || officetolocationdist <= 0)
                                    {
                                        break;
                                    }
                                    if (car.defaultuom == MileageUOM.KM)
                                    {
                                        hometolocationdist = clsmileagecats.convertMilesToKM(hometolocationdist);
                                        officetolocationdist = clsmileagecats.convertMilesToKM(officetolocationdist);
                                    }

                                    if (homeDiffDeducted == false)
                                    {
                                        if (hometolocationdist > officetolocationdist)
                                        {
                                            homeDiffDeducted = true;
                                            stepIndex = step.stepnumber;
                                            diff = hometolocationdist - officetolocationdist;
                                            step.nummiles = step.nummiles - diff;
                                        }
                                    }
                                }
                            }
                        }

                        if (reqmileage.thresholdType == ThresholdType.Journey) //Journey calculation
                        {
                            amountpay = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Journey);
                        }
                        else //Annual calculation
                        {
                            amountpay = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Annual);
                        }

                        amountpayableCalculated = true;
                        expitem.journeysteps[stepIndex].nummiles = expitem.journeysteps[stepIndex].nummiles + diff;
                    }
                }
            }

            if (total != 0)
            {
                expitem.total = total;
            }
            if (amountpay != 0)
            {
                expitem.amountpayable = amountpay;
            }

            if ((expitem.total == 0 && subcat.calculation != CalculationType.PencePerMile && subcat.calculation != CalculationType.DailyAllowance && subcat.calculation != CalculationType.PencePerMileReceipt) || (expitem.miles == 0 && (subcat.calculation == CalculationType.PencePerMile || subcat.calculation == CalculationType.PencePerMileReceipt)))
            {
                return 0;
            }

            if (expitem.primaryitem)
            {
                //convert net to gross
                ConvertNetToGross(ref expitem);
                //convert back to base currency
                convertTotals(ref expitem);
                //convert to global base currency
                convertGlobalTotals(ref expitem);

                cExpenseItem tempItem;

                if (expitem.splititems.Count > 0)
                {
                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        tempItem = splititem;
                        //convert net to gross
                        ConvertNetToGross(ref tempItem);
                        //convert back to base currency
                        convertTotals(ref tempItem);
                        //convert to global base currency
                        convertGlobalTotals(ref tempItem);
                    }
                }
            }

            splitEntertainment(ref expitem, null);
            string refnum = generateRefnum(reqclaim.employeeid);
            expitem.refnum = refnum;

            setAccountCode(ref expitem, subcat);
            if (expitem.allowanceid != 0)
            {
                cAllowances clsallowances = new cAllowances(accountid);
                clsallowances.calculateDailyAllowance(reqemp.employeeid, ref expitem);
            }

            clsvat.calculateVAT();

            if (amountpayableCalculated == false)
            {
                calculateAmountPayable(ref expitem, null, subcat);
            }

            if ((expitem.total != 0 || subcat.calculation == CalculationType.PencePerMileReceipt) || (expitem.total == 0 && subcat.calculation == CalculationType.PencePerMile))
            {
                expenseid = RunInsertSQL(ref expitem, employeeid);
            }
            else
            {
                expenseid = 0;
            }
            if (expenseid != 0)
            {
                expitem.expenseid = expenseid;
                InsertCostCodeBreakdown(false, expitem);
                saveJourneySteps(expitem);
                if (expitem.floatid != 0)
                {
                    cFloats clsfloats = new cFloats(accountid);
                    cFloat reqfloat = clsfloats.GetFloatById(expitem.floatid);
                    decimal floatallocation;
                    if (expitem.amountpayable == 0)
                    {
                        floatallocation = expitem.total;
                    }
                    else
                    {
                        floatallocation = expitem.total - expitem.amountpayable;
                    }
                    floatallocation = Math.Round(floatallocation, 2);
                    clsfloats.addAllocation(expitem.floatid, expitem.expenseid, floatallocation);
                }
                cValidate clsvalidate = new cValidate(accountid, reqemp);
                clsvalidate.checkForDuplicates(expitem); //check to see if the item is a duplicate
                //checkForDuplicates(i); 
                clsvalidate.checkLimits(expitem, true); //check to see if limits have been exceeded
                //checkLimits(i,true); 
                clsvalidate.checkWeekend(expitem); //check the date to see if it falls on a weekend
                //checkWeekend(i); 
                clsvalidate.checkDates(expitem);//check the date to see if it is before allowed start date
                //checkDates(i); 
                clsvalidate.checkReimbursableFlag(expitem); //check to see if item will be reimbursed
                //Home to Location greater than office to location
                if (clsproperties.calchometolocation)
                {
                    clsvalidate.checkHomeToLocationMileage(expitem, reqclaim.employeeid);
                }
                
                //tip limit
                if (subcat.tipapp)
                {
                    clsvalidate.checkTipLimit(expitem, true);
                }
                if (expitem.floatid == 0)
                {
                    
                    
                    
                    if (expitem.floatid == 0)
                    {
                        clsvalidate.checkAvailableFloat(expitem);
                    }
                    
                }
                if (subcat.calculation == CalculationType.PencePerMile)
                {
                    clsvalidate.checkMileage(expitem, reqclaim.employeeid);
                }
                if (expitem.hotelid != 0)
                {
                    cReviews clsreviews = new cReviews();
                    if (clsreviews.alreadyReviewed(expitem.hotelid, reqemp.employeeid) == false)
                    {
                        cHotels clshotels = new cHotels(accountid);
                        //clshotels.requestReview(expitem.hotelid, reqemp.employeeid);
                    }

                }

                clsuserdefined.addValues(AppliesTo.ExpenseItem, expitem.expenseid, expitem.userdefined);
                
                //if (expitem.itemtype == ItemType.CreditCard) //need to match items
                //{
                //    cCardUploads clsuploads = new cCardUploads(accountid);

                //    clsuploads.matchItems(expitem, expitem.transactionid);
                //}

                if (expitem.splititems.Count > 0)
                {
                    decimal tempTotal = expitem.grandtotal;
                    int counter = expitem.splititems.Count;

                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        addItem(splititem, employeeid);
                        linkSplitItem(splititem);

                        tempTotal -= splititem.total;
                    }

                    decimal roundedTotal = (expitem.grandtotal - tempTotal);
                    expitem.splititems[counter - 1].total = decimal.Round(roundedTotal, 2);
                }
                cAuditLog clsaudit = addAuditRecord(employeeid);
                //cAuditLog clsaudit = new cAuditLog(employeeid);
                clsaudit.addRecord(auditcat, expitem.expenseid + "_" + expitem.date.ToShortDateString() + "_" + subcat.subcat + "_" + expitem.total.ToString("£###,##,##0.00"));
                if (expitem.primaryitem)
                {

                    reqclaim = clsclaims.getClaimById(expitem.claimid);
                    reqclaim.addItem(expitem);
                }
            }
            return expenseid;
        }

        
        
        public cAuditLog addAuditRecord(int employeeid)
        {
            if (employeeid != 0)
            {
                cAuditLog clsaudit = new cAuditLog(accountid, employeeid);
                return clsaudit;
            }
            else
            {
                cAuditLog clsaudit = new cAuditLog();
                return clsaudit;
            }
        }

        private void linkSplitItem(cExpenseItem item)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = "insert into savedexpenses_splititems (primaryitem, splititem) values (@primaryitem, @splititem)";
            expdata.sqlexecute.Parameters.AddWithValue("@primaryitem", item.parent.expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@splititem", item.expenseid);
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }
        public int updateItem(cExpenseItem expitem, int employeeid, int oldclaimid, bool offlineitem)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cNewUserDefined clsuserdefined = new cNewUserDefined(accountid, cAccounts.getConnectionString(accountid), new cTables(accountid)); ;
            cSubcats clssubcats = new cSubcats(accountid);
            cSubcat subcat = clssubcats.getSubcatById(expitem.subcatid);
            bool vatchanged = false;
            cClaims clsclaims = new cClaims(accountid);
            cClaim reqclaim = clsclaims.getClaimById(expitem.claimid);
            cClaim oldclaim = null;
            bool amountpayableCalculated = false;
            decimal dtotal = 0;
            decimal amountpay = 0;

            if (oldclaimid > 0)
            {
                oldclaim = clsclaims.getClaimById(oldclaimid);
            }
            cExpenseItem olditem = null;

            if (expitem.primaryitem)
            {
                if (oldclaim != null)
                {
                    olditem = oldclaim.getExpenseItemById(expitem.expenseid);
                }
                else
                {
                    olditem = reqclaim.getExpenseItemById(expitem.expenseid);
                }
            }
            else
            {
                if (oldclaim != null)
                {
                    olditem = oldclaim.getExpenseItemById(expitem.parent.expenseid);
                }
                else
                {
                    olditem = reqclaim.getExpenseItemById(expitem.parent.expenseid);
                }
                if (olditem == null) //split of split
                {
                    if (oldclaim != null)
                    {
                        olditem = oldclaim.getExpenseItemById(expitem.parent.parent.expenseid);
                    }
                    else
                    {
                        olditem = reqclaim.getExpenseItemById(expitem.parent.parent.expenseid);
                    }
                    foreach (cExpenseItem splititem in olditem.splititems)
                    {
                        if (splititem.expenseid == expitem.parent.expenseid)
                        {
                            olditem = splititem;
                            break;
                        }
                    }

                }
                foreach (cExpenseItem splititem in olditem.splititems)
                {
                    if (splititem.expenseid == expitem.expenseid)
                    {
                        olditem = splititem;
                        break;
                    }
                }
            }

            if (olditem == null)
            {
                return 0;
            }

            if (olditem.vat.ToString("########0.00") != expitem.vat.ToString("########0.00") && subcat.calculation != CalculationType.PencePerMile || (olditem.date != expitem.date))
            {
                vatchanged = true;
            }
            if (expitem.allowanceid != 0)
            {
                cAllowances clsallowances = new cAllowances(accountid);
                clsallowances.calculateDailyAllowance(reqemp.employeeid, ref expitem);
            }

            cMileagecats clsmileagecats = new cMileagecats(accountid);

            

            cVat clsvat = new cVat(accountid, ref expitem, reqemp, clsmisc, clsproperties, olditem);

            cMileageCat reqmileage = clsmileagecats.GetMileageCatById(expitem.mileageid);

            if (reqmileage != null)
            {
                if (reqmileage.thresholdType == ThresholdType.Journey) //Journey calculation
                {
                    dtotal = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Journey);
                }
                else //Annual calculation
                {
                    dtotal = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Annual);
                }

                if (clsproperties.autoCalcHomeToLocation)
                {
                    cCompanies clscomps = new cCompanies(accountid);
                    cCar car = reqemp.getCarById(expitem.carid);

                    if (expitem.journeysteps.Count > 0)
                    {
                        decimal diff = 0;
                        bool homeDiffDeducted = false;
                        int stepIndex = 0;

                        foreach (cJourneyStep step in expitem.journeysteps.Values)
                        {
                            if (step.startlocation != null && step.endlocation != null)
                            {
                                if (step.startlocation.companyid == reqemp.homelocationid)
                                {
                                    decimal hometolocationdist = step.nummiles;//clscomps.getDistance(reqemp.homelocationid, step.endlocation.companyid, employeeid);
                                    decimal officetolocationdist = clscomps.getDistance(reqemp.officelocationid, step.endlocation.companyid, employeeid);

                                    if (hometolocationdist <= 0 || officetolocationdist <= 0)
                                    {
                                        break;
                                    }
                                    if (car.defaultuom == MileageUOM.KM)
                                    {
                                        hometolocationdist = clsmileagecats.convertMilesToKM(hometolocationdist);
                                        officetolocationdist = clsmileagecats.convertMilesToKM(officetolocationdist);
                                    }

                                    if (homeDiffDeducted == false)
                                    {
                                        if (hometolocationdist > officetolocationdist)
                                        {
                                            homeDiffDeducted = true;
                                            stepIndex = step.stepnumber;
                                            diff = hometolocationdist - officetolocationdist;
                                            step.nummiles = step.nummiles - diff;
                                        }
                                    }
                                }
                            }
                        }

                        if (reqmileage.thresholdType == ThresholdType.Journey) //Journey calculation
                        {
                            amountpay = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Journey);
                        }
                        else //Annual calculation
                        {
                            amountpay = clsmileagecats.calculateVehicleJourneyTotal(subcat, expitem, reqemp, clsvat, ThresholdType.Annual);
                        }

                        amountpayableCalculated = true;
                        expitem.journeysteps[stepIndex].nummiles = expitem.journeysteps[stepIndex].nummiles + diff;
                    }
                }
            }

            if (dtotal != 0)
            {
                expitem.total = dtotal;
            }
            if (amountpay != 0)
            {
                expitem.amountpayable = amountpay;
            }

            if (expitem.primaryitem)
            {
                convertTotals(ref expitem);  //convert to sterling if necessary
                convertGlobalTotals(ref expitem);

                cExpenseItem tempItem;

                if (expitem.splititems.Count > 0)
                {
                    foreach (cExpenseItem splititem in expitem.splititems)
                    {
                        tempItem = splititem;
                        convertTotals(ref tempItem);
                        convertGlobalTotals(ref tempItem);
                    }
                }
            }

            splitEntertainment(ref expitem, olditem);
            setAccountCode(ref expitem, subcat);

            if ((olditem.vat != expitem.vat || olditem.receipt != expitem.receipt || olditem.home != expitem.home || olditem.total != expitem.total || olditem.miles != expitem.miles || olditem.nopassengers != expitem.nopassengers) && vatchanged == false && subcat.calculation != CalculationType.PencePerMileReceipt || olditem.date != expitem.date)
            {
                clsvat.calculateVAT();

            }
            else
            {
                decimal total = expitem.total;
                decimal vat = expitem.vat;
                decimal net = total - vat;
                expitem.updateVAT(net, vat, total);

            }

            if (amountpayableCalculated == false)
            {
                calculateAmountPayable(ref expitem, olditem, subcat);
            }


            RunUpdateSQL(ref expitem, employeeid);
            saveJourneySteps(expitem);
            InsertCostCodeBreakdown(true, expitem);
            cValidate clsvalidate = new cValidate(accountid, reqemp);
            clsvalidate.deleteAllFlags(expitem);

            clsvalidate.checkForDuplicates(expitem);//check to see if the item is a duplicate
            //checkForDuplicates(i); 
            clsvalidate.checkLimits(expitem, true);//check to see if limits have been exceeded
            //checkLimits(i,true); 
            clsvalidate.checkWeekend(expitem); //check the date to see if it falls on a weekend
            //checkWeekend(i); 
            clsvalidate.checkDates(expitem); //check the date to see if is it before the allowed start date
            //checkDates(i); 
            clsvalidate.checkReimbursableFlag(expitem); //check to see if item will be reimbursed
            //Home to Location greater than office to location
            if (clsproperties.calchometolocation)
            {
                clsvalidate.checkHomeToLocationMileage(expitem, reqclaim.employeeid);
            }
            
            //tip limit
            if (subcat.tipapp)
            {
                clsvalidate.checkTipLimit(expitem, true);
            }
            cFloats clsfloats = new cFloats(accountid);
            cFloat reqfloat;
            if (expitem.floatid == 0)
            {
                clsvalidate.checkAvailableFloat(expitem);
                if (olditem.floatid > 0)
                {
                    reqfloat = clsfloats.GetFloatById(olditem.floatid);
                    clsfloats.deleteAllocation(expitem.expenseid, reqfloat.floatid);
                }
            }
            else
            {
                reqfloat = clsfloats.GetFloatById(expitem.floatid);
                if (olditem.floatid > 0 && olditem.floatid != expitem.floatid)
                {
                    clsfloats.deleteAllocation(olditem.expenseid, olditem.floatid);
                }
                if (reqfloat.allocations.ContainsKey(expitem.expenseid))
                {
                    clsfloats.updateAllocation(expitem.floatid, expitem.expenseid, expitem.total);
                }
                else
                {
                    clsfloats.addAllocation(expitem.floatid, expitem.expenseid, expitem.total);
                }
            }

            if (subcat.calculation == CalculationType.PencePerMile)
            {
                clsvalidate.checkMileage(expitem, reqclaim.employeeid);
            }


            if (expitem.returned == true) //add return comment
            {

                clsclaims.updateReturned(reqclaim, expitem, employeeid);
            }
            clsuserdefined.addValues(AppliesTo.ExpenseItem, expitem.expenseid, expitem.userdefined);
            if (expitem.splititems.Count != 0)
            {
                System.Data.SqlClient.SqlDataReader reader;
                int expid = 0;
                decimal tempTotal = expitem.grandtotal;
                int counter = expitem.splititems.Count;

                foreach (cExpenseItem splititem in expitem.splititems)
                {
                    if (offlineitem)
                    {
                        strsql = "SELECT expenseid FROM savedexpenses_current WHERE expenseid = @expenseid";
                        expdata.sqlexecute.Parameters.AddWithValue("@expenseid", splititem.expenseid);
                        reader = expdata.GetReader(strsql);

                        while (reader.Read())
                        {
                            expid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                        }
                        reader.Close();
                    }

                    tempTotal -= splititem.total;    
                    
                    if (splititem.expenseid == 0 && !offlineitem || expid == 0 && offlineitem) //new item
                    {
                        addItem(splititem, employeeid);
                        if (splititem.expenseid > 0)
                        {
                            linkSplitItem(splititem);
                        }
                    }
                    else
                    {
                        updateItem(splititem, employeeid, expitem.claimid, offlineitem);
                    }
                }
                decimal roundedTotal = (expitem.grandtotal - tempTotal);
                expitem.splititems[counter - 1].total = decimal.Round(roundedTotal, 2);
            }

            if (expitem.splititems.Count < olditem.splititems.Count) //split has reduced so delete
            {
                bool delete = false;
                foreach (cExpenseItem splititem in olditem.splititems)
                {
                    delete = true;
                    foreach (cExpenseItem checkitem in expitem.splititems)
                    {
                        if (checkitem.expenseid == splititem.expenseid)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if (delete)
                    {
                        clsclaims.deleteExpense(reqclaim, splititem, false);
                    }
                }
            }
            //if (expitem.transactionid > 0) //need to match items
            //{
            //    cCardStatements clsstatements = new cCardStatements(accountid);
            //    cCardTransaction transaction = clsstatements.getTransactionById(expitem.transactionid);
            //    cCardStatement statement = clsstatements.getStatementById(transaction.transactionid);
            //    clsstatements.matchTransaction(statement, transaction, expitem);
                
            //}

            
            if (reqclaim.expenseitems.ContainsKey(expitem.expenseid))
            {
                reqclaim.expenseitems[expitem.expenseid] = expitem;
            }
            updateAuditLog(olditem, ref expitem, employeeid);
            return 0;
        }

        

        
        private void updateAuditLog(cExpenseItem olditem, ref cExpenseItem newitem, int employeeid)
        {
            cAuditLog clsaudit = addAuditRecord(employeeid);
            cCompanies clscompanies = new cCompanies(accountid);
            cSubcats clssubcats = new cSubcats(accountid);
            string oldval = "", newval = "";

            cSubcat reqsubcat = clssubcats.getSubcatById(olditem.subcatid);
            cSubcat newsubcat = clssubcats.getSubcatById(newitem.subcatid);
            string itemdesc = olditem.expenseid + "_" + olditem.date.ToShortDateString() + "_" + reqsubcat.subcat + "_" + olditem.total.ToString("£###,##,##0.00");
            if (olditem.claimid != newitem.claimid)
            {
                cClaims clsclaims = new cClaims(accountid);
                if (olditem.claimid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cClaim oldclaim = clsclaims.getClaimById(olditem.claimid);
                    oldval = oldclaim.name;
                }
                if (newitem.claimid == 0)
                {
                    newval = "";
                }
                else
                {
                    cClaim newclaim = clsclaims.getClaimById(newitem.claimid);
                    newval = newclaim.name;
                }
                clsaudit.editRecord(itemdesc, "Claim Name", auditcat, oldval, newval);
            }

            if (reqsubcat.categoryid != newsubcat.categoryid)
            {
                cCategories clscategories = new cCategories(accountid);

                cCategory oldcat = clscategories.FindById(reqsubcat.categoryid);
                cCategory newcat = clscategories.FindById(newsubcat.categoryid);
                clsaudit.editRecord(itemdesc, "Expense Category", auditcat, oldcat.category, newcat.category);
            }
            if (olditem.miles != newitem.miles)
            {
                clsaudit.editRecord(itemdesc, "No Miles", auditcat, olditem.miles.ToString(), newitem.miles.ToString());
            }
            if (olditem.bmiles != newitem.bmiles)
            {
                clsaudit.editRecord(itemdesc, "Business Miles", auditcat, olditem.bmiles.ToString(), newitem.bmiles.ToString());
            }
            if (olditem.pmiles != newitem.pmiles)
            {
                clsaudit.editRecord(itemdesc, "Personal Miles", auditcat, olditem.pmiles.ToString(), newitem.pmiles.ToString());
            }
            if (olditem.reason != newitem.reason)
            {
                clsaudit.editRecord(itemdesc, "Other Details", auditcat, olditem.reason, newitem.reason);
            }
            if (olditem.receipt != newitem.receipt)
            {
                clsaudit.editRecord(itemdesc, "VAT Receipt", auditcat, olditem.receipt.ToString(), newitem.receipt.ToString());
            }
            if (olditem.net != newitem.net)
            {
                clsaudit.editRecord(itemdesc, "NET", auditcat, olditem.net.ToString("£###,##,##0.00"), newitem.net.ToString("£###,##,##0.00"));
            }
            if (olditem.vat != newitem.vat)
            {
                clsaudit.editRecord(itemdesc, "VAT", auditcat, olditem.vat.ToString("£###,##,##0.00"), newitem.vat.ToString("£###,##,##0.00"));
            }
            if (olditem.total != newitem.total)
            {
                clsaudit.editRecord(itemdesc, "Total", auditcat, olditem.total.ToString("£###,##,##0.00"), newitem.total.ToString("£###,##,##0.00"));
            }
            if (olditem.subcatid != newitem.subcatid)
            {
                cSubcat oldsubcat = clssubcats.getSubcatById(olditem.subcatid);

                clsaudit.editRecord(itemdesc, "Total", auditcat, oldsubcat.subcat, newsubcat.subcat);
            }
            if (olditem.date != newitem.date)
            {
                clsaudit.editRecord(itemdesc, "Date", auditcat, olditem.date.ToShortDateString(), newitem.date.ToShortDateString());
            }
            if (olditem.staff != newitem.staff)
            {
                clsaudit.editRecord(itemdesc, "No Staff", auditcat, olditem.staff.ToString(), newitem.staff.ToString());
            }
            if (olditem.others != newitem.others)
            {
                clsaudit.editRecord(itemdesc, "No Others", auditcat, olditem.others.ToString(), newitem.others.ToString());
            }
            if (olditem.companyid != newitem.companyid)
            {

                if (olditem.companyid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cCompany oldcomp = clscompanies.GetCompanyById(olditem.companyid);

                    if (oldcomp != null)
                    {
                        oldval = oldcomp.company;
                    }
                }
                if (newitem.companyid == 0)
                {
                    newval = "";
                }
                else
                {
                    cCompany newcomp = clscompanies.GetCompanyById(newitem.companyid);
                    if (newcomp != null)
                    {
                        newval = newcomp.company;
                    }
                }
                clsaudit.editRecord(itemdesc, "To Location", auditcat, oldval, newval);
            }
            if (olditem.home != newitem.home)
            {
                clsaudit.editRecord(itemdesc, "Event in home city", auditcat, olditem.home.ToString(), newitem.home.ToString());
            }
            
            if (olditem.plitres != newitem.plitres)
            {
                clsaudit.editRecord(itemdesc, "Personal Litres", auditcat, olditem.plitres.ToString(), newitem.plitres.ToString());
            }
            if (olditem.blitres != newitem.blitres)
            {
                clsaudit.editRecord(itemdesc, "Business Litres", auditcat, olditem.blitres.ToString(), newitem.blitres.ToString());
            }
            //			if (olditem.allowanceamount != newitem.allowanceamount)
            //			{
            //				clsaudit.editRecord(itemdesc,"Allowance Amount",auditcat,olditem.allowanceamount.ToString(),newitem.allowanceamount.ToString());
            //			}
            if (olditem.currencyid != newitem.currencyid)
            {
                cGlobalCurrencies clsglobalcurrencies = new cGlobalCurrencies();
                cCurrencies clscurrencies = new cCurrencies(accountid);
                if (olditem.currencyid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cCurrency oldcur = clscurrencies.getCurrencyById(olditem.currencyid);
                    oldval = clsglobalcurrencies.getGlobalCurrencyById(oldcur.globalcurrencyid).label;
                }
                if (newitem.currencyid == 0)
                {
                    newval = "";
                }
                else
                {
                    cCurrency newcur = clscurrencies.getCurrencyById(newitem.currencyid);
                    newval = clsglobalcurrencies.getGlobalCurrencyById(newcur.globalcurrencyid).label;
                }
                clsaudit.editRecord(itemdesc, "Currency", auditcat, oldval, newval);
            }
            if (olditem.attendees != newitem.attendees && newitem.attendees != null)
            {

                clsaudit.editRecord(itemdesc, "Attendees", auditcat, olditem.attendees, newitem.attendees);
            }
            if (olditem.tip != newitem.tip)
            {
                clsaudit.editRecord(itemdesc, "Tip", auditcat, olditem.tip.ToString("£###,##,##0.00"), newitem.tip.ToString("£###,##,##0.00"));
            }
            if (olditem.countryid != newitem.countryid)
            {
                cCountries clscountries = new cCountries(accountid);
                cGlobalCountries clsglobalcountries = new cGlobalCountries();
                if (olditem.countryid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cCountry oldcountry = clscountries.getCountryById(olditem.countryid);
                    oldval = clsglobalcountries.getGlobalCountryById(oldcountry.globalcountryid).country;
                }
                if (newitem.countryid == 0)
                {
                    newval = "";
                }
                else
                {
                    cCountry newcountry = clscountries.getCountryById(newitem.countryid);
                    newval = clsglobalcountries.getGlobalCountryById(newcountry.globalcountryid).country;
                }
                clsaudit.editRecord(itemdesc, "Country", auditcat, oldval, newval);
            }
            if (olditem.foreignvat != newitem.foreignvat)
            {
                clsaudit.editRecord(itemdesc, "Foreign VAT", auditcat, olditem.foreignvat.ToString(), newitem.foreignvat.ToString());
            }
            if (olditem.convertedtotal != newitem.convertedtotal)
            {
                clsaudit.editRecord(itemdesc, "Total prior to convert", auditcat, olditem.convertedtotal.ToString(), newitem.convertedtotal.ToString());
            }
            if (olditem.exchangerate != newitem.exchangerate)
            {
                clsaudit.editRecord(itemdesc, "Exchange Rate", auditcat, olditem.exchangerate.ToString(), newitem.exchangerate.ToString());
            }
            if (olditem.normalreceipt != newitem.normalreceipt)
            {
                clsaudit.editRecord(itemdesc, "Normal Receipt", auditcat, olditem.normalreceipt.ToString(), newitem.normalreceipt.ToString());
            }
            if (olditem.floatid != newitem.floatid)
            {
                cFloats clsfloats = new cFloats(accountid);
                if (olditem.floatid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cFloat oldfloat = clsfloats.GetFloatById(olditem.floatid);
                    oldval = oldfloat.name;
                }
                if (newitem.floatid == 0)
                {
                    newval = "";
                }
                else
                {
                    cFloat newfloat = clsfloats.GetFloatById(newitem.floatid);
                    newval = newfloat.name;
                }
                clsaudit.editRecord(itemdesc, "Float", auditcat, oldval, newval);
            }

            if (olditem.reasonid != newitem.reasonid)
            {
                cReasons clsreasons = new cReasons(accountid);
                if (olditem.reasonid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cReason oldreas = clsreasons.getReasonById(olditem.reasonid);
                    oldval = oldreas.reason;
                }
                if (newitem.reasonid == 0)
                {
                    newval = "";
                }
                else
                {
                    cReason newreas = clsreasons.getReasonById(newitem.reasonid);
                    newval = newreas.reason;
                }
                clsaudit.editRecord(itemdesc, "Reason", auditcat, oldval, newval);
            }
            if (olditem.fromid != newitem.fromid)
            {

                if (olditem.fromid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cCompany oldfrom = clscompanies.GetCompanyById(olditem.fromid);
                    if (oldfrom != null)
                    {
                        oldval = oldfrom.company;
                    }
                    else
                    {
                        oldval = "";
                    }
                }
                if (newitem.fromid == 0)
                {
                    newval = "";
                }
                else
                {
                    cCompany newfrom = clscompanies.GetCompanyById(newitem.fromid);
                    newval = newfrom.company;
                }
                clsaudit.editRecord(itemdesc, "From Location", auditcat, oldval, newval);
            }

            if (olditem.allowancestartdate != newitem.allowancestartdate)
            {
                clsaudit.editRecord(itemdesc, "Start Date", auditcat, olditem.allowancestartdate.ToString(), newitem.allowancestartdate.ToString());
            }
            if (olditem.allowanceenddate != newitem.allowanceenddate)
            {
                clsaudit.editRecord(itemdesc, "End Date", auditcat, olditem.allowanceenddate.ToString(), newitem.allowanceenddate.ToString());
            }
            if (olditem.nopassengers != newitem.nopassengers)
            {
                clsaudit.editRecord(itemdesc, "No Passengers", auditcat, olditem.nopassengers.ToString(), newitem.nopassengers.ToString());
            }
            if (olditem.carid != newitem.carid)
            {

                if (olditem.carid == 0)
                {
                    oldval = "";
                }
                else
                {
                    cCar oldcar = reqemp.getCarById(olditem.carid);
                    if (oldcar != null)
                    {
                        oldval = oldcar.make + " " + oldcar.model;
                    }
                    else
                    {
                        oldval = "";
                    }
                }
                if (newitem.carid == 0)
                {
                    newval = "";
                }
                else
                {
                    cCar newcar = reqemp.getCarById(newitem.carid);
                    if (newcar != null)
                    {
                        newval = newcar.make + " " + newcar.model;
                    }
                    else
                    {
                        newval = "";
                    }
                }
                clsaudit.editRecord(itemdesc, "Car", auditcat, oldval, newval);
            }
            if (olditem.allowancededuct != newitem.allowancededuct)
            {
                clsaudit.editRecord(itemdesc, "Allowance Deduct", auditcat, olditem.allowancededuct.ToString("£###,##,##0.00"), newitem.allowancededuct.ToString("£###,##,##0.00"));
            }
            if (olditem.allowanceid != newitem.allowanceid)
            {
                cAllowances clsallowances = new cAllowances(accountid);
                cAllowance oldallowance = clsallowances.getAllowanceById(olditem.allowanceid);
                cAllowance newallowance = clsallowances.getAllowanceById(newitem.allowanceid);
                if (newallowance != null)
                {
                    newval = newallowance.allowance;
                }
                else
                {
                    newval = "";
                }
                if (oldallowance != null)
                {
                    oldval = oldallowance.allowance;
                }
                else
                {
                    oldval = "";
                }
                clsaudit.editRecord(itemdesc, "Allowance", auditcat, oldval, newval);
            }

        }
        private string generateRefnum(int employeeid)
        {
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            string strcurrefnum = "";
            string refnum = "";



            refnum = reqemp.employeeid + "-";



            strcurrefnum = reqemp.currefnum.ToString("000000");


            refnum += strcurrefnum;


            clsemployees.incrementRefnum(employeeid);
            return refnum;
        }

        private void saveJourneySteps(cExpenseItem expenseitem)
        {
            if (expenseitem.journeysteps != null)
            {
                DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
                string strsql;
                strsql = "delete from savedexpenses_journey_steps where expenseid = @expenseid";
                expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseitem.expenseid);
                expdata.ExecuteSQL(strsql);
                expdata.sqlexecute.Parameters.Clear();

                foreach (cJourneyStep step in expenseitem.journeysteps.Values)
                {
                    strsql = "insert into savedexpenses_journey_steps (expenseid, step_number, start_location, end_location, num_miles,num_passengers) " +
                        "values (@expenseid, @stepnumber, @startlocation, @endlocation, @nummiles, @numpassengers)";
                    expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseitem.expenseid);
                    expdata.sqlexecute.Parameters.AddWithValue("@stepnumber", step.stepnumber);
                    if (step.startlocation != null)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@startlocation", step.startlocation.companyid);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@startlocation", DBNull.Value);
                    }
                    if (step.endlocation != null)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@endlocation", step.endlocation.companyid);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@endlocation", DBNull.Value);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@nummiles", step.nummiles);
                    if (step.numpassengers > 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@numpassengers", step.numpassengers);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@numpassengers", DBNull.Value);
                    }
                    expdata.ExecuteSQL(strsql);
                    expdata.sqlexecute.Parameters.Clear();
                }
            }
        }
        private int RunInsertSQL(ref cExpenseItem expenseitem, int userid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            int expenseid;
            
            DateTime createdon = DateTime.Now.ToUniversalTime();
            expenseitem.createdon = createdon;
            

            
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", expenseitem.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@bmiles", expenseitem.bmiles);
            expdata.sqlexecute.Parameters.AddWithValue("@pmiles", expenseitem.pmiles);
            if (expenseitem.reason == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reason", "");
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reason", expenseitem.reason);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@receipt", Convert.ToByte(expenseitem.receipt));
            expdata.sqlexecute.Parameters.AddWithValue("@net", expenseitem.net);
            expdata.sqlexecute.Parameters.AddWithValue("@vat", expenseitem.vat);
            expdata.sqlexecute.Parameters.AddWithValue("@total", expenseitem.total);
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", expenseitem.subcatid);
            expdata.sqlexecute.Parameters.AddWithValue("@date", expenseitem.date.Year + "/" + expenseitem.date.Month + "/" + expenseitem.date.Day);
            expdata.sqlexecute.Parameters.AddWithValue("@staff", expenseitem.staff);
            expdata.sqlexecute.Parameters.AddWithValue("@others", expenseitem.others);

            if (expenseitem.companyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", expenseitem.companyid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@home", Convert.ToByte(expenseitem.home));
            expdata.sqlexecute.Parameters.AddWithValue("@refnum", expenseitem.refnum);
            expdata.sqlexecute.Parameters.AddWithValue("@plitres", expenseitem.plitres);
            expdata.sqlexecute.Parameters.AddWithValue("@blitres", expenseitem.blitres);
            expdata.sqlexecute.Parameters.AddWithValue("@allowanceamount", expenseitem.total);
            if (expenseitem.currencyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", expenseitem.currencyid);
            }
            if (expenseitem.attendees == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@attendees", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@attendees", expenseitem.attendees);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@tip", expenseitem.tip);
            if (expenseitem.countryid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", expenseitem.countryid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@foreignvat", expenseitem.foreignvat);
            expdata.sqlexecute.Parameters.AddWithValue("@convertedtotal", expenseitem.convertedtotal);
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", expenseitem.exchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@normalreceipt", Convert.ToByte(expenseitem.normalreceipt));

            if (expenseitem.reasonid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", expenseitem.reasonid);
            }
            

            if (expenseitem.allowancestartdate.Date == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", expenseitem.allowancestartdate.Year + "/" + expenseitem.allowancestartdate.Month + "/" + expenseitem.allowancestartdate.Day + " " + expenseitem.allowancestartdate.Hour + ":" + expenseitem.allowancestartdate.Minute);
            }
            if (expenseitem.allowanceenddate.Date == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", expenseitem.allowanceenddate.Year + "/" + expenseitem.allowanceenddate.Month + "/" + expenseitem.allowanceenddate.Day + " " + expenseitem.allowanceenddate.Hour + ":" + expenseitem.allowanceenddate.Minute);
            }
            
            if (expenseitem.carid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", expenseitem.carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@allowancededuct", expenseitem.allowancededuct);
            if (expenseitem.allowanceid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", expenseitem.allowanceid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@amountpayable", expenseitem.amountpayable);
            expdata.sqlexecute.Parameters.AddWithValue("@nonights", expenseitem.nonights);
            expdata.sqlexecute.Parameters.AddWithValue("@quantity", expenseitem.quantity);
            expdata.sqlexecute.Parameters.AddWithValue("@directors", expenseitem.directors);
            if (expenseitem.hotelid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", expenseitem.hotelid);
            }
            if (expenseitem.vatnumber == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@vatnumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@vatnumber", expenseitem.vatnumber);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@primaryitem", Convert.ToByte(expenseitem.primaryitem));
            expdata.sqlexecute.Parameters.AddWithValue("@norooms", expenseitem.norooms);
            expdata.sqlexecute.Parameters.AddWithValue("@personalguests", expenseitem.personalguests);
            expdata.sqlexecute.Parameters.AddWithValue("@remoteworkers", expenseitem.remoteworkers);
            if (expenseitem.accountcode != null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@accountcode", expenseitem.accountcode);
            }
            else
            {

                expdata.sqlexecute.Parameters.AddWithValue("@accountcode", DBNull.Value);
            }

            //System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;


            expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", expenseitem.basecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globalexchangerate", expenseitem.globalexchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@globalbasecurrency", expenseitem.globalbasecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globaltotal", expenseitem.globaltotal);
            
            expdata.sqlexecute.Parameters.AddWithValue("@itemtype", (byte)expenseitem.itemtype);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", expenseitem.mileageid);
            }
            if (expenseitem.transactionid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", expenseitem.transactionid);
            }
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", expenseitem.journeyunit);
            }
            if (expenseitem.assignmentnum == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", expenseitem.assignmentnum);
            }
            strsql = "insert into savedexpenses_current (claimid, itemtype, bmiles, pmiles, reason, receipt, net, vat, total, subcatid, [date], staff, others, companyid, home, refnum,  plitres, blitres, allowanceamount, currencyid, attendees, tip, countryid, foreignvat, convertedtotal, exchangerate, normalreceipt, reasonid, allowancestartdate, allowanceenddate, carid, allowancededuct, allowanceid, nonights, quantity, directors, amountpayable, hotelid,primaryitem, norooms, vatnumber, personalguests, remoteworkers, accountcode, basecurrency, globalexchangerate, globalbasecurrency, globaltotal, createdon, createdby, mileageid, transactionid, journey_unit, AssignmentNumber) " +
                    "values (@claimid,@itemtype,@bmiles,@pmiles,@reason,@receipt,@net,@vat,@total,@subcatid,@date," +
                    "@staff,@others,@companyid,@home,@refnum,@plitres,@blitres,@allowanceamount,@currencyid, @attendees,@tip,@countryid,@foreignvat,@convertedtotal,@exchangerate,@normalreceipt";

            strsql = strsql + ",@reasonid,@allowancestartdate,@allowanceenddate,@carid,@allowancededuct,@allowanceid, @nonights, @quantity, @directors, @amountpayable, @hotelid,@primaryitem, @norooms, @vatnumber, @personalguests, @remoteworkers, @accountcode, @basecurrency, @globalexchangerate, @globalbasecurrency, @globaltotal, @createdon, @userid, @mileageid, @transactionid, @journey_unit, @assignmentnum);set @identity = @@identity";


            expdata.sqlexecute.Parameters.AddWithValue("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.Output;

            expdata.ExecuteSQL(strsql);
            expenseid = (int)expdata.sqlexecute.Parameters["@identity"].Value;
            expdata.sqlexecute.Parameters.Clear();


            return expenseid;
        }
        private void RunUpdateSQL(ref cExpenseItem expenseitem, int userid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DateTime modifiedon = DateTime.Now.ToUniversalTime();
            

            
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", expenseitem.claimid);
            expdata.sqlexecute.Parameters.AddWithValue("@bmiles", expenseitem.bmiles);
            expdata.sqlexecute.Parameters.AddWithValue("@pmiles", expenseitem.pmiles);
            expdata.sqlexecute.Parameters.AddWithValue("@reason", expenseitem.reason);
            expdata.sqlexecute.Parameters.AddWithValue("@receipt", Convert.ToByte(expenseitem.receipt));
            expdata.sqlexecute.Parameters.AddWithValue("@net", expenseitem.net);
            expdata.sqlexecute.Parameters.AddWithValue("@vat", expenseitem.vat);
            expdata.sqlexecute.Parameters.AddWithValue("@total", expenseitem.total);
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", expenseitem.subcatid);
            expdata.sqlexecute.Parameters.AddWithValue("@date", expenseitem.date.Year + "/" + expenseitem.date.Month + "/" + expenseitem.date.Day);
            expdata.sqlexecute.Parameters.AddWithValue("@staff", expenseitem.staff);
            expdata.sqlexecute.Parameters.AddWithValue("@others", expenseitem.others);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", reqemp.employeeid);
            if (expenseitem.companyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@companyid", expenseitem.companyid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@home", Convert.ToByte(expenseitem.home));
            expdata.sqlexecute.Parameters.AddWithValue("@refnum", expenseitem.refnum);
            
            expdata.sqlexecute.Parameters.AddWithValue("@plitres", expenseitem.plitres);
            expdata.sqlexecute.Parameters.AddWithValue("@blitres", expenseitem.blitres);
            //expdata.sqlexecute.Parameters.AddWithValue("@allowanceamount",expenseitem.allowanceamount);
            if (expenseitem.currencyid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@currencyid", expenseitem.currencyid);
            }
            if (expenseitem.attendees == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@attendees", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@attendees", expenseitem.attendees);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@tip", expenseitem.tip);
            if (expenseitem.countryid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@countryid", expenseitem.countryid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@foreignvat", expenseitem.foreignvat);
            expdata.sqlexecute.Parameters.AddWithValue("@convertedtotal", expenseitem.convertedtotal);
            expdata.sqlexecute.Parameters.AddWithValue("@exchangerate", expenseitem.exchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@normalreceipt", Convert.ToByte(expenseitem.normalreceipt));
            if (expenseitem.vatnumber == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@vatnumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@vatnumber", expenseitem.vatnumber);
            }

            if (expenseitem.reasonid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@reasonid", expenseitem.reasonid);
            }
            

            if (expenseitem.allowancestartdate.Date == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowancestartdate", expenseitem.allowancestartdate.Year + "/" + expenseitem.allowancestartdate.Month + "/" + expenseitem.allowancestartdate.Day + " " + expenseitem.allowancestartdate.Hour + ":" + expenseitem.allowancestartdate.Minute);
            }
            if (expenseitem.allowanceenddate.Date == DateTime.Parse("01/01/1900"))
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceenddate", expenseitem.allowanceenddate.Year + "/" + expenseitem.allowanceenddate.Month + "/" + expenseitem.allowanceenddate.Day + " " + expenseitem.allowanceenddate.Hour + ":" + expenseitem.allowanceenddate.Minute);
            }
            if (expenseitem.carid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@carid", expenseitem.carid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@allowancededuct", expenseitem.allowancededuct);
            if (expenseitem.allowanceid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@allowanceid", expenseitem.allowanceid);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@nonights", expenseitem.nonights);
            expdata.sqlexecute.Parameters.AddWithValue("@quantity", expenseitem.quantity);
            expdata.sqlexecute.Parameters.AddWithValue("@directors", expenseitem.directors);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseitem.expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@amountpayable", expenseitem.amountpayable);
            expdata.sqlexecute.Parameters.AddWithValue("@personalguests", expenseitem.personalguests);
            expdata.sqlexecute.Parameters.AddWithValue("@remoteworkers", expenseitem.remoteworkers);
            if (expenseitem.hotelid != 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", expenseitem.hotelid);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hotelid", DBNull.Value);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@norooms", expenseitem.norooms);
            expdata.sqlexecute.Parameters.AddWithValue("@primaryitem", Convert.ToByte(expenseitem.primaryitem));
            if (expenseitem.accountcode == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@accountcode", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@accountcode", expenseitem.accountcode);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@basecurrency", expenseitem.basecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globalexchangerate", expenseitem.globalexchangerate);
            expdata.sqlexecute.Parameters.AddWithValue("@globalbasecurrency", expenseitem.globalbasecurrency);
            expdata.sqlexecute.Parameters.AddWithValue("@globaltotal", expenseitem.globaltotal);
            
            expdata.sqlexecute.Parameters.AddWithValue("@modifiedon", modifiedon);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userid);
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileageid", expenseitem.mileageid);
            }
            if (expenseitem.transactionid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@transactionid", expenseitem.transactionid);
            }
            if (expenseitem.mileageid == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@journey_unit", expenseitem.journeyunit);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@itemtype", (byte)expenseitem.itemtype);
            if (expenseitem.assignmentnum == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@assignmentnum", expenseitem.assignmentnum);
            }
            strsql = "update savedexpenses_current set claimid = @claimid, bmiles = @bmiles, pmiles = @pmiles, reason = @reason, receipt = @receipt, net = @net, vat = @vat, total = @total" +
                ", subcatid = @subcatid, [date] = @date, staff = @staff, others = @others, companyid = @companyid, home = @home, plitres = @plitres, blitres = @blitres" +
                ", currencyid = @currencyid, attendees = @attendees, tip = @tip, countryid = @countryid, foreignvat = @foreignvat, convertedtotal = @convertedtotal, exchangerate = @exchangerate, reasonid = @reasonid, normalreceipt = @normalreceipt";
            strsql = strsql + ", allowancestartdate = @allowancestartdate, allowanceenddate = @allowanceenddate, carid = @carid, allowancededuct = @allowancededuct, allowanceid = @allowanceid, nonights = @nonights, quantity = @quantity, directors = @directors, amountpayable = @amountpayable, hotelid = @hotelid, primaryitem = @primaryitem, norooms = @norooms, vatnumber = @vatnumber, personalguests = @personalguests, remoteworkers = @remoteworkers, accountcode = @accountcode, basecurrency = @basecurrency, globalexchangerate = @globalexchangerate, globalbasecurrency = @globalbasecurrency, globaltotal = @globaltotal, ModifiedOn = @modifiedon, ModifiedBy = @userid, mileageid = @mileageid, transactionid = @transactionid, journey_unit = @journey_unit, itemtype = @itemtype, AssignmentNumber = @assignmentnum";

            strsql = strsql + " where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public System.Data.DataSet getGrid(int employeeid, int claimid, UserView viewtype, Filter filter, bool printview)
        {
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            cUserView view = clsemployees.getUserView(reqemp.employeeid,viewtype, printview);

            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            strsql = view.sql;
            switch (viewtype)
            {
                case UserView.Current:
                case UserView.Submitted:
                case UserView.Previous:
                case UserView.CurrentPrint:
                case UserView.SubmittedPrint:
                case UserView.PreviousPrint:
                    strsql += " where claimid = @claimid and primaryitem = 1";
                    break;
                case UserView.CheckAndPay:

                    strsql += " where savedexpenses_current.claimid = @claimid and tempallow = 0 and returned = 0 and primaryitem = 1";
                    break;
                case UserView.CheckAndPayPrint:
                    strsql += " where savedexpenses_current.claimid = @claimid and primaryitem = 1";
                    break;
            }

            switch (filter)
            {
                case Filter.Cash:
                    strsql += " and itemtype = 1";
                    break;
                case Filter.CreditCard:
                    strsql += " and itemtype = 2";
                    break;
                case Filter.PurchaseCard:
                    strsql += " and itemtype = 3";
                    break;

            }
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            
            System.Data.DataSet ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return ds;
        }

        public System.Data.DataSet getGrid(int employeeid, int claimid, ItemState state)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            cEmployees clsemployees = new cEmployees(accountid);
            cEmployee reqemp = clsemployees.GetEmployeeById(employeeid);
            cUserView view = clsemployees.getUserView(reqemp.employeeid,UserView.CheckAndPay, false);


            strsql += view.sql;
            switch (state)
            {
                case ItemState.Unapproved:
                    strsql += " where savedexpenses_current.claimid = @claimid and tempallow = 0 and returned = 0 and primaryitem = 1";
                    break;
                case ItemState.Returned:
                    strsql += " where savedexpenses_current.claimid = @claimid and returned = 1 and primaryitem = 1";
                    break;
                case ItemState.Approved:
                    strsql += " where savedexpenses_current.claimid = @claimid and tempallow = 1 and returned = 0 and primaryitem = 1";
                    break;
            }
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            
            System.Data.DataSet ds = expdata.GetDataSet(strsql);
            expdata.sqlexecute.Parameters.Clear();
            return ds;
        }
        #region Utility Functions
        private void ConvertNetToGross(ref cExpenseItem expenseitem)
        {

            cSubcat reqsubcat;
            cSubcats clssubcats = new cSubcats(accountid);

            reqsubcat = clssubcats.getSubcatById(expenseitem.subcatid);
            if (reqsubcat == null)
            {
                return;
            }
            if (reqsubcat.addasnet == false)
            {
                return;
            }

            decimal net;
            decimal total;
            double vatamount;

            cSubcatVatRate clsvatrate = reqsubcat.getVatRateByDate(expenseitem.date);

            if (clsvatrate != null && ((clsvatrate.vatreceipt == true && expenseitem.receipt == true) || clsvatrate.vatreceipt == false)) //vat exists so calculate total
            {
                vatamount = clsvatrate.vatamount;
                net = expenseitem.total;
                total = Math.Round(((net / 100) * (decimal)vatamount) + net,2);
                expenseitem.total = total;
                expenseitem.amountpayable = total;

            }

        }
        private void convertTotals(ref cExpenseItem expenseitem)
        {


            int basecurrency;



            if (reqemp.primarycurrency != 0)
            {
                basecurrency = reqemp.primarycurrency;
            }
            else
            {
                basecurrency = clsproperties.basecurrency;
            }

            if (expenseitem.currencyid == basecurrency)
            {
                return;
            }

            double exchangerate;
            decimal convertedtotal;
            decimal convertedNet, net;
            decimal convertedVat, vat;
            decimal total;

            convertedtotal = expenseitem.total;
            convertedNet = expenseitem.net;
            convertedVat = expenseitem.vat;

            if (expenseitem.floatid > 0)
            {
                cFloats clsfloats = new cFloats(accountid);
                cFloat reqfloat = clsfloats.GetFloatById(expenseitem.floatid);
                exchangerate = reqfloat.exchangerate;
                total = Math.Round(convertedtotal * (decimal)exchangerate, 2);
                net = Math.Round(convertedNet * (decimal)exchangerate, 2);
                vat = Math.Round(convertedVat * (decimal)exchangerate, 2);
            }
            else
            {
                exchangerate = expenseitem.exchangerate;
                total = Math.Round(convertedtotal * (1 / (decimal)exchangerate), 2);
                net = Math.Round(convertedNet * (1 / (decimal)exchangerate), 2);
                vat = Math.Round(convertedVat * (1 / (decimal)exchangerate), 2);
            }
            

            if (exchangerate == 0)
            {
                exchangerate = 1;
            }

            expenseitem.convertedtotal = Math.Round(convertedtotal, 2);
            expenseitem.total = total;

            //If this is a mileage item the Net and Vat doesn't get converted when calculated so needs to here.
            expenseitem.updateVAT(net, vat, total); 
            expenseitem.amountpayable = total;
        }
        private void convertGlobalTotals(ref cExpenseItem expenseitem)
        {

            cCurrencies clscurrencies = new cCurrencies(accountid);
            int basecurrency;
            int globalcurrency;
            int employeebasecurrency;
            double globalexchangerate;
            decimal globaltotal;

            basecurrency = clsproperties.basecurrency;

            if (reqemp.primarycurrency != basecurrency)
            {
                employeebasecurrency = reqemp.primarycurrency;
            }
            else
            {
                employeebasecurrency = basecurrency;
            }

            if (expenseitem.currencyid == basecurrency || expenseitem.basecurrency == basecurrency)
            {
                globalcurrency = basecurrency;
                globalexchangerate = 1;
                if (expenseitem.basecurrency != basecurrency)
                {
                    globaltotal = expenseitem.convertedtotal;
                }
                else
                {
                    globaltotal = expenseitem.total;
                }
            }
            else
            {
                double exchangerate;
                decimal convertedtotal;
                decimal total;

                if (expenseitem.floatid > 0)
                {
                    cFloats clsfloats = new cFloats(accountid);
                    cFloat reqfloat = clsfloats.GetFloatById(expenseitem.floatid);
                    exchangerate = reqfloat.exchangerate;
                }
                else
                {
                    exchangerate = clscurrencies.getExchangeRate(basecurrency, expenseitem.basecurrency, expenseitem.date);
                }
                
                convertedtotal = expenseitem.total;
                total = Math.Round(convertedtotal * (1 / (decimal)exchangerate),2);


                globalcurrency = basecurrency;
                globalexchangerate = exchangerate;
                globaltotal = total;
            }


            expenseitem.setGlobalTotal(globalcurrency, globalexchangerate, globaltotal);


        }
        private void splitEntertainment(ref cExpenseItem item, cExpenseItem olditem)
        {
            cSubcats clssubcats = new cSubcats(accountid);

            cMisc clsmisc = new cMisc(accountid);
            cSubcat reqsubcat = clssubcats.getSubcatById(item.subcatid);
            cExpenseItem splititem;
            cExpenseItem tempitem;
            decimal staffportion, otherportion, personalportion, remoteportion;
            decimal staffportionconverted, otherportionconverted, personalportionconverted, remoteportionconverted;

            byte numstaff, numothers, numpersonal, numremote;
            int expenseid;
            DateTime nulldate = DateTime.Parse("01/01/1900");

            if (reqsubcat == null)
            {
                return;
            }
            if (item.total == 0)
            {
                return;
            }
            if (reqsubcat.calculation != CalculationType.Meal || (reqsubcat.splitentertainment == false && reqsubcat.splitpersonal == false && reqsubcat.splitremote == false))
            {
                return;
            }
            
            numstaff = (byte)(item.staff + item.directors);
            numothers = item.others;

            if (reqsubcat.splitpersonal == false)
            {
                numpersonal = 0;
                numothers += item.personalguests;
            }
            else
            {
                numpersonal = item.personalguests;
            }
            if (reqsubcat.splitremote == false)
            {
                numremote = 0;
                numstaff += item.remoteworkers;
            }
            else
            {
                numremote = item.remoteworkers;
            }
            //first item is staff portion

            decimal amounttoallocate = item.total;
            decimal amounttoallocateconverted = item.convertedtotal;

            staffportion = Math.Round((amounttoallocate / (item.staff + item.others + item.directors + item.personalguests + item.remoteworkers)) * numstaff,2);
            if (item.convertedtotal > 0)
            {
                staffportionconverted = Math.Round((amounttoallocateconverted / (item.staff + item.others + item.directors + item.personalguests + item.remoteworkers)) * numstaff, 2);
            }
            else
            {
                staffportionconverted = 0;
            }
            amounttoallocate -= staffportion;
            amounttoallocateconverted -= staffportionconverted;

            item.total = staffportion;
            item.convertedtotal = staffportionconverted;
            if (amounttoallocate > 0)
            {
                otherportion = Math.Round((amounttoallocate / (item.others + item.personalguests + item.remoteworkers)) * numothers, 2);
                if (item.convertedtotal > 0)
                {
                    otherportionconverted = Math.Round((amounttoallocateconverted / (item.others + item.personalguests + item.remoteworkers)) * numothers, 2);
                }
                else
                {
                    otherportionconverted = 0;
                }
                amounttoallocate -= otherportion;
                amounttoallocateconverted -= otherportionconverted;
            }
            else
            {
                otherportion = 0;
                otherportionconverted = 0;
            }
            if (amounttoallocate > 0)
            {
                personalportion = Math.Round((amounttoallocateconverted / (item.personalguests + item.remoteworkers)) * numpersonal, 2);
                if (item.convertedtotal > 0)
                {
                    personalportionconverted = Math.Round((amounttoallocate / (item.personalguests + item.remoteworkers)) * numpersonal, 2);
                }
                else
                {
                    personalportionconverted = 0;
                }
                amounttoallocate -= personalportion;
                amounttoallocateconverted -= personalportionconverted;
            }
            else
            {
                personalportion = 0;
                personalportionconverted = 0;
            }

            if (amounttoallocate > 0)
            {
                remoteportion = (amounttoallocate / (item.remoteworkers)) * numremote;
                if (item.convertedtotal > 0)
                {
                    remoteportionconverted = (amounttoallocateconverted / (item.remoteworkers)) * numremote;
                }
                else
                {
                    remoteportionconverted = 0;
                }
                item.total = staffportion;
                item.convertedtotal = staffportionconverted;
            }
            else
            {
                remoteportion = 0;
                remoteportionconverted = 0;
            }
            //no of others split
            if (reqsubcat.splitentertainment == true && numothers != 0) 
            {
                if (olditem != null)
                {
                    tempitem = olditem.entertainmentsplititem();
                    if (tempitem != null)
                    {
                        expenseid = tempitem.expenseid;
                    }
                    else
                    {
                        expenseid = 0;
                    }
                }
                else
                {
                    expenseid = 0;
                }
                splititem = new cExpenseItem(accountid, expenseid, item.itemtype, item.miles, item.pmiles, item.reason, item.receipt, item.net, 0, otherportion, reqsubcat.entertainmentid, item.date, 0, item.others, item.companyid, item.returned, item.home, item.refnum, item.claimid, item.plitres, item.blitres, item.currencyid, item.attendees, 0, item.countryid, item.foreignvat, otherportionconverted, item.exchangerate, item.tempallow, item.reasonid, item.normalreceipt, item.allowancestartdate, item.allowanceenddate, item.carid, item.allowancededuct, item.allowanceid, item.nonights, item.quantity, 0, item.amountpayable, item.hotelid, false, item.norooms, item.vatnumber, 0, 0, item.accountcode, item.basecurrency, item.globalbasecurrency, item.globalexchangerate, item.globaltotal, item.userdefined, item.floatid, item.corrected, item.receiptattached, item.transactionid, item.createdon, item.createdby, item.modifiedon, item.modifiedby, cAccounts.getConnectionString(accountid), item.mileageid, new SortedList<int,cJourneyStep>(), item.journeyunit, item.flags, item.costcodebreakdown, item.assignmentnum);

                splititem.setPrimaryItem(item);
                item.addSplitItem(splititem);

                item.others = 0;

            }

            if (reqsubcat.splitpersonal == true && numpersonal != 0)
            {
                if (olditem != null)
                {
                    tempitem = olditem.personalguestssplititem();
                    if (tempitem != null)
                    {
                        expenseid = tempitem.expenseid;
                    }
                    else
                    {
                        expenseid = 0;
                    }
                }
                else
                {
                    expenseid = 0;
                }
                splititem = new cExpenseItem(accountid, expenseid, item.itemtype, item.miles, item.pmiles, item.reason, item.receipt, item.net, 0, personalportion, reqsubcat.entertainmentid, item.date, 0, 0, item.companyid, item.returned, item.home, item.refnum, item.claimid, item.plitres, item.blitres, item.currencyid, item.attendees, 0, item.countryid, item.foreignvat, personalportionconverted, item.exchangerate, item.tempallow, item.reasonid, item.normalreceipt, item.allowancestartdate, item.allowanceenddate, item.carid, item.allowancededuct, item.allowanceid, item.nonights, item.quantity, 0, item.amountpayable, item.hotelid, false, item.norooms, item.vatnumber, numpersonal,0, item.accountcode, item.basecurrency, item.globalbasecurrency, item.globalexchangerate, item.globaltotal, item.userdefined, item.floatid, item.corrected, item.receiptattached, item.transactionid, item.createdon, item.createdby, item.modifiedon, item.modifiedby, cAccounts.getConnectionString(accountid), item.mileageid, new SortedList<int,cJourneyStep>(), item.journeyunit, item.flags, item.costcodebreakdown, item.assignmentnum);

                item.personalguests = 0;
                splititem.setPrimaryItem(item);
                item.addSplitItem(splititem);

            }


            if (reqsubcat.splitremote == true && numremote != 0)
            {
                if (olditem != null)
                {
                    tempitem = olditem.remoteworkerssplititem();
                    if (tempitem != null)
                    {
                        expenseid = tempitem.expenseid;
                    }
                    else
                    {
                        expenseid = 0;
                    }
                }
                else
                {
                    expenseid = 0;
                }
                splititem = new cExpenseItem(accountid, expenseid, item.itemtype, item.miles, item.pmiles, item.reason, item.receipt, item.net, 0, remoteportion, reqsubcat.entertainmentid, item.date, 0, 0, item.companyid, item.returned, item.home, item.refnum, item.claimid, item.plitres, item.blitres, item.currencyid, item.attendees, 0, item.countryid, item.foreignvat, remoteportionconverted, item.exchangerate, item.tempallow, item.reasonid, item.normalreceipt, item.allowancestartdate, item.allowanceenddate, item.carid, item.allowancededuct, item.allowanceid, item.nonights, item.quantity, 0, item.amountpayable, item.hotelid, false, item.norooms, item.vatnumber, 0, numremote, item.accountcode, item.basecurrency, item.globalbasecurrency, item.globalexchangerate, item.globaltotal, item.userdefined, item.floatid, item.corrected, item.receiptattached, item.transactionid, item.createdon, item.createdby, item.modifiedon, item.modifiedby, cAccounts.getConnectionString(accountid), item.mileageid, new SortedList<int,cJourneyStep>(), item.journeyunit, item.flags, item.costcodebreakdown, item.assignmentnum);
                splititem.setPrimaryItem(item);
                item.addSplitItem(splititem);
                item.remoteworkers = 0;

            }



        }

        public SortedList<int,Dictionary<FlagType, cFlaggedItem>> getFlags(int claimid)
        {
            int expenseid;
            SortedList<int, Dictionary<FlagType, cFlaggedItem>> lst = new SortedList<int, Dictionary<FlagType, cFlaggedItem>>();
            Dictionary<FlagType, cFlaggedItem> flags;
            FlagType flagtype;
            string comment;
            cFlaggedItem item;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            System.Data.SqlClient.SqlDataReader reader;
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);
            strsql = "select [savedexpenses_flags].expenseid, flagtype, comment from [savedexpenses_flags] inner join savedexpenses on savedexpenses.expenseid = savedexpenses_flags.expenseid where claimid = @claimid";
            reader = expdata.GetReader(strsql);
            expdata.sqlexecute.Parameters.Clear();
            while (reader.Read())
            {
                expenseid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                flagtype = (FlagType)reader.GetInt32(reader.GetOrdinal("flagtype"));
                comment = reader.GetString(reader.GetOrdinal("comment"));
                lst.TryGetValue(expenseid, out flags);
                if (flags == null)
                {
                    flags = new Dictionary<FlagType, cFlaggedItem>();
                    lst.Add(expenseid, flags);
                }
                item = new cFlaggedItem(flagtype, comment);
                flags.Add(flagtype, item);
            }
            reader.Close();
            return lst;
        }

        private void setAccountCode(ref cExpenseItem item, cSubcat subcat)
        {
            string accountcode = "";
            cReasons clsreasons = new cReasons(accountid);


            cReason reason = clsreasons.getReasonById(item.reasonid);

            if (reason == null)
            {
                if (subcat.alternateaccountcode != "")
                {
                    accountcode = subcat.alternateaccountcode;
                    item.accountcode = accountcode;
                }
                return;
            }

            if (subcat.alternateaccountcode != "")
            {
                accountcode = subcat.alternateaccountcode;
            }
            else if (item.receipt == true && reason.accountcodevat != "")
            {
                accountcode = reason.accountcodevat;
            }
            else if (item.receipt == false && reason.accountcodenovat != "")
            {
                accountcode = reason.accountcodenovat;
            }
            item.accountcode = accountcode;

        }

        public SortedList<int,List<cDepCostItem>> getCostCodeBreakdown(int claimid)
        {
            SortedList<int, List<cDepCostItem>> lst = new SortedList<int, List<cDepCostItem>>();
            List<cDepCostItem> breakdown;
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            int departmentid, costcodeid, percentused, projectcodeid, expenseid;
            expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);

            System.Data.SqlClient.SqlDataReader reader;





            strsql = "select savedexpenses_costcodes.expenseid, departmentid, costcodeid, percentused, projectcodeid from [savedexpenses_costcodes] inner join savedexpenses on savedexpenses_costcodes.expenseid = savedexpenses.expenseid where savedexpenses.claimid = @claimid";

            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                expenseid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                if (reader.IsDBNull(reader.GetOrdinal("departmentid")) == false)
                {
                    departmentid = reader.GetInt32(reader.GetOrdinal("departmentid"));
                }
                else
                {
                    departmentid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("costcodeid")) == false)
                {
                    costcodeid = reader.GetInt32(reader.GetOrdinal("costcodeid"));
                }
                else
                {
                    costcodeid = 0;
                }
                if (reader.IsDBNull(reader.GetOrdinal("projectcodeid")) == false)
                {
                    projectcodeid = reader.GetInt32(reader.GetOrdinal("projectcodeid"));
                }
                else
                {
                    projectcodeid = 0;
                }
                percentused = reader.GetInt32(reader.GetOrdinal("percentused"));
                lst.TryGetValue(expenseid, out breakdown);
                if (breakdown == null)
                {
                    breakdown = new List<cDepCostItem>();
                    lst.Add(expenseid, breakdown);
                }
                breakdown.Add(new cDepCostItem(departmentid, costcodeid, projectcodeid, percentused));

            }
            reader.Close();

            expdata.sqlexecute.Parameters.Clear();
            return lst;
        }

        private void InsertCostCodeBreakdown(bool edit, cExpenseItem item)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;

            List<cDepCostItem> breakdown = item.costcodebreakdown;
            if (item.parent != null)
            {
                breakdown = item.parent.costcodebreakdown;
            }

            if (breakdown.Count == 0) //no items
            {
                return;
            }

            if (edit) //delete current breakdown
            {
                deleteCostCodeBreakdown(item.expenseid);
            }

            int i = 0;

            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
            foreach (cDepCostItem costcode in breakdown)
            {
                if (costcode.percentused > 0)
                {
                    if (costcode.departmentid == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@departmentid" + i, DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@departmentid" + i, costcode.departmentid);
                    }
                    if (costcode.costcodeid == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@costcodeid" + i, DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@costcodeid" + i, costcode.costcodeid);
                    }
                    if (costcode.projectcodeid == 0)
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid" + i, DBNull.Value);
                    }
                    else
                    {
                        expdata.sqlexecute.Parameters.AddWithValue("@projectcodeid" + i, costcode.projectcodeid);
                    }
                    expdata.sqlexecute.Parameters.AddWithValue("@percentused" + i, costcode.percentused);
                    strsql = "insert into [savedexpenses_costcodes] (expenseid, departmentid, costcodeid, percentused,projectcodeid) " +
                        "values (" + item.expenseid + ",@departmentid" + i + ",@costcodeid" + i + ",@percentused" + i + ",@projectcodeid" + i + ")";


                    expdata.ExecuteSQL(strsql);
                    i++;
                }
            }

            expdata.sqlexecute.Parameters.Clear();




        }
        private void deleteCostCodeBreakdown(int expenseid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            string strsql;
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            strsql = "delete from [savedexpenses_costcodes] where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        private void calculateAmountPayable(ref cExpenseItem item, cExpenseItem olditem, cSubcat subcat)
        {
            cMisc clsmisc = new cMisc(accountid);
            cGlobalProperties clsproperties = clsmisc.getGlobalProperties(accountid);
            decimal amountpayable;
            bool ccusersettles = false;

            if ((item.itemtype == ItemType.CreditCard || item.itemtype == ItemType.PurchaseCard) && item.transactionid > 0)
            {
                cCardStatements clsstatements = new cCardStatements(accountid);
                cCardTransaction transaction = clsstatements.getTransactionById(item.transactionid);
                cCardStatement statement = clsstatements.getStatementById(transaction.statementid);
                ccusersettles = statement.corporatecard.claimantsettlesbill;
            }
            if (((item.itemtype == ItemType.CreditCard || item.itemtype == ItemType.PurchaseCard) && !ccusersettles) || !subcat.reimbursable)
            {
                amountpayable = 0;
            }

            else if (item.floatid != 0)
            {
                decimal allocatedamount = 0;
                cFloats clsfloats = new cFloats(accountid);
                cFloat reqfloat = clsfloats.GetFloatById(item.floatid);
                if (olditem != null)
                {
                    if (olditem.floatid == item.floatid) //add allocation back on, adding exisitng app
                    {
                        allocatedamount = olditem.total;
                    }
                }
                amountpayable = reqfloat.calculateFloatValue(item.expenseid, item.total, allocatedamount);
            }
            else
            {
                amountpayable = item.total;
            }

            item.amountpayable = Math.Round(amountpayable, 2);
        }
        #endregion

        #region receipts
        public void attachreceipt(cExpenseItem item, string filename, int employeeid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            DateTime createdon = DateTime.Now.ToUniversalTime();
            
            
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
            expdata.sqlexecute.Parameters.AddWithValue("@filename", filename);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", createdon);
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeid);

            strsql = "insert into receipts (expenseid, filename, createdon, createdby) " +
                "values (@expenseid,@filename, @createdon, @createdby)";
            expdata.ExecuteSQL(strsql);

            strsql = "update savedexpenses_current set receiptattached = 1 where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();

            item.receiptattached = true;
        }

        public void deleteReceipt(cExpenseItem item)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            sAttachedReceipt receipt;
            receipt = getReceiptById(item.expenseid);
            System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
            System.IO.File.Delete(appinfo.Server.MapPath(receipt.filename));

            expdata.sqlexecute.Parameters.AddWithValue("@receiptid", receipt.receiptid);
            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", item.expenseid);
            strsql = "delete from receipts where receiptid = @receiptid";
            expdata.ExecuteSQL(strsql);

            strsql = "update savedexpenses_current set receiptattached = 0 where expenseid = @expenseid;update savedexpenses_previous set receiptattached = 0 where expenseid = @expenseid";
            expdata.ExecuteSQL(strsql);

            expdata.sqlexecute.Parameters.Clear();
            item.receiptattached = false;
        }

        public SortedList<int, sReceiptFileInfo> getModifiedReceipts(DateTime date, int employeeid)
        {
            cAccounts acc = new cAccounts();
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            expdata.sqlexecute.Parameters.AddWithValue("@globalOffDate", date);
            int expid;
            int claimid = 0;
            int claimEmpId = 0;
            string filename;
            SortedList<int, string> lstOnlineReceipts = new SortedList<int, string>();

            strsql = "SELECT expenseid, filename FROM receipts WHERE createdon > @globalOffDate;";
            reader = expdata.GetReader(strsql);

            while (reader.Read())
            {
                expid = reader.GetInt32(reader.GetOrdinal("expenseid"));
                filename = reader.GetString(reader.GetOrdinal("filename"));
                if (!lstOnlineReceipts.ContainsKey(expid))
                {
                    lstOnlineReceipts.Add(expid, filename);
                }
            }
            reader.Close();
            expdata.sqlexecute.Parameters.Clear();

            int curItemCount;
            SortedList<int, sReceiptFileInfo> lstReceiptFiles = new SortedList<int, sReceiptFileInfo>();

            foreach (KeyValuePair<int, string> kp in lstOnlineReceipts)
            {
                curItemCount = 0;
                expdata.sqlexecute.Parameters.AddWithValue("@expenseid", kp.Key);
                strsql = "SELECT claimid FROM savedexpenses_current WHERE expenseid = @expenseid;";

                reader = expdata.GetReader(strsql);

                while (reader.Read())
                {
                    claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                    curItemCount++;
                }
                reader.Close();
                if (curItemCount == 0)
                {
                    strsql = "SELECT claimid FROM savedexpenses_previous WHERE expenseid = @expenseid;";

                    reader = expdata.GetReader(strsql);

                    while (reader.Read())
                    {
                        claimid = reader.GetInt32(reader.GetOrdinal("claimid"));
                    }
                    reader.Close();
                }

                if (claimid != 0)
                {
                    expdata.sqlexecute.Parameters.AddWithValue("@claimid", claimid);

                    strsql = "SELECT employeeid FROM claims_base WHERE claimid = @claimid;";

                    reader = expdata.GetReader(strsql);

                    while (reader.Read())
                    {
                        claimEmpId = reader.GetInt32(reader.GetOrdinal("employeeid"));
                    }
                    reader.Close();
                    sReceiptFileInfo recInfo = new sReceiptFileInfo();

                    if (claimEmpId == employeeid)
                    {
                        recInfo.expid = 0;
                        recInfo.filename = kp.Value; 
                        string fileName = kp.Value.Remove(0, 9);

                        try
                        {
                            recInfo.recFile = System.IO.File.ReadAllBytes(acc.getFilePath(FilePathType.receipt, accountid) + "\\" + fileName);

                            lstReceiptFiles.Add(kp.Key, recInfo);
                        }
                        catch
                        {
                        }
                    }
                }
                expdata.sqlexecute.Parameters.Clear();

            }
            

            return lstReceiptFiles;
        }

        public sAttachedReceipt getReceiptById(int expenseid)
        {
            DBConnection expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            System.Data.SqlClient.SqlDataReader reader;
            sAttachedReceipt receipt = new sAttachedReceipt();

            expdata.sqlexecute.Parameters.AddWithValue("@expenseid", expenseid);
            strsql = "select * from receipts where expenseid = @expenseid";
            reader = expdata.GetReader(strsql);
            while (reader.Read())
            {
                receipt.expenseid = expenseid;
                receipt.receiptid = reader.GetInt32(reader.GetOrdinal("receiptid"));
                receipt.filename = reader.GetString(reader.GetOrdinal("filename"));
                if (reader.IsDBNull(reader.GetOrdinal("createdon")) == true)
                {
                    receipt.createdon = new DateTime(1900, 01, 01);
                }
                else
                {
                    receipt.createdon = reader.GetDateTime(reader.GetOrdinal("createdon"));
                }
                if (reader.IsDBNull(reader.GetOrdinal("createdby")) == true)
                {
                    receipt.createdby = 0;
                }
                else
                {
                    receipt.createdby = reader.GetInt32(reader.GetOrdinal("createdby"));
                }
            }
            reader.Close();
            

            expdata.sqlexecute.Parameters.Clear();
            return receipt;
        }
        #endregion

    }


	public struct sAttachedReceipt
	{
		public int receiptid;
		public int expenseid;
		public string filename;
        public DateTime createdon;
        public int createdby;
	}

}

public struct sExpenseItemDetails
{
    public int expenseid;
    public ItemType itItemtype;
    public bool receipt;
    public decimal net;
    public decimal vat;
    public decimal total;
    public int subcatid;
    public decimal miles;
    public decimal bmiles;
    public decimal pmiles;
    public byte staff;
    public byte others;
    public int floatid;
    public int companyid;

    public bool home;
    public int plitres;
    public int blitres;
    public string attendees;
    public decimal tip;
    public bool normalreceipt;
    public bool receiptattached;
    public DateTime allowancestartdate;
    public DateTime allowanceenddate;
    public int nopassengers;
    public int carid;
    public decimal allowancededuct;
    public int allowanceid;
    public byte nonights;
    public byte norooms;
    public double quantity;
    public byte directors;
    public decimal amountpayable;
    public int hotelid;

    public string vatnumber;
    public byte personalguests;
    public byte remoteworkers;
    public string accountcode;
    public bool purchasecard;
    public bool creditcard;
    public int transactionid;
    public Dictionary<int, object> userdefined;
    public int mileageid;
    public SortedList<int, cJourneyStep> journeysteps;
    public int currencyid;
    public MileageUOM unit;
    public double exchangerate;
    public DateTime date;
    public int reasonid;
    public string otherdetails;
    public string assignmentnum;
}




