namespace EsrGo2FromNhsWcfLibrary.ESR
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A class to compare to <see cref="EmployeeLocation"/> instances.
    /// </summary>
    public class EmployeeLocationComparer : IEqualityComparer<EmployeeLocation>
    {
        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <param name="x">The first object of type <paramref name="EmployeeLocation" /> to compare.</param>
        /// <param name="y">The second object of type <paramref name="EmployeeLocation" /> to compare.</param>
        public bool Equals(EmployeeLocation x, EmployeeLocation y)
        {
            //Check whether the objects are the same object. 
            if (Object.ReferenceEquals(x, y))
            {
                return true;
            }

            //Check whether the addresses' properties are equal. 
            return x != null && y != null && x.EmployeeId.Equals(y.EmployeeId) && x.PostCode.Equals(y.PostCode);
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <returns>A hash code for the specified object.</returns>
        /// <param name="obj">The <see cref="T:System.Object" /> for which a hash code is to be returned.</param>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj" /> is a reference type and <paramref name="obj" /> is null.</exception>
        public int GetHashCode(EmployeeLocation obj)
        {
            //Get hash code for the postcode field if it is not null. 
            int hashPostcode = obj.PostCode == null ? 0 : obj.PostCode.GetHashCode();

            //Get hash code for the employeeid field. 
            int hashEmployeeId = obj.EmployeeId.GetHashCode();

            //Calculate the hash code for the product. 
            return hashPostcode ^ hashEmployeeId;
        }
    }
}
