namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reflection;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    /// <summary>
    /// The ESR organisation crud.
    /// </summary>
    public class EsrOrganisationCrud : EntityBase, IDataAccess<EsrOrganisation>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrOrganisationCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrOrganisationCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> Create(List<EsrOrganisation> entities)
        {
            entities = this.CreateUpdateEsrOrganisations(entities);
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Read(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrOrganisation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Read(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrOrganisation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> ReadAll()
        {
            return this.EsrApiHandler.Execute<EsrOrganisation>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The esr id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> ReadByEsrId(long esrId)
        {
            throw new NotImplementedException(); ;
        }

        public List<EsrOrganisation> ReadSpecial(string reference)
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
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisation> Update(List<EsrOrganisation> entities)
        {
            entities = this.CreateUpdateEsrOrganisations(entities);
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrOrganisation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisation Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrOrganisation>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        public EsrOrganisation Delete(EsrOrganisation entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create update ESR organisations.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        private List<EsrOrganisation> CreateUpdateEsrOrganisations(List<EsrOrganisation> entities)
        {
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);

            if (mapping != null)
            {
                ////TODO: Mapping for country?

                var organisations = new List<EsrOrganisation>();
                FieldInfo[] fields = null;
                foreach (EsrOrganisation organisation in entities)
                {
                    var esrOrganisation = organisation;
                    var baseAttribute = organisation.ClassAttribute.ElementType;
                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        if (templateMapping.importElementType == baseAttribute)
                        {
                            if (fields == null)
                            {
                                fields = organisation.ClassFields();
                            }

                            foreach (FieldInfo field in fields)
                            {
                                var attribute = field.GetCustomAttribute<DataClassAttribute>();
                                if (attribute != null && attribute.FieldId != null)
                                {
                                    if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                    {
                                        {
                                            if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                            {
                                                string fieldValue;
                                                esrOrganisation =
                                                    (EsrOrganisation)this.LookupField(
                                                        templateMapping,
                                                        organisation,
                                                        esrOrganisation,
                                                        out fieldValue,
                                                        field,
                                                        Mapper<EsrOrganisation>.EsrToDataClass(entities));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    organisations.Add(esrOrganisation);
                }

                return organisations;
            }

            return new List<EsrOrganisation>();
        }
    }
}