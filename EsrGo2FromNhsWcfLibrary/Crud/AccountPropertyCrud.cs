namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System;
    using System.Collections.Generic;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The account property CRUD.
    /// </summary>
    public class AccountPropertyCrud : EntityBase, IDataAccess<AccountProperty>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AccountPropertyCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public AccountPropertyCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see />.
        /// </returns>
        public List<AccountProperty> Create(List<AccountProperty> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entityId">
        /// The entityId.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        public AccountProperty Read(int entityId)
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
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public AccountProperty Read(long entityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see />.
        /// </returns>
        public List<AccountProperty> ReadAll()
        {
            return this.EsrApiHandler.Execute<AccountProperty>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="esrId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        public List<AccountProperty> ReadByEsrId(long esrId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The read special.  Not implimented
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        public List<AccountProperty> ReadSpecial(string reference)
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
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public List<AccountProperty> Update(List<AccountProperty> entities)
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
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public AccountProperty Delete(int entityId)
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
        /// The <see cref="AccountProperty"/>.
        /// </returns>
        public AccountProperty Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public AccountProperty Delete(AccountProperty entity)
        {
            throw new NotImplementedException();
        }
    }
}