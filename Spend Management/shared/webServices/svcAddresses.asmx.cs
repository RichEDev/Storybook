
using System.Linq;
using SpendManagementLibrary.Employees;
using SpendManagementLibrary.Helpers;

namespace Spend_Management.shared.webServices
{
    using AutoMapper;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Web.Script.Services;
    using System.Web.Services;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;

    /// <summary>
    /// Address related web services
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    [ScriptService]
    public class svcAddresses : WebService
    {
        #region Public Methods and Operators

        /// <summary>
        /// Delete an address
        /// </summary>
        /// <param name="addressIdentifier">The address to delete</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int Delete(int addressIdentifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.Addresses, true, false) == false)
            {
                return -999;
            }

            return Address.Delete(currentUser, addressIdentifier);
        }

        /// <summary>
        /// Delete an address favourite
        /// </summary>
        /// <param name="addressFavouriteId">The ID of the address label to delete</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int DeleteFavourite(int addressFavouriteId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            return Favourite.Delete(currentUser, addressFavouriteId);
        }
        
        /// <summary>
        /// Delete an address label
        /// </summary>
        /// <param name="addressLabelId">The ID of the address label to delete</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int DeleteLabel(int addressLabelId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var labels = new AddressLabels(currentUser.AccountID);
            return labels.DeleteAddressLabel(currentUser, addressLabelId);
        }

        /// <summary>
        /// Obtain an address object
        /// </summary>
        /// <param name="addressIdentifier">The address to get</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public Address Get(int addressIdentifier, int labelId)
        {
            return GetForClaimOwner(addressIdentifier, labelId, null);
        }

        /// <summary>
        /// Obtain an address object
        /// </summary>
        /// <param name="addressIdentifier">The address to get</param>
        /// <param name="claimOwnerId">The employee who this claim is for, if not the current user (e.g. when approving)</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public Address GetForClaimOwner(int addressIdentifier, int labelId, int? claimOwnerId)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            Address returnValue;
            int? accountId = null;
            if (currentUser == null)
            {
                if ((accountId = Session["accountid"] as int?).HasValue)
                {
                    returnValue = Address.Get(accountId.Value, addressIdentifier);
                }
                else
                {
                    returnValue = new Address
                    {
                        Identifier = -999
                    };
                }
            }
            else
            {
                Lazy<cEmployees> cEmployees = new Lazy<cEmployees>(() => new cEmployees(currentUser.AccountID));
                if ((currentUser.isDelegate && IsHomeAddress(currentUser.Employee, addressIdentifier))
                    || (claimOwnerId.HasValue && claimOwnerId.Value != currentUser.EmployeeID && IsHomeAddress(cEmployees.Value.GetEmployeeById(claimOwnerId.Value), addressIdentifier)))
                {
                    returnValue = new Address { IsPrivate = true };
                }
                else
                {
                    returnValue = Address.Get((accountId = currentUser.AccountID).Value, addressIdentifier);
                }
            }

            if (accountId.HasValue)
            {
                if (labelId > 0)
                {
                    UpdateLabelLastUsed(accountId.Value, labelId);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Whether the address is the home address of the employee
        /// </summary>
        /// <param name="employee">The employee</param>
        /// <param name="addressId">The ID of the address</param>
        /// <returns></returns>
        private bool IsHomeAddress(Employee employee, int addressId)
        {
            return employee.GetHomeAddresses().HomeLocations.Any(h => h.LocationID == addressId);
        }

        /// <summary>
        /// Save an address from the administration interface
        /// </summary>
        /// <param name="addressIdentifier">The address to save, positive integer for editing an existing address or 0 for adding a new address</param>
        /// <param name="addressName">The name of the address, generally a company name</param>
        /// <param name="line1">The first line of an address usually combining a number and street</param>
        /// <param name="line2">The second line of the address</param>
        /// <param name="line3">The third line of the address</param>
        /// <param name="city">The city the address is in</param>
        /// <param name="county">The county, region or locality the address is in</param>
        /// <param name="alpha3CountryCode">The ISO 3166-1 alpha-3 country code from the list of global countries</param>
        /// <param name="postcode">The postal code for the address</param>
        /// <param name="latitude">The latitude of the address</param>
        /// <param name="longitude">The longitude of the address</param>
        /// <param name="capturePlusIdentifier">The capture plus identity if the address came from capture plus search results</param>
        /// <param name="accountWideFavourite">Is the address favourited for everyone on the account</param>
        /// <param name="recommendedDistances">A list of recommended distances to other addresses from this address</param>
        /// <param name="accessRoleCheck">A value indicating whether or not an access role check should be performed when saving an address</param>
        /// <returns>The positive identity of the saved address or a negative return code</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int Save(int addressIdentifier, string addressName, string line1, string line2, string line3, string city, string county, string alpha3CountryCode, string postcode, string latitude, string longitude, string capturePlusIdentifier, bool accountWideFavourite, List<AddressDistanceLookup> recommendedDistances, bool accessRoleCheck)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            AccessRoleType accessRoleType = addressIdentifier == 0 ? AccessRoleType.Add : AccessRoleType.Edit;

            if (accessRoleCheck)
            {
                if (currentUser.CheckAccessRole(accessRoleType, SpendManagementElement.Addresses, true, false) == false)
                {
                    return -999;
                }
            }
            else
            {
                var clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
                cAccountProperties reqProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;
                if (!reqProperties.AddLocations)
                {
                    return -999;
                }
            }

            addressName = addressName.Replace("<", "&lt;");
            line1 = line1.Replace("<", "&lt;");
            line2 = line2.Replace("<", "&lt;");
            line3 = line3.Replace("<", "&lt;"); 
            city = city.Replace("<", "&lt;");
            county = county.Replace("<", "&lt;");
            postcode = postcode.Replace("<", "&lt;");
            latitude = latitude.Replace("<", "&lt;");
            longitude = longitude.Replace("<", "&lt;");
            capturePlusIdentifier = capturePlusIdentifier.Replace("<", "&lt;");

            cCountry country = new cCountries(currentUser.AccountID, currentUser.CurrentSubAccountId).getCountryByCode(alpha3CountryCode);
            int globalCountryId = (country != null) ? country.GlobalCountryId : 0;
            int udprn = addressIdentifier > 0 ? Address.Get(currentUser.AccountID, addressIdentifier).Udprn : 0;

            int returnValue = Address.Save(currentUser, addressIdentifier, addressName, line1, line2, line3, city, county, globalCountryId, postcode, latitude, longitude, capturePlusIdentifier, udprn, accountWideFavourite, !string.IsNullOrWhiteSpace(capturePlusIdentifier) ? Address.AddressCreationMethod.CapturePlus : Address.AddressCreationMethod.ManualByAdministrator);
            if (returnValue > 0 && recommendedDistances != null && recommendedDistances.Count > 0)
            {
                AddressDistance.SaveForAddress(currentUser, returnValue, recommendedDistances);
            }

            return returnValue;
        }

        /// <summary>
        /// Increments the number of PCA lookups an anonymous user has done, so we can restrict them to a certain number.
        /// </summary>
        /// <returns></returns>
        private int IncrementAnonymousLookups()
        {
            var anonymousLookupUsages = (Session["anonymousLookupUsages"] as int?) ?? 0;
            Session["anonymousLookupUsages"] = ++anonymousLookupUsages;
            return anonymousLookupUsages;
        }

        /// <summary>
        /// Save an Address Favourite
        /// </summary>
        /// <param name="identifier">Accepts Capture Plus ID(string) or an existing address id (as string)</param>
        /// <returns>The positive identity of the saved label or a negative return code</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int SaveFavourite(string identifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            int addressId;

            if (!int.TryParse(identifier, out addressId))
            {
                var countries = new cGlobalCountries();
                addressId = (Address.Get(currentUser, identifier, countries) ?? new Address { Identifier = -999 }).Identifier;
            }
            
            return Favourite.Save(currentUser, addressId);
        }

        /// <summary>
        /// Save an Address Label
        /// </summary>
        /// <param name="identifier">Accepts Capture Plus ID(string) or an existing address id (as string)</param>
        /// <param name="labelText">The text for the new label</param>
        /// <returns>The positive identity of the saved label or a negative return code</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int SaveLabel(string identifier, string labelText)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var countries = new cGlobalCountries();
            return AddressLabels.SaveAddressLabel(currentUser, identifier, labelText, countries);
        }

        /// <summary>
        /// Get an Account Wide Address Label
        /// </summary>
        /// <param name="identifier">The label to get</param>
        /// <returns>The label or null if it doesn't exist, if the user doesn't have permission they will also get null</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public AddressLabel GetAccountWideLabel(int identifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, true, false) == false ? null : AddressLabel.Get(currentUser, identifier);
        }

        /// <summary>
        /// Save an Account Wide Address Label
        /// </summary>
        /// <param name="addressIdentifier">The address that the label is for</param>
        /// <param name="identifier">The label</param>
        /// <param name="label">The text for the label</param>
        /// <param name="primary">Whether or not the label is the primary label for the address</param>
        /// <returns>The positive identity of the saved label or a negative return code</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int SaveAccountWideLabel(int addressIdentifier, int identifier, string label, bool primary)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false) == false)
            {
                return -999;
            }

            AddressLabel addressLabel = new AddressLabel { AddressID = addressIdentifier, AddressLabelID = identifier, EmployeeID = null, Text = label, Primary = primary };

            return AddressLabel.Save(currentUser, addressLabel);
        }

        /// <summary>
        /// Delete an Account Wide Address Label
        /// </summary>
        /// <param name="identifier">The label</param>
        /// <returns>The zero on completion or a negative return code</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int DeleteAccountWideLabel(int identifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false) == false)
            {
                return -999;
            }

            return AddressLabel.Delete(currentUser, identifier);
        }

        /// <summary>
        /// Archive or unarchive an address
        /// </summary>
        /// <param name="addressIdentifier">The address to toggle archive on</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int ToggleArchive(int addressIdentifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false) == false)
            {
                return -999;
            }

            return Address.ToggleArchive(currentUser, addressIdentifier);
        }

        /// <summary>
        /// Provides autocomplete results for the address widget
        /// </summary>
        /// <param name="searchTerm">The search text</param>
        /// <param name="alpha3CountryCode">The country in which to search</param>
        /// <param name="date">The date of the expense item, used to determine the home/office address, use an empty string for none</param>
        /// <param name="esrAssignmentId">The ESR Assignment ID associated with the expense item, also used to determine the home/office address, use 0 for none</param>
        /// <returns>The search results as an AutocompleteResult</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public AutocompleteResult AutocompleteForWeb(string searchTerm, string alpha3CountryCode, string date, int esrAssignmentId)
        {
            var response = new AutocompleteResult();
            response.ManualAddresses = new List<ManualAddress>();
            int? accountId = null;
            cAccountProperties accountProperties = null;
            
            var currentUser = cMisc.GetCurrentUser();
            accountId = currentUser.AccountID;
            var clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
            accountProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;

            // if a date was passed then attempt to return the home/office address
            if (string.IsNullOrWhiteSpace(date) == false)
            {
                    
                var homeOrOfficeAddress = Address.GetAllByReservedKeyword(currentUser, searchTerm, Convert.ToDateTime(date), accountProperties, esrAssignmentId);
                if (homeOrOfficeAddress != null)
                {
                   response.ManualAddresses.AddRange(Mapper.Map<List<Address>, List<ManualAddress>>(homeOrOfficeAddress));
                }
            }
            
            if (!(searchTerm == "office" || searchTerm == accountProperties.WorkAddressKeyword))
            {
                var addresses = new Addresses(accountId.Value);
                var displayEsrAddresses = accountProperties.DisplayEsrAddressesInSearchResults;

                cGlobalCountry country = new cGlobalCountries().GetGlobalCountryByAlpha3Code(alpha3CountryCode);
                response.ManualAddresses.AddRange(Mapper.Map<List<Address>, List<ManualAddress>>(addresses.Search(searchTerm, country.GlobalCountryId, displayEsrAddresses)));
            }

            return response;
        }

        /// <summary>
        /// Provides autocomplete results for the address widget
        /// </summary>
        /// <param name="searchTerm">The search text</param>
        /// <param name="alpha3CountryCode">The country in which to search</param>
        /// <param name="date">The date of the expense item, used to determine the home/office address, use an empty string for none</param>
        /// <param name="esrAssignmentId">The ESR Assignment ID associated with the expense item, also used to determine the home/office address, use 0 for none</param>
        /// <returns>The search results as an AutocompleteResult</returns>
        public AutocompleteResult Autocomplete(string searchTerm, string alpha3CountryCode, string date, int esrAssignmentId)
        {
            var response = new AutocompleteResult();
            response.ManualAddresses = new List<ManualAddress>();
            int? accountId = null;
            cAccountProperties accountProperties = null;

            var currentUser = cMisc.GetCurrentUser();
            if (currentUser != null)
            {
                accountId = currentUser.AccountID;

                // if a date was passed then attempt to return the home/office address
                if (string.IsNullOrWhiteSpace(date) == false)
                {
                    var clsSubAccounts = new cAccountSubAccounts(currentUser.AccountID);
                    accountProperties = clsSubAccounts.getSubAccountById(currentUser.CurrentSubAccountId).SubAccountProperties;

                    var homeOrOfficeAddress = Address.GetByReservedKeyword(currentUser, searchTerm, Convert.ToDateTime(date), accountProperties, esrAssignmentId);
                    if (homeOrOfficeAddress != null)
                    {
                        response.ManualAddresses.AddRange(Mapper.Map<List<Address>, List<ManualAddress>>(new List<Address> { homeOrOfficeAddress }));
                    }
                }
            }
            else
            {
                accountId = Session["accountid"] as int?;
            }

            if (accountId.HasValue)
            {
                var addresses = new Addresses(accountId.Value);
                bool displayEsrAddresses = false;
                if (accountProperties != null)
                {
                    displayEsrAddresses = accountProperties.DisplayEsrAddressesInSearchResults;
                }

                cGlobalCountry country = new cGlobalCountries().GetGlobalCountryByAlpha3Code(alpha3CountryCode);
                response.ManualAddresses.AddRange(Mapper.Map<List<Address>, List<ManualAddress>>(addresses.Search(searchTerm, country.GlobalCountryId, displayEsrAddresses)));
            }

            return response;
        }

        /// <summary>
        /// Provides autocomplete results for the address widget when it knows the account id.
        /// </summary>
        /// <param name="searchTerm">The search text</param>
        /// <param name="alpha3CountryCode">The country in which to search</param>
        /// <param name="accountId">The account ID</param>
        /// <returns>The search results as an AutocompleteResult</returns>
        [WebMethod(EnableSession=false)]
        [ScriptMethod]
        public AutocompleteResult AutocompleteByAccount(string searchTerm, string alpha3CountryCode, int accountId)
        {
            var response = new AutocompleteResult();
            var addresses = new Addresses(accountId);

            cGlobalCountry country = new cGlobalCountries().GetGlobalCountryByAlpha3Code(alpha3CountryCode);
            response.ManualAddresses = Mapper.Map<List<Address>, List<ManualAddress>>(addresses.Search(searchTerm, country.GlobalCountryId));
            
            return response;
        }

        /// <summary>
        /// Removes all labels, awabels, favourites, awavourites and recommended distances associated with an address
        /// </summary>
        /// <param name="addressId">The address identifier</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int Cleanse(int addressId)
        {
            var user = cMisc.GetCurrentUser();
            if (user.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false) == false)
            {
                return -999;
            }

            return Address.Cleanse(user, addressId);
        }

        /// <summary>
        /// Retrieves full address information from storage, if it's not there then we pull it from PostcodeAnywhere's Capture+ API and store it.
        /// </summary>
        /// <param name="capturePlusId">The unique Capture Plus ID, e.g. ADB|PR|235013555</param>
        /// <param name="labelId">The ID of the label (0 if no label was used when selecting the address)</param>
        /// <returns>The retrieved address</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public Address GetByCapturePlusId(string capturePlusId, int labelId)
        {
            var user = cMisc.GetCurrentUser();
            var countries = new cGlobalCountries();

            Address address = Address.Get(user, capturePlusId, countries);
            if (user == null)
            {
                //anonymous, so increment the number of lookup usages so it can be limited
                address.AnonymousLookupUsages = IncrementAnonymousLookups();
            }
            else
            {
                if (labelId > 0)
                {
                    UpdateLabelLastUsed(user.AccountID, labelId);
                }
            }

            return address;
        }

        /// <summary>
        /// Retrieves full address information from storage, if it's not there then we pull it from PostcodeAnywhere's Capture+ API and store it.
        /// Then creates a manual address using the retrieved address data and the supplied address name
        /// </summary>
        /// <param name="capturePlusId">The unique Capture Plus ID, e.g. ADB|PR|235013555</param>
        /// <param name="labelId">The ID of the label (0 if no label was used when selecting the address)</param>
        /// <param name="addressName">The name of the address, used with the retrieved address to create a new manual address</param>
        /// <returns>The retrieved address</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public SaveAddressResult GetByCapturePlusIdAndCreateManualAddress(string capturePlusId, int labelId, string addressName)
        {
            var user = cMisc.GetCurrentUser();

            var address = this.GetByCapturePlusId(capturePlusId, labelId);
            int returnCode = Address.Save(user, 0, addressName, address.Line1, address.Line2, address.Line3, address.City, address.County, address.Country, address.Postcode, address.Latitude, address.Longitude,  string.Empty, 0, false, Address.AddressCreationMethod.ManualByClaimant);

            return new SaveAddressResult
            {
                ReturnCode = returnCode,
                Address = returnCode > 0 ? Address.Get(user.AccountID, returnCode) : null
            }; 
        }

        private void UpdateLabelLastUsed(int accountId, int labelId)
        {
            using (DatabaseConnection dbConnection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                dbConnection.sqlexecute.Parameters.AddWithValue("@labelId", labelId);
                dbConnection.sqlexecute.Parameters.AddWithValue("@lastUsed", DateTime.Now);
                dbConnection.ExecuteSQL("update AddressLabels set LastUsed = @LastUsed where AddressLabelID = @labelId");
            }
        }

        /// <summary>
        /// Return all labels, favourites and various configuration options for the address widget
        /// When used on the self-registration page all values are derived from the accountId held in session
        /// </summary>
        /// <returns>A list of labels and a list of favourites</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public WidgetConfigurationAndData GetWidgetConfigurationAndData()
        {
            var currentUser = cMisc.GetCurrentUser();
            var widgetData = new WidgetConfigurationAndData();

            cAccount account = null;
            cAccountSubAccount subAccount = null;

            int accountId;

            if (currentUser != null)
            {
                account = currentUser.Account;
                subAccount = new cAccountSubAccounts(currentUser.AccountID).getSubAccountById(currentUser.CurrentSubAccountId);

                // Get Address Labels
                var addressLabels = new AddressLabels(currentUser.AccountID);
                var employeeLabels = (currentUser.Delegate == null) ? addressLabels.Search(string.Empty, AddressFilter.Employee, currentUser.EmployeeID) : new List<AddressLabel>();
                var accountLabels = addressLabels.Search(string.Empty, AddressFilter.AccountWide);
                addressLabels.Dispose();

                // Get Favourites
                var favourites = new Favourites(currentUser.AccountID);
                var employeeFavourites = (currentUser.Delegate == null) ? favourites.Get(currentUser, AddressFilter.Employee, currentUser.EmployeeID) : new List<Favourite>();
                var accountFavourites = favourites.Get(currentUser, AddressFilter.AccountWide);
                favourites.Dispose();

                widgetData = new WidgetConfigurationAndData
                {
                    EmployeeLabels = employeeLabels,
                    AWLabels = accountLabels,
                    EmployeeFavourites = employeeFavourites,
                    AWFavourites = accountFavourites,
                    HomeAddresses = new List<HomeOrOfficeAddress>(),
                    OfficeAddresses = new List<HomeOrOfficeAddress>()
                };

                // add the home addresses and associated dates
                // add the office addresses and associated dates
                var employeeHomeLocations = (currentUser.Delegate == null) ? new EmployeeHomeAddresses(currentUser.AccountID, currentUser.EmployeeID).Get() : null;
                var employeeOfficeLocations = (currentUser.Delegate == null) ? new EmployeeWorkAddresses(currentUser.AccountID, currentUser.EmployeeID).Get() : null;

                List<Address> addresses = Address.ProcessHomeAndOfficeLocationIds(employeeHomeLocations, employeeOfficeLocations, currentUser);

                if (employeeHomeLocations != null)
                {
                    foreach (var homeLocation in employeeHomeLocations)
                    {
                        var homeAddress = addresses.FirstOrDefault(x => x.Identifier == homeLocation.LocationID);

                        if (homeAddress != null)
                        {
                            widgetData.HomeAddresses.Add(new HomeOrOfficeAddress
                                                             {
                                                                 Identifier = homeAddress.Identifier,
                                                                 GlobalIdentifier = homeAddress.GlobalIdentifier,
                                                                 StartDate = homeLocation.StartDate.ToString(),
                                                                 EndDate = homeLocation.EndDate.ToString(),
                                                                 AddressFriendlyName = homeAddress.FriendlyName
                                                             });
                        }
                    }
                }

                if (employeeOfficeLocations != null)
                {
                    foreach (var officeLocation in employeeOfficeLocations.Values)
                    {
                        var officeAddress = addresses.FirstOrDefault(x => x.Identifier == officeLocation.LocationID);

                        if (officeAddress != null)
                        {
                            widgetData.OfficeAddresses.Add(new HomeOrOfficeAddress
                                                             {
                                                                 Identifier = officeAddress.Identifier,
                                                                 GlobalIdentifier = officeAddress.GlobalIdentifier,
                                                                 StartDate = officeLocation.StartDate.ToString(),
                                                                 EndDate = officeLocation.EndDate.ToString(),
                                                                 AddressFriendlyName = officeAddress.FriendlyName
                                                             });
                        }
                    }
                }
            } 
            else if (int.TryParse(Session["accountId"].ToString(), out accountId))
            {
                account = new cAccounts().GetAccountByID(accountId);

                // Only works with the first sub account at present, as is the case with the self-registration page
                subAccount = new cAccountSubAccounts(accountId).getFirstSubAccount();
                    
                widgetData = new WidgetConfigurationAndData
                {
                    EmployeeLabels = new List<AddressLabel>(),
                    AWLabels = new List<AddressLabel>(),
                    EmployeeFavourites = new List<Favourite>(),
                    AWFavourites = new List<Favourite>()
                };
            }

            if (account != null)
            {
                var countries = new cCountries(account.accountid, subAccount.SubAccountID);
                var filteredCountries = countries.GetPostcodeAnywhereEnabledCountries();
                string defaultCountryAlpha3Code = new cGlobalCountries().getGlobalCountryById(countries.getCountryById(subAccount.SubAccountProperties.HomeCountry).GlobalCountryId).Alpha3CountryCode;
                cEmployees employees = new cEmployees(currentUser.AccountID);
                Employee employee = employees.GetEmployeeById(currentUser.EmployeeID);
                string employeePrimaryAlphaCode = new cGlobalCountries().getGlobalCountryById(countries.getCountryById(employee.PrimaryCountry).GlobalCountryId).Alpha3CountryCode;                
                foreach (cGlobalCountry filteredCountry in filteredCountries)
                {
                    if (filteredCountry.Alpha3CountryCode == employeePrimaryAlphaCode)
                    {
                        defaultCountryAlpha3Code = employeePrimaryAlphaCode;
                        break;
                    }
                }                

                widgetData.Countries = filteredCountries;
                widgetData.DefaultCountryAlpha3Code = defaultCountryAlpha3Code;
                widgetData.PostcodeAnywhereKey = account.PostcodeAnywhereKey;
                widgetData.MandatoryPostcodeForAddresses = subAccount.SubAccountProperties.MandatoryPostcodeForAddresses;
                widgetData.AllowMultipleWorkAddresses = subAccount.SubAccountProperties.MultipleWorkAddress;

            }

            return widgetData;
        }

        /// <summary>
        /// Gets a users "home" or "office" address
        /// </summary>
        /// <param name="keyword">The special address keyword, "office" or "home"</param>
        /// <param name="date">The date of the expense</param>
        /// <param name="esrAssignmentId">The selected ESR Assignment for the expense item, pass 0 if not applicable</param>
        /// <returns>The retrieved address</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public Address GetByKeyword(string keyword, string date, int esrAssignmentId)
        {
            var user = cMisc.GetCurrentUser();

            var clsSubAccounts = new cAccountSubAccounts(user.AccountID);
            cAccountProperties accountProperties = clsSubAccounts.getSubAccountById(user.CurrentSubAccountId).SubAccountProperties;

            return Address.GetByReservedKeyword(user, keyword, Convert.ToDateTime(date), accountProperties, esrAssignmentId);
        }

        /// <summary>
        /// Favourite or un-favourite an address for the account
        /// </summary>
        /// <param name="addressIdentifier">The address to toggle the account wide favourite on</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int ToggleAccountWideFavourite(int addressIdentifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false) == false)
            {
                return -999;
            }

            return Address.ToggleAccountWideFavourite(currentUser, addressIdentifier);
        }

        /// <summary>
        /// Obtain an address object's recommended distances
        /// </summary>
        /// <param name="addressIdentifier">The address to get</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public List<AddressDistanceLookup> GetRelatedAddressDistances(int addressIdentifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            if (currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, true, false) == false)
            {
                return new List<AddressDistanceLookup>
                {
                    new AddressDistanceLookup { DestinationIdentifier = -999 }
                };
            }

            var addresses = AddressDistance.GetForAddress(currentUser.AccountID, addressIdentifier);
            addresses.Sort((x, y) => string.Compare(x.DestinationFriendlyName, y.DestinationFriendlyName, StringComparison.Ordinal));

            return addresses;
        }

        /// <summary>
        /// Gets the distance between two addresses, using the overridden (custom) value if there is one, otherwise, looks it up.
        /// </summary>
        /// <param name="fromAddress">The start <see cref="Address"/></param>
        /// <param name="toAddress">The endd <see cref="Address"/></param>
        /// <returns>The distance between the two addresses</returns>
        [WebMethod, ScriptMethod]
        public decimal? GetCustomOrRecommendedDistance(Address fromAddress, Address toAddress)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var subAccounts = new cAccountSubAccounts(currentUser.AccountID);
            return AddressDistance.GetRecommendedOrCustomDistance(fromAddress, toAddress, currentUser.AccountID, subAccounts.getSubAccountById(currentUser.Employee.DefaultSubAccount), currentUser);
        }

        /// <summary>
        /// Obtains route (turn by turn directions and map line coordinates) information for a list of (contiguous) address pairs
        /// </summary>
        /// <param name="addressIdentifiers">A list of Address Identifiers</param>
        /// <returns>Directions and map line coordinates</returns>
        [WebMethod(EnableSession = true)]
        public Route GetRouteByAddressIds(List<int> addressIdentifiers)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var account = new cAccounts().GetAccountByID(user.AccountID);
            // create a list of postcodes from the address Ids
        
            var addresses = Address.Get(user.AccountID, addressIdentifiers);

            if (!Address.CheckAddressesHavePostcode(addressIdentifiers, addresses, user, account))
            {
                return new Route(Route.InvalidWayPoints);
            }

            List<string> wayPoints = Address.GetAddressCoordinates(addressIdentifiers, addresses, account);

            return this.GetRouteByWaypoints(wayPoints);
        }

        /// <summary>
        /// Obtains route (turn by turn directions and map line coordinates) information for a list of (contiguous) address pairs
        /// for a certain claim - in order to respect Data Protection rules (approver/delegate can't see claimant's home address)
        /// </summary>
        /// <param name="addressIdentifiers">A list of Address Identifiers</param>
        /// <param name="claimEmpId">The employee ID of the employee whose claim it is (not necessarily the logged on user).</param>
        /// <returns>Directions and map line coordinates</returns>
        [WebMethod(EnableSession = true)]
        public Route GetRouteByAddressIdsForEmployee(List<int> addressIdentifiers, int claimEmpId)
        {

            if (addressIdentifiers.Any(addressIdentifier => addressIdentifier == -100))
            {
                return new Route(Route.InvalidWayPointSequence);
            }

            CurrentUser user = cMisc.GetCurrentUser();
            var accountSubAccount = new cAccountSubAccounts(user.AccountID);
            var employees = new cEmployees(user.AccountID);
            var homeAddresses = employees.GetEmployeeById(claimEmpId).GetHomeAddresses().HomeLocations;

            if (!Address.CanViewRoute(claimEmpId, addressIdentifiers, user, accountSubAccount, homeAddresses))
            {
                return new Route(Route.NoPermission);
            }
                                        
            return GetRouteByAddressIds(addressIdentifiers);
        }
   
        /// <summary>
        /// Obtains route (turn by turn directions and map line coordinates) information for an expense item
        /// </summary>
        /// <param name="expenseId">The expense ID to retrieve waypoints for</param>
        /// <returns>Directions and map line coordinates</returns>
        [WebMethod(EnableSession = true)]
        public Route GetRouteByExpenseId(int expenseId)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            List<string> waypoints = cExpenseItem.GetWaypointsByExpenseId(user.AccountID, expenseId);
            return this.GetRouteByWaypoints(waypoints);
        }

        /// <summary>
        /// Obtains route (turn by turn directions and map line coordinates) information for a list of contiguous waypoints (postcodes),
        /// </summary>
        /// <param name="waypoints">A list of waypoints (postcodes)</param>
        /// <returns>Directions and map line coordinates</returns>
        [WebMethod(EnableSession = true)]
        public Route GetRouteByWaypoints(List<string> waypoints)
        {
            CurrentUser user = cMisc.GetCurrentUser();
            var postcodeAnywhere = new PostcodeAnywhere(user.AccountID);
            cAccountSubAccount subAccount = new cAccountSubAccounts(user.AccountID).getSubAccountById(user.CurrentSubAccountId);
            DistanceType distanceType = subAccount.SubAccountProperties.MileageCalcType == 1 ? DistanceType.Shortest : DistanceType.Fastest;

            return postcodeAnywhere.GetRoute(waypoints, distanceType, DateTime.Now, GlobalVariables.StaticContentLibrary + "/images/routing/");
        }

        /// <summary>
        /// Obtain the recommended distances for a journey between two addresses 
        /// </summary>
        /// <param name="originIdentifier">The origin address</param>
        /// <param name="destinationIdentifier">The destination address</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public AddressDistanceLookup GetAddressDistance(int originIdentifier, int destinationIdentifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            var addresses = new Addresses(currentUser.AccountID);
            var origin = addresses.GetAddressById(originIdentifier);
            var destination = addresses.GetAddressById(destinationIdentifier);
            return currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Addresses, true, false) == false
                ? new AddressDistanceLookup { DestinationIdentifier = -999 }
                : AddressDistance.Get(currentUser.Account, origin, destination);
        }

        /// <summary>
        /// Delete the custom and recommended distances for an address to address journey
        /// </summary>
        /// <param name="addressDistanceIdentifier">The address distance to delete</param>
        /// <returns>-999 if the user does not have permission, a negative code on error or a positive identifier of the deleted item on success</returns>
        [WebMethod(EnableSession = true), ScriptMethod]
        public int DeleteAddressDistance(int addressDistanceIdentifier)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            return currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Addresses, true, false) == false
                ? -999
                : AddressDistance.Delete(currentUser, addressDistanceIdentifier);
        }

        #endregion Public Methods and Operators

        /// <summary>
        /// Holds the various result types for an autocomplete result for the address widget
        /// </summary>
        public struct AutocompleteResult
        {
            /// <summary>
            /// Holds manually added addresses
            /// </summary>
            public List<ManualAddress> ManualAddresses;

            /// <summary>
            /// A placeholder for users address labels
            /// </summary>
            public List<AddressLabel> Labels;

            /// <summary>
            /// A placeholder for account wide address labels
            /// </summary>
            public List<AddressLabel> AWLabels;

            /// <summary>
            /// A placeholder for users address favourites
            /// </summary>
            public List<string> Favourites;

            /// <summary>
            /// A placeholder for account wide address favourites
            /// </summary>
            public List<string> AWFavourites;
        }

        /// <summary>
        /// This is a lightweight version of the Address struct to return data to the page from Autocomplete
        /// </summary>
        public struct ManualAddress
        {
            public string GlobalIdentifier { get; set; }
            public string FriendlyName { get; set; }
            public int Identifier { get; set; }
            public string Line1 { get; set; }
            public string Line2 { get; set; }
            public string Postcode { get; set; }
        }

        /// <summary>
        /// This is a ligthweight version of the EmployeeHomeAddress/EmployeeWorkAddress object 
        /// </summary>
        public struct HomeOrOfficeAddress
        {
            public string AddressFriendlyName { get; set; }
            public string GlobalIdentifier { get; set; }
            public int Identifier { get; set; }
            public string StartDate { get; set; }
            public string EndDate { get; set; }
        }

        /// <summary>
        /// Contains an address and the code returned from Address.Save when it was created
        /// </summary>
        public struct SaveAddressResult
        {
            public int ReturnCode { get; set; }
            public Address Address { get; set; }
        }

        /// <summary>
        /// Container for passing everything that the Address widget needs: Labels, Favourites, Awabels, Awavourites, Countries and the PostcodeAnywhere key
        /// </summary>
        public struct WidgetConfigurationAndData
        {
            /// <summary>
            /// List of Employee's cut-down labels
            /// </summary>
            public List<AddressLabel> EmployeeLabels;

            /// <summary>
            /// List of Account Wide cut-down labels
            /// </summary>
            public List<AddressLabel> AWLabels;

            /// <summary>
            /// List of Employee's favourites (Postcode Anywhere ID's)
            /// </summary>
            public List<Favourite> EmployeeFavourites;

            /// <summary>
            /// List of Account Wide favourites (Postcode Anywhere ID's)
            /// </summary>
            public List<Favourite> AWFavourites;

            /// <summary>
            /// List of countries
            /// </summary>
            public List<cGlobalCountry> Countries;

            /// <summary>
            /// List of home addresses and associated dates
            /// </summary>
            public List<HomeOrOfficeAddress> HomeAddresses;

            /// <summary>
            /// List of office addresses and associated dates
            /// </summary>
            public List<HomeOrOfficeAddress> OfficeAddresses; 

            /// <summary>
            /// the postcode anywhere license key for this account, for Capture+ requests
            /// </summary>
            public string PostcodeAnywhereKey;

            /// <summary>
            /// Default country for this customer
            /// </summary>
            public string DefaultCountryAlpha3Code;

            /// <summary>
            /// Determines if postcodes must be entered when users add new addresses
            /// </summary>
            public bool MandatoryPostcodeForAddresses;

            /// <summary>
            /// Determines if claimant is allowed to choose from multiple work addresses
            /// </summary>
            public bool AllowMultipleWorkAddresses;
        }
    }
}