namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;

    using Interfaces;

    using Models.Common;
    using Models.Responses;
    using Models.Types;

    using Newtonsoft.Json;

    using SpendManagementApi.Common;
    using SpendManagementApi.Common.Enum;

    using SpendManagementLibrary;
    using SpendManagementLibrary.Addresses;
    using SpendManagementLibrary.Employees;

    using Spend_Management;

    using global::Utilities;

    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Utilities;

    using SpendManagementLibrary.Mobile;
    using Spend_Management.shared.webServices;
    using Spend_Management.shared.code;

    using Address = SpendManagementLibrary.Addresses.Address;
    using AddressLabel = SpendManagementLibrary.Addresses.AddressLabel;
    using Favourite = SpendManagementApi.Models.Types.Favourite;

    internal class AddressRepository : ArchivingBaseRepository<Models.Types.Address>, ISupportsActionContext
    {
        private Addresses _addresses;

        /// <summary>
        /// Archiving Base repository constructor, taking an action context.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="actionContext">ActionContext</param>
        public AddressRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, a => a.Id, null)
        {
            _addresses = ActionContext.Addresses;
        }

        /// <summary>Gets all Addresses from the Database.</summary>
        /// <returns>A List of Addresses.</returns>
        public override IList<Models.Types.Address> GetAll()
        {
            return _addresses.All().Select(x => new Models.Types.Address().From(x, ActionContext)).ToList();
        }

        /// <summary>
        /// Get item with id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Models.Types.Address Get(int id)
        {
            return new Models.Types.Address().From(_addresses.GetAddressById(id), ActionContext);
        }

        /// <summary>
        /// Get account wide label with the supplied Id and populate the provided response.
        /// </summary>
        /// <param name="id">The id if the account wide label to get.</param>
        /// <param name="response">The response to populate.</param>
        /// <returns>The passed in response.</returns>
        public AccountWideLabelResponse GetAccountWideLabel(int id, AccountWideLabelResponse response)
        {
            var empId = User.EmployeeID;
            User.EmployeeID = 0;
            var item = AddressLabel.Get(User, id);
            User.EmployeeID = empId;

            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressLabelNotFound);
            }

            response.AddressId = item.AddressID;
            response.Id = item.AddressLabelID;
            response.Label = item.Text;
            return response;
        }

        /// <summary>
        /// Searches the Postcode Anywhere and SEL Addresses table for matchiing Addresses.
        /// This does not work like the normal repository pattern we have, 
        /// since the search is implemented in the DB.
        /// </summary>
        /// <param name="criteria">
        /// The criterion or criteria to search with.
        /// </param>
        /// <param name="countryId">
        /// The GlobalCountry Id to search in.
        /// </param>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <param name="esrAssignmentId">
        /// The ESR Assignment Id
        /// </param>
        /// <returns>
        /// A List of <see cref="Models.Types.Address">Address</see> from both sources.
        /// </returns>
        public List<Models.Types.Address> Search(string criteria, int countryId, DateTime expenseDate, int? esrAssignmentId = null)
        {
            // Get results from Postcode Anywhere lookup
            var account = new cAccounts().GetAccountByID(User.AccountID);
            string licenseKey = account.PostcodeAnywhereKey;

            var countries = new cGlobalCountries();
            cGlobalCountry country = countries.getGlobalCountryById(countryId);

            var countryAlphaCode = country != null ? country.Alpha3CountryCode : "GBR";

            string url = string.Format("http://services.postcodeanywhere.co.uk/CapturePlus/Interactive/AutoComplete/v2.00/json3.ws?" + "&Key={0}&Country={1}&searchTerm={2}", licenseKey, countryAlphaCode, criteria);

            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync(url).Result;

            List<Models.Types.Address> postCodeAnywhereResults = new List<Models.Types.Address>();

            if (response.IsSuccessStatusCode)
            {
                var jsonResults = response.Content.ReadAsStringAsync().Result;
                PostCodeAnywhereLookupResults listOfAddresses = JsonConvert.DeserializeObject<PostCodeAnywhereLookupResults>(jsonResults);

                if (listOfAddresses != null)
                {
                    postCodeAnywhereResults = ConvertPostcodeAnywhereResultsToListOfAddressAPIType(listOfAddresses);
                }

            }

            //Get results from SEL data sources
            var subAccounts = new cAccountSubAccounts(this.User.AccountID);
            var accountProperties =
                subAccounts.getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;
                         
            Models.Types.Address keyWordAddress = this.CheckIfSearchContainsReservedKeyword(criteria, expenseDate, accountProperties, esrAssignmentId);

            var SELResults = new List<Models.Types.Address>();

            if (keyWordAddress != null)
            {
                SELResults.Add(keyWordAddress);
            }

            var apiSearchResults = this._addresses.ApiSearch(criteria, countryId, accountProperties.DisplayEsrAddressesInSearchResults);

            SELResults.AddRange(apiSearchResults.Select(result => new Models.Types.Address().From(result, this.ActionContext)));

            if (!User.isDelegate)
            {
                this.SetFavouriteFlags(SELResults);
            }

            var labelResults = this.GetAccountWideAndEmployeeLabels(criteria);

            //merge together the Postcode Anywhere results, SEL data and label sources to return a combined result set
            List<Models.Types.Address> addressSearchResults = SELResults.Concat(labelResults).Concat(postCodeAnywhereResults).ToList();

            // remove duplicate addresses by Id unless 0

            var processedAddressRusults = new List<Models.Types.Address>();

            foreach (var result in addressSearchResults)
            {
                if (result.Id == 0)
                {
                    processedAddressRusults.Add(result);
                }
                else
                {
                    //is address already in the list?
                    if (!processedAddressRusults.Any(x => x.Id == result.Id))
                    {
                        processedAddressRusults.Add(result);
                    }
                }

            }

            return processedAddressRusults;
        }

        /// <summary>
        /// Gets an Address from Postcode Anywhere on its Postcode Anywhere global identifier
        /// </summary>
        /// <param name="GlobalIdentifier">The Global Identifier in the Postcode Anywhere data source.</param>
        /// <returns>The <see cref="AddressResponse">AddressResponse</see></returns>
        public Models.Types.Address GetAddressDetailsFromPostCodeAnywhere(string GlobalIdentifier)
        {

            var countries = new cGlobalCountries();
            Address address = Address.Get(User, GlobalIdentifier, countries);

            return new Models.Types.Address().From(address, ActionContext);
        }

        /// <summary>
        /// Gets an more detail on an Address from Postcode Anywhere on its Postcode Anywhere global identifier. Used by enhanced PCA subscribers
        /// </summary>
        /// <param name="GlobalIdentifier">The Global Identifier in the Postcode Anywhere data source.</param>
        /// <returns>A list of <see cref="Address">Address</see></returns>
        public List<Models.Types.Address> GetAddressDetailsFromPostCodeAnywhereInteractiveFind(string GlobalIdentifier)
        {
            var pca = new PostcodeAnywhere(User.AccountID);
            var lookupResults = pca.CapturePlusInteractiveFind(GlobalIdentifier);
            PostCodeAnywhereLookupResults listOfAddresses = new PostCodeAnywhereLookupResults();
            List<PostcodeAnywhereLookupResult> LookupResults  = new List<PostcodeAnywhereLookupResult>();

            foreach (DataRow row in lookupResults.Rows)
            {
                PostcodeAnywhereLookupResult result = new PostcodeAnywhereLookupResult();
                result.Id = Convert.ToString(row["Id"]);
                result.Suggestion = Convert.ToString(row["Suggestion"]).Trim();
                result.IsRetrievable = Convert.ToBoolean(row["IsRetrievable"]);
                LookupResults.Add(result);
            }


            listOfAddresses.LookupResults = LookupResults;

            List<Models.Types.Address> postCodeAnywhereResults = new List<Models.Types.Address>();

            if (listOfAddresses != null)
            {
                postCodeAnywhereResults = ConvertPostcodeAnywhereResultsToListOfAddressAPIType(listOfAddresses);
            }

            return postCodeAnywhereResults;

        }



        /// <summary>
        /// Gets an Address from SEL data sources based on its AddressId
        /// </summary>
        /// <param name="addressId">The AddressId of the Address to lookup.</param>
        /// <param name="claimOwnerId">The id of the claim owner</param>
        /// <returns>A <see cref="Address">Address</see></returns>
        public Models.Types.Address GetAddressDetailsFromSEL(int addressId, int? claimOwnerId)
        {
            Address address = null;

            if ((User.isDelegate && Address.IsHomeAddress(User.Employee, addressId))
                || (claimOwnerId.HasValue && claimOwnerId.Value != User.EmployeeID && Address.IsHomeAddress(ActionContext.Employees.GetEmployeeById(claimOwnerId.Value), addressId)))
            {
                address = new Address { IsPrivate = true };
            }
            else
            {
                address = Address.Get(User.AccountID, addressId);
            }

            //Todo:call UpdateLabelLastUsed method when address label implemented

            return new Models.Types.Address().From(address, ActionContext);
        }

        /// <summary>
        /// Adds a Manual Address
        /// </summary>
        /// <param name="item">The manual address to add.</param>
        /// <returns></returns>
        public override Models.Types.Address Add(Models.Types.Address item)
        {
            item = base.Add(item);
            return this.Save(item);
        }

        /// <summary>
        /// The add address.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="isMobileRequest">
        /// The is mobile request.
        /// </param>
        /// <returns>
        /// The <see cref="Address"/>.
        /// </returns>
        public Models.Types.Address AddAddress(Models.Types.Address request, bool isMobileRequest)
        {
            request = base.Add(request);
            return this.Save(request, isMobileRequest);
        }

        /// <summary>
        /// Updates a manual address.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated Address.</returns>
        public override Models.Types.Address Update(Models.Types.Address item)
        {
            item = base.Update(item);

            var address = ActionContext.Addresses.GetAddressById(item.Id);
            if (address.CreationMethod != Address.AddressCreationMethod.ManualByClaimant && address.CreationMethod != Address.AddressCreationMethod.ManualByAdministrator)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressEditManualOnly);
            }

            return Save(item);
        }

        /// <summary>
        /// Deletes a manual address, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the Address to delete.</param>
        /// <returns>The deleted manual address.</returns>
        public override Models.Types.Address Delete(int id)
        {
            var item = base.Delete(id);

            var result = Address.Delete(User, id);
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }

        /// <summary>
        /// Archives / unarchives the item with the specified id.
        /// </summary>
        /// <param name="id">The Id of the address to archive or unarchive.</param>
        /// <param name="archive">Whether or not to archive.</param>
        /// <returns></returns>
        public override Models.Types.Address Archive(int id, bool archive)
        {
            var item = Get(id);
            if (item.Archived != archive)
            {
                Address.ToggleArchive(User, id);
            }
            item.Archived = archive;
            return item;
        }

        /// <summary>
        /// Sets the acctoun-wide favourite status on or off.
        /// </summary>
        /// <param name="id">The Id of the address to change favourite status for.</param>
        /// <param name="toFavour">Whether the addres should be a favourite or not</param>
        /// <returns></returns>
        public Models.Types.Address SetAccountWideFavourite(int id, bool toFavour)
        {
            var item = Get(id);
            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressNotFound);
            }

            if (item.IsAccountWideFavourite != toFavour)
            {
                Address.ToggleAccountWideFavourite(User, id);
            }
            item.IsAccountWideFavourite = toFavour;
            return item;
        }

        /// <summary>
        /// Makes an existing label primary.
        /// </summary>
        /// <param name="labelId">The Id of the label.</param>
        /// <returns>The Address with the label set.</returns>
        public Models.Types.Address MakeAccountWideLabelPrimary(int labelId)
        {
            var label = AddressLabel.Get(User, labelId);
            if (label == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressLabelNotFound);
            }

            var item = Get(label.AddressID);
            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressNotFound);
            }

            label.Primary = true;
            var result = AddressLabel.Save(User, new AddressLabel
            {
                EmployeeID = null,
                AddressID = label.AddressID,
                AddressLabelID = label.AddressLabelID,
                GlobalIdentifier = label.GlobalIdentifier,
                Primary = true,
                Text = label.Text
            });

            if (result == -3)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressLabelReservedWord);
            }

            if (result == -2)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressLabelExistsPersonal);
            }

            if (result == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressLabelExists);
            }

            if (result == 0)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            item.PrimaryAccountWideLabel = label.AddressLabelID;
            return item;
        }

        /// <summary>
        /// Adds an account-wide label to an address.
        /// </summary>
        /// <param name="id">The Id of the address to label.</param>
        /// <param name="label">The label to give the Address.</param>
        /// <param name="primary">Whether this label should be the new primary label.</param>
        /// <returns>The Address with the label set.</returns>
        public Models.Types.Address AddAccountWideLabel(int id, string label, bool primary)
        {
            var address = Get(id);
            if (address == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressNotFound);
            }

            // whip out EmployeeID before save to make the label account-wide, perform the save and then replace the Id.
            var empId = User.EmployeeID;
            User.EmployeeID = 0;
            var result = AddressLabel.Save(User, new AddressLabel
            {
                EmployeeID = null,
                AddressID = address.Id,
                AddressLabelID = 0,
                Primary = primary || address.AccountWideLabels.Count == 0,
                Text = label
            });
            User.EmployeeID = empId;

            ValidateAddressLabelReturnCode(result);

            // add the new int to the item.
            address.AccountWideLabels.Add(result);

            return address;
        }

        /// <summary>
        /// Adds an account-wide label to an address.
        /// </summary>
        /// <param name="id">The Id of the address to label.</param>
        /// <param name="label">The label to give the Address.</param>
        /// <param name="primary">Whether this label should be the new primary label.</param>
        /// <returns>The Address with the label set.</returns>
        public Models.Types.Address EditAccountWideLabel(int id, string label, bool primary)
        {
            var item = AddressLabel.Get(User, id);
            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressLabelNotFound);
            }

            var address = Get(item.AddressID);
            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressNotFound);
            }

            // whip out EmployeeID before save to make the label account-wide, perform the save and then replace the Id.
            var empId = User.EmployeeID;
            User.EmployeeID = 0;
            var result = AddressLabel.Save(User, new AddressLabel
            {
                EmployeeID = null,
                AddressID = address.Id,
                AddressLabelID = id,
                Primary = primary || address.AccountWideLabels.Count == 0,
                Text = label
            });
            User.EmployeeID = empId;

            ValidateAddressLabelReturnCode(result);

            return address;
        }


        /// <summary>
        /// Removes an account-wide label from an address.
        /// </summary>
        /// <param name="id">The Id of the account-wide label.</param>
        /// <returns>The Address.</returns>
        public Models.Types.Address RemoveAccountWideLabel(int id)
        {
            var item = AddressLabel.Get(User, id);

            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorAddressLabelNotFound);
            }

            var addressId = item.AddressID;

            var result = AddressLabel.Delete(User, id);

            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return Get(addressId);
        }

        /// <summary>
        /// The remove label.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int RemoveLabel(int id)
        {
            var labels = new AddressLabels(this.User.AccountID);
            return labels.DeleteAddressLabel(this.User, id);
        }

        /// <summary>
        /// The save label.
        /// </summary>
        /// <param name="identifier">
        /// The identifier.
        /// </param>
        /// <param name="label">
        /// The label.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int SaveLabel(string identifier, string label)
        {
            var countries = new cGlobalCountries();
            return AddressLabels.SaveAddressLabel(this.User, identifier, label, countries);
        }

        /// <summary>
        /// Cleanses an address.
        /// </summary>
        /// <param name="id">The Id of the account-wide label.</param>
        /// <returns>The cleansed address.</returns>
        public Models.Types.Address CleanseAddress(int id)
        {
            var item = Get(id);
            if (item == null)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful, ApiResources.ApiErrorAddressLabelNotFound);
            }

            var result = Address.Cleanse(User, id);

            if (result == -6)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressCleanseFailOrganisation);
            }

            if (result == -5)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressCleanseFailEmployeeWork);
            }

            if (result == -4)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressCleanseFailEmployeeHome);
            }

            if (result == -2)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressCleanseFailEsr);
            }

            if (result == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressCleanseFailJourneySteps);
            }

            if (result != 1)
            {
                throw new ApiException(ApiResources.ApiErrorAddressCleanseFail, ApiResources.ApiErrorAddressCleanseFailMessage);
            }

            return Get(id);
        }


        public HomeAddressLinkage LinkHomeAddressToEmployee(HomeAddressLinkage linkage)
        {
            var homeAddresses = new EmployeeHomeAddresses(this.User.AccountID, linkage.EmployeeId);
            linkage.CreatedById = (int) (linkage.ModifiedById = this.User.EmployeeID);
            linkage.CreatedOn = (DateTime) (linkage.ModifiedOn = DateTime.UtcNow);

            // validate
            this.ActionContext.HomeAddresses = homeAddresses;
            linkage.Validate(this.ActionContext);

            // save
            var result = homeAddresses.Add(linkage.Cast(), this.User);
            linkage.Id = result;

            new EmailNotificationHelper(this.User.Employee).ExcessMileage();

            //Re-cache employee home locations
            homeAddresses.Get();

            return linkage;
        }

        public HomeAddressLinkage EditHomeAddressLinkage(HomeAddressLinkage linkage)
        {
            if (linkage.EmployeeId <= 0)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var homeAddresses = new EmployeeHomeAddresses(this.User.AccountID, linkage.EmployeeId);
            linkage.ModifiedById = this.User.EmployeeID;
            linkage.ModifiedOn = DateTime.UtcNow;

            var dbLinkage = homeAddresses.HomeLocations.FirstOrDefault(x => x.EmployeeLocationID == linkage.Id);
            if (dbLinkage == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageId);
            }
            if (linkage.AddressId != dbLinkage.LocationID)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageId);
            }

            if (linkage.EmployeeId != dbLinkage.EmployeeID)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageId);
            }

            // validate
            this.ActionContext.HomeAddresses = homeAddresses;
            linkage.Validate(this.ActionContext);

            // save
            var location = new cEmployeeHomeLocation(linkage.Id, linkage.EmployeeId, linkage.AddressId, linkage.StartDate, linkage.EndDate, linkage.CreatedOn, linkage.CreatedById, DateTime.UtcNow, User.EmployeeID);
            var result = homeAddresses.Add(location, this.User);
            linkage.Id = result;

            new EmailNotificationHelper(this.User.Employee).ExcessMileage();

            //Re-cache employee home locations
            homeAddresses.Get();

            return linkage;
        }

        public WorkAddressLinkage LinkWorkAddressToEmployee(WorkAddressLinkage linkage)
        {
            var workAddresses = new EmployeeWorkAddresses(this.User.AccountID, linkage.EmployeeId);

            linkage.CreatedById = (int)(linkage.ModifiedById = this.User.EmployeeID);
            linkage.CreatedOn = (DateTime)(linkage.ModifiedOn = DateTime.UtcNow);

            // validate
            ActionContext.WorkAddresses = workAddresses;
            linkage.Validate(this.ActionContext);

            // save
            var location = new cEmployeeWorkLocation(linkage.Id, linkage.EmployeeId, linkage.AddressId, linkage.StartDate, linkage.EndDate, linkage.IsActive, linkage.IsTemporary, linkage.CreatedOn, linkage.CreatedById, DateTime.UtcNow, User.EmployeeID, linkage.EsrAssignmentLocationId, linkage.PrimaryRotational);
            var result = workAddresses.Add(location, this.User);
            linkage.Id = result;

            new EmailNotificationHelper(this.User.Employee).ExcessMileage();

            //Re-cache employee work locations
            workAddresses.Get();

            return linkage;
        }

        public WorkAddressLinkage EditWorkAddressLinkage(WorkAddressLinkage linkage)
        {
            if (linkage.EmployeeId <= 0)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var workAddresses = new EmployeeWorkAddresses(this.User.AccountID, linkage.EmployeeId);
            linkage.ModifiedById = this.User.EmployeeID;
            linkage.ModifiedOn = DateTime.UtcNow;

            var dbLinkage = workAddresses.WorkLocations.FirstOrDefault(x => x.Key == linkage.Id);
            if (dbLinkage.Value == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageId);
            }
            if (linkage.AddressId != dbLinkage.Value.LocationID)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageId);
            }

            if (linkage.EmployeeId != dbLinkage.Value.EmployeeID)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageId);
            }

            // validate
            this.ActionContext.WorkAddresses = workAddresses;
            linkage.Validate(this.ActionContext);

            // save
            var location = new cEmployeeWorkLocation(linkage.Id, linkage.EmployeeId, linkage.AddressId, linkage.StartDate, linkage.EndDate, linkage.IsActive, linkage.IsTemporary, linkage.CreatedOn, linkage.CreatedById, DateTime.UtcNow, User.EmployeeID, linkage.EsrAssignmentLocationId, linkage.PrimaryRotational);
            var result = workAddresses.Add(location, this.User);
            linkage.Id = result;

            new EmailNotificationHelper(this.User.Employee).ExcessMileage();

            //Re-cache employee work locations
            workAddresses.Get();

            return linkage;
        }

        /// <summary>
        /// Unlinks an address from and Employee.
        /// </summary>
        /// <param name="addressLinkageId">The Id of the AddressLinkage, rather than the address.</param>
        /// <param name="employeeId">The Id of the employee</param>
        /// <returns></returns>
        public Models.Types.Address UnlinkAddressFromEmployee(int addressLinkageId, int employeeId)
        {
            var employee = this.ActionContext.Employees.GetEmployeeById(employeeId);

            if (employee == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (addressLinkageId <= 0)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageId);
            }

            var homeAddresses = new EmployeeHomeAddresses(this.User.AccountID, employeeId);
            var workAddresses = new EmployeeWorkAddresses(this.User.AccountID, employeeId);

            var homeItem = homeAddresses.GetBy(addressLinkageId);
            var workItem = workAddresses.GetBy(addressLinkageId);
            int addressId = 0;

            if (homeItem == null && workItem == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLinkageIdEmployee);
            }

            if (homeItem != null)
            {
                addressId = homeItem.LocationID;
                homeAddresses.Remove(addressLinkageId, this.User);

                new EmailNotificationHelper(this.User.Employee).ExcessMileage();
            }
            else
            {
                addressId = workItem.LocationID;
                workAddresses.Remove(addressLinkageId, this.User);

                new EmailNotificationHelper(this.User.Employee).ExcessMileage();
            }

            return this.Get(addressId);
        }

        /// <summary>
        /// Adds or removes a favourite address for an employee
        /// </summary>
        /// <param name="request">
        /// The <see cref="AddRemovePersonalFavouriteAddressRequest">AddRemovePersonalFavouriteAddressRequest</see>.
        /// </param>
        /// <returns>
        /// The new favourite Id if saved <see cref="int">int</see>
        /// </returns>
        public int AddRemovePersonalFavouriteAddress(AddRemovePersonalFavouriteAddressRequest request)
        {
            if (request.FavouriteAction == FavouriteAction.Add)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                int addressId;

                if (!int.TryParse(request.Identifier, out addressId))
                {
                    var countries = new cGlobalCountries();
                    addressId = (Address.Get(currentUser, request.Identifier, countries) ?? new Address { Identifier = -999 }).Identifier;
                }

                return SpendManagementLibrary.Addresses.Favourite.Save(User, addressId);
            }
            else
            {
                return SpendManagementLibrary.Addresses.Favourite.Delete(User, request.FavouriteId);
            }
        }
        /// <summary>
        /// Saves an Address, performing some validation.
        /// </summary>
        /// <param name="item">
        /// The address to save.
        /// </param>
        /// <param name="isMobileRequest">
        /// The is Mobile Request.
        /// </param>
        /// <returns>
        /// The saved item.
        /// </returns>
        private Models.Types.Address Save(Models.Types.Address item, bool isMobileRequest = false)
        {
            item.CreatedById = (int) (item.ModifiedById = User.EmployeeID);
            item.CreatedOn = (DateTime) (item.ModifiedOn = DateTime.UtcNow);

            if (string.IsNullOrWhiteSpace(item.Line1))
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressLine1);
            }

            var accountSubs = new cAccountSubAccounts(User.AccountID);
            var properties = accountSubs.getFirstSubAccount().SubAccountProperties;
            if (properties.MandatoryPostcodeForAddresses && string.IsNullOrWhiteSpace(item.Postcode))
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressPostCode);
            }

            if (properties.ForceAddressNameEntry && string.IsNullOrWhiteSpace(item.AddressName))
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressAddressName);
            }

            // check employee here.
            var employees = new cEmployees(User.AccountID);
            Models.Types.Address addressItem;

            // ReSharper disable once PossibleInvalidOperationException
            if (employees.GetEmployeeById(item.EmployeeId.Value) == null)
            {
                if (isMobileRequest)
                {
                    addressItem = new Models.Types.Address
                    {
                        AddressActionOutcome = AddressActionOutcome.WrongEmployeeId
                    };
                    return addressItem;
                }

                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId,
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            var id = item.To(ActionContext).Save(User);
            if (id == -1)
            {
                if (isMobileRequest)
                {
                    addressItem = new Models.Types.Address
                    {
                        AddressActionOutcome = AddressActionOutcome.AddressExists
                    };
                    return addressItem;
                }

                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                        ApiResources.ApiErrorAddressExists);
            }

            if (id == 0)
            {
                if (isMobileRequest)
                {
                    addressItem = new Models.Types.Address
                    {
                        AddressActionOutcome = AddressActionOutcome.SaveUnsuccessfulMessage
                    };
                    return addressItem;
                }

                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                        ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            addressItem = this.Get(id);
            addressItem.AddressActionOutcome = AddressActionOutcome.Success;
            return addressItem;
        }

        /// <summary>
        /// Validates the return code from saving an address, throwing the appropriate error.
        /// </summary>
        /// <param name="result"></param>
        private void ValidateAddressLabelReturnCode(int result)
        {
            if (result == -3)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressLabelReservedWord);
            }

            if (result == -2)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressLabelExistsPersonal);
            }

            if (result == -1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorAddressLabelExists);
            }

            if (result == 0)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }
        }

        /// <summary>
        /// Gets home and office addresses for the current user
        /// </summary>
        /// <param name="employeeId">Employee id</param>
        /// <returns>
        /// A list of <see cref="Models.Types.Address">Address</see>
        /// </returns>
        public List<Models.Types.Address> GetHomeAndOfficeAddresses(int employeeId)
        {
            var employeeHomeLocations = (User.Delegate == null) ? new EmployeeHomeAddresses(User.AccountID, employeeId).Get() : null;
            IDictionary<int, cEmployeeWorkLocation> employeeOfficeLocations = (User.Delegate == null) ? new EmployeeWorkAddresses(User.AccountID, employeeId).Get() : null;

            List<Address> addresses = Address.ProcessHomeAndOfficeLocationIds(employeeHomeLocations, employeeOfficeLocations, User);

            var homeAndOfficeAddresses = new List<Models.Types.Address>();

            if (employeeHomeLocations != null)
            {
                foreach (var homeLocation in employeeHomeLocations)
                {
                    var homeAddress = addresses.FirstOrDefault(x => x.Identifier == homeLocation.LocationID);

                    if (homeAddress != null)
                    {
                        homeAndOfficeAddresses.Add(new Models.Types.Address()
                        {
                            Id = homeAddress.Identifier,
                            GlobalIdentifier = homeAddress.GlobalIdentifier,
                            AddressName = homeAddress.AddressName,
                            Line1 = homeAddress.Line1,
                            Line2 = homeAddress.Line2,
                            Line3 = homeAddress.Line3,
                            City = homeAddress.City,
                            County = homeAddress.County,
                            Postcode = homeAddress.Postcode,
                            StartDate = homeLocation.StartDate,
                            EndDate = homeLocation.EndDate,
                            FriendlyName = homeAddress.FriendlyName,
                            IsHomeAddress = true
                        });
                    }
                }
            }

            if (employeeOfficeLocations != null)
            {
                foreach (var officeLocation in employeeOfficeLocations.Values)
                {
                    int assignmentId = 0;
                    var officeAddress = addresses.FirstOrDefault(x => x.Identifier == officeLocation.LocationID);

                    if (officeAddress != null)
                    {
                        if (User.Account.IsNHSCustomer)
                        {
                            EsrAssignmentLocation assignmentLocation = officeLocation.GetEsrLocation(this.User);

                            if (assignmentLocation != null && assignmentLocation.EsrAssignId != null)
                            {
                                assignmentId = assignmentLocation.EsrAssignId;
                            }
                            else
                            {
                                assignmentId = 0;
                            }                  
                        }

                        homeAndOfficeAddresses.Add(new Models.Types.Address()
                        {
                            Id = officeAddress.Identifier,
                            GlobalIdentifier = officeAddress.GlobalIdentifier,                                        
                            Line1 = officeAddress.Line1,
                            Line2 = officeAddress.Line2,
                            Line3 = officeAddress.Line3,
                            City = officeAddress.City,
                            County = officeAddress.County,
                            Postcode = officeAddress.Postcode,
                            StartDate = officeLocation.StartDate,
                            EndDate = officeLocation.EndDate,
                            FriendlyName = officeAddress.FriendlyName,
                            IsOfficeAddress = true,
                            EsrAssignmentId = assignmentId
                        });
                    }
                }
            }

            return homeAndOfficeAddresses;
        }

        /// <summary>
        /// Converts a <see cref="PostCodeAnywhereLookupResults">PostCodeAnywhereLookupResults</see> to a list of <see cref="Models.Types.Address">Address</see>
        /// </summary>
        /// <param name="pcaResults"></param>
        /// <returns> a list of <see cref="Models.Types.Address">Address</see></returns>
        private List<Models.Types.Address> ConvertPostcodeAnywhereResultsToListOfAddressAPIType(PostCodeAnywhereLookupResults pcaResults)
        {
            var postCodeAnywhereResults = new List<Models.Types.Address>();

            foreach (var results in pcaResults.LookupResults)
            {
                if (results.Id != null)
                {
                    var address = new Models.Types.Address
                    {
                        Line1 = results.Match + results.Suggestion,
                        GlobalIdentifier = results.Id,
                        AddressSource = AddressSource.PCA,
                        IsRetriveable = results.IsRetrievable
                    };

                    postCodeAnywhereResults.Add(address);
                }
            }

            return postCodeAnywhereResults;
        }

        /// <summary>
        /// Gets all the account wide and employee favourites and converts them to a list of <see cref="Models.Types.Address"/>
        /// </summary>
        /// <returns>A list of <see cref="Models.Types.Address"/></returns>
        public List<Models.Types.Address> GetAccountWideAndEmployeeFavourites()
        {
            IEnumerable<Favourite> employeeFavourties = this.GetFavourites(AddressFilter.Employee);
            IEnumerable<Favourite> accountWideFavourties = this.GetFavourites(AddressFilter.AccountWide);

            var addresses = new List<Models.Types.Address>();

            foreach (var favourite in employeeFavourties.Concat(accountWideFavourties))
            {
                var address = this.Get(favourite.AddressId);
                address.IsAccountWideFavourite = favourite.FavouriteId <= 0;
                address.FavouriteId = favourite.FavouriteId;

                addresses.Add(address);
            }

            return addresses;
        }

        /// <summary>
        /// Gets all the account wide and employee labels and converts them to a list of <see cref="Models.Types.AddressLabel">AddressLabel</see>
        /// </summary>
        /// <returns>A list of <see cref="Models.Types.AddressLabel"/></returns>
        public List<Models.Types.AddressLabel> GetAccountWideAndEmployeeAddressLabels()
        {
            var labels = new AddressLabels(this.User.AccountID);
            List<AddressLabel> employeeLabels = labels.Search(string.Empty, AddressFilter.Employee, User.EmployeeID);
            List<AddressLabel> accountLabels = labels.Search(string.Empty, AddressFilter.AccountWide);

            var addresseLabels = new List<Models.Types.AddressLabel>();

            foreach (AddressLabel employeeLabel in employeeLabels)
            {
                var label = new Models.Types.AddressLabel().ToApiType(employeeLabel, this.ActionContext);
                addresseLabels.Add(label);
            }

            foreach (AddressLabel accountLabel in accountLabels)
            {
                var label = new Models.Types.AddressLabel().ToApiType(accountLabel, this.ActionContext);
                label.PrimaryAccountWideLabel = 1;
                addresseLabels.Add(label);
            }

            return addresseLabels;
        }

        /// <summary>
        /// Gets all the account wide and employee favourites and converts them to a list of <see cref="Models.Types.Address"/>
        /// </summary>
        /// <returns>A list of <see cref="Models.Types.Address"/></returns>
        public List<Models.Types.Address> GetAccountWideAndEmployeeLabels(string searchTerm = "")
        {
            var labels = new AddressLabels(this.User.AccountID);
            var employeeLabels = labels.Search(searchTerm, AddressFilter.Employee, User.EmployeeID);
            var accountLabels = labels.Search(searchTerm, AddressFilter.AccountWide);

            var addresses = new List<Models.Types.Address>();

            foreach (var employeeLabel in employeeLabels)
            {
                var address = this.Get(employeeLabel.AddressID);
                address.Label = employeeLabel.Text;
                address.LabelId = employeeLabel.AddressLabelID;
                address.PrimaryAccountWideLabel = null;

                addresses.Add(address);
            }

            foreach (var accountLabel in accountLabels)
            {
                var address = this.Get(accountLabel.AddressID);
                address.Label = accountLabel.Text;
                address.LabelId = accountLabel.AddressLabelID;
                address.PrimaryAccountWideLabel = address.PrimaryAccountWideLabel;

               addresses.Add(address);
            }

            return addresses;
        }

        /// <summary>
        /// Gets the journey route and mapping details form a list of address identifiers
        /// </summary>
        /// <param name="addressIdentifiers">
        /// The address identifiers.
        /// </param>
        /// <param name="claimEmployeeId">
        /// The employee id of the claim owner
        /// </param>
        /// <returns>
        /// The <see cref="Route">Route</see>.
        /// </returns>
        public Models.Types.Addresses.Route GetRouteForAddressIdentifiers(List<int> addressIdentifiers, int claimEmployeeId)
        {

            if (!this.User.Account.MapsEnabled)
            {
                return new Models.Types.Addresses.Route(ApiResources.APIErrorMappingNotEnabledForAccount);
            }

            if (addressIdentifiers.Any(addressIdentifier => addressIdentifier == -100))
            {
                return new Models.Types.Addresses.Route(ApiResources.APIErrorAddressInvalidWayPointsSequence);
            }
        
            var accountSubAccount = new cAccountSubAccounts(this.User.AccountID);
            var employees = new cEmployees(this.User.AccountID);
            var homeAddresses = employees.GetEmployeeById(claimEmployeeId).GetHomeAddresses().HomeLocations;

            if (!Address.CanViewRoute(claimEmployeeId, addressIdentifiers, this.User, accountSubAccount, homeAddresses))
            {
                return new Models.Types.Addresses.Route(ApiResources.APIErrorAddressNoPermission);
            }

            var account = new cAccounts().GetAccountByID(this.User.AccountID);
            var addresses = Address.Get(this.User.AccountID, addressIdentifiers);

            if (!Address.CheckAddressesHavePostcode(addressIdentifiers, addresses, this.User, account))
            {
               return new Models.Types.Addresses.Route(ApiResources.APIErrorAddressInvalidWayPoints);
            }

            List<string> waypoints = Address.GetAddressCoordinates(addressIdentifiers, addresses, account);
            cAccountSubAccount subAccount = new cAccountSubAccounts(this.User.AccountID).getSubAccountById(this.User.CurrentSubAccountId);
      
            Route route = this.GetRouteFromPCA(waypoints, subAccount);

            var apiRoute = new Models.Types.Addresses.Route();

            if (route.Error != string.Empty)
            {
                apiRoute.Error = route.Error;
            }
            else
            {
                apiRoute = apiRoute.From(route, this.ActionContext);
            }

            return apiRoute;
        }

        /// <summary>
        /// Gets the journey route and mapping details for an Expense Id
        /// </summary>
        /// <param name="expenseId">
        /// The expense Id
        /// </param>
        /// <returns>
        /// The <see cref="Route">Route</see>.
        /// </returns>
        public Models.Types.Addresses.Route GetRouteForExpenseItem(int expenseId)
        {
            if (!this.User.Account.MapsEnabled)
            {
                return new Models.Types.Addresses.Route(ApiResources.APIErrorMappingNotEnabledForAccount);
            }

            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            cExpenseItem item = expenseItemRepository.GetExpenseItem(expenseId);
       
            //Checks claim ownership before proceeding
            new ClaimRepository(this.User, this.ActionContext).Get(item.claimid);

            List<string> waypoints = cExpenseItem.GetWaypointsByExpenseId(this.User.AccountID, expenseId);
            cAccountSubAccount subAccount = new cAccountSubAccounts(this.User.AccountID).getSubAccountById(this.User.CurrentSubAccountId);      
            Route route = this.GetRouteFromPCA(waypoints, subAccount);

            var apiRoute = new Models.Types.Addresses.Route();

            if (route.Error != string.Empty)
            {
                apiRoute.Error = route.Error;
            }
            else
            {
                apiRoute = apiRoute.From(route, this.ActionContext);
            }

            return apiRoute;
        }

        /// <summary>
        /// Gets the address route and mapping details from PCA for the supplied waypoints.
        /// </summary>
        /// <param name="waypoints">
        /// The waypoints.
        /// </param>
        /// <param name="subAccount">
        /// The sub account.
        /// </param>    
        /// <returns>
        /// The <see cref="Route">Route</see>.
        /// </returns>
        private Route GetRouteFromPCA(List<string> waypoints, cAccountSubAccount subAccount)
        {        
            DistanceType distanceType = subAccount.SubAccountProperties.MileageCalcType == 1
                                            ? DistanceType.Shortest
                                            : DistanceType.Fastest;

            return ActionContext.PostcodeAnywhere.GetRoute(
                waypoints,
                distanceType,
                DateTime.Now,
                GlobalVariables.StaticContentLibrary + "/images/routing/");
        }

        /// <summary>
        /// Gets the journey route and mapping details for an Expense Id
        /// </summary>
        /// <param name="identifiers">Identifiers</param>
        /// <param name="claimId">Claim id</param>
        /// <returns>
        /// The <see cref="Route">Route</see>.
        /// </returns>
        public Models.Types.Addresses.Route GetRouteForAddresses(List<int> identifiers, int claimId)
        {
            if (!this.User.Account.MapsEnabled)
            {
                return new Models.Types.Addresses.Route(ApiResources.APIErrorMappingNotEnabledForAccount);
            }

            //Checks claim ownership before proceeding
            var claim = new ClaimRepository(this.User, this.ActionContext).Get(claimId);

            var svcAddesses = new svcAddresses();
            Route route = svcAddesses.GetRouteByAddressIdsForEmployee(identifiers, claim.EmployeeId);

            var apiRoute = new Models.Types.Addresses.Route();

            if (route.Error != string.Empty)
            {
                apiRoute.Error = route.Error;
            }
            else
            {
                apiRoute = apiRoute.From(route, this.ActionContext);
            }

            return apiRoute;
        }

        private IEnumerable<Favourite> GetFavourites(AddressFilter filter)
        {
            var favoritesRepo = new FavouritesRepository(User, ActionContext);
            return favoritesRepo.GetFavourites(User.EmployeeID, filter);
        }

        private void SetFavouriteFlags(IEnumerable<Models.Types.Address> addresses)
        {
            IEnumerable<Favourite> favourites = this.GetFavourites(AddressFilter.Employee);

            foreach (var address in addresses)
            {
                // If the addrssId is in the list of personal favourites, then set the address favouriteId
                foreach (var favourite in favourites)
                {
                    if (favourite.AddressId == address.Id)
                    {
                        address.FavouriteId = favourite.FavouriteId;
                    }
                }
            }
        }

        /// <summary>
        /// The check if address search contains a reserved address related keyword
        /// </summary>
        /// <param name="criteria">
        /// The address search criteria.
        /// </param>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <param name="accountProperties">
        /// The account properties.
        /// </param>
        /// <param name="esrAssignmentId">
        /// The ESR Assignment Id.
        /// </param>
        /// <returns>
        /// The <see cref="Address">Address</see> for the matching keyword, or null
        /// </returns>
        private Models.Types.Address CheckIfSearchContainsReservedKeyword(string criteria, DateTime expenseDate, cAccountProperties accountProperties, int? esrAssignmentId = null)
        {
            Address homeOrOfficeAddress = Address.GetByReservedKeyword(
                this.User,
                criteria,
                expenseDate,
                accountProperties,
                esrAssignmentId);

            if (homeOrOfficeAddress != null)
            {
                var address = new Models.Types.Address().From(homeOrOfficeAddress, this.ActionContext);
                address.FriendlyName = homeOrOfficeAddress.FriendlyName;
                return address;
            }

            return null;
        }
    }
}