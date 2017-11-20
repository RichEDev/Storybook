using SpendManagementApi.Interfaces;

using System.Collections.Generic;
using System.Linq;

namespace SpendManagementApi.Repositories
{
    using System;

    using SpendManagementApi.Models.Types;

    using SpendManagementLibrary;

    using Spend_Management;

    /// <summary>
    /// ESRRepository manages data access for ESRAssignment.
    /// </summary>
    internal class ESRRepository : BaseRepository<ESRAssignments>, ISupportsActionContext
    {
        private cESRAssignments _data;

        /// <summary>
        /// Creates a new ESRRepository with the passed in user.
        /// </summary>
        /// <param name="user">The current user.</param>
        /// <param name="actionContext">An implementation of ISupportsActionContext</param>
        public ESRRepository(ICurrentUser user, IActionContext actionContext)
            : base(user, actionContext, x => (int)x.Assignmentid, x => x.AssignmentNumber)
        {
            _data = ActionContext.EsrAssignments;
        }

        /// <summary>
        /// Gets all the cESRAssignment within the system.
        /// </summary>
        /// <returns></returns>
        public override IList<ESRAssignments> GetAll()
        {
            return _data.GetCacheList().Select(b => new ESRAssignments().From(b.Value, ActionContext)).ToList();
        }

        /// <summary>
        /// Gets a single ESRAssignment by it's assignmentId.
        /// </summary>
        /// <param name="id">The id of the ESRAssignment to get.</param>
        /// <returns>An <see cref="ESRAssignments">ESRAssignments</see></returns>
        public override ESRAssignments Get(int id)
        {
            var item = _data.getAssignmentById(id);
            return item == null ? null : new ESRAssignments().From(item, this.ActionContext);
        }

        /// <summary>
        /// Saves an <see cref="ESRAssignments">ESRAssignments</see>
        /// </summary>
        /// <param name="item">
        /// The <see cref="ESRAssignments">ESRAssignments</see> to save
        /// </param>
        /// <returns>
        /// The saved <see cref="ESRAssignments">ESRAssignments</see>
        /// </returns>
        public override ESRAssignments Add(ESRAssignments item)
        {
            int assignmentId = Save(item);

            if (this.User.EmployeeID != item.EmployeeId)
            {
                // create a new instance of cESRAssignment, so the correct assignment gets returned for the employee specified in EsrAssignments parameter.          
                _data = new cESRAssignments(User.AccountID, item.EmployeeId);
                return this.Get(assignmentId);
            }
            else
            {
                // re-instialize the repository using the current user so the assignment is re-cached and returned correctly.
                var esrRepository = new ESRRepository(this.User, this.ActionContext);
                return esrRepository.Get(assignmentId);
            }
        }

        /// <summary>
        /// Gets a single ESRAssignment by it's assignmentNumber.
        /// </summary>
        /// <param name="assignmentNumber">The assignment number of the ESRAssignment to get.</param>
        /// <returns>An <see cref="ESRAssignments">ESRAssignments</see></returns>
        public ESRAssignments GetAssignmentByAssignmentNumber(string assignmentNumber)
        {
            var item = _data.getAssignmentByAssignmentNumber(assignmentNumber);
            return item == null ? null : new ESRAssignments().From(item, this.ActionContext);
        }

        /// <summary>
        /// Gets a single ESRAssignment by it's assignment id.
        /// </summary>
        /// <param name="assignmentId">The assignment id of the ESRAssignment to get.</param>
        /// <returns>An <see cref="ESRAssignments">ESRAssignments</see></returns>
        public ESRAssignments GetAssignmentByAssignmentId(int assignmentId)
        {
            var item = _data.GetAssignmentByAssignmentId(assignmentId);
            return item == null ? null : new ESRAssignments().From(item, this.ActionContext);
        }

        /// <summary>
        /// Gets a list of <see cref="ESRAssignmentBasic">ESRAssignmentBasic</see> for the current user and expense date
        /// </summary>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <returns>
        /// Gets a list of <see cref="ESRAssignmentBasic">ESRAssignmentBasic</see>
        /// </returns>
        public List<ESRAssignmentBasic> GetActiveAssignmentsForEmployee()
        {
            var esrAssignments = new List<ESRAssignmentBasic>();

            if (User.Account.IsNHSCustomer)
            {
                var subAccounts = new cAccountSubAccounts(User.AccountID);
                cAccountProperties subAccountProperties =
                    subAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;

                cESRAssignments assignments = new cESRAssignments(User.AccountID, User.EmployeeID);

                foreach (KeyValuePair<int, cESRAssignment> assignment in assignments.getAssignmentsAssociated())
                {
                    var esrAssignmentBasic = new ESRAssignmentBasic
                    {
                        AssignmentId = assignment.Value.sysinternalassignmentid,
                        AssignmentNumber = assignment.Value.assignmentnumber,
                        AssignmentText = assignments.GenerateAssignmentText(assignment.Value, subAccountProperties),
                        EarliestAssignmentStartDate = assignment.Value.earliestassignmentstartdate,
                        EffectiveStartDate = assignment.Value.EffectiveStartDate,
                        EffectiveEndDate = assignment.Value.EffectiveEndDate,
                        Active = assignment.Value.active
                    };

                    esrAssignments.Add(esrAssignmentBasic);
                }
            }

            return esrAssignments;
        }

        /// <summary>
        /// Gets active assignments for current user by date.
        /// </summary>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <returns>
        /// A list of <see cref="ESRAssignmentBasic"/>
        /// </returns>
        public List<ESRAssignmentBasic> GetActiveAssignmentsForCurrentUserByDate(DateTime expenseDate)
        {
            return this.GetActiveAssignmentsforExpenseDateAndEmployee(expenseDate, this.User.EmployeeID);
        }

        /// <summary>
        /// Gets active assignments for employee id by date.
        /// </summary>
        /// <param name="expenseDate">
        /// The expense date.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="expenseId">
        /// The expense Id.
        /// </param>
        /// <returns>
        /// A list of <see cref="ESRAssignmentBasic"/>
        /// </returns>
        public List<ESRAssignmentBasic> GetActiveAssignmentsForEmployeeByDate(DateTime expenseDate, int employeeId, int expenseId)
        {
            var expenseItemRepository = new ExpenseItemRepository(this.User, this.ActionContext);
            expenseItemRepository.ClaimantDataPermissionCheck(expenseId, employeeId);

            return this.GetActiveAssignmentsforExpenseDateAndEmployee(expenseDate, employeeId);
        }

        /// <summary>
        /// Deletes an ESR Assginment by its Id
        /// </summary>
        /// <param name="id">
        /// The assignment Id
        /// </param>
        /// <returns>
        /// The <see cref="ESRAssignments">ESRAssignments</see>
        /// </returns>
        public override ESRAssignments Delete(int id)
        {
            var item = base.Delete(id);
            _data.deleteESRAssignment(id);
            return item;
        }

        /// <summary>
        /// Saves an Esr Assignment
        /// </summary>
        /// <param name="assignments">
        /// The assignment to save.
        /// </param>
        /// <returns>
        /// The new assignment id
        /// </returns>
        private int Save(ESRAssignments assignments)
        {
            assignments.CreatedBy = User.EmployeeID;
            assignments.CreatedOn = DateTime.UtcNow;

            cESRAssignment esrAssigment = assignments.To(ActionContext);
         
            int assignmentId = 0;

            if (this.User.EmployeeID != assignments.EmployeeId)
            {
                // create an instance of cESRAssignment, so the correct assignment gets saved against the employee specified in EsrAssignments.    
                var esrAssignments = new cESRAssignments(User.AccountID, assignments.EmployeeId);
                assignmentId = esrAssignments.saveESRAssignment(esrAssigment);
            }
            else
            {
                 assignmentId = ActionContext.EsrAssignments.saveESRAssignment(esrAssigment);
            }
    
            return assignmentId;
        }



        /// <summary>
        /// Gets a list of <see cref="ESRAssignmentBasic">ESRAssignmentBasic</see> for the current user and expense date
        /// </summary>
        /// <param name="expenseDate">
        /// The expense Date.
        /// </param>
        /// <param name="employeeId">
        /// The employee Id the assignments belong to.
        /// </param>
        /// <returns>
        /// Gets a list of <see cref="ESRAssignmentBasic">ESRAssignmentBasic</see>
        /// </returns>
        private List<ESRAssignmentBasic> GetActiveAssignmentsforExpenseDateAndEmployee(DateTime expenseDate, int employeeId)
        {
            var esrAssignments = new List<ESRAssignmentBasic>();

            if (User.Account.IsNHSCustomer)
            {
                var subAccounts = new cAccountSubAccounts(User.AccountID);
                cAccountProperties subAccountProperties =
                    subAccounts.getSubAccountById(User.CurrentSubAccountId).SubAccountProperties;

                cESRAssignments assignments = new cESRAssignments(User.AccountID, employeeId);

                foreach (KeyValuePair<int, cESRAssignment> assignment in assignments.getAssignmentsAssociated())
                {
                    if (!esrAssignments.Any(x => x.AssignmentNumber == assignment.Value.assignmentnumber))
                    {
                        if (assignments.IsAssignmentIsValidForExpenseDate(expenseDate, assignment.Value))
                        {
                            var esrAssignmentBasic = new ESRAssignmentBasic
                            {
                                AssignmentId = assignment.Value.sysinternalassignmentid,
                                AssignmentNumber = assignment.Value.assignmentnumber,
                                AssignmentText = assignments.GenerateAssignmentText(assignment.Value, subAccountProperties)
                            };
                            esrAssignments.Add(esrAssignmentBasic);
                        }
                    }
                }
            }

            return esrAssignments;
        }
    }
}