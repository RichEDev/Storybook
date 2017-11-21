namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using SpendManagementLibrary.Addresses;
    using Spend_Management;
    using Interfaces;
    using Models.Common;
    using Utilities;
    using Models.Types;

    using SpendManagementLibrary;

    internal class AddressRecommendedDistanceRepository : BaseRepository<AddressRecommendedDistance>, ISupportsActionContext
    {
        private Addresses _addresses;

        /// <summary>
        /// Archiving Base repository constructor, taking an action context.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="actionContext">ActionContext</param>
        public AddressRecommendedDistanceRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => x.Id, null)
        {
            _addresses = ActionContext.Addresses;
        }

        /// <summary>Gets all Addresses from the Database.</summary>
        /// <returns>A List of Addresses.</returns>
        public override IList<AddressRecommendedDistance> GetAll()
        {
            throw new NotImplementedException(ApiResources.ApiErrorAddressDistanceGetAllNotImplmented);
        }

        /// <summary>Gets all AddressDistances for an Address.</summary>
        /// <param name="id">The Id of the Address to fetch distances for.</param>
        /// <returns>A List of Addresses.</returns>
        public IList<AddressRecommendedDistance> GetByAddress(int id)
        {
            if (id < 1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressNotFound);
            }

            var address = ActionContext.Addresses.GetAddressById(id);
            if (address == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressNotFound);
            }

            var distances = AddressDistance.GetForAddressApi(User.AccountID, id).Select(x => new AddressRecommendedDistance
            {
                Id = x.DestinationIdentifier,
                AddressAId = x.OutboundIdentifier,
                AddressBId = x.ReturnIdentifier,
                RecommendedDistance = x.Outbound
            }).ToList();

            return distances;
        }

        /// <summary>
        /// Get item by Id.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns></returns>
        public override AddressRecommendedDistance Get(int id)
        {
            AddressDistanceLookup distance = AddressDistance.GetById(User.AccountID, id);
            if (distance == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist, ApiResources.ApiErrorAddressDistanceIdMustBeValid);
            }
            return new AddressRecommendedDistance().From(distance, ActionContext);
        }

        /// <summary>
        /// Adds a Manual Address
        /// </summary>
        /// <param name="item">The manual address to add.</param>
        /// <returns></returns>
        public override AddressRecommendedDistance Add(AddressRecommendedDistance item)
        {
            if (item.Id != 0)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            return Save(item);
        }

        /// <summary>
        /// Updates a manual address.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated Address.</returns>
        public override AddressRecommendedDistance Update(AddressRecommendedDistance item)
        {
            if (item.Id < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorUpdateObjectWithWrongIdMessage);
            }

            return Save(item);
        }

        /// <summary>
        /// Deletes a manual address, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the Address to delete.</param>
        /// <returns>The deleted manual address.</returns>
        public override AddressRecommendedDistance Delete(int id)
        {
            var item = base.Delete(id);

            var result = AddressDistance.Delete(User, id);
            if (result < 1)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return Get(id);
        }

        /// <summary>
        /// Gets the recommended or custom distance for origin and destination address Ids.
        /// </summary>
        /// <param name="originAddressId"> The origin Address Id. </param>
        /// <param name="destinationAddressId"> The destination Address Id </param>
        /// <param name="vehicleId"> The Id of the vehicle the distance is calculated for. Will be 0 for reconcile mobile journys</param>
        /// <returns>
        /// The decimal mileage distance. <see cref="decimal"/>.
        /// </returns>
        public decimal? GetRecommendedOrCustomDistance(int originAddressId, int destinationAddressId, int vehicleId = 0)
        {
            cAccountProperties subAccountProperties = this.ActionContext.SubAccounts.getFirstSubAccount().SubAccountProperties;
            var origin = this.ActionContext.Addresses.GetAddressById(originAddressId);
            var destination = this.ActionContext.Addresses.GetAddressById(destinationAddressId);
            var distance = AddressDistance.GetRecommendedOrCustomDistance(origin, destination, subAccountProperties.UseMapPoint, subAccountProperties.MileageCalcType, this.User);

            if (distance == null)
            {
                return 0;
            }

            if (vehicleId == 0)
            {
                return distance;
            }

            bool convertToKilometer = false;
            var vehicleRepository = new VehicleRepository(User);
            var vehicle = vehicleRepository.Get(vehicleId);

            convertToKilometer = vehicle.UnitOfMeasure == MileageUOM.KM;
          
            return convertToKilometer ? this.ActionContext.MileageCategories.convertMilesToKM((decimal)distance) : distance;
        }

        /// <summary>
        /// Saves an Address, performing some validation.
        /// </summary>
        /// <param name="item">The address to save.</param>
        /// <returns>The saved item.</returns>
        private AddressRecommendedDistance Save(AddressRecommendedDistance item)
        {
            item.EmployeeId = User.EmployeeID;
            item.ModifiedById = User.EmployeeID;
            item.ModifiedOn = DateTime.UtcNow;

            // check employee here.
            var employees = ActionContext.Employees;

            // ReSharper disable once PossibleInvalidOperationException
            if (employees.GetEmployeeById(item.EmployeeId.Value) == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId,
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            // grab and validate any linked addresses
            var addresses = ActionContext.Addresses;
            var attemptedAddressA = addresses.GetAddressById(item.AddressAId);
            var attemptedAddressB = addresses.GetAddressById(item.AddressBId);

            if (attemptedAddressA == null)
            {
                throw new ApiException(ApiResources.ApiErrorAddressNotFound, ApiResources.ApiErrorAddressANotFound);
            }

            if (attemptedAddressB == null)
            {
                throw new ApiException(ApiResources.ApiErrorAddressNotFound, ApiResources.ApiErrorAddressBNotFound);
            }

            // check that these two aren't already used if the ID is 0
            if (item.Id == 0)
            {
                var distancesForA = AddressDistance.GetForAddress(User.AccountID, item.AddressAId);
                if (distancesForA.Select(d => d.DestinationIdentifier).Contains(item.AddressBId))
                {
                    throw new InvalidDataException(ApiResources.ApiErrorAddressDistanceExists);
                }
            }
            else
            {
                // otherwise, check the user is not trying to modify the two address endpoints
                var existingDistance = Get(item.Id);
                if (existingDistance.AddressAId != item.AddressAId || existingDistance.AddressBId != item.AddressBId)
                {
                    throw new InvalidDataException(ApiResources.ApiErrorAddressDistanceAttemptEndPointEdit);
                }

            }
            if (item.RecommendedDistance < 0)
            {
                throw new InvalidDataException(ApiResources.ApiErrorAddressDistanceCannotBeNegative);
            }

            var id = AddressDistance.Save(User, item.AddressAId, item.AddressBId, 1, item.RecommendedDistance, null, true);

            if (id < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            return Get(id);
        }
    }
}