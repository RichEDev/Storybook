namespace ApiRpc.Classes.Code
{
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.ESR;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The ESR vehicles crud helper class.
    /// </summary>
    public class EsrVehiclesCrud : EntityBase, IDataAccess<EsrVehicle>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrVehiclesCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API Collection.
        /// </param>
        public EsrVehiclesCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.EsrVehicleApi == null)
            {
                this.DataSources.EsrVehicleApi = new VehicleApi(metabase, accountid);
            }
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrVehicle> Create(List<EsrVehicle> entities)
        {
            return this.DataSources.EsrVehicleApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Read(int entityId)
        {
            return this.DataSources.EsrVehicleApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Read(long entityId)
        {
            return this.DataSources.EsrVehicleApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrVehicle> ReadAll()
        {
            return this.DataSources.EsrVehicleApi.ReadAll();
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrVehicle> ReadByEsrId(long personId)
        {
            return this.DataSources.EsrVehicleApi.ReadByEsrId(personId);
        }

        public List<EsrVehicle> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<EsrVehicle> Update(List<EsrVehicle> entities)
        {
            return this.DataSources.EsrVehicleApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Delete(int entityId)
        {
            return this.DataSources.EsrVehicleApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        public EsrVehicle Delete(long entityId)
        {
            return this.DataSources.EsrVehicleApi.Delete(entityId);
        }

        public EsrVehicle Delete(EsrVehicle entity)
        {
            return this.DataSources.EsrVehicleApi.Delete(entity);
        }
    }
}