namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;

    /// <summary>
    /// The ESR assignments crud helper class.
    /// </summary>
    public class EsrAssignmentsCrud : EntityBase, IDataAccess<EsrAssignment>
    {
        #region Public Methods

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrAssignmentsCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler"></param>
        /// <param name="logger"></param>
        public EsrAssignmentsCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
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
        public List<EsrAssignment> Create(List<EsrAssignment> entities)
        {
            var assignments = this.EsrApiHandler.Execute(DataAccessMethod.Create, "", entities);
            this.UpdateLocations(assignments);
            return assignments;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Read(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrAssignment>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Read(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrAssignment>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
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
        public List<EsrAssignment> ReadAll()
        {
            return this.EsrApiHandler.Execute<EsrAssignment>(DataAccessMethod.ReadAll);
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
        public List<EsrAssignment> ReadByEsrId(long personId)
        {
            return this.EsrApiHandler.Execute<EsrAssignment>(DataAccessMethod.ReadByEsrId, personId.ToString(CultureInfo.InvariantCulture), null, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
        }

        public List<EsrAssignment> ReadSpecial(string reference)
        {
            throw new System.NotImplementedException();
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
        public List<EsrAssignment> Update(List<EsrAssignment> entities)
        {
            var assignments = this.EsrApiHandler.Execute(DataAccessMethod.Update, "", entities);
            this.UpdateLocations(assignments);
            return assignments;
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Delete(int entityId)
        {
            return this.EsrApiHandler.Execute<EsrAssignment>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Delete(long entityId)
        {
            return this.EsrApiHandler.Execute<EsrAssignment>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        public EsrAssignment Delete(EsrAssignment entity)
        {
            var result = new EsrAssignment { ActionResult = new ApiResult { Result = ApiActionResult.NoAction } };
            if (entity.AssignmentID != 0)
            {
                var currentAssignment = this.EsrApiHandler.Execute<EsrAssignment>(DataAccessMethod.ReadSpecial, entity.AssignmentID.ToString(CultureInfo.InvariantCulture));
                foreach (EsrAssignment assignment in currentAssignment)
                {
                    var thisResult = this.EsrApiHandler.Execute<EsrAssignment>(assignment.esrAssignID.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.NewObjectWithFailureFlag).ActionResult;
                    if (thisResult.Result != ApiActionResult.Success)
                    {
                        result.ActionResult = thisResult;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Updates the ESR Location for each assignment passed
        /// </summary>
        /// <param name="assignments">The assignments to update</param>
        private void UpdateLocations(IEnumerable<EsrAssignment> assignments)
        {
            this.EsrApiHandler.Execute(DataAccessMethod.Create, "", assignments.Where(ass => ass.EsrLocationId.HasValue).Select(ass =>
                new EsrAssignmentLocation
                {
                    Action = Action.Create,
                    EsrAssignId = ass.esrAssignID,
                    EsrLocationId = ass.EsrLocationId.Value,
                    StartDate = ass.EffectiveStartDate
                }).ToList());
        }

        #endregion
    }
}