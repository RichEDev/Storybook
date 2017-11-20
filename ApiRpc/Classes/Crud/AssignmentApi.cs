namespace ApiRpc.Classes.Crud
{
    using System.Linq;
    using System.Collections.Generic;
    using ApiLibrary.DataObjects.Base;
    using global::ApiCrud;
    using ApiLibrary.DataObjects.ESR;
    using Interfaces;

    public class AssignmentApi : IDataAccess<EsrAssignment>
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string metaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        private readonly int accountId;

        /// <summary>
        /// The crud API.
        /// </summary>
        private ApiService crudApi;

        /// <summary>
        /// Initialises a new instance of the <see cref="AssignmentApi"/> class. 
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        public AssignmentApi(string metaBase, int accountId)
        {
            this.metaBase = metaBase;
            this.accountId = accountId;
            this.crudApi = new ApiService();
        }
        public List<EsrAssignment> Create(List<EsrAssignment> entities)
        {
            var assignments = this.crudApi.CreateEsrAssignments(this.metaBase, this.accountId.ToString(), entities);
            new AssignmentLocationApi(this.metaBase, this.accountId).Create(assignments.Where(ass => ass.EsrLocationId.HasValue).Select(ass =>
                new EsrAssignmentLocation
                {
                    Action = Action.Create,
                    EsrAssignId = ass.esrAssignID,
                    EsrLocationId = ass.EsrLocationId.Value,
                    StartDate = System.DateTime.Now
                }).ToList());
            return assignments;
        }

        public EsrAssignment Read(int entityId)
        {
            return this.crudApi.ReadEsrAssignment(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrAssignment Read(long entityId)
        {
            return this.crudApi.ReadEsrAssignment(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public List<EsrAssignment> ReadAll()
        {
            return this.crudApi.ReadAllEsrAssignment(this.metaBase, this.accountId.ToString());
        }

        public List<EsrAssignment> ReadByEsrId(long personId)
        {
            return this.crudApi.ReadEsrAssignmentByPerson(this.metaBase, this.accountId.ToString(), personId.ToString());
        }

        public List<EsrAssignment> ReadSpecial(string reference)
        {
            return this.crudApi.ReadEsrAssignmentByAssignment(this.metaBase, this.accountId.ToString(), reference);
        }

        public List<EsrAssignment> Update(List<EsrAssignment> entities)
        {
            return this.crudApi.UpdateEsrAssignments(this.metaBase, this.accountId.ToString(), entities);
        }

        public EsrAssignment Delete(long entityId)
        {
            return this.crudApi.DeleteEsrAssignment(this.metaBase, this.accountId.ToString(), entityId.ToString());
        }

        public EsrAssignment Delete(EsrAssignment entity)
        {
            var result = new EsrAssignment { ActionResult = new ApiResult { Result = ApiActionResult.NoAction } };
            if (entity.AssignmentID != 0)
            {
                var current = this.ReadSpecial(entity.AssignmentID.ToString());
                foreach (EsrAssignment assignment in current)
                {
                    var thisResult =
                        this.crudApi.DeleteEsrAssignment(
                            this.metaBase, this.accountId.ToString(), assignment.esrAssignID.ToString()).ActionResult;
                    if (thisResult.Result != ApiActionResult.Success)
                    {
                        result.ActionResult = thisResult;
                    }
                }
            }

            return result;
        }
    }
}