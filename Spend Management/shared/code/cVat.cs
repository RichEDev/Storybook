using System;
using SpendManagementLibrary;
using Spend_Management;

namespace Spend_Management
{
    using SpendManagementLibrary.Employees;

    /// <summary>
	/// Summary description for cVat.
	/// </summary>
    public class cVat : IVat
    {

        private int nAccountid;


        private DBConnection expdata;
        private string strsql;

        private int homeCountry;

        cExpenseItem clsexpenseitem;
        cExpenseItem olditem;
        Employee reqemp;
        cMisc clsmisc;
        cGlobalProperties clsproperties;

        //public cVat(ref cExpenseItem item, int employeeid, int action,cExpenseItem itemold)
        //{
        //    System.Web.HttpApplication appinfo = (System.Web.HttpApplication)System.Web.HttpContext.Current.ApplicationInstance;
        //    cEmployees clsemployees = new cEmployees();
        //    cEmployee reqemp = clsemployees.GetEmployeeById(int.Parse(appinfo.User.Identity.Name));
        //    nAccountid = reqemp.accountid;
        //    nAction = action;

        //    reqitem = item;
        //    olditem = itemold;
        //    nEmployeeid = employeeid;

        //    basecurrency = getBaseCurrency();

        //}

        public cVat(int accountid, ref cExpenseItem expenseitem, Employee employee, cMisc misc, cGlobalProperties properties, cExpenseItem itemold)
        {
            nAccountid = accountid;
            expdata = new DBConnection(cAccounts.getConnectionString(accountid));
            clsexpenseitem = expenseitem;
            olditem = itemold;
            reqemp = employee;
            clsmisc = misc;
            clsproperties = properties;
        }

        public int accountid
        {
            get { return nAccountid; }
        }

        private cExpenseItem expenseitem
        {
            get { return clsexpenseitem; }
        }
        public void calculateVAT()
        {
            int subcatid = 0;

            cSubcat reqsubcat;

            cSubcats clssubcats = new cSubcats(accountid);

            subcatid = expenseitem.subcatid;
            reqsubcat = clssubcats.GetSubcatById(subcatid);

            if (reqsubcat == null)
            {
                return;
            }

            cSubcatVatRate clsvatrate = reqsubcat.getVatRateByDate(expenseitem.date);
            if (clsvatrate != null)
            {
                if (expenseitem.receipt == true && clsvatrate.vatlimitwith != 0)
                {
                    if (expenseitem.total > clsvatrate.vatlimitwith)
                    {
                        expenseitem.updateVAT(expenseitem.total, 0, expenseitem.total);
                        return;
                    }
                }
                else if (expenseitem.receipt == false && clsvatrate.vatlimitwithout != 0)
                {
                    if (expenseitem.total > clsvatrate.vatlimitwithout)
                    {
                        expenseitem.updateVAT(expenseitem.total, 0, expenseitem.total);
                        return;
                    }

                }
            }

            //check vat limit
            int employeeCountry;

            if (reqemp.PrimaryCountry != 0)
            {
                employeeCountry = reqemp.PrimaryCountry;
            }
            else
            {
                employeeCountry = clsproperties.homecountry;
            }

            if (reqsubcat.calculation == CalculationType.PencePerMile && expenseitem.countryid == employeeCountry)
            {
                homeCountry = employeeCountry;
            }
            else
            {
                homeCountry = clsproperties.homecountry;
            }
            if (homeCountry != expenseitem.countryid) //foreign country. calculate foreign vat
            {
                calculateForeignVAT();
                return;
            }

            switch (reqsubcat.calculation)
            {
                case CalculationType.NormalItem: //normal expense
                case CalculationType.DailyAllowance:
                case CalculationType.FuelReceipt:
                case CalculationType.PencePerMileReceipt:
                case CalculationType.FixedAllowance:
                case CalculationType.FuelCardMileage:
                    calculateNormalVAT(reqsubcat);

                    break;

                case CalculationType.Meal: //meal
                    calculateMealVAT(reqsubcat);

                    break;
                //case CalculationType.PencePerMile: //mileage
                //    if (olditem != null) //see if the mileage has changed
                //    {

                //        if (olditem.miles == expenseitem.miles && olditem.nopassengers == expenseitem.nopassengers && olditem.mileageid == expenseitem.mileageid)
                //        {
                //            expenseitem.updateVAT(olditem.net, olditem.vat, olditem.total);
                //            return;
                //        }
                //    }
                //    calculateMileageVAT(reqsubcat);
                //    break;

            }

            return;
        }

        private void calculateForeignVAT()
        {
            int count = 0;
            decimal foreignvat = 0;
            expdata.sqlexecute.Parameters.AddWithValue("@countryid", expenseitem.countryid);
            expdata.sqlexecute.Parameters.AddWithValue("@subcatid", expenseitem.subcatid);
            strsql = "select count(*) as vatcount from countrysubcats where countryid = @countryid and subcatid = @subcatid";
            count = expdata.getcount(strsql);


            if (count != 0) //foreign vat rate exists for subcat/country
            {
                System.Data.SqlClient.SqlDataReader vatreader;
                double vatpercent = 0;
                double vat = 0;
                strsql = "select vat, vatpercent from countrysubcats where countryid = @countryid and subcatid = @subcatid";

                using (vatreader = expdata.GetReader(strsql))
                {
                    while (vatreader.Read())
                    {
                        vat = vatreader.GetDouble(vatreader.GetOrdinal("vat"));
                        if (vatreader.IsDBNull(vatreader.GetOrdinal("vatpercent")) == false)
                        {
                            vatpercent = vatreader.GetDouble(vatreader.GetOrdinal("vatpercent"));
                        }
                        else
                        {
                            vatpercent = 0;
                        }
                    }
                    vatreader.Close();
                }

                if (reqemp.PrimaryCountry == expenseitem.countryid)
                {

                    foreignvat = (expenseitem.total / (100 + (decimal)vat)) * (decimal)vat;
                }
                else
                {
                    if (expenseitem.convertedtotal > 0)
                    {
                        foreignvat = (expenseitem.convertedtotal / (100 + (decimal)vat)) * (decimal)vat;
                    }
                    else
                    {
                        //Different country but same currency
                        foreignvat = (expenseitem.total / (100 + (decimal)vat)) * (decimal)vat;
                    }
                }

                foreignvat = Math.Round((foreignvat / 100) * (decimal)vatpercent, 2, MidpointRounding.AwayFromZero);



                expenseitem.updateVAT(expenseitem.total, 0, expenseitem.total, foreignvat);
            }
            else
            {
                expenseitem.updateVAT(expenseitem.total, 0, expenseitem.total);
            }



            expdata.sqlexecute.Parameters.Clear();
        }
        private void calculateNormalVAT(cSubcat reqsubcat)
        {
            decimal net = 0;
            decimal vat = 0;
            decimal total = 0;

            double vatrate = 0;
            int vatpercent = 0;
            total = expenseitem.total;

            cSubcatVatRate clsvatrate = reqsubcat.getVatRateByDate(expenseitem.date);

            if (clsvatrate != null && expenseitem.countryid == homeCountry) //is vat applicable and a UK expense
            {
                //only calculate vat if don't need a receipt or they have a receipt
                if ((clsvatrate.vatreceipt == true && expenseitem.receipt == true) || clsvatrate.vatreceipt == false)
                {
                    vatrate = clsvatrate.vatamount;
                    vatpercent = clsvatrate.vatpercent;

                    total = expenseitem.total;
                    vat = Math.Round((total / (100 + (decimal)vatrate)) * (decimal)vatrate, 2, MidpointRounding.AwayFromZero);
                    if (vatpercent != 100)
                    {
                        vat = Math.Round((vat / 100) * vatpercent, 2, MidpointRounding.AwayFromZero);
                    }
                    net = total - vat;
                }
                else
                {

                    net = total;
                    vat = 0;
                }
            }
            else
            {
                net = total;
            }

            expenseitem.updateVAT(net, vat, expenseitem.total);

        }

        public void calculateVehicleJourneyVAT(cSubcat reqsubcat, ref decimal vat, decimal costperunit, decimal fuelcost, decimal total)
        {
            double vatrate = 0;
            int vatpercent = 0;

            if (costperunit == 0)
            {
                return;
            }

            cSubcatVatRate clsvatrate = reqsubcat.getVatRateByDate(expenseitem.date);

            if (clsvatrate != null && expenseitem.currencyid == clsproperties.basecurrency) //is vat applicable and a UK expense
            {
                //only calculate vat if don't need a receipt or they have a receipt


                if ((clsvatrate.vatreceipt == true && expenseitem.receipt == true) || (expenseitem.receipt == false && clsvatrate.vatreceipt == false))
                {
                    vatrate = clsvatrate.vatamount;
                    vatpercent = clsvatrate.vatpercent;


                    decimal tempvat = 0;
                    if (vatpercent != 100)
                    {
                        tempvat = (decimal)(fuelcost / costperunit) * (total / (decimal)(100 + vatrate)) * (decimal)vatrate;
                        tempvat = (vat / 100) * vatpercent;

                        vat += tempvat;
                    }
                    else
                    {
                        tempvat = (decimal)(fuelcost / costperunit) * (total / (decimal)(100 + vatrate)) * (decimal)vatrate;

                        vat += tempvat;
                    }
                }
                else
                {
                    vat += 0;
                }
            }

            //update the users mileage
            //if (reqmileage.calcmilestotal == true)
            //{
            //    int year = expenseitem.date.Year;

            //    totalmiles = calculateTotalMiles(totalmiles);

            //    if (expenseitem.date < new DateTime(year,04,06))
            //    {
            //        year--;
            //    }
            //    //updateTotalMiles(year,totalmiles);
            //    //clsemployees.removeActiveEmployee(employeeid);
            //}
            return;
        }


        //private void updateTotalMiles (int year, int totalmiles)
        //{
        //    int count;
        //    strsql = "select count(*) from employee_mileagetotals where employeeid = @employeeid and financial_year = @year";
        //    expdata.sqlexecute.Parameters.AddWithValue("@employeeid",reqemp.employeeid);
        //    expdata.sqlexecute.Parameters.AddWithValue("@year", year);
        //    expdata.sqlexecute.Parameters.AddWithValue("@totalmiles", totalmiles);

        //    count = expdata.getcount(strsql);

        //    if (count == 0) //insert
        //    {
        //        strsql = "insert into employee_mileagetotals (employeeid, financial_year, mileagetotal) " +
        //            "values (@employeeid, @year, @totalmiles)";
        //    }
        //    else
        //    {
        //        strsql = "update employee_mileagetotals set mileagetotal = @totalmiles where employeeid = @employeeid and financial_year = @year";
        //    }

        //    expdata.ExecuteSQL(strsql);

        //    expdata.sqlexecute.Parameters.Clear();
        //}
        private void calculateMealVAT(cSubcat reqsubcat)
        {
            decimal net = 0;
            decimal vat = 0;
            decimal total = 0;
            double vatrate = 0;
            int vatpercent = 0;
            int staff = 0;
            int others = 0;
            int directors = 0;
            decimal tip = 0;
            bool athome = false;
            if (expenseitem.staff == 0 && expenseitem.directors == 0) //no staff specified so can't calculate
            {
                return;
            }

            total = expenseitem.total;

            cSubcatVatRate clsvatrate = reqsubcat.getVatRateByDate(expenseitem.date);

            if (clsvatrate != null && expenseitem.countryid == homeCountry) //is vat applicable and a UK expense
            {
                if ((clsvatrate.vatreceipt == true && expenseitem.receipt == true) || clsvatrate.vatreceipt == false)
                {
                    vatrate = (double)clsvatrate.vatamount;
                    vatpercent = clsvatrate.vatpercent;

                    total = expenseitem.total;
                    directors = expenseitem.directors;
                    staff = expenseitem.staff + expenseitem.remoteworkersgrandtotal;
                    if (expenseitem.staff == 0 && expenseitem.directors == 0)
                    {
                        staff = 1;
                    }
                    others = expenseitem.othergrandtotal + expenseitem.personalguestsgrandtotal;
                    athome = expenseitem.home;
                    tip = expenseitem.tip;

                    if (athome == false) //we can claim VAT for staff and directors
                    {


                        //tip = (tip / (others + staff + directors)) * (staff + directors);
                        vat = ((expenseitem.grandtotal - (decimal)tip) / (100 + (decimal)vatrate)) * (decimal)vatrate;
                        if (vatpercent != 100)
                        {
                            vat = (vat / 100) * vatpercent;
                        }

                        vat = Math.Round((vat / (others + staff + directors)) * (staff + directors), 2, MidpointRounding.AwayFromZero);
                        net = total - vat;
                    }
                    else //we can only claim the VAT for staff
                    {

                        //tip = (tip / (others + staff + directors)) * staff;
                        vat = ((expenseitem.grandtotal - (decimal)tip) / (100 + (decimal)vatrate)) * (decimal)vatrate;
                        if (vatpercent != 100)
                        {
                            vat = (vat / 100) * vatpercent;
                        }
                        if (staff > 0)
                        {
                            vat = Math.Round((vat / (others + staff + directors)) * (staff), 2, MidpointRounding.AwayFromZero);
                        }
                        else
                        {
                            vat = 0;
                        }
                            net = total - vat;
                        
                    }

                    expenseitem.updateVAT(net, vat, total);
                }
                else
                {
                    net = total;
                    expenseitem.updateVAT(net, 0, total);

                }

            }
            else
            {
                net = total;
                expenseitem.updateVAT(net, 0, total);
            }
        }

        private decimal calculateTotalMiles(decimal totalmiles)
        {
            decimal difference = 0;
            if (olditem == null) //just add normal amount
            {
                totalmiles += expenseitem.miles;
            }
            else
            {
                if (olditem.miles != expenseitem.miles) //need to update
                {
                    if (olditem.miles < expenseitem.miles) //mileage has increased
                    {
                        difference = expenseitem.miles - expenseitem.miles;
                        totalmiles += difference;
                    }
                    else //mileage has decreased
                    {
                        difference = olditem.miles - expenseitem.miles;
                        totalmiles -= difference;
                    }
                }
            }

            return totalmiles;

        }


    }
}
