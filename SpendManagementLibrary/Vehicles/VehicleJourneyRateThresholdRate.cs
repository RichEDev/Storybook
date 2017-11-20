namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;
    using System.Linq;

    using Exceptions;
    using Helpers;

    public class VehicleJourneyRateThresholdRate
    {

        public int? MileageThresholdRateId { get; set; }
	    public int CreatedBy { get; set; }
	    public DateTime CreatedOn { get; set; }
	    public int? ModifiedBy { get; set; }
	    public DateTime? ModifiedOn { get; set; }
        public int? MileageThresholdId { get; set; }
        public int? VehicleEngineTypeId { get; set; }
        public decimal? RatePerUnit { get; set; }
        public decimal? AmountForVat { get; set; }

        /// <summary>
        /// Queries the customer database and returns the VJR threshold rate with the specified ID
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="mileageThresholdRateId">The MileageThresholdRateId to get</param>
        /// <returns>VJR threshold rate, or null if none is set</returns>
        public static VehicleJourneyRateThresholdRate Get(ICurrentUserBase currentUser, int mileageThresholdRateId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@MileageThresholdRateId", mileageThresholdRateId);
                using (IDataReader reader = connection.GetReader("GetMileageThresholdRates", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        return new VehicleJourneyRateThresholdRate().Populate(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Queries the customer database and returns the VJR threshold rate with the specified engine type for the specified Threshold
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="mileageThresholdId">mileageThresholdId</param>
        /// <param name="vehicleEngineTypeId">vehicleEngineTypeId</param>
        /// <returns>VJR threshold rate, or null if none is set</returns>
        public static VehicleJourneyRateThresholdRate Get(ICurrentUserBase currentUser, int mileageThresholdId, int vehicleEngineTypeId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@MileageThresholdId", mileageThresholdId);
                connection.AddWithValue("@VehicleEngineTypeId", vehicleEngineTypeId);
                using (IDataReader reader = connection.GetReader("GetMileageThresholdRateByThresholdAndEngine", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        return new VehicleJourneyRateThresholdRate().Populate(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Queries the customer database and returns all VJR threshold rates sorted by Mileage Threshold ID and Engine type name
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <returns>A collection of Vehicle engine types</returns>
        public static ReadOnlyCollection<VehicleJourneyRateThresholdRate> GetAll(ICurrentUserBase currentUser)
        {
            var mileageThresholdRates = new List<VehicleJourneyRateThresholdRate>();
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                using (IDataReader reader = connection.GetReader("GetMileageThresholdRates", CommandType.StoredProcedure))
                {
                    while (reader.Read())
                    {
                        mileageThresholdRates.Add(new VehicleJourneyRateThresholdRate().Populate(reader));
                    }
                }
            }

            return mileageThresholdRates.AsReadOnly();
        }

        /// <summary>
        /// Create a new VehicleEngineType and VehicleJourneyRateThresholdRate for the given engineName
        /// with the provided details or update the existing records if they already exist.
        /// </summary>
        /// <param name="currentUser">The current user.</param>
        /// <param name="isNhsCustomer">if the Account is an NHS customer.</param>
        /// <param name="engines">A collection of all VehicleEngineTypes currently in the system.</param>
        /// <param name="engineName">The name of the engine type (e.g. Petrol, Diesel).</param>
        /// <param name="nhsCode">The code to set if the Account is an NHS customer.</param>
        /// <param name="mileageThresholdId">The ID of the parent MileageThreshold.</param>
        /// <param name="ratePerUnit">The rate per mile/KM.</param>
        /// <param name="amountForVat">The amount which is subject to VAT.</param>
        /// <returns></returns>
        public static VehicleJourneyRateThresholdRate CreateOrUpdate(ICurrentUserBase currentUser, bool isNhsCustomer, List<VehicleEngineType> engines,
            string engineName, string nhsCode, int mileageThresholdId, decimal ratePerUnit, decimal amountForVat)
        {
            var engine = engines.FirstOrDefault(vet => vet.Name.Equals(engineName, StringComparison.InvariantCultureIgnoreCase));

            if (engine == null)
            {
                engine = new VehicleEngineType
                {
                    Name = engineName,
                    Code = (isNhsCustomer ? nhsCode : null)
                }.Save(currentUser);
                engines.Add(engine);
            }
            else if (isNhsCustomer && engine.Code != nhsCode)
            {
                engine.Code = nhsCode;
                engine.Save(currentUser);
            }

            var rate = VehicleJourneyRateThresholdRate.Get(currentUser, mileageThresholdId, (int)engine.VehicleEngineTypeId) ??
                       new VehicleJourneyRateThresholdRate
                       {
                           MileageThresholdId = mileageThresholdId,
                           VehicleEngineTypeId = engine.VehicleEngineTypeId
                       };
            rate.RatePerUnit = ratePerUnit;
            rate.AmountForVat = amountForVat;
            return rate.Save(currentUser);
        }

        /// <summary>
        /// Saves this instance of a VJR threshold rate to the database
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <returns>The saved instance</returns>
        public VehicleJourneyRateThresholdRate Save(ICurrentUserBase currentUser)
        {
            this.Validate(currentUser);

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@MileageThresholdRateId", (object)this.MileageThresholdRateId ?? DBNull.Value);
                connection.AddWithValue("@MileageThresholdId", this.MileageThresholdId);
                connection.AddWithValue("@VehicleEngineTypeId", this.VehicleEngineTypeId);
                connection.AddWithValue("@RatePerUnit", this.RatePerUnit);
                connection.AddWithValue("@AmountForVat", this.AmountForVat);

                connection.AddWithValue("@CuEmployeeId", currentUser.EmployeeID);
                connection.AddWithValue("@CuDelegateId", (currentUser.isDelegate ? (object)currentUser.Delegate.EmployeeID : DBNull.Value));

                connection.AddReturn("@Result");

                connection.ExecuteProc("SaveMileageThresholdRate");

                var result = connection.GetReturnValue<int>("@Result");
                switch (result)
                {
                    case 0: // Unknown error
                        throw new Exception("Save failed.");

                    case -1: // Duplicate Vehicle engine type
                        throw new ValidationException("A Fuel rate for the selected Vehicle engine type already exists.", "VehicleEngineType");

                }

                connection.ClearParameters();
                connection.AddWithValue("@MileageThresholdRateId", result);

                using (IDataReader reader = connection.GetReader("GetMileageThresholdRates", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        this.Populate(reader);
                    }
                }
            }

            return this;
        }

        /// <summary>
        /// Delete a VJR threshold rate from the database
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="mileageThresholdRateId">The VJR threshold rate ID to delete</param>
        public static void Delete(ICurrentUserBase currentUser, int mileageThresholdRateId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@MileageThresholdRateId", mileageThresholdRateId);

                connection.AddWithValue("@CuEmployeeId", currentUser.EmployeeID);
                connection.AddWithValue("@CuDelegateId", (currentUser.isDelegate ? (object)currentUser.Delegate.EmployeeID : DBNull.Value));

                connection.AddReturn("@Result");

                connection.ExecuteProc("DeleteMileageThresholdRate");

                var result = connection.GetReturnValue<int>("@Result");
                if (result <= 0)
                {
                    throw new Exception("Delete failed.");
                }

            }
        }

        private void Validate(ICurrentUserBase currentUser)
        {
            if (this.MileageThresholdId == null || this.MileageThresholdId == 0)
            {
                throw new ValidationException("Please select a VJR threshold.", "MileageThreshold");
            }
            if (this.VehicleEngineTypeId == null || this.VehicleEngineTypeId == 0)
            {
                throw new ValidationException("Please select a Vehicle engine type.", "VehicleEngineType");
            }
            if (VehicleEngineType.Get(currentUser, (int)this.VehicleEngineTypeId) == null)
            {
                throw new ValidationException("Please select a valid Vehicle engine type.", "VehicleEngineType");
            }
            if (this.RatePerUnit == null)
            {
                throw new ValidationException("Please enter a Rate per mile/KM.", "RatePerUnit");
            }
            if (this.AmountForVat == null)
            {
                throw new ValidationException("Please enter an Amount for VAT.", "AmountForVat");
            }
        }

        private VehicleJourneyRateThresholdRate Populate(IDataReader reader)
        {
            this.MileageThresholdRateId = reader.GetRequiredValue<int>("MileageThresholdRateId");
            this.CreatedBy = reader.GetRequiredValue<int>("CreatedBy");
            this.CreatedOn = reader.GetRequiredValue<DateTime>("CreatedOn");
            this.ModifiedBy = reader.GetNullable<int>("ModifiedBy");
            this.ModifiedOn = reader.GetNullable<DateTime>("ModifiedOn");
            this.MileageThresholdId = reader.GetRequiredValue<int>("MileageThresholdId");
            this.VehicleEngineTypeId = reader.GetRequiredValue<int>("VehicleEngineTypeId");
            this.RatePerUnit = reader.GetRequiredValue<decimal>("RatePerUnit");
            this.AmountForVat = reader.GetRequiredValue<decimal>("AmountForVat");

            return this;
        }

    }
}
