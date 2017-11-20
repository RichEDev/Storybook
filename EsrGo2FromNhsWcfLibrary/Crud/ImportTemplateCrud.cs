namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The import template crud.
    /// </summary>
    public class TemplateCrud : EntityBase, IDataAccess<Template>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TemplateCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public TemplateCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see cref="Template"/>.
        /// </returns>
        public List<Template> Create(List<Template> entities)
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
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Read(int entityId)
        {
            return this.EsrApiHandler.Execute<Template>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Read(long entityId)
        {
            return this.EsrApiHandler.Execute<Template>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="Template"/>.
        /// </returns>
        public List<Template> ReadAll()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see cref="Template"/>.
        /// </returns>
        public List<Template> ReadByEsrId(long esrId)
        {
            throw new NotImplementedException();
        }

        public List<Template> ReadSpecial(string reference)
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
        /// The <see cref="Template"/>.
        /// </returns>
        public List<Template> Update(List<Template> entity)
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
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Delete(int entityId)
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
        /// The <see cref="Template"/>.
        /// </returns>
        public Template Delete(long entityId)
        {
            throw new NotImplementedException();
        }

        public Template Delete(Template entity)
        {
            throw new NotImplementedException();
        }
    }
}