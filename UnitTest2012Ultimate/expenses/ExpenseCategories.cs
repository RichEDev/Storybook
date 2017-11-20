namespace UnitTest2012Ultimate
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using Moq;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.FinancialYears;
    using SpendManagementLibrary.Employees;

    using Spend_Management;

    /// <summary>
    /// Units tests for Class "ExpenseCategories"
    /// </summary>
    [TestClass]
    public class ExpenseCategories
    {
        /// <summary>
        /// The employee id.
        /// </summary>
        private int employeeid;

        /// <summary>
        /// The account id.
        /// </summary>
        private int accountId;

        /// <summary>
        /// The  sub accounts.
        /// </summary>
        private cAccountSubAccounts clsSubAccounts;

        /// <summary>
        /// The employee.
        /// </summary>
        private Employee employee;

        /// <summary>
        /// The account properties.
        /// </summary>
        private cAccountProperties clsProperties;

        /// <summary>
        /// The expense category.
        /// </summary>
        private cSubcat subcat;

        /// <summary>
        /// The journey steps.
        /// </summary>
        private SortedList<int, cJourneyStep> journeySteps;

        /// <summary>
        /// The created on.
        /// </summary>
        private DateTime createdon;

        /// <summary>
        /// The expense item.
        /// </summary>
        private cExpenseItem expItem;

        /// <summary>
        /// The amount to pay.
        /// </summary>
        private decimal amountpay;

        /// <summary>
        /// The total.
        /// </summary>
        private decimal total;

        /// <summary>
        /// The employees.
        /// </summary>
        private cEmployees employees;

        /// <summary>
        /// The miscellaneous.
        /// </summary>
        private cMisc misc;

        /// <summary>
        /// The properties.
        /// </summary>
        private cGlobalProperties properties;

        /// <summary>
        /// The old item.
        /// </summary>
        private cExpenseItem oldItem = cExpenseItemObject.Template(1, 1);

        /// <summary>
        /// The vat object.
        /// </summary>
        private cVat clsvat;

        /// <summary>
        /// The car.
        /// </summary>
        private cCar car;

        /// <summary>
        /// The mileage thresholds.
        /// </summary>
        private List<cMileageThreshold> mileageThresholds;

        /// <summary>
        /// The date range.
        /// </summary>
        private cMileageDaterange dateRange;

        /// <summary>
        /// The date ranges.
        /// </summary>
        private List<cMileageDaterange> dateRanges;

        /// <summary>
        /// The mileage category.
        /// </summary>
        private cMileageCat reqmileage;

        /// <summary>
        /// The expense items.
        /// </summary>
        private cExpenseItems expenseItems;

        /// <summary>
        /// The mileage cats.
        /// </summary>
        private cMileagecats mileageCats;

        /// <summary>
        /// The mileage item.
        /// </summary>
        private cMileageCat mileageItem;

        /// <summary>
        /// The test context instance.
        /// </summary>
        private TestContext testContextInstance;

        /// <summary>
        /// Gets or sets the test context which provides
        /// information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext
        {
            get
            {
                return this.testContextInstance;
            }

            set
            {
                this.testContextInstance = value;
            }
        }

        #region Additional test attributes

        /// <summary>
        /// The my class initialize.
        /// </summary>
        /// <param name="testContext">
        /// The test context.
        /// </param>
        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }

        #endregion

        /// <summary>
        /// Test to verify the calculation of mileage Home to Office
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Base Definitions"), TestCategory("Mileage")]
        public void ExpItemsCalculateMileageWhenUsingHomeToOfficeAsNone()
        {
            Mock<ICurrentUser> currentUser = Moqs.CurrentUserMock();
            currentUser.SetupGet(x => x.Account).Returns(new cAccounts().GetAccountByID(GlobalTestVariables.AccountId));

            this.subcat = SubcatObject.Template(1);
            this.SetupDataForMileageTests();

            this.expItem.addJourneyStep();

            this.expItem.journeysteps[0].startlocation = new Address { Identifier = 12345, Postcode = "YO14 0AE" };
            this.expItem.journeysteps[0].endlocation = new Address { Identifier = 12346, Postcode = "LN5 8NW" };
            this.expItem.journeysteps[0].nummiles = 100;

            var homeAddressId = 12347;
            var workAddressId = 12348;
            var homeToOfficeDistance = 10.4m;
            var officeToHomeDistance = 11.1m;

            this.expenseItems.IterateThroughStepsToCalculateHomeToOfficeMileage(ref this.expItem, this.subcat, this.car, homeAddressId, workAddressId, homeToOfficeDistance, officeToHomeDistance, this.mileageCats, currentUser.Object, null);
            
            Assert.IsTrue(this.expItem.journeysteps[0].nummiles == 100, string.Format("Miles = {0}, was expecting 100.", this.expItem.journeysteps[0].nummiles));
        }

        /// <summary>
        /// Test of cSubCat to make sure that all relevant Expense Types are included in the returned list.
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Base Definitions"), TestCategory("Expenses")]
        public void SubCatTestThatAllRelevantExpenseItemsAreIncludedInListForCardReconcilation()
        {
            this.employeeid = GlobalTestVariables.EmployeeId;
            this.accountId = GlobalTestVariables.AccountId;
            var subCatList = new List<int>();
            var cats = new cCategories(this.accountId);
            var category = cats.CachedList()[0];
            var subCats = new cSubcats(this.accountId);

            subCatList.Add(CreateSubCatTypeX(CalculationType.ItemReimburse, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.DailyAllowance, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.FixedAllowance, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.FuelCardMileage, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.FuelReceipt, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.Meal, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.NormalItem, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.PencePerMile, subCats, category));
            subCatList.Add(CreateSubCatTypeX(CalculationType.PencePerMileReceipt, subCats, category));
            subCats = new cSubcats(this.accountId);
            var result = subCats.CreateDropDownForCardTransactions();
            bool foundNormal = false, foundFuelReceipt = false, foundFuelCardMileage = false, foundMeal = false;
            foreach (ListItem listItem in result)
            {
                if (listItem.Text.StartsWith("UT "))
                {
                    if (listItem.Text.Contains(CalculationType.NormalItem.ToString()))
                    {
                        foundNormal = true;
                    }

                    if (listItem.Text.Contains(CalculationType.FuelReceipt.ToString()))
                    {
                        foundFuelReceipt = true;
                    }

                    if (listItem.Text.Contains(CalculationType.FuelCardMileage.ToString()))
                    {
                        foundFuelCardMileage = true;
                    }

                    if (listItem.Text.Contains(CalculationType.Meal.ToString()))
                    {
                        foundMeal = true;
                    }   
                }
            }

            // Clear up created data
            foreach (int i in subCatList)
            {
                subCats.DeleteSubcat(i);
            }

            Assert.IsTrue(foundNormal, "Normal Item not found in list");
            Assert.IsTrue(foundFuelReceipt, "FuelReceipt Item not found in list");
            Assert.IsTrue(foundFuelCardMileage, "fuel Card Mileage Item not found in list");
            Assert.IsTrue(foundMeal, "Meal Item not found in list");
        }

        /// <summary>
        /// Test that the correct mileage comment (that appears on the Add claim and my claims page) is returned for each mileage type.
        /// </summary>
        [TestMethod, TestCategory("Expenses"), TestCategory("Mileage")]
        public void MileageExpenseGetMileageComment()
        {
            this.accountId = GlobalTestVariables.AccountId;
            var cats = new cCategories(this.accountId);
            var category = cats.CachedList()[0];
            var subCats = new cSubcats(this.accountId);
            string comment;

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.DeductHomeToOfficeFromEveryJourney);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("for your home to office distance will be made to your journey."), string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.CalculateHomeAndOfficeToLocationDiff);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("If you start your journey from home and the distance to your location is greater than if you start your journey from the office, then the 'Amount Payable' total will deduct the difference between home to your location and office to your address."), string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.DeductFirstAndLastHome);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("The deduction will be made on the first and last home address found in your journey."), string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.DeductFullHomeToOfficeEveryTimeHomeIsVisited);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("The full distance deduction will be made each and every time home is visited during a journey, deducting off adjacent steps if less than required deductible distance."), string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.DeductHomeToOfficeEveryTimeHomeIsVisited);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("The deduction will be made every time your home address is visited in your journey."), string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.DeductHomeToOfficeIfStartOrFinishHome);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("The deduction will be made if you start or end the journey from your home address."), string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.FlagHomeAndOfficeToLocationDiff);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("If you start your journey from home and the distance to your location is greater than if you start your journey from the office, then the 'Amount Payable' total will flag the difference between home to your location and office to your address."), string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.None);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment == string.Empty, string.Format("Incorrect message returned for {0}", this.subcat.HomeToLocationType));

            this.subcat = SubcatObject.Template(1, hometolocationtype: HomeToLocationType.None, homeToOfficeAsZero: true);
            comment = cSubcat.GetMileageText(this.subcat, 1, "mile", "miles", 2, "miles");
            Assert.IsTrue(comment.EndsWith("will have mileage enforced to zero."), string.Format("Incorrect message returned for {0}, when Home to office enforced as zero.", this.subcat.HomeToLocationType));
        }

        /// <summary>
        /// Test to verify the calculation of mileage Home to Office
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Base Definitions"), TestCategory("Mileage")]
        public void ExpItemsCalculateMileageWhenUsingCalculateHomeAndOfficeToLocationDiffWithHomeToOfficeAsZeroSet()
        {
            this.RunTestForHomeToOfficeAlwaysZero(HomeToLocationType.CalculateHomeAndOfficeToLocationDiff, true, true, 0, "Mileage should be zero as mileage should be adjusted by home to office mileage.");
        }

        /// <summary>
        /// Test to verify the calculation of mileage Home to Office
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Base Definitions"), TestCategory("Mileage")]
        public void ExpItemsCalculateMileageWhenUsingFlagHomeAndOfficeToLocationDiffWithHomeToOfficeAsZeroSet()
        {
            this.RunTestForHomeToOfficeAlwaysZero(HomeToLocationType.FlagHomeAndOfficeToLocationDiff, true, true, 0, "Mileage should be 0 as mileage should be adjusted by home to office mileage.");
        }

        /// <summary>
        /// Test to verify the calculation of mileage Home to Office
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Base Definitions"), TestCategory("Mileage")]
        public void ExpItemsCalculateMileageWhenUsingCalculateHomeAndOfficeToLocationDiffWithHomeToOfficeAsZeroNotSet()
        {
            this.RunTestForHomeToOfficeAlwaysZero(HomeToLocationType.CalculateHomeAndOfficeToLocationDiff, false, true, 100, "Mileage should be 100 as mileage should not be adjusted by home to office mileage.");
        }

        /// <summary>
        /// Test to verify the calculation of mileage Home to Office
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Base Definitions"), TestCategory("Mileage")]
        public void ExpItemsCalculateMileageWhenUsingFlagHomeAndOfficeToLocationDiffWithHomeToOfficeAsZeroNotSet()
        {
            this.RunTestForHomeToOfficeAlwaysZero(HomeToLocationType.FlagHomeAndOfficeToLocationDiff, false, true, 100, "Mileage should not be 100 as mileage should be adjusted by home to office mileage.");
        }

        /// <summary>
        /// Test to verify the calculation of mileage Home to Office
        /// </summary>
        [TestMethod, TestCategory("Spend Management"), TestCategory("Base Definitions"), TestCategory("Mileage")]
        public void ExpItemsCalculateMileageWhenUsingNoneWithHomeToOfficeAsZeroNotSet()
        {
            this.RunTestForHomeToOfficeAlwaysZero(HomeToLocationType.None, false, true, 100, "Mileage should not be 100 as mileage should be adjusted by home to office mileage.");
        }

        /// <summary>
        /// The run test for home to office always zero.
        /// </summary>
        /// <param name="calcType">
        /// The calculation type.
        /// </param>
        /// <param name="homeToOfficeAlwaysZero">
        /// The home to office always zero.
        /// </param>
        /// <param name="assertEquals">
        /// The assert equals.
        /// </param>
        /// <param name="assertValue">
        /// The assert value.
        /// </param>
        /// <param name="assertMessage">
        /// The assert message.
        /// </param>
        private void RunTestForHomeToOfficeAlwaysZero(HomeToLocationType calcType, bool homeToOfficeAlwaysZero, bool assertEquals, decimal assertValue, string assertMessage)
        {
            Mock<ICurrentUser> currentUserMock = Moqs.CurrentUserMock();
            currentUserMock.SetupGet(x => x.Account).Returns(new cAccounts().GetAccountByID(GlobalTestVariables.AccountId));
            ICurrentUser currentUser = currentUserMock.Object;

            this.subcat = SubcatObject.Template(1, hometolocationtype: calcType, homeToOfficeAsZero: homeToOfficeAlwaysZero);
            this.SetupDataForMileageTests();

            this.expItem.addJourneyStep();

            var homeAddressId = 12347;
            var workAddressId = 12348;
            var homeToOfficeDistance = 10.4m;
            var officeToHomeDistance = 11.1m;

            var homeAddress = new Address { Identifier = homeAddressId, Postcode = "YO14 0AE" };
            var officeAddress = new Address { Identifier = workAddressId, Postcode = "LN5 8NW" };

            // Test that home to office returns zero mileage
            this.expItem.journeysteps[0].startlocation = homeAddress;
            this.expItem.journeysteps[0].endlocation = officeAddress;
            this.expItem.journeysteps[0].nummiles = 100;

            this.expenseItems.IterateThroughStepsToCalculateHomeToOfficeMileage(ref this.expItem, this.subcat, this.car, homeAddressId, workAddressId, homeToOfficeDistance, officeToHomeDistance, this.mileageCats, currentUser, null);
            if (assertEquals)
            {
                Assert.IsTrue(this.expItem.journeysteps[0].nummiles == assertValue, assertMessage);    
            }
            else
            {
                Assert.IsTrue(this.expItem.journeysteps[0].nummiles != assertValue, assertMessage);
            }
            
            // Test that office to home returns zero mileage
            this.expItem.journeysteps[0].startlocation = officeAddress;
            this.expItem.journeysteps[0].endlocation = homeAddress;
            this.expItem.journeysteps[0].nummiles = 100;

            this.expenseItems.IterateThroughStepsToCalculateHomeToOfficeMileage(ref this.expItem, this.subcat, this.car, homeAddressId, workAddressId, homeToOfficeDistance, officeToHomeDistance, this.mileageCats, currentUser, null);
            if (assertEquals)
            {
                Assert.IsTrue(this.expItem.journeysteps[0].nummiles == assertValue, assertMessage);    
            }
            else
            {
                Assert.IsTrue(this.expItem.journeysteps[0].nummiles != assertValue, assertMessage);
            }
        }

        /// <summary>
        /// The create sub cat type x.
        /// </summary>
        /// <param name="calculationType">
        /// The calculation type.
        /// </param>
        /// <param name="subCats">
        /// The subcategories.
        /// </param>
        /// <param name="category">
        /// The category.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int CreateSubCatTypeX(CalculationType calculationType, cSubcats subCats, cCategory category)
        {
            var catText = "UT " + calculationType;
            return subCats.SaveSubcat(SubcatObject.Template(category.categoryid, calculation: calculationType, description: catText, subcat: catText));
        }

        /// <summary>
        /// The setup data for mileage tests.
        /// </summary>
        private void SetupDataForMileageTests()
        {
            this.employeeid = GlobalTestVariables.EmployeeId;
            this.accountId = GlobalTestVariables.AccountId;
            this.clsSubAccounts = new cAccountSubAccounts(this.accountId);
            this.employee = new cEmployees(this.accountId).GetEmployeeById(this.employeeid);
            this.clsProperties = this.clsSubAccounts.getSubAccountById(this.employee.DefaultSubAccount).SubAccountProperties;
            if (!this.clsProperties.UseMapPoint)
            {
                this.clsProperties.UseMapPoint = true;
                this.clsSubAccounts.SaveAccountProperties(this.clsProperties, this.employeeid, null);
            }

            this.journeySteps = new SortedList<int, cJourneyStep>();
            this.createdon = DateTime.Parse("01/01/2012");

            this.expItem = cExpenseItemObject.Template(this.subcat.subcatid, 1, journeysteps: this.journeySteps);
            this.amountpay = new decimal();
            this.total = new decimal();
            this.employees = new cEmployees(this.accountId);
            this.misc = new cMisc(this.accountId);
            this.properties = null;
            this.oldItem = cExpenseItemObject.Template(1, 1);
            this.clsvat = new cVat(this.accountId, ref this.expItem, this.employees.GetEmployeeById(this.employeeid), this.misc, this.properties, this.oldItem);
            this.car = new cCar();
            this.mileageThresholds = new List<cMileageThreshold>();
            this.dateRange = new cMileageDaterange(1, 1, null, null, this.mileageThresholds, DateRangeType.Any, this.createdon, this.employeeid, null, null);
            this.dateRanges = new List<cMileageDaterange> { this.dateRange };
            var financialYear = FinancialYear.GetPrimary(cMisc.GetCurrentUser());
            this.reqmileage = new cMileageCat(1, string.Empty, string.Empty, ThresholdType.Journey, true, this.dateRanges, MileageUOM.Mile, true, string.Empty, 1, this.createdon, this.employeeid, null, null, string.Empty, 0, 0, financialYear.FinancialYearID);

            this.expenseItems = new cExpenseItems(this.accountId);
            this.mileageCats = new cMileagecats(this.accountId);
            var list = this.mileageCats.CreateDropDown();
            this.mileageItem = this.mileageCats.GetMileageCatById(int.Parse(list[0].Value));
            this.expItem.mileageid = this.mileageItem.mileageid;
        }
    }
}
