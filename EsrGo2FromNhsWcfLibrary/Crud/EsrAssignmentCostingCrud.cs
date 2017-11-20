namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;
    using System.Globalization;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;

    /// <summary>
    /// The ESR assignment costing crud.
    /// </summary>
    public class EsrAssignmentCostingCrud : EntityBase, IDataAccess<EsrAssignmentCostings>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentCostingCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrAssignmentCostingCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        public List<EsrAssignmentCostings> Create(List<EsrAssignmentCostings> entities)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignmentCostings Read(int entityId)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignmentCostings Read(long entityId)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> ReadAll()
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> ReadByEsrId(long esrId)
        {
            return this.EsrApiHandler.Execute<EsrAssignmentCostings>(DataAccessMethod.ReadByEsrId, esrId.ToString());
        }

        public List<EsrAssignmentCostings> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
        }

        public List<EsrAssignmentCostings> Update(List<EsrAssignmentCostings> entities)
        {
            throw new System.NotImplementedException();
        }

        public EsrAssignmentCostings Delete(long entityId)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentCostings"/>.
        /// </returns>
        public EsrAssignmentCostings Delete(EsrAssignmentCostings entity)
        {
            return this.EsrApiHandler.Execute<EsrAssignmentCostings>(entity.KeyValue.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
        }
    }
}