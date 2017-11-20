namespace SQLDataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using BusinessLogic;
    using BusinessLogic.DataConnections;
    using BusinessLogic.Identity;

    /// <summary>
    /// A collection of <see cref="SqlParameter"/> objects.
    /// </summary>
    public class SqlDataParameters : DataParameters<SqlParameter>
    {
        /// <summary>
        /// Gets the <see cref="SqlParameter"/> with the matching key/<c>SqlParameter.ParameterName</c>.
        /// </summary>
        /// <param name="key">The key to match on <c>SqlParameter.ParameterName</c></param>
        /// <returns></returns>
        public override SqlParameter this[string key]
        {
            get { return this.ParametersCollection.FirstOrDefault(x => x.ParameterName == key); }
        }

        /// <summary>
        /// Adds the specified <see cref="SqlParameter"/> to the end of the <see cref="SqlDataParameters"/> collection.
        /// </summary>
        /// <param name="value">The <see cref="SqlParameter"/> to add to the end of the <see cref="SqlDataParameters"/> collection.</param>
        public override void Add(SqlParameter value)
        {
            if (value == null)
            {
                return;
            }

            this.ParametersCollection.RemoveAll(x => x.ParameterName == value.ParameterName);
            this.ParametersCollection.Add(value);
        }

        /// <summary>
        /// Adds the specified <see cref="SqlParameter"/> objects to the end of the <see cref="SqlDataParameters"/> collection.
        /// </summary>
        /// <param name="values">A collection of <see cref="SqlParameter"/> objects to add to the end of the <see cref="SqlDataParameters"/> collection.</param>
        public override void Add(IEnumerable<SqlParameter> values)
        {
            if (values != null)
            {
                foreach (SqlParameter sqlParameter in values)
                {
                    this.Add(sqlParameter);
                }
            }
        }

        /// <summary>
        /// Adds the standard parameters as <see cref="SqlParameter"/> objects for auditing purposes.
        /// @CUdelegateID and @CUemployeeID
        /// </summary>
        /// <param name="currentUser">An instance of <see cref="UserIdentity"/> performing the action to be audited.</param>
        public override void AddAuditing(UserIdentity currentUser)
        {
            Guard.ThrowIfNull(currentUser, nameof(currentUser));

            if (currentUser.EmployeeId == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(currentUser.AccountId));
            }

            this.Add(new SqlParameter("@CUemployeeID", SqlDbType.Int) { Value = currentUser.EmployeeId });
            this.Add(currentUser.DelegateId.HasValue ? new SqlParameter("@CUdelegateID", SqlDbType.Int) {Value = currentUser.DelegateId.Value} : new SqlParameter("@CUdelegateID", SqlDbType.Int) {Value = DBNull.Value});
        }
    }
}
