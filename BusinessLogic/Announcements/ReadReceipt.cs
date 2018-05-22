namespace BusinessLogic.Announcements
{
    using System;

    /// <summary>
    /// A read receipt used in <seealso cref="IAnnouncement"/>.
    /// </summary>
    [Serializable]
    public class ReadReceipt : IEquatable<ReadReceipt>
    {
        /// <summary>
        /// Gets or sets the account ID.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public int EmployeeId { get; set; }

        /// <summary>Indicates whether the current object is equal to another object of the same type.</summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        /// <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.</returns>
        public bool Equals(ReadReceipt other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.AccountId == other.AccountId && this.EmployeeId == other.EmployeeId;
        }

        /// <summary>Determines whether the specified object is equal to the current object.</summary>
        /// <param name="obj">The object to compare with the current object. </param>
        /// <returns>
        /// <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((ReadReceipt)obj);
        }

        /// <summary>Serves as the default hash function. </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return (this.AccountId * 397) ^ this.EmployeeId;
            }
        }
    }
}