namespace SpendManagementApi.Controllers.V1
{
    using System.Collections.Generic;
    using System.Web.Http;

    using SpendManagementApi.Attributes;
    using SpendManagementApi.Common;
    using SpendManagementApi.Models.Common;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Requests.Address;
    using SpendManagementApi.Models.Responses;
    using SpendManagementApi.Models.Responses.Addresses;
    using SpendManagementApi.Models.Types;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Repositories;

    /// <summary>
    /// Contains operations for handling Address Lookups.
    /// </summary>
    [RoutePrefix("Addresses")]
    [Version(1)]
    public class AddressesV1Controller : ArchivingApiController<Address>
    {
        #region Api Methods

        /// <summary>
        /// Gets all of the available end points from the <see cref="Address">Address</see> part of the API.
        /// </summary>
        /// <returns>A list of available resource Links</returns>
        [HttpOptions]
        [Route("")]
        public List<Link> Options()
        {
            return this.Links("Options");
        }

        /// <summary>
        /// Gets all <see cref="Address">Addresss</see> in the system.
        /// </summary>
        [HttpGet, Route("")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.View)]
        public GetAddressesResponse GetAll()
        {
            return this.GetAll<GetAddressesResponse>();
        }

        /// <summary>
        /// Gets a single <see cref="Address">Address</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the item to get.</param>
        /// <returns>An AddresssResponse object, which will contain an <see cref="Address">Address</see> if one was found.</returns>
        [HttpGet, Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.View)]
        public AddressResponse Get([FromUri] int id)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).GetAndAudit(id);
            return response;
        }

        /// <summary>
        /// Gets an account wide label for an <see cref="Address">Address</see>, by its Id.
        /// </summary>
        /// <param name="id">The Id of the label to get.</param>
        /// <returns>An AccountWideLabelResponse object, which will contain the label, and also the AddressId of the <see cref="Address">Address</see> if one was found.</returns>
        [HttpGet, Route("GetAccountWideLabel/{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.View)]
        public AccountWideLabelResponse GetAccountWideLabel([FromUri] int id)
        {
            var response = this.InitialiseResponse<AccountWideLabelResponse>();
            response = ((AddressRepository) this.Repository).GetAccountWideLabel(id, response);
            return response;
        }

        /// <summary>
        /// Adds an <see cref="Address">Address</see>.
        /// Do not try to set AccountWide labels or favourites here. Use the patch methods.
        /// Similarly, setting distances here wont work either. Use the AddressDistances resource instead.
        /// </summary>
        /// <param name="request">The <see cref="Address">Address</see> to add. <br/>
        /// When adding a new <see cref="Address">Address</see> through the API, the following properties are required:<br/>
        /// Id: Must be set to 0, or the add will throw an error.
        /// Label: Must be set to something meaningful, or the add will throw an error.
        /// </param>
        /// <returns>An AddressResponse.</returns>
        [Route("")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Add)]
        public AddressResponse Post([FromBody] Address request)
        {
            return this.Post<AddressResponse>(request);
        }

        /// <summary>
        /// The save address.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <returns>
        /// The <see cref="AddressResponse">AddressResponse</see>
        /// </returns>
        [Route("SaveAddress")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AddressResponse SaveAddress([FromBody] Address request)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).AddAddress(request, this.IsMobileRequest());
            return response;
        }

        /// <summary>
        /// Links an <see cref="Address">Address</see> to an <see cref="Employee">Employee</see> as a HOME address.
        /// This is achieved by creating a <see cref="HomeAddressLinkage">HomeAddressLinkage</see>, which will be visible on the Employee.
        /// Note that rather than edit an exsiting linkage, you should remove the existing linkage and recreate it.
        /// </summary>
        /// <param name="request">The linkage data, which defines extra information about the user's relationship with the address.</param>
        /// <returns>A WorkAddressLinkageResponse, containing the <see cref="WorkAddressLinkageResponse">WorkAddressLinkageResponse</see>.</returns>
        [HttpPatch, Route("AssignToEmployeeAsHome")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public HomeAddressLinkageResponse AssignToEmployeeAsHome([FromBody] HomeAddressLinkage request)
        {
            var response = this.InitialiseResponse<HomeAddressLinkageResponse>();
            response.Item = ((AddressRepository) this.Repository).LinkHomeAddressToEmployee(request);
            return response;
        }

        /// <summary>
        /// Edits a <see cref="HomeAddressLinkage">HomeAddressLinkage</see>, which will be visible on the Employee.
        /// StartDate<br/>
        /// EndDate<br/>
        /// Note that you cannot edit the EmployeeId or the AddressId - you must remove and recreate to do this.
        /// </summary>
        /// <param name="request">The linkage data, which defines extra information about the user's relationship with the address.</param>
        /// <returns>A HomeAddressLinkageResponse, containing the <see cref="HomeAddressLinkage">HomeAddressLinkag</see>.</returns>
        [HttpPatch, Route("EditEmployeeHomeAddressLinkage")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public HomeAddressLinkageResponse EditEmployeeHomeAddressLinkage([FromBody] HomeAddressLinkage request)
        {
            var response = this.InitialiseResponse<HomeAddressLinkageResponse>();
            response.Item = ((AddressRepository)this.Repository).EditHomeAddressLinkage(request);
            return response;
        }
        
        /// <summary>
        /// Applies the supplied <see cref="Address">Address</see> to the supplied <see cref="Employee">Employee</see> as a WORK address.
        /// This is achieved by creating a <see cref="WorkAddressLinkage">WorkAddressLinkage</see>, which will be visible on the Employee.
        /// Note that rather than edit an existing linkage, you should remove the existing linkage and recreate it.
        /// </summary>
        /// <param name="request">The linkage data, which defines extra information about the user's relationship with the address.</param>
        /// <returns>A WorkAddressLinkageResponse, containing the <see cref="WorkAddressLinkageResponse">WorkAddressLinkageResponse</see>.</returns>
        [HttpPatch, Route("AssignToEmployeeAsWork")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public WorkAddressLinkageResponse AssignToEmployeeAsWork([FromBody] WorkAddressLinkage request)
        {
            var response = this.InitialiseResponse<WorkAddressLinkageResponse>();
            response.Item = ((AddressRepository)this.Repository).LinkWorkAddressToEmployee(request);
            return response;
        }
        
        /// <summary>
        /// Edits a <see cref="WorkAddressLinkage">WorkAddressLinkage</see>, which will be visible on the Employee.
        /// StartDate<br/>
        /// EndDate<br/>
        /// IsTemporary<br/>
        /// IsActive<br/>
        /// Note that you cannot edit the EmployeeId or the AddressId - you must remove and recreate to do this.
        /// </summary>
        /// <param name="request">The linkage data, which defines extra information about the user's relationship with the address.</param>
        /// <returns>A WorkAddressLinkageResponse, containing the <see cref="WorkAddressLinkageResponse">WorkAddressLinkageResponse</see>.</returns>
        [HttpPatch, Route("EditEmployeeWorkAddressLinkage")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public WorkAddressLinkageResponse EditEmployeeWorkAddressLinkage([FromBody] WorkAddressLinkage request)
        {
            var response = this.InitialiseResponse<WorkAddressLinkageResponse>();
            response.Item = ((AddressRepository)this.Repository).EditWorkAddressLinkage(request);
            return response;
        }

        /// <summary>
        /// Unlinks the supplied <see cref="Address">Address</see> from the supplied <see cref="Employee">Employee</see>. 
        /// These links are represented by either a <see cref="WorkAddressLinkage">WorkAddressLinkage</see> or a <see cref="HomeAddressLinkage">HomeAddressLinkage</see>.
        /// Rather than providing the id of the Adress and the Employee, just provide the Id of the Home/WorkAddressLinkage and it will be removed.
        /// Note that rather than edit an existing linkage, you should remove the existing linkage and recreate it.
        /// </summary>
        /// <param name="linkageId">The Id of the <see cref="WorkAddressLinkage">WorkAddressLinkage</see> to unlink.</param>
        /// <param name="eid">The Id of the <see cref="Employee">Employee</see> on which this linkage sits.</param>
        /// <returns>An AddressResponse, which will contain the modified <see cref="Address">Address</see>.</returns>
        [HttpPatch, Route("UnlinkFromEmployee/{linkageId:int}/{eid:int}")]
        [AuthAudit(SpendManagementElement.Employees, AccessRoleType.Edit)]
        public AddressResponse UnlinkFromEmployee([FromUri] int linkageId, [FromUri] int eid)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).UnlinkAddressFromEmployee(linkageId, eid);
            return response;
        }

        /// <summary>
        /// Edits an <see cref="Address">Address</see>.
        /// Do not try to set AccountWide labels or favourites here. Use the patch methods.
        /// Similarly, setting distances here wont work either. Use the AddressDistances resource instead.
        /// </summary>
        /// <param name="id">The Id of the Item to edit.</param>
        /// <param name="request">The Item to edit.</param>
        /// <returns>The edited <see cref="Address">Address</see> in an AddressResponse.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse Put([FromUri] int id, [FromBody] Address request)
        {
            request.Id = id;
            return this.Put<AddressResponse>(request);
        }

        /// <summary>
        /// Deletes an <see cref="Address">Address</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Address">Address</see> to be deleted.</param>
        /// <param name="archive">Whether to archive or un-archive this <see cref="Address">Address</see>.</param>
        /// <returns>An AddressResponse with the item set to null upon a successful delete.</returns>
        [HttpPatch, Route("{id:int}/Archive/{archive:bool}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse Archive(int id, bool archive)
        {
            return this.Archive<AddressResponse>(id, archive);
        }

        /// <summary>
        /// Sets whether or not this <see cref="Address">Address</see> is an Account-wide favourite.
        /// </summary>
        /// <param name="id">The id of the <see cref="Address">Address</see> to be made into a favourite (or not).</param>
        /// <param name="isToBeFavourite">Whether to make the <see cref="Address">Address</see> a favourite or not.</param>
        /// <returns>An AddressResponse with the modified Address.</returns>
        [HttpPatch, Route("{id:int}/SetAccountWideFavourite/{isToBeFavourite:bool}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse SetAccountWideFavourite(int id, bool isToBeFavourite)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository) this.Repository).SetAccountWideFavourite(id, isToBeFavourite);
            return response;
        }

        /// <summary>
        /// Adds an Account-wide label to this <see cref="Address">Address</see>.
        /// Note that if an address has any labels, then one must be the primary label. 
        /// Set this with the primary option.
        /// </summary>
        /// <param name="id">The id of the <see cref="Address">Address</see> to be labelled.</param>
        /// <param name="label">The label to give the <see cref="Address">Address</see>.</param>
        /// <param name="primary">Whether the label should be the primary label for the address.</param>
        /// <returns>An AddressResponse with the labelled Address.</returns>
        [HttpPatch, Route("{id:int}/AddAccountWideLabel/{label}/{primary:bool}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse AddAccountWideLabel(int id, string label, bool primary)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).AddAccountWideLabel(id, label, primary);
            return response;
        }

        /// <summary>
        /// Edits an Account-wide label.
        /// Note that if an address has any labels, then one must be the primary label. 
        /// Set this with the primary option.
        /// Note that the id parameter here refers to the Label's Id, NOT the Id of the Address...
        /// </summary>
        /// <param name="id">The id of the LABEL to be re-labelled.</param>
        /// <param name="label">The new label to give the AddressLabel.</param>
        /// <param name="primary">Whether the label should be the primary label for its parent address.</param>
        /// <returns>An AddressResponse with the re-labelled Address.</returns>
        [HttpPatch, Route("EditAccountWideLabel/{id:int}/{label}/{primary:bool}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse EditAccountWideLabel(int id, string label, bool primary)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).EditAccountWideLabel(id, label, primary);
            return response;
        }

        /// <summary>
        /// Makes a label the primary label for an address. 
        /// The primary status of current primary label (if there is one) will be removed.
        /// </summary>
        /// <param name="id">The id of the label to make the primary label.</param>
        /// <returns>An AddressResponse with the labelled Address.</returns>
        [HttpPatch, Route("MakeAccountWideLabelPrimary/{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse MakeAccountWideLabelPrimary(int id)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).MakeAccountWideLabelPrimary(id);
            return response;
        }

        /// <summary>
        /// Removes an Account-wide label from this <see cref="Address">Address</see>.
        /// </summary>
        /// <param name="id">The Id of the label to be removed.</param>
        /// <returns>An AddressResponse with the newly un-labelled Address.</returns>
        [HttpPatch, Route("RemoveAccountWideLabel/{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse RemoveAccountWideLabel(int id)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).RemoveAccountWideLabel(id);
            return response;
        }
        
        /// <summary>
        /// Cleanses an <see cref="Address">Address</see>, meaning that the Account-wide labels, the favourite status, and any recommended distances, are removed.
        /// </summary>
        /// <param name="id">The id of the <see cref="Address">Address</see> to be cleansed.</param>
        /// <returns>An AddressResponse with the newly un-labelled Address.</returns>
        [HttpPatch, Route("{id:int}/Cleanse")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Edit)]
        public AddressResponse Cleanse(int id)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).CleanseAddress(id);
            return response;
        }
        
        /// <summary>
        /// Deletes an <see cref="Address">Address</see>.
        /// </summary>
        /// <param name="id">The id of the <see cref="Address">Address</see> to be deleted.</param>
        /// <returns>An AddressResponse with the item set to null upon a successful delete.</returns>
        [Route("{id:int}")]
        [AuthAudit(SpendManagementElement.Addresses, AccessRoleType.Delete)]
        public AddressResponse Delete(int id)
        {
            return this.Delete<AddressResponse>(id);
        }

        /// <summary>
        /// Gets an Address via a simple search lookup.
        /// </summary>
        /// <param name="addressRequest">
        /// The address Request.
        /// </param>
        /// <returns>
        /// The <see cref="GetAddressesResponse">GetAddressesResponse</see>
        /// </returns>
        [HttpPost, Route("FindAddress")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetAddressesResponse FindAddress([FromBody] AddressRequest addressRequest)
        {
            var response = this.InitialiseResponse<GetAddressesResponse>();
            response.List = ((AddressRepository)this.Repository).Search(addressRequest.SearchTerm, addressRequest.CountryId, addressRequest.ExpenseDate, addressRequest.EsrAssignmentId);
            return response;
        }

        /// <summary>
        /// Gets an Address from Postcode Anywhere on its Postcode Anywhere global identifier
        /// </summary>
        /// <param name="request">
        /// The GlobalIdentifier request.
        /// </param>
        /// <returns>
        /// The <see cref="AddressResponse">AddressResponse</see>
        /// </returns>
        [HttpPost, Route("GetAddressDetailsFromPostCodeAnywhere")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AddressResponse GetAddressDetailsFromPostCodeAnywhere([FromBody] GlobalIdentifierRequest request)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).GetAddressDetailsFromPostCodeAnywhere(request.GlobalIdentifier);
            return response;
        }


        /// <summary>
        /// Gets an more detail on an Address from Postcode Anywhere on its Postcode Anywhere global identifier. Used by enhanced PCA subscribers
        /// </summary>
        /// <param name="request">
        /// The GlobalIdentifier request.
        /// </param>
        /// <returns>The <see cref="AddressResponse">AddressResponse</see></returns>
        [HttpPost, Route("GetAddressDetailsFromPostCodeAnywhereInteractiveFind")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetAddressesResponse GetAddressDetailsFromPostCodeAnywhereInteractiveFind([FromBody] GlobalIdentifierRequest request)
        {
            var response = this.InitialiseResponse<GetAddressesResponse>();
            response.List = ((AddressRepository)this.Repository).GetAddressDetailsFromPostCodeAnywhereInteractiveFind(request.GlobalIdentifier);
            return response;
        }

        /// <summary>
        /// Gets an Address from SEL data sources based on its AddressId
        /// </summary>
        /// <param name="id">The AddressId of the Address to lookup.</param>
        /// <returns>The <see cref="AddressResponse>AddressResponse</returns>
        [HttpGet, Route("GetAddressDetailsFromSEL/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AddressResponse GetAddressDetailsFromSEL(int id)
        {
            var response = this.InitialiseResponse<AddressResponse>();
            response.Item = ((AddressRepository)this.Repository).GetAddressDetailsFromSEL(id, null);
            return response;
        }


        /// <summary>
        /// Adds or removes a favourite address for an employee
        /// </summary>
        /// <param name="request">
        /// The <see cref="AddRemovePersonalFavouriteAddressRequest">AddRemovePersonalFavouriteAddressRequest</see>.
        /// </param>
        /// <returns>
        /// The <see cref="NumericResponse">NumericResponse</see>
        /// </returns>
        [HttpPost, Route("AddRemovePersonalFavouriteAddress")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NumericResponse AddRemovePersonalFavouriteAddress(AddRemovePersonalFavouriteAddressRequest request)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((AddressRepository)this.Repository).AddRemovePersonalFavouriteAddress(request);
            return response;
        }

        /// <summary>
        /// Gets all the account wide and employee favourites
        /// </summary>      
        /// <returns>
        /// The <see cref="GetAddressesResponse">GetAddressesResponse</see>
        /// </returns>     
        [HttpGet, Route("GetAccountWideAndEmployeeFavourites")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetAddressesResponse GetAccountWideAndEmployeeFavourites()
        {
            var response = this.InitialiseResponse<GetAddressesResponse>();
            response.List = ((AddressRepository)this.Repository).GetAccountWideAndEmployeeFavourites();
            return response;
        }

        /// <summary>
        /// Gets all the account wide and employee labels, with only basic address data for the labels returned.
        /// </summary>      
        /// <returns>
        /// The <see cref="AddressLabelResponse">AddressLabelResponse</see>
        /// </returns>     
        [HttpGet, Route("GetAccountWideAndEmployeeAddressLabels")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public AddressLabelResponse GetAccountWideAndEmployeeAddressLabels()
        {
            var response = this.InitialiseResponse<AddressLabelResponse>();
            response.List = ((AddressRepository)this.Repository).GetAccountWideAndEmployeeAddressLabels();
            return response;
        }

        /// <summary>
        /// Gets all the account wide and employee labels
        /// </summary>      
        /// <returns>
        /// The <see cref="GetAddressesResponse">GetAddressesResponse</see>
        /// </returns>     
        [HttpGet, Route("GetAccountWideAndEmployeeLabels")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetAddressesResponse GetAccountWideAndEmployeeLabels()
        {
            var response = this.InitialiseResponse<GetAddressesResponse>();
            response.List = ((AddressRepository)this.Repository).GetAccountWideAndEmployeeLabels();
            return response;
        }


        /// <summary>
        /// Gets the home and office addresses for the current employee
        /// </summary>      
        /// <returns>
        /// The <see cref="GetAddressesResponse">GetAddressesResponse</see>
        /// </returns>     
        [HttpGet, Route("GetHomeAndOfficeAddresses")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public GetAddressesResponse GetHomeAndOfficeAddresses()
        {
            var response = this.InitialiseResponse<GetAddressesResponse>();
            response.List = ((AddressRepository)this.Repository).GetHomeAndOfficeAddresses(this.CurrentUser.EmployeeID);
            return response;
        }

        /// <summary>
        /// Gets the address route and mapping details form a list of address identifiers
        /// </summary>
        /// <param name="request">
        /// The <see cref="RouteForAddressIdentifiersRequest">RouteForAddressIdentifiersRequest</see>
        /// </param>
        /// <returns>
        /// The <see cref="RouteResponse">RouteResponse</see> with the route and mapping details
        /// </returns>
        [HttpPost, Route("GetRouteForAddressIdentifiers")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public RouteResponse GetRouteForAddressIdentifiers([FromBody] RouteForAddressIdentifiersRequest request)
        {
            var response = this.InitialiseResponse<RouteResponse>();
            response.Item = ((AddressRepository)this.Repository).GetRouteForAddressIdentifiers(request.AddressIdentifiers, request.ClaimEmployeeId);
            return response;
        }

        /// <summary>
        /// Gets the journey route and mapping details for an Expense Id
        /// </summary>
        /// <param name="expenseId">
        /// The expense Id
        /// </param>
        /// <returns>
        /// The <see cref="RouteResponse">RouteResponse</see> with the route and mapping details
        /// </returns>
        [HttpGet, Route("GetRouteForExpenseItem")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public RouteResponse GetRouteForExpenseItem(int expenseId)
        {
            var response = this.InitialiseResponse<RouteResponse>();
            response.Item = ((AddressRepository)this.Repository).GetRouteForExpenseItem(expenseId);
            return response;
        }

        /// <summary>
        /// Gets the journey route and mapping details for address identifiers
        /// </summary>
        /// <param name="request">
        /// The <see cref="RouteForAddressIdentifiersRequest">RouteForAddressIdentifiersRequest</see>
        /// </param>
        /// <returns>
        /// The <see cref="RouteResponse">RouteResponse</see> with the route and mapping details
        /// </returns>
        [HttpPost, Route("GetRouteForAddresses")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public RouteResponse GetRouteForAddresses([FromBody] RouteForAddressIdentifiersRequest request)
        {
            var response = this.InitialiseResponse<RouteResponse>();
            response.Item = ((AddressRepository)this.Repository).GetRouteForAddresses(request.AddressIdentifiers, request.ClaimEmployeeId);
            return response;
        }

        /// <summary>
        /// Removes label from this <see cref="Address">Address</see>.
        /// </summary>
        /// <param name="id">The Id of the label to be removed.</param>
        /// <returns>A NumericResponse.</returns>
        [HttpPatch, Route("RemoveLabel/{id:int}")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.Edit)]
        public NumericResponse RemoveLabel(int id)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((AddressRepository)this.Repository).RemoveLabel(id);
            return response;
        }

        /// <summary>
        /// Adds label to this <see cref="Address">Address</see>.
        /// Note that if an address has any labels, then one must be the primary label. 
        /// Set this with the primary option.
        /// </summary>
        /// <param name="addressLabelRequest">
        /// The address Label Request.
        /// </param>
        /// <returns>
        /// A NumericResponse.
        /// </returns>
        [HttpPost, Route("AddLabel")]
        [AuthAudit(SpendManagementElement.None, AccessRoleType.View)]
        public NumericResponse AddLabel([FromBody] AddressLabelRequest addressLabelRequest)
        {
            var response = this.InitialiseResponse<NumericResponse>();
            response.Item = ((AddressRepository)this.Repository).SaveLabel(addressLabelRequest.Identifier, addressLabelRequest.Label);
            return response;
        }

        /// <summary>
        /// The is mobile request.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsMobileRequest()
        {
            return Helper.IsMobileRequest(this.Request.Headers.UserAgent.ToString());
        }


        #endregion Api Methods
    }

}