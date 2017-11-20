using System;
using System.Collections.Generic;
using OdometerReading = SpendManagementApi.Models.Types.OdometerReading;

namespace UnitTest2012Ultimate.API.Stubs
{
    using System.IO;
    using System.Reflection;

    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;

    using SpendManagementLibrary;
    using DateRangeType = SpendManagementLibrary.DateRangeType;
    using OdometerReading = OdometerReading;
    using RangeType = SpendManagementApi.Common.Enums.RangeType;
    using SignoffType = SpendManagementApi.Common.Enums.SignoffType;

    class RequestStubCreator<T>
    {
        private string ReadResource(string testKey)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "UnitTest2012Ultimate.API.Stubs." + testKey + ".json";
            string result = string.Empty;

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (var reader = new StreamReader(stream))
                {
                    result = reader.ReadToEnd();
                }
            }

            return result;
        }

        public T GetRequest(string testKey)
        {
            string jsonRequest = this.ReadResource(testKey);
            return new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<T>(jsonRequest);
        }

        internal static Department GetValidDepartment()
        {
            return new Department
            {
                Archived = false,
                Description = "TestDescription",
                EmployeeId = GlobalTestVariables.EmployeeId,
                Label = "TestLabel"
            };
        }

        internal static Employee GetValidEmployee()
        {
            Employee employee = new Employee
                                    {
                                        EmployeeDetails = new EmployeeDetails
                                                             {
                                                                 ContactDetails = new EmploymentContactDetails
                                                                 {
                                                                     ExtensionNumber = "4023",
                                                                     FaxNumber = "01234567890",
                                                                     MobileNumber = "01234567890",
                                                                     PagerNumber = "01234567890",
                                                                     EmailAddress = "work@business.com",
                                                                     TelephoneNumber = "01234567890"
                                                                 },
                    EmployeePermissions =  new EmployeePermissions
                    {
                        DefaultSubAccountId = 1,
                        AccessRoles = new List<int>()
                    },
                                                                 WorkDetails = new WorkDetails
                                                                                   {
                                                                                       CreditAccount = "Creditor",
                                                                                       PayRollNumber = "P00001",
                                                                                       Position = "Position",
                                                                                       NationalInsuranceNumber = "HH060666B",
                                                                                       HireDate = new DateTime(2014, 02, 17),
                                                                                       TerminationDate = new DateTime(2014, 04, 27),
                                                                                       EmployeeNumber = "E1000",
                                                                                       PrimaryCountryId = 1258,
                        PrimaryCurrencyId = 0,
                        LineManagerUserId = GlobalTestVariables.EmployeeId,
                                                                                       StartMileage = 1000,
                                                                                       StartMileageDate = new DateTime(2014, 04, 28),
                        CostCentreBreakdowns = new List<CostCentreBreakdown>{
                                                                                                                         new CostCentreBreakdown
                                                                                                                             {
                                                                                                                                 DepartmentId = 0,
                                                                                                        CostCodeId  = 0,
                                                                                                                                 ProjectCodeId = 0,
                                                                                                                                 Percentage = 100}
                                                                                                                             }
                                                                                                                  },
                                                                                  
                                                                 NhsDetails = new NhsDetails
                                                                 {
                        TrustId = cGlobalVariables.NHSTrustID,
                                                                     NhsUniqueId = "Nhs123"
                                                                 },
                                                                 PersonalDetails = new PersonalDetails
                                                                 {
                                                                     BasicInfo = new OptionalGeneralDetails
                                                                     {
                                                                         Gender = "Male",
                                                                         DateOfBirth = new DateTime(1980, 1, 1),
                                                                         MiddleName = "Middle",
                                                                         MaidenName = "Maiden",
                                                                         PreferredName = "Tester",
                                                                         LocaleId = 50
                                                                     },
                                                                     HomeContactDetails = new HomeContactDetails
                                                                     {
                                                                         EmailAddress = "test@test.com",
                                                                         TelephoneNumber = "20202020202",
                                                                         FaxNumber = "01234567890"
                                                                     }
                                                                 },

                                                                 ClaimSignOffDetails = new ClaimSignOffDetails
                                                                 {
                                                                     SignOffGroupId = 3148,
                                                                     CreditCardSignOffGroupId = 2204,
                                                                     PurchaseCardSignOffGroupId = 879,
                                                                     AdvancesSignOffGroupId = 2584
                                                                 },
                                                             },
                UserName = "Test" + DateTime.UtcNow,
                                        Title = "Mr",
                                        Forename = "Test1",
                                        Surname = "Test1",
                                        IsActive = true,
                                        Archived = true
                                    };
            return employee;
        }

        internal static Currency GetValidCurrency()
        {
            Currency currency = new Currency
                                    {
                                        Archived = true,
                                        GlobalCurrencyId = 1
                                    };
            return currency;
        }

        internal static Country GetValidCountry()
        {
            Country country = new Country
            {
                Archived = true,
                VatRates = new List<VatRate>
                               {
                                   new VatRate
                                       {
                                           Vat = 100,
                                           VatPercent = 100
                                       }
                               }
            };
            return country;
        }

        internal static ExpenseSubCategory GetValidExpenseSubCategory()
        {
            ExpenseSubCategory expenseSubCategory = new ExpenseSubCategory
                                                        {
                                                            AccountCode = "Code",
                                                            AccountId = GlobalTestVariables.AccountId,
                                                            AddAsNet = true,
                                                            AllowanceAmount = 10,
                                                            AlternateAccountCode = "Alt",
                                                            CalculationType = SpendManagementApi.Common.Enums.CalculationType.DailyAllowance,
                                                            ParentCategoryId = 0,
                                                            Comment = "Comment",
                                                            Description = "Description",
                                                            EmployeeId = GlobalTestVariables.EmployeeId,
                                                            PdCatId = 0,
                                                            Reimbursable = true,
                                                            ShortSubCategory = "Short",
                SubCat = "Long" + DateTime.UtcNow,
                                                            SubCatId = 0,
                                                            VatRates = new List<SubCatVatRate>
                                                                           {
                                                                                new SubCatVatRate
                                                                                    {
                                                                                        RangeType = (SpendManagementApi.Common.Enums.DateRangeType)(Enum.Parse(typeof(SpendManagementApi.Common.Enums.DateRangeType), DateRangeType.Between.ToString())),
                                                                                        DateValue1 = new DateTime(2012, 1, 1),
                                                                                        DateValue2 = new DateTime(2012, 2, 2),
                                                                                        VatAmount = 100,
                                                                                        VatPercent = 100,
                                                                                        VatLimitWith = 100,
                                                                                        VatLimitWithout = 100,
                                                                                        VatReceipt = true
                                                                                    }                       
                                                                           },
                                                            AllowHeavyBulkyMileage = true,
                Allowances = new List<int> (),
                                                            AssociatedUdfs = new List<int> { 2387 },
                                                            AttendeesApplicable = true,
                                                            AttendeesMandatory = true,
                                                            BusinessMilesApplicable = true,
                                                            CompanyApplicable = true,
              
                Countries = new List<CountrySubCat>(),
                                                            EnableHomeToLocationMileage = true,
                                                            EndDate = new DateTime(2012, 12, 12),
                                                            EventInHomeApp = true,
                                                            FromApplicable = true,
                HomeToLocationType = SpendManagementApi.Common.Enums.HomeToLocationType.None,
                                                            HomeToOfficeAsZero = true,
                                                            HotelApplicable = true,
                                                            HotelMandatory = true,
                                                            IsRelocationMileage = true,
                                                            MileageApplicable = true,
                                                            NoDirectorsApplicable = true,
                                                            NoNightsApplicable = true,
                                                            NoPassengersApplicable = true,
                                                            NoPersonalGuestApplicable = true,
                                                            NoRemoteWorkersApplicable = true,
                                                            NoRoomsApplicable = true,
                                                            OtherDetailsApplicable = true,
                                                            OthersApplicable = true,
                                                            PassengersApplicable = true,
                                                            PassengersNameApplicable = true,
                                                            PersonalMilesApplicable = true,
                                                            ReasonApplicable = true,
                                                            ReceiptApplicable = true,
                                                            SplitRemote = true,
                                                            SplitEntertainment = true,
                                                            SplitPersonal = true,
                                                            StaffApplicable = true,
                                                            StartDate = new DateTime(2012, 1, 1),
                                                            TipApplicable = true,
                                                            ToApplicable = true,
                                                            VatNumberApplicable = true,
                                                            VatNumberMandatory = true
                                                        };
            return expenseSubCategory;
        }

        internal static Vehicle GetValidEmployeeCar()
        {
            Vehicle vehicle = new Vehicle
            {
                Approved = true,
                CarUsageStartDate = new DateTime(2012, 1, 1),
                CarUsageEndDate = new DateTime(2014, 1, 1),
                EngineSize = 1800,
                EmployeeId = GlobalTestVariables.EmployeeId,
                FuelType = 2,
                IsActive = true,
                IsExemptFromHomeToLocationMileage = false,
                Make = "TestMake",
                Model = "TestModel",
                Registration = "TestReg",
                UnitOfMeasure = MileageUOM.Mile
            };
            return vehicle;
        }

        internal static SignOffGroup GetValidSignOffGroup()
        {
            SignOffGroup group = new SignOffGroup();
            group.GroupId = 10;
            group.GroupName = "TestGroupName";
            group.Description = "TestGroupDescription";
            group.OneClickAuthorization = true;
            group.Stages = new List<Stage>
                               {
                                   new Stage
                                       {
                                           Amount = 10,
                                           ApproveHigherLevelsOnly = false,
                                           ClaimantMail = true,
                                           DisplayDeclaration = true,
                                           ExtraLevels = 1,
                                           HolidayId = 1,
                                           HolidayType = (SignoffType)1,
                                           IncludeId = 1,
                                           StageInclusionType = (SpendManagementApi.Common.Enums.StageInclusionType)1,
                                           Notify = (Notify)2,
                                           OnHolidayProvision = (HolidayProvision)3,
                                           Relid = 1,
                                           SendMail = false,
                                           SignOffId = 1,
                                           SignOffStage = 1,
                                           SignOffType = SignoffType.BudgetHolder,
                                           SingleSignOff = false
                                       }
                               };
            return group;
        }

        internal static MileageCategory GetValidMileageCategory()
        {
            MileageCategory mileageCategory = new MileageCategory
                                                  {
                                                      AccountId = GlobalTestVariables.AccountId,
                                                      CalculateNewJourneyTotal = false,
                                                      Label = "Label",
                                                      Comment = "Comment",
                                                      Currency = 1,
                                                      NhsMileageCode = "Nhs Mileage Code",
                                                      DateRanges =
                                                          new List<DateRange>
                                                              {
                                                                  new DateRange
                                                                      {
                                                                          DateRangeType = SpendManagementApi.Common.Enums.DateRangeType.Between,
                                                                          DateValue1 = new DateTime(2012, 1, 1),
                                                                          DateValue2 = new DateTime(2012, 12, 12),
                                                                          MileageCategoryId = 10,
                                                                          Thresholds = new List<Threshold>{
                                                                                      new Threshold{
                                                                                              HeavyBulkyEquipmentRate
                                                                                                  =
                                                                                                  2,
                                                                                              MileageDateId
                                                                                                  =
                                                                                                  10,
                                                                                              Passenger1Rate
                                                                                                  =
                                                                                                  1,
                                                                                              PassengerXRate
                                                                                                  =
                                                                                                  2,
                                                                                              PencePerMileDiesel
                                                                                                  =
                                                                                                  1,
                                                                                              PencePerMileDieselEuroV
                                                                                                  =
                                                                                                  0,
                                                                                              PencePerMileElectric
                                                                                                  =
                                                                                                  0,
                                                                                              PencePerMileHybrid
                                                                                                  =
                                                                                                  0,
                                                                                              PencePerMileLpg
                                                                                                  =
                                                                                                  0,
                                                                                              PencePerMilePetrol
                                                                                                  =
                                                                                                  0,
                                                                                              RangeType
                                                                                                  =
                                                                                                  RangeType
                                                                                                  .Any,
                                                                                              RangeValue1
                                                                                                  =
                                                                                                  1,
                                                                                              RangeValue2
                                                                                                  =
                                                                                                  2,
                                                                                              VatAmountDiesel
                                                                                                  =
                                                                                                  20,
                                                                                              VatAmountDieselEuroV
                                                                                                  =
                                                                                                  0,
                                                                                              VatAmountElectric
                                                                                                  =
                                                                                                  0,
                                                                                              VatAmountHybrid
                                                                                                  =
                                                                                                  0,
                                                                                              VatAmountLpg
                                                                                                  =
                                                                                                  0,
                                                                                              VatAmountPetrol
                                                                                                  =
                                                                                                  0
                                                                                          }
                                                                                  }
                                                                      }
                                                              }
                                                  };
            return mileageCategory;
        }


        internal static ItemRole GetValidItemRole()
        {
            return new ItemRole
                       {
                           AccountId = GlobalTestVariables.AccountId,
                           Description = "Description",
                           EmployeeId = GlobalTestVariables.EmployeeId,
                           RoleName = "RoleName"
                       };
        }
    }
}
