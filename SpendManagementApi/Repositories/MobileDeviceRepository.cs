using System;

namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Spend_Management;
    using Models.Common;
    using Models.Types;
    using Utilities;

    /// <summary>
    /// Manages data acceess for <see cref="MobileDevice">MobileDevices</see>.
    /// </summary>
    internal class MobileDeviceRepository : BaseRepository<MobileDevice>
    {
        private cMobileDevices _data;
        private cEmployees _employees;

        /// <summary>
        /// Creates a new OdometerReadingRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public MobileDeviceRepository(ICurrentUser user)
            : base(user, x => x.Id, x => x.DeviceName)
        {
            _data = ActionContext.MobileDevices;
            _employees = ActionContext.Employees;
        }

        /// <summary>
        /// Gets the list of all mobile device types from metabase.
        /// </summary>
        /// <returns></returns>
        public List<MobileDeviceType> GetMobileDeviceTypes()
        {
            return _data.MobileDeviceTypes.Select(t => new MobileDeviceType().From(t.Value, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets all entries
        /// </summary>
        /// <returns>A List of MobileDevice</returns>
        public override IList<MobileDevice> GetAll()
        {
            return _data.GetAllDevicesAsList().Select(d => new MobileDevice().From(d, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a single entry by it's id.
        /// </summary>
        /// <param name="id">The id of the entry.</param>
        /// <returns>The Entry, of Type MobileDevice</returns>
        public override MobileDevice Get(int id)
        {
            var item = _data.GetMobileDeviceById(id);
            return item == null ? null : new MobileDevice().From(item, ActionContext);
        }

        /// <summary>
        /// Adds a new Entry to the repository.
        /// </summary>
        /// <param name="dataToAdd">The item to add</param>
        /// <returns>The added item</returns>
        public override MobileDevice Add(MobileDevice dataToAdd)
        {
            var empId = dataToAdd.EmployeeId;
            dataToAdd = base.Add(dataToAdd);
            dataToAdd.EmployeeId = empId;
            dataToAdd.AccountId = User.AccountID;
            dataToAdd.CreatedById = User.EmployeeID;
            dataToAdd.CreatedOn = DateTime.UtcNow;

            return SaveWithChecks(dataToAdd, true);
        }

        /// <summary>
        /// Updates an item in the repository.
        /// </summary>
        /// <param name="dataToUpdate">The item to update.</param>
        /// <returns>The updated item.</returns>
        public override MobileDevice Update(MobileDevice dataToUpdate)
        {
            var dbItem = Get(dataToUpdate.Id);
            dataToUpdate.AccountId = User.AccountID;
            dataToUpdate.ModifiedById = User.EmployeeID;
            dataToUpdate.ModifiedOn = DateTime.UtcNow;

            if (dbItem.EmployeeId != dataToUpdate.EmployeeId)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, ApiResources.ApiErrorCannotReAssignMobileByChangingEmployee);
            }
            if (dbItem.Type != dataToUpdate.Type)
            {
                throw new ApiException(ApiResources.ApiErrorUpdateObjectWithWrongId, ApiResources.ApiErrorCannotChangeDeviceType);
            }
            return SaveWithChecks(dataToUpdate, false);
        }

        /// <summary>
        /// Deletes an existing entry. Checks should be made to ensure that the record exists
        /// This method should throw an exception if data cannot be saved successfully
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public override MobileDevice Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteMobileDevice(item.Id, User.EmployeeID, User.isDelegate ? User.Delegate.EmployeeID : (int?)null);
            if (!result)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }


        /// <summary>
        /// Saves the item, validating too.
        /// </summary>
        /// <param name="item">The item to validate and save.</param>
        /// <returns></returns>
        private MobileDevice SaveWithChecks(MobileDevice device, bool generatePairingKey)
        {
            if (_employees.GetEmployeeById(device.EmployeeId) == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (!device.Type.HasValue || !_data.MobileDeviceTypes.ContainsKey(device.Type.Value))
            {
                throw new InvalidDataException(ApiResources.ApiErrorDeviceTypeInvalid);
            }

            if (_data.GetAllDevicesAsList().FirstOrDefault(x => x.DeviceName == device.DeviceName) != null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorDeviceNameExists);
            }

            var item = device.To(ActionContext);

            if (generatePairingKey)
            {
                item.PairingKey = _data.GeneratePairingKey(item.EmployeeID);
            }

            var result = _data.SaveMobileDevice(item, User.EmployeeID);

            if (result < 0)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful, ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            return Get(result);
        }
    }
}