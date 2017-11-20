namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The UDF crud.
    /// </summary>
    public class UdfCrud : EntityBase, IDataAccess<UserDefinedField>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UdfCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API collection.
        /// </param>
        public UdfCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.UdfApi == null)
            {
                this.DataSources.UdfApi = new UdfApi(metabase, accountid);
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
        public List<UserDefinedField> Create(List<UserDefinedField> entities)
        {
            return this.DataSources.UdfApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public UserDefinedField Read(int entityId)
        {
            return this.DataSources.UdfApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public UserDefinedField Read(long entityId)
        {
            return this.DataSources.UdfApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<UserDefinedField> ReadAll()
        {
            return this.DataSources.UdfApi.ReadAll();
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
        public List<UserDefinedField> ReadByEsrId(long personId)
        {
            return this.DataSources.UdfApi.ReadByEsrId(personId);
        }

        public List<UserDefinedField> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
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
        public List<UserDefinedField> Update(List<UserDefinedField> entities)
        {
            return this.DataSources.UdfApi.Update(entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public UserDefinedField Delete(int entityId)
        {
            return this.DataSources.UdfApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public UserDefinedField Delete(long entityId)
        {
            return this.DataSources.UdfApi.Delete(entityId);
        }

        public UserDefinedField Delete(UserDefinedField entity)
        {
            throw new NotImplementedException();
        }
    }
}