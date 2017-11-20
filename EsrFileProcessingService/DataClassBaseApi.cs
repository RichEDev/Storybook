namespace EsrFileProcessingService
{
    using System.Collections.Generic;

    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;

    using EsrFileProcessingService.ApiService;

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
            var svc = new ApiRpcClient();
            return svc.DeleteEntity(accountId,  entity);
        }
    }
}