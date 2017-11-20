namespace EsrGo2FromNhs.Crud
{
    using System.Collections.Generic;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Interfaces;

    /// <summary>
    /// The vehicle API.
    /// </summary>
    public class DataClassBaseApi : IApi<DataClassBase>
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
        /// The <see cref="List"/>.
        /// </returns>
        public List<DataClassBase> Send(int accountId, string trustVpd, List<DataClassBase> dataRecords)
        {
            return new List<DataClassBase>();    
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="entity">
        /// The vehicle.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public DataClassBase Delete(int accountId, DataClassBase entity)
        {
            var apiRpc = new ApiRpc();
            return apiRpc.DeleteEntity(accountId, entity);
        }
    }
}