namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;
    using Models.Types;
    using Models.Common;
    using Utilities;

    using Spend_Management;
    using SMLA = SpendManagementLibrary.Addresses;

    using System.Collections.ObjectModel;
    using Models.Requests;

    using SpendManagementApi.Common.Enums;



    /// <summary>
    /// OrganisationRepository manages data access for Organisations.
    /// </summary>
    internal class OrganisationRepository : ArchivingBaseRepository<Organisation>, ISupportsActionContext
    {

        /// <summary>
        /// Creates a new OrganisationRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public OrganisationRepository(ICurrentUser user, IActionContext actionContext) 
            : base(user, actionContext, x => x.Id, x => x.Label)
        {
        }

        /// <summary>
        /// Gets all the Organisations within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<Organisation> GetAll()
        {
            return SMLA.Organisations.GetAll(this.User.AccountID).Select(o => new Organisation().From(o, this.ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a single Organisation by it's id.
        /// </summary>
        /// <param name="id">The id of the Organisation to get.</param>
        /// <returns>The Organisation.</returns>
        public override Organisation Get(int id)
        {
            return new Organisation().From(SMLA.Organisation.Get(this.User.AccountID, id), this.ActionContext);
        }

        /// <summary>
        /// Gets a List of <see cref="Organisation">Organisation</see> for the specified criteria
        /// </summary>
        /// <param name="request">The <see cref="FindOrganisationRequest">FindOrganisationRequest</see></param>
        /// <returns>A List of <see cref="Organisation">Organisation</see></returns>
        public List<Organisation> GetOrganisationByCriteria(FindOrganisationRequest request)
        {
            SMLA.Organisations organisations = ActionContext.Organisations;

            ReadOnlyCollection<SMLA.Organisation> filteredOrgs = organisations.GetByCriteria(this.User.AccountID, request.Label, request.Comment, request.Code,
                request.AddressLine1, request.City, request.PostCode, request.Archived);

            List<Organisation> list = new List<Organisation>();

            foreach (SMLA.Organisation filteredOrganisation in filteredOrgs)
            {
                Organisation organisation = new Organisation().From(filteredOrganisation, ActionContext);

                if (filteredOrganisation.PrimaryAddress != null)
                {
                    SMLA.Address address = this.ActionContext.Addresses.GetAddressById(filteredOrganisation.PrimaryAddress.Identifier);
                    organisation.AddressLine1 = address.Line1;
                    organisation.City = address.City;
                    organisation.PostCode = address.Postcode;
                }
              
                list.Add(organisation);
            }

            return list;
        }

        /// <summary>
        /// Adds an Organisation.
        /// </summary>
        /// <param name="dataToAdd">The Organisation to add.</param>
        /// <param name="isMobileRequest">Is the request from mobile?</param>
        /// <returns></returns>
        public Organisation Add(Organisation dataToAdd, bool isMobileRequest)
        {
            dataToAdd = base.Add(dataToAdd);

            var properties = new cAccountSubAccounts(this.User.AccountID).getSubAccountById(this.User.CurrentSubAccountId).SubAccountProperties;
            if (!properties.ClaimantsCanAddCompanyLocations)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.HttpStatusCodeForbidden);
            }

            dataToAdd.Validate(ActionContext);

            var result = SMLA.Organisation.Save(this.User, dataToAdd.Id, dataToAdd.ParentOrganisationId,
                dataToAdd.PrimaryAddressId, dataToAdd.Label, dataToAdd.Comment, dataToAdd.Code);

            switch (result)
            {
                case 0:  // Unknown error

                    if (!isMobileRequest)
                    {
                        throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
                    }

                    return  new Organisation {OrganisationActionOutcome = OrganisationActionOutcome.UnexpectedError};                   

                case -1: // Name already exists

                    if (!isMobileRequest)
                    {
                        throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorInvalidNameAlreadyExists);
                    }

                    return new Organisation { OrganisationActionOutcome = OrganisationActionOutcome.OrganisationNameAlreadyExists };
            }

            return this.Get(result);
        }

        /// <summary>
        /// Updates an Organisation.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated Organisation.</returns>
        public override Organisation Update(Organisation item)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an Organisation, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the Organisation to delete.</param>
        /// <returns>The deleted Organisation.</returns>
        public override Organisation Delete(int id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Archives / unarchives the item with the specified id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="archive"></param>
        /// <returns></returns>
        public override Organisation Archive(int id, bool archive)
        {
            throw new NotImplementedException();
        }

    }
}