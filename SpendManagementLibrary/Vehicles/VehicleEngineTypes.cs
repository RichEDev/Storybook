namespace SpendManagementLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;

    using Exceptions;
    using Helpers;

    public class VehicleEngineType
    {

        public static string[] EsrReservedCodes = { "Petrol", "Diesel", "Liquid Petroleum Gas", "Electric Only", "Diesel - Euro IV compliant", "Bi-Fuel", "Conversion", "E85", "Hybrid Electric" };

        public int? VehicleEngineTypeId { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        /// <summary>
        /// Queries the customer database and returns the Vehicle engine type with the specified ID
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="vehicleEngineTypeId">The VehicleEngineTypeId to get</param>
        /// <returns>Vehicle engine type, or null if none is set</returns>
        public static VehicleEngineType Get(ICurrentUserBase currentUser, int vehicleEngineTypeId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@VehicleEngineTypeId", vehicleEngineTypeId);
                using (IDataReader reader = connection.GetReader("GetVehicleEngineTypes", CommandType.StoredProcedure))
                {
                    if (reader.Read())
                    {
                        return new VehicleEngineType().Populate(reader);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Queries the customer database and returns all Vehicle engine types sorted by Name
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <returns>A collection of Vehicle engine types</returns>
        public static ReadOnlyCollection<VehicleEngineType> GetAll(ICurrentUserBase currentUser)
        {
            var vehicleEngineTypes = new List<VehicleEngineType>();
            if (currentUser != null)
            {
                using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
                {
                    using (IDataReader reader = connection.GetReader("GetVehicleEngineTypes", CommandType.StoredProcedure))
                    {
                        while (reader.Read())
                        {
                            vehicleEngineTypes.Add(new VehicleEngineType().Populate(reader));
                        }
                    }
                }
            }

            return vehicleEngineTypes.AsReadOnly();
        }

        /// <summary>
        /// Saves this instance of a Vehicle engine type to the database
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <returns>The saved instance</returns>
        public VehicleEngineType Save(ICurrentUserBase currentUser)
        {
            if (String.IsNullOrWhiteSpace(this.Name))
            {
                throw new ValidationException("Please enter a Vehicle engine type.", "Name");
            }

            var html = new System.Text.RegularExpressions.Regex("[<>]");
            if (html.IsMatch(this.Name))
            {
                throw new ValidationException("Please enter a valid Vehicle engine type.", "Name");
            }
            if (this.Code != null && html.IsMatch(this.Code))
            {
                throw new ValidationException("Please enter a valid Code.", "Code");
            }

            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@VehicleEngineTypeId", (object)this.VehicleEngineTypeId ?? DBNull.Value);
                connection.AddWithValue("@Code", (object)this.Code ?? DBNull.Value);
                connection.AddWithValue("@Name", this.Name);

                connection.AddWithValue("@CuEmployeeId", currentUser.EmployeeID);
                connection.AddWithValue("@CuDelegateId", (currentUser.isDelegate ? (object)currentUser.Delegate.EmployeeID : DBNull.Value));

                connection.AddReturn("@Result");

                connection.ExecuteProc("SaveVehicleEngineType");

                var result = connection.GetReturnValue<int>("@Result");
                switch (result)
                {
                    case 0: // Unknown error
                        throw new Exception("Save failed.");

                    case -1: // Duplicate Name
                        throw new ValidationException("The Vehicle engine type already exists.", "Name");

                    case -2: // Duplicate Code
                        throw new ValidationException("The Code already exists.", "Code");
                }

                connection.ClearParameters();
                connection.AddWithValue("@VehicleEngineTypeId", result);

                using (IDataReader reader = connection.GetReader("GetVehicleEngineTypes", CommandType.StoredProcedure))
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
        /// Delete a Vehicle engine type from the database
        /// </summary>
        /// <param name="currentUser">The current user</param>
        /// <param name="vehicleEngineTypeId">The Vehicle engine type ID to delete</param>
        public static void Delete(ICurrentUserBase currentUser, int vehicleEngineTypeId)
        {
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(currentUser.AccountID)))
            {
                connection.AddWithValue("@VehicleEngineTypeId", vehicleEngineTypeId);

                connection.AddWithValue("@CuEmployeeId", currentUser.EmployeeID);
                connection.AddWithValue("@CuDelegateId", (currentUser.isDelegate ? (object)currentUser.Delegate.EmployeeID : DBNull.Value));

                connection.AddReturn("@Result");

                connection.ExecuteProc("DeleteVehicleEngineType");

                var result = connection.GetReturnValue<int>("@Result");
                if (result <= 0)
                {
                    throw new Exception("Delete failed.");
                }

            }
        }

        private VehicleEngineType Populate(IDataReader reader)
        {
            if (reader.IsClosed)
            {
                throw new InvalidOperationException("Cannot populate with a closed DataReader");
            }

            this.VehicleEngineTypeId = reader.GetRequiredValue<int>("VehicleEngineTypeId");
            this.CreatedBy = reader.GetRequiredValue<int>("CreatedBy");
            this.CreatedOn = reader.GetRequiredValue<DateTime>("CreatedOn");
            this.ModifiedBy = reader.GetNullable<int>("ModifiedBy");
            this.ModifiedOn = reader.GetNullable<DateTime>("ModifiedOn");
            this.Code = reader.GetNullableRef<string>("Code");
            this.Name = reader.GetRequiredValue<string>("Name");

            return this;
        }

    }
}
