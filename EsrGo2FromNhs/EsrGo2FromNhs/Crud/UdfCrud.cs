namespace EsrGo2FromNhs.Crud
{
    using System;
    using System.Collections.Generic;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The UDF crud.
    /// </summary>
    public class UdfCrud : EntityBase, IDataAccess<UserDefinedField>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UdfCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public UdfCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public List<UserDefinedField> Create(List<UserDefinedField> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public List<UserDefinedField> ReadAll()
        {
            return this.EsrApiHandler.Execute<UserDefinedField>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public List<UserDefinedField> ReadByEsrId(long esrId)
        {
            throw new NotImplementedException();
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
        /// The <see cref="UserDefinedField"/>.
        /// </returns>
        public List<UserDefinedField> Update(List<UserDefinedField> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
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
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public UserDefinedField Delete(UserDefinedField entity)
        {
            throw new NotImplementedException();
        }
    }
}