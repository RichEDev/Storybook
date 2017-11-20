namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Spend_Management;
    using Models.Common;
    using Models.Types;
    using Utilities;
    
    /// <summary>
    /// OdometerReadingRepository manages data access for OdometerReadings.
    /// </summary>
    internal class OdometerReadingRepository : BaseRepository<OdometerReading>
    {
        private readonly cEmployees _employees;
        private readonly cPoolCars _data;
    
        /// <summary>
        /// Creates a new OdometerReadingRepository with the passed in user.
        /// </summary>
        /// <param name="user"></param>
        public OdometerReadingRepository(ICurrentUser user) : base(user, x => x.OdometerReadingId, null)
        {
            _data = new cPoolCars(User.AccountID);
             _employees = new cEmployees(User.AccountID);
        }

        /// <summary>
        /// Gets all the OdometerReadings within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<OdometerReading> GetAll()
        {
            return null;
        }

        /// <summary>
        /// Gets a single OdometerReading by it's id.
        /// </summary>
        /// <param name="id">The id of the OdometerReading to get.</param>
        /// <returns>The OdometerReading.</returns>
        public override OdometerReading Get(int id)
        {
            var item = _data.GetOdometerReadingById(id);
            return item == null ? null : item.Cast<OdometerReading>();
        }

        /// <summary>
        /// Adds a OdometerReading.
        /// </summary>
        /// <param name="item">The OdometerReading to add.</param>
        /// <returns></returns>
        public override OdometerReading Add(OdometerReading item)
        {
            item = base.Add(item);
            return Save(item);
        }
        
        /// <summary>
        /// Updates a OdometerReading.
        /// </summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated OdometerReading.</returns>
        public override OdometerReading Update(OdometerReading item)
        {
            item = base.Update(item);
            return Save(item);
        }

        /// <summary>
        /// Deletes a OdometerReading, given it's ID.
        /// </summary>
        /// <param name="id">The Id of the OdometerReading to delete.</param>
        /// <returns>The deleted OdometerReading.</returns>
        public override OdometerReading Delete(int id)
        {
            var item = base.Delete(id);
            _employees.deleteOdometerReading(item.EmployeeId.HasValue ? item.EmployeeId.Value : item.CreatedById, item.CarId, item.OdometerReadingId);
            return Get(id);
        }



        private OdometerReading Save(OdometerReading item)
        {
            item.AccountId = User.AccountID;
            var id = _employees.saveOdometerReading(item.OdometerReadingId, User.EmployeeID, item.CarId, item.ReadingDate, item.OldReading, item.NewReading, 2);

            if (id == -1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                    ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (id < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorSaveUnsuccessfulMessage + ApiResources.ApiErrorOdometerReading);
            }
            item.OdometerReadingId = id;
     
            return item;
        }



        /// <summary>
        /// Gets all the Odometer readings for a specific car.
        /// </summary>
        /// <param name="carId">The Id of the car.</param>
        /// <returns>A List of Odometer readings.</returns>
        public IList<OdometerReading> ForCar(int carId)
        {
            return _data.GetOdometerReadings(carId).Select(x => x.Cast<OdometerReading>()).ToList();
        }

    }
}
