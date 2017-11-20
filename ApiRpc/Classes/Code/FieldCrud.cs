namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.Spend_Management;
    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    /// <summary>
    /// The field crud.
    /// </summary>
    public class FieldCrud : EntityBase, IDataAccess<Field>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FieldCrud"/> class.
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
        public FieldCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            if (this.DataSources.FieldApi == null)
            {
                this.DataSources.FieldApi = new FieldApi(metabase, accountid);
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
        public List<Field> Create(List<Field> entities)
        {
            return this.DataSources.FieldApi.Create(entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Read(int entityId)
        {
            return this.DataSources.FieldApi.Read(entityId);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Read(long entityId)
        {
            return this.DataSources.FieldApi.Read(entityId);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Field> ReadAll()
        {
            return this.DataSources.FieldApi.ReadAll();
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
        public List<Field> ReadByEsrId(long personId)
        {
            return this.DataSources.FieldApi.ReadByEsrId(personId);
        }

        public List<Field> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public List<Field> Update(List<Field> entity)
        {
            return this.DataSources.FieldApi.Update(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Delete(int entityId)
        {
            return this.DataSources.FieldApi.Delete(entityId);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Delete(long entityId)
        {
            return this.DataSources.FieldApi.Delete(entityId);
        }

        public Field Delete(Field entity)
        {
            throw new NotImplementedException();
        }
    }
}