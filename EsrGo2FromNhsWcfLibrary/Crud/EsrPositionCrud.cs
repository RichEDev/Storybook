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
    /// The ESR position crud.
    /// </summary>
    public class EsrPositionCrud : EntityBase, IDataAccess<EsrPosition>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPositionCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrPositionCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="positions">
        /// The positions.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> Create(List<EsrPosition> positions)
        {
            positions = this.CreateUpdateEsrPositions(positions);
            return this.EsrApiHandler.Execute(DataAccessMethod.Create, "", positions);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Read(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrPosition>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Read(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrPosition>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
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
        public List<EsrPosition> ReadAll()
        {
            return this.EsrApiHandler.Execute<EsrPosition>(DataAccessMethod.ReadAll);
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="personId">
        /// The person id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> ReadByEsrId(long personId)
        {
            throw new NotImplementedException();
        }

        public List<EsrPosition> ReadSpecial(string reference)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="positions">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPosition> Update(List<EsrPosition> positions)
        {
            positions = this.CreateUpdateEsrPositions(positions);
            return this.EsrApiHandler.Execute(DataAccessMethod.Update, "", positions);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrPosition>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPosition Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrPosition>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }

        public EsrPosition Delete(EsrPosition entity)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The create update ESR positions.
        /// </summary>
        /// <param name="positions">
        /// The entities.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPositionCrud"/>.
        /// </returns>
        private List<EsrPosition> CreateUpdateEsrPositions(List<EsrPosition> positions)
        {
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);
            ////TODO: Mapping for country?
            var esrPositions = new List<EsrPosition>();

            if (mapping != null)
            {
                foreach (EsrPosition position in positions)
                {
                    var esrPosition = position;
                    var baseAttribute = position.ClassAttribute.ElementType;

                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        if (templateMapping.importElementType != baseAttribute)
                        {
                            continue;
                        }

                        foreach (FieldInfo field in esrPosition.ClassFields())
                        {
                            string fieldValue;
                            var attribute = field.GetCustomAttribute<DataClassAttribute>();

                            if ((attribute == null || attribute.FieldId == null) || (attribute.FieldId != templateMapping.fieldID.ToString().ToUpper()) || (templateMapping.dataType != 10 && templateMapping.dataType != 12))
                            {
                                continue;
                            }

                            esrPosition = (EsrPosition)this.LookupField(templateMapping, position, esrPosition, out fieldValue, field, Mapper<EsrPosition>.EsrToDataClass(positions));
                        }
                    }

                    esrPositions.Add(esrPosition);
                }
            }

            return esrPositions;
        }
    }
}