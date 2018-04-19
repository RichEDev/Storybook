namespace BusinessLogic.Employees
{
    using System;

    /// <summary>
    /// Defines an <see cref="Employee"/> and all it's members
    /// </summary>
    [Serializable]
    public class Employee : IEmployee
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Employee"/> class.
        /// </summary>
        /// <param name="id">The id of the <see cref="Employee"/></param>
        /// <param name="subAccountId">The sub account id of the <see cref="Employee"/></param>
        public Employee(int id, int subAccountId)
        {
            this.Id = id;
            this.SubAccountId = subAccountId;
        }

        /// <summary>
        /// Gets or sets the id for this <see cref="Employee"/>.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the sub account id of this <see cref="Employee"/>
        /// </summary>
        public int SubAccountId { get; set; }
    }
}
