namespace EsrGo2FromNhs.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.Interfaces;
    using EsrGo2FromNhs.Spend_Management;

    /// <summary>
    /// The import template crud.
    /// </summary>
    public class TemplateMappingCrud : EntityBase, IDataAccess<TemplateMapping>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TemplateMappingCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public TemplateMappingCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public List<TemplateMapping> Create(List<TemplateMapping> entities)
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
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Read(int entityId)
        {
            return this.EsrApiHandler.Execute<TemplateMapping>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Read(long entityId)
        {
            return this.EsrApiHandler.Execute<TemplateMapping>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public List<TemplateMapping> ReadAll()
        {
            return this.EsrApiHandler.Execute<TemplateMapping>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public List<TemplateMapping> ReadByEsrId(long esrId)
        {
            throw new NotImplementedException();
        }

        public List<TemplateMapping> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entities">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public List<TemplateMapping> Update(List<TemplateMapping> entities)
        {
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<TemplateMapping>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        public TemplateMapping Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<TemplateMapping>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        public TemplateMapping Delete(TemplateMapping entity)
        {
            throw new NotImplementedException();
        }
    }
}