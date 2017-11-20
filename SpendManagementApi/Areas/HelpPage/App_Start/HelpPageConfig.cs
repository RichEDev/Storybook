using SpendManagementLibrary; // Must be left outside the namespace

namespace SpendManagementApi.Areas.HelpPage
{
    using System;
using System.Collections.Concurrent;
    using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Web;
using System.Web.Http;
    using System.Web.Http.Description;

    using Common.Enums;

    using SpendManagementApi.Common.Enum;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Types.Expedite;
    using SpendManagementApi.Models.Requests.SelfRegistration;
    using SpendManagementApi.Models.Responses.SelfRegistration;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;

    using SpendManagementLibrary.Mobile;

    using AttributeFormat = SpendManagementLibrary.AttributeFormat;
    using CostCentreBreakdown = SpendManagementApi.Models.Types.Employees.CostCentreBreakdown;
    using Currency = SpendManagementApi.Models.Types.Currency;
    using EmployeeBasic = SpendManagementApi.Models.Types.Employees.EmployeeBasic;
    using ExpenseItem = SpendManagementApi.Models.Types.ExpenseItem;

    /// <summary>
    /// Use this class to customize the Help Page.
    /// For example you can set a custom <see cref="IDocumentationProvider"/> to supply the documentation
    /// or you can provide the samples for the requests/responses.
    /// </summary>
    public static class HelpPageConfig
    {
        /// <summary>
        /// An in-memory version of the API Type Library, to save all the reflection time in constantly regenerating the type docs.
        /// </summary>
        public static ConcurrentBag<TypeLibrary> ApiTypeLibrary { get; set; }

        public static void Register(HttpConfiguration config)
        {
            //// Uncomment the following to use the documentation from XML documentation file.
            config.SetDocumentationProvider(
                new XmlDocumentationProvider(
                    HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["XmlDocumentationUrl"])));

            var expenseSubCat = new ExpenseSubCategory
            {
                AccountCode = "Code",
                AddAsNet = true,
                AllowanceAmount = 10,
                AlternateAccountCode = "Alt",
                CalculationType = CalculationType.DailyAllowance,
                ParentCategoryId = 0,
                Comment = "Comment",
                Description = "Description",
                PdCatId = 0,
                Reimbursable = true,
                ShortSubCategory = "Short",
                SubCat = "Long",
                SubCatId = 0,
                VatRates = new List<SubCatVatRate>
                {
                    new SubCatVatRate
                    {
                        RangeType = DateRangeType.Between,
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
                Allowances = new List<int> {1287},
                AssociatedUdfs = new List<int> {2387},
                AttendeesApplicable = true,
                AttendeesMandatory = true,
                BusinessMilesApplicable = true,
                CompanyApplicable = true,
                Countries = null,
                EnableHomeToLocationMileage = true,
                EndDate = new DateTime(2014, 12, 12),
                EventInHomeApp = true,
                FromApplicable = true,
                HomeToLocationType = HomeToLocationType.None,
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
                StartDate = new DateTime(2014, 1, 1),
                TipApplicable = true,
                ToApplicable = true,
                VatNumberApplicable = true,
                VatNumberMandatory = true
            };

            var udf = new UserDefinedField
            {
                Attribute =
                    new cListAttribute(0, "AttributeName", "DisplayName", "Description", "Tooltip", false,
                        FieldType.List, new DateTime(2000, 1, 1), 1, null, null, null, Guid.Empty, false, true,
                        (AttributeFormat)Common.Enum.AttributeFormat.ListStandard, true, true, false, false)
            };


            var expenseItem = new ExpenseItem
                                  {
                                      Flags = null,
                                      AccountId = 1,
                                      Id = 0,
                                      CurrencySymbol = "£",
                                      ModifiedOn = new DateTime(2014, 12, 25),
                                      ReferenceNumber = "Ref234234",
                                      EmployeeId = 12323,
                                      ExpenseActionOutcome = ExpenseActionOutcome.Successs,
                                      BaseCurrency = 1,
                                      CountryId = 1,
                                      BankAccountId = 1,
                                      CreatedOn = new DateTime(2014, 12, 25),
                                      Reason = "Expense Reason",
                                      JourneySteps = null,
                                      ClaimId = 1223,
                                      AccountCode = "Acc1637",
                                      AddedAsMobileExpense = true,
                                      AddedByMobileDeviceTypeId = 1,
                                      AllowanceDeduction = 1,
                                      AllowanceEndDate = new DateTime(2014, 12, 25),
                                      AllowanceId = 1,
                                      AllowanceStartDate = new DateTime(2014, 12, 25),
                                      AmountPayable = 100,
                                      Attendees = "staff",
                                      BaseCurrencyGlobal = 1,
                                      BlockedFlags = null,
                                      CarId = 1,
                                      ClaimReasonId = 1,
                                      ClaimReasonInfo = "Claim Reason",
                                      CompanyId = 1,
                                      CompanyName = "Dummy Company",
                                      ConvertedTotal = 10,
                                      Corrected = false,
                                      CostCentreBreakdowns = null,
                                      CreatedById = 0,
                                      CurrencyId = 1,
                                      Date = new DateTime(2014, 12, 25),
                                      DisputeNotes = "Dispute Note",
                                      DistanceUom = MileageUom.Mile,
                                      ESRAssignmentId = 1,
                                      Edited = false,
                                      EsrAssignmentLocationId = 1,
                                      EventInHomeCity = false,
                                      ExchangeRate = 1,
                                      ExchangeRateGlobal = 2,
                                      ExpenseSubCategoryId = 2123,
                                      FloatId = 1,
                                      ForeignVAT = 10,
                                      FromId = 1,
                                      FuelLitresBusiness = 1,
                                      FuelLitresPersonal = 2,
                                      GlobalTotal = 100,
                                      GrandTotal = 50,
                                      GrandTotalAmountPayable = 20,
                                      GrandTotalConverted = 0,
                                      GrandTotalGlobal = 20,
                                      GrandTotalNet = 10,
                                      GrandTotalOther = 1,
                                      GrandTotalPersonalGuests = 5,
                                      GrandTotalRemoteWorkers = 2,
                                      GrandTotalVAT = 10,
                                      HomeToOfficeDeductionMethod =
                                          HomeToLocationType.CalculateHomeAndOfficeToLocationDiff,
                                      HotelId = 1,
                                      HotelName = "California",
                                      ItemCheckerId = null,
                                      ItemCheckerTeamId = null,
                                      ItemType = ExpenseItemType.Cash,
                                      MileageBusiness = 1,
                                      MileageId = 1,
                                      MileagePersonal = 10,
                                      Miles = 50,
                                      ModifiedById = 2,
                                      Net = 100,
                                      NormalReceipt = true,
                                      Notes = "Notes",
                                      NumDirectors = 3,
                                      NumNights = 1,
                                      NumOthers = 1,
                                      NumPassengers = 2,
                                      NumPersonalGuests = 3,
                                      NumRemoteWorkers = 5,
                                      NumRooms = 1,
                                      NumStaff = 10,
                                      OperatorValidationProgress =
                                          ExpediteOperatorValidationProgress.Available,
                                      OriginalExpenseId = 10,
                                      Paid = false,
                                      ParentId = 10,
                                      PrimaryItem = true,
                                      Quantity = 1,
                                      ReceiptAttached = true,
                                      Returned = true,
                                      SplitItems = null,
                                      TempAllow = false,
                                      Tip = 10,
                                      ToId = 1,
                                      Total = 100,
                                      TransactionId = 0,
                                      UserDefined = null,
                                      UserSaysHasReceipts = true,
                                      VAT = 10,
                                      VATNumber = "VAT34245",
                                      ValidationProgress = ExpenseValidationProgress.InProgress,
                                      ValidationResults = null
                                  };


            var expenseItemDefinition = new ExpenseItemDefinition
            {
                ExpenseItem = expenseItem,
                ClaimReasons = new List<ClaimReason>
                {
                    new ClaimReason
                    {
                         AccountId = 1,
                         AccountCodeNoVat = "No VAT Code",
                          AccountCodeVat = "No VAT Code",
                          CreatedById = 234234,
                          CreatedOn = new DateTime(2014, 12, 25),
                          Description = "Description",
                          EmployeeId = 35543,
                          EsrAssignmentLocationId = 42345,
                          Id = 45,
                          Label = "Reason Label",
                          ModifiedById = 442,
                          ModifiedOn = new DateTime(2014, 12, 25)
                    }
                },
                CostCenterBreakdowns = new List<CostCentreBreakdown>
                                           {
                                               new CostCentreBreakdown
                                                   {
                                                       AccountId = 23432,
                                                       CostCodeId = 5345,
                                                       CreatedById = 345,
                                                       CreatedOn = new DateTime(2014, 12, 25),
                                                       DepartmentId = 55,
                                                       EmployeeId = 4421,
                                                       EsrAssignmentLocationId = 3323,
                                                       ModifiedOn = new DateTime(2014, 12, 25),
                                                       ModifiedById = 52334,
                                                       Percentage = 100
                                                   }
                                           },
                CountryList = new List<Country>
                                  {
                                      new Country
                                          {
                                              AccountId = 34234,
                                              Archived = true,
                                              CountryId = 24423,
                                              CountryName = "UK",
                                              CreatedById = 434,
                                              CreatedOn = new DateTime(2014, 12, 25),
                                              EmployeeId = 343423,
                                              EsrAssignmentLocationId = 3442,
                                              GlobalCountryId = 534,
                                              ModifiedById = 234234
                                          }
                                  },
                CanEditCostCode = true,
                CanEditDepartment = true,
                CanEditProjectCode = true,
                CurrencyList = new List<Currency>
                                   {
                                       new Currency
                                       {
                                              AccountId = 34234,
                                              Archived = true,
                                              CurrencyId = 24423,
                                              CurrencyName = "Sterling",
                                              CreatedById = 434,
                                              CreatedOn = new DateTime(2014, 12, 25),
                                              EmployeeId = 343423,
                                              EsrAssignmentLocationId = 3442,
                                              CurrencyType = CurrencyType.Static,
                                              ModifiedById = 234234
                                       }
                                   },

                DefaultGlobalCountryId = 133,
                EmployeeBankAccounts = null,
                ExchangeRateReadOnly = false,
                PostCodeAnywhereCountries = new List<GlobalCountry>
                                                {
                                                    new GlobalCountry
                                                        {
                                                           Alpha3CountryCode = "GBR",
                                                            Country = "United Kingdom",
                                                            CountryCode = "345345",
                                                            GlobalCountryid = 2323,
                                                            Numeric3CountryCode = 346,
                                                            PostcodeAnywhereEnabled = true,
                                                            PostcodeRegex = "?%%$£"
                                                        }
                                                },
                PrimaryCountryId = 34345,
                PrimaryCurrencyId = 5353,
                CountrySettings = new CostcodeSettings()
                {
                    CanAddFromField = true,
                    DisplayForCash = true,
                    DisplayAs = "Country",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true
                },
                CostcodeSettings = new CostcodeSettings
                {
                    CanAddFromField = true,
                    DisplayForCash = true,
                    DisplayAs = "Costcode",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true
                },
                CurrencySettings = new CurrencySettings()
                {
                    DisplayForCash = true,
                    DisplayAs = "Currency",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddNewCurrency = true
                },
                DepartmentSettings = new DepartmentSettings()
                {
                    DisplayForCash = true,
                    DisplayAs = "Department",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddFromField = true
                },
                FromSettings = new FromSettings()
                {
                    DisplayForCash = true,
                    DisplayAs = "From",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddFromField = true
                },
                OrganisationSettings = new OrganisationSettings()
                {
                    DisplayForCash = true,
                    DisplayAs = "Company",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddNewOrganisation = true
                },
                OtherDetailsSettings = new OtherDetailsSettings()
                {
                    DisplayForCash = true,
                    DisplayAs = "Other",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddNewOtherDetails = true
                },
                ProjectcodeSettings = new ProjectcodeSettings
                {
                    DisplayForCash = true,
                    DisplayAs = "Other",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddFromField = true
                },
                ReasonSettings = new ReasonSettings()
                {
                    DisplayForCash = true,
                    DisplayAs = "Other",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddNewReason = true
                },
                ToSettings = new ToSettings()
                {
                    DisplayForCash = true,
                    DisplayAs = "Other",
                    DisplayForCreditCard = true,
                    DisplayForPurchaseCard = true,
                    DisplayOnIndividualItem = true,
                    MandatoryForCash = true,
                    MandatoryForCreditCard = true,
                    MandatoryForPurchaseCard = true,
                    CanAddToField = true
                },
            };

            var claimExpenseItem = new ClaimExpenseItem { ExpenseItem = expenseItem, Subcat = expenseSubCat };
            var claimExpenseItems = new List<ClaimExpenseItem>();
            claimExpenseItems.Add(claimExpenseItem);

            //// Uncomment the following to use "sample string" as the sample for all actions that have string as the body parameter or return type.
            //// Also, the string arrays will be used for IEnumerable<string>. The sample objects will be serialized into different media type 
            //// formats by the available formatters.

            config.SetSampleObjects(new Dictionary<Type, object>
            {
                                                            {
                    typeof (SelfRegistrationInitiatorRequest), new SelfRegistrationInitiatorRequest
                    {
                                                                EmailAddress = "john.doe@software-europe.com",
                                                                Firstname = "John",
                                                                Surname = "Doe",
                                                                Title = "Mr",
                                                                Username = "john.doe",
                                                                Password = "TopSecret"
                    }
                },
                                                            {
                    typeof (SelfRegistrationResponse), new SelfRegistrationResponse
                    {
                        AccessRoles =
                            new List<IAccessRole>
                            {
                                new AccessRoleBasic
                                {
                                    Id = 1,
                                    Label = "Administrator",
                                    Description = "Description of the administrator role."
                                }
                            },
                        CarEngineTypes = new List<Tuple<int, string>> {new Tuple<int, string>(1, "Petrol")},
                        CostCodes = new List<Tuple<int, string>> {new Tuple<int, string>(1, "Sample Costcode")},
                        Countries = new List<Tuple<int, string>> {new Tuple<int, string>(1, "England")},
                        Currencies = new List<Tuple<int, string>> {new Tuple<int, string>(1, "Pound Sterling")},
                        Departments = new List<Tuple<int, string>> {new Tuple<int, string>(1, "Accounts Departments")},
                        LineManagers =
                            new List<IEmployee>
                            {
                                new EmployeeBasic
                                {
                                    Id = 1,
                                    Title = "Mr",
                                    Forename = "John",
                                    Surname = "Doe",
                                    UserName = "john.doe"
                                }
                            },
                        MileageUoMs = new List<Tuple<int, string>> {new Tuple<int, string>(1, "Diesel")},
                                                                PostCodeAnywhereLicenseKey = "SAMPLE_POSTCODEANYWHERE_KEY",
                        ProjectCodes = new List<Tuple<int, string>> {new Tuple<int, string>(1, "Sample Project Code")},
                        SignoffGroups =
                            new List<SignoffGroupBasic>
                            {
                                new SignoffGroupBasic
                                {
                                    GroupId = 1,
                                    GroupName = "Sample Signoff Group",
                                    Description = "Description of sample signoff group."
                                }
                            },
                        UserDefinedFields = new List<UserDefinedField> {udf}
                    }
                },
                {typeof (ExpenseSubCategory), expenseSubCat},
                {
                    typeof (GetUserDefinedFieldsResponse),
                    new GetUserDefinedFieldsResponse {List = new List<UserDefinedField> {udf}}
                },
                {typeof (UserDefinedFieldResponse), new UserDefinedFieldResponse {Item = udf}},
                {
                    typeof (ExpenseSubCategoryResponse),
                    new ExpenseSubCategoryResponse
                    {
                        Item = expenseSubCat,
                        ResponseInformation = new ApiResponseInformation()
                    }
                },
                {
                    typeof (List<Address>),
                    new List<Address>
                    {
                        new Address
                        {
                            Id = 1,
                            AddressName = "Address",
                            Archived = false,
                            City = "Lincoln",
                            Postcode = "LN63JY"
                        }
                    }
                },
                {
                    typeof (SignOffGroup),
                    new SignOffGroup
                {
                        GroupId = 10,
                        GroupName = "TestGroupName",
                        Description = "TestGroupDescription",
                        OneClickAuthorization = true,
                        Stages = new List<Stage>
                {
                                   new Stage
                                       {
                                           //AccountId = GlobalTestVariables.AccountId,
                                           Amount = 10,
                                           ApproveHigherLevelsOnly = false,
                                           ClaimantMail = true,
                                           DisplayDeclaration = true,
                                           ExtraLevels = 1,
                                           HolidayId = 1,
                                HolidayType = (SignoffType) 1,
                                           IncludeId = 1,
                                StageInclusionType = (StageInclusionType) 1,
                                Notify = (Notify) 2,
                                OnHolidayProvision = (HolidayProvision) 3,
                                           Relid = 1,
                                           SendMail = false,
                                           SignOffId = 1,
                                           SignOffStage = 1,
                                           SignOffType = SignoffType.BudgetHolder,
                                           SingleSignOff = false
                                       }
                               }
                    }
                },
                {
                    typeof (Currency),
                    new Currency
                    {
                        AccountId = 35,
                        Archived = false,
                        CurrencyType = CurrencyType.Monthly,
                        GlobalCurrencyId = 1,
                        EmployeeId = 21301
                    }
                },
                {
                    typeof (Country),
                    new Country
                    {
                        AccountId = 35,
                        Archived = false,
                        GlobalCountryId = 1,
                        EmployeeId = 21301,
                        VatRates = new List<VatRate>
                        {new VatRate {ExpenseSubCategoryId = 1, Vat = 100, VatPercent = 20}}
                    }
                },
                {
                    typeof (Employee),
                        new Employee
                        {
                            AccountId = 35,
                            Title = "MANDATORY",
                            UserName = "MANDATORY",
                            Forename = "MANDATORY",
                            Surname = "MANDATORY",
                            CurrentClaimReference = 0,
                            CurrentExpenseItemReference = 0,
                            Password = "",
                            LastPasswordChange = DateTime.UtcNow,

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
                                                        EmployeePermissions = new EmployeePermissions
                                                                                {
                                                                                    DefaultSubAccountId = 0,
                                                                                    AccessRoles = new List<int>()
                                                                                },

                                                        WorkDetails = new WorkDetails
                                                                        {

                                                                            CreditAccount = "Creditor",
                                                                            PayRollNumber = "P00001",
                                                                            Position = "Position",
                                                                            NationalInsuranceNumber = "HH060666B",
                                                                            HireDate = new DateTime(2014, 02, 17),
                                                                            TerminationDate = null,
                                                                            EmployeeNumber = "E1000",
                                                                            PrimaryCountryId = 0,
                                                                            PrimaryCurrencyId = 0,
                                                                            LineManagerUserId = 0,
                                                                            StartMileage = 1000,
                                                                            StartMileageDate = new DateTime(2014, 04, 28),
                                CostCentreBreakdowns = new List<CostCentreBreakdown>
                                {
                                                                                                                new CostCentreBreakdown
                                                                                                        {
                                                                                                                        DepartmentId = 0,
                                        CostCodeId = 0,
                                                                                                                        ProjectCodeId = 0,
                                        Percentage = 100
                                    }
                                                                                                                    }
                                                                                                        },
                                                        NhsDetails = new NhsDetails
                                                        {
                                                            TrustId = 0,
                                                            NhsUniqueId = "Nhs123"
                                                        },
                                                        PersonalDetails = new PersonalDetails
                                                        {

                                                            BasicInfo = new OptionalGeneralDetails
                                                            {
                                                                DateOfBirth = new DateTime(1980, 1, 1),
                                                                Gender = "Male",
                                                                MiddleName = "Middle",
                                                                MaidenName = "Maiden",
                                                                PreferredName = "Tester",
                                                                LocaleId = 0
                                                            },
                                                            HomeContactDetails = new HomeContactDetails
                                                            {
                                                                EmailAddress = "test@test.com",
                                                                TelephoneNumber = "20202020202",
                                                                FaxNumber = "01234567890"
                                                            },

                                                          BankAccount = new  SpendManagementApi.Models.Types.Employees.BankAccount
                                                            {
                                                                AccountHolderName = "Name",
                                                                AccountNumber = "0123456789",
                                                                AccountReference = "REFERENCE",
                                                                AccountType = "AccountType",
                                                                SortCode = "00-00-00"
                                                            }
                                                        },
                            ClaimSignOffDetails = new ClaimSignOffDetails
                            {
                                                            SignOffGroupId = 0,
                                                            CreditCardSignOffGroupId = 0,
                                                            PurchaseCardSignOffGroupId = 0,
                                                            AdvancesSignOffGroupId = 0
                                                        },
                                                        },
                        OwnedVehicles = new List<int> {0},
                        PoolCars = new List<int> {0},
                        HomeAddresses = new List<HomeAddressLinkage>
                        {
                            new HomeAddressLinkage
                            {
                                StartDate = new DateTime(2014, 02, 02),
                                                        Id = 0,
                                                        }
                                                    },
                        WorkAddresses = new List<WorkAddressLinkage>
                        {
                            new WorkAddressLinkage
                            {
                                                        IsTemporary = false,
                                                        IsActive = false,
                                StartDate = new DateTime(2014, 02, 02),
                                                        Id = 0,
                                                        }
                                                    },

                        CorporateCards = new List<int> {0},
                                                    IsActive = true,
                                                    Archived = true
                        }
                    },
                    {
                    typeof (List<cEmailNotification>),
                    new List<cEmailNotification>
                    {
                        new cEmailNotification(1, "Email", "Description", 1, true, CustomerType.Standard,
                            EmailNotificationType.StandardNotification)
                    }

                    },
                    {
                    typeof (Vehicle),
                        new Vehicle
                        {
                            AccountId = 0,
                            EmployeeId = null,
                            Id = 0,
                            Make = "Make",
                            Model = "Model",
                            Registration = "Registration",
                            UnitOfMeasure = (MileageUOM) MileageUom.Mile,
                            FuelType = 1,
                            EngineSize = 2000,
                            IsActive = true,
                            IsExemptFromHomeToLocationMileage = false,
                            CarUsageStartDate = null,
                            CarUsageEndDate = null,
                            MileageCategoryIds = new List<int> {1, 3},
                            Approved = true,
                            PoolCarUsers = new List<int>() {1},
                            OdometerReadings = new OdometerReadings
                            {
                                StartOdometerReading = 0,
                                EndOdometerReading = 0,
                            OdometerReadingList = new List<int>() {1, 2, 3},
                                OdometerReadingRequired = false
                            },
                            UserDefined = null,
                        }
                    },
                {
                    typeof (Address),
                    new Address
                    {
                        Id = 0,
                        AddressName = "Your Business",
                        Line1 = "Business House",
                        Line2 = "Business Lane",
                        Line3 = "Business Area",
                        County = "BusinessShire",
                        City = "Business City",
                        Country = 1,
                        Postcode = "BI2 N1S",
                        IsAccountWideFavourite = false,
                        AccountWideLabels = new List<int>(),
                        PrimaryAccountWideLabel = null,
                        RecommendedDistances = new List<int>(),
                        Archived = false,

                    }
                },
                {
                    typeof (HomeAddressLinkage),
                    new HomeAddressLinkage
                    {
                        Id = 0,
                        AddressId = 1,
                        EmployeeId = 1,
                        StartDate = new DateTime(2014, 02, 20),
                        EndDate = new DateTime(2014, 08, 25),
                    }
                },
                {
                    typeof (WorkAddressLinkage),
                    new WorkAddressLinkage
                    {
                        Id = 0,
                        AddressId = 1,
                        EmployeeId = 1,
                        StartDate = new DateTime(2014, 02, 20),
                        EndDate = new DateTime(2014, 08, 25),
                        IsActive = true,
                        IsTemporary = false
                    }
                },
                {
                    typeof (ExpenseValidationResult),
                    new ExpenseValidationResult
                    {
                        Id = 0,
                        Timestamp = new DateTime(2014, 12, 25),
                        BusinessStatus = ExpenseValidationStatus.Pass,
                        VATStatus = ExpenseValidationStatus.Fail,
                        PossiblyFraudulent = false,
                        CriterionId = 1,
                        Comments = "Custom Comments",
                        ExpenseItemId = 1213106
                    }
                },
                {
                    typeof(ExpenseItemResponse),
                    new ExpenseItemResponse
                    {
                        Item = expenseItem
                    }
                },
                  {
                    typeof(UpdateExpenseItemRequest),
                    new UpdateExpenseItemRequest
                    {
                        ExpenseItem = expenseItem,
                        OfflineItem = false,
                        OldClaimId = 1234445
                    }
                },
                   {
                    typeof(ExpenseItemDefinitionResponse),
                    new ExpenseItemDefinitionResponse
                    {
                        Item = expenseItemDefinition                   
                    }
                },

                {
                    typeof(ExpenseItem),
                    expenseItem
                },
                {
                    typeof(MobileJourney),
                    new MobileJourney
                        {
                             JourneyDateTime = new DateTime(2014, 02, 20),
                             JourneyId = 1,
                             SubcatId = 342,
                             Active = true,
                             JourneyDate = "20161117",
                             CreatedBy = 23424,
                             Steps = null,
                             EndTime = "11:00",
                             JourneyEndTime = new DateTime(2014, 02, 20),
                             JourneyJson = "[{\"StepId\":3,\"JourneyId\":2,\"StepNumber\":1,\"Line1\":\"Test Road\",\"City\":\"Lincoln\",\"Postcode\":\"LN6 0FF\",\"Latitude\":0.0,\"Longitude\":0.0,\"Country\":\"GB\",\"HeavyBulkyEquipment\":false,\"NumberOfPassengers\":0,\"PassengerNames\":\"\",\"NumberOfMiles\":0.0,\"RecordedMiles\":0.0},{\"StepId\":4,\"JourneyId\":2,\"StepNumber\":2,\"Line1\":\"Low Moor Road\",\"City\":\"Lincoln\",\"Postcode\":\"LN6 3JY\\",
                             JourneyStartTime = new DateTime(2014, 02, 20),
                             StartTime = "12:00",
                        }
                },
                {
                     typeof(ClaimExpenseItemsResponse),
                     new ClaimExpenseItemsResponse
                        {
                            List = claimExpenseItems,
                            DisplayFields = new List<DisplayField>()                           
                        }
                }
                       
            });

            //// Uncomment the following to use "[0]=foo&[1]=bar" directly as the sample for all actions that support form URL encoded format
            //// and have IEnumerable<string> as the body parameter or return type.
            //config.SetSampleForType("[0]=foo&[1]=bar", new MediaTypeHeaderValue("application/x-www-form-urlencoded"), typeof(IEnumerable<string>));

            //// Uncomment the following to use "1234" directly as the request sample for media type "text/plain" on the controller named "Values"
            //// and action named "Put".
            //config.SetSampleRequest("1234", new MediaTypeHeaderValue("text/plain"), "Values", "Put");

            //// Uncomment the following to use the image on "../images/aspNetHome.png" directly as the response sample for media type "image/png"
            //// on the controller named "Values" and action named "Get" with parameter "id".
            //config.SetSampleResponse(new ImageSample("../images/aspNetHome.png"), new MediaTypeHeaderValue("image/png"), "Values", "Get", "id");

            //// Uncomment the following to correct the sample request when the action expects an HttpRequestMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Get" were having string as the body parameter.
            //config.SetActualRequestType(typeof(string), "Values", "Get");

            //// Uncomment the following to correct the sample response when the action returns an HttpResponseMessage with ObjectContent<string>.
            //// The sample will be generated as if the controller named "Values" and action named "Post" were returning a string.
            //config.SetActualResponseType(typeof(string), "Values", "Post");



        }
    }
}