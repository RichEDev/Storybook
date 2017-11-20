namespace SpendManagementApi.Models.Types.Employees
{
    using System;
    using System.IO;

    using Interfaces;

    /// <summary>
    /// Represents a shorthand way of linking an Employee to a <see cref="PoolCar">PoolCar</see>, without needing all the nested properties inherent in the PoolCar.
    /// </summary>
    public class EmployeePoolCar : Deleteable, IRequiresValidation, IEquatable<EmployeePoolCar>
    {
        /// <summary>
        /// The Id of the <see cref="PoolCar">PoolCar</see>.
        /// </summary>
        public int PoolCarId { get; set; }

        /// <summary>
        /// Validates the Employee Pool Car.
        /// </summary>
        /// <param name="actionContext"></param>
        /// <exception cref="InvalidDataException"></exception>
        public void Validate(IActionContext actionContext)
        {
            if (PoolCarId <= 0)
            {
                throw new InvalidDataException("Valid PoolCarId must be provided");
            }
        }

        public bool Equals(EmployeePoolCar other)
        {
            if (other == null)
            {
                return false;
            }
            return this.PoolCarId.Equals(other.PoolCarId);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as EmployeePoolCar);
        }
    }
}