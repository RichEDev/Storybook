namespace EsrGo2FromNhsWcfLibrary.Interfaces
{
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary.Base;

    /// <summary>
    /// The API interface.
    /// </summary>
    /// <typeparam name="T">
    /// Data Class Base
    /// </typeparam>
    public interface IApi<T> where T : DataClassBase
    {
        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="dataRecords">
        /// The data records.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        List<T> Send(int accountId, string trustVpd, List<T> dataRecords);
    }
}
