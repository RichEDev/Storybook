namespace EsrGo2FromNhs.Crud
{
    using System;
    using System.Collections.Generic;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The field crud.
    /// </summary>
    public class FieldCrud : EntityBase, IDataAccess<Field>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FieldCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public FieldCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see cref="Field"/>.
        /// </returns>
        public List<Field> Create(List<Field> entities)
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
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Read(int entityId)
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
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Read(long entityId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="Field"/>.
        /// </returns>
        public List<Field> ReadAll()
        {
            return this.EsrApiHandler.Execute<Field>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see cref="Field"/>.
        /// </returns>
        public List<Field> ReadByEsrId(long esrId)
        {
            throw new NotImplementedException();
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
        /// The <see cref="Field"/>.
        /// </returns>
        public List<Field> Update(List<Field> entity)
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
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Delete(int entityId)
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
        /// The <see cref="Field"/>.
        /// </returns>
        public Field Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public Field Delete(Field entity)
        {
            throw new NotImplementedException();
        }
    }
}