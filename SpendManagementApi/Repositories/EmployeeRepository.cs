using SpendManagementApi.Models.Responses;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Common;
    using Interfaces;
    using Models.Common;
    using Utilities;
    using SpendManagementLibrary;
    using Spend_Management;
    using Models.Types.Employees;
    using System.IO;

    internal class EmployeeRepository : ArchivingBaseRepository<Employee>, ISupportsActionContext
    {
        #region Properties
        
        private cEmployees _employees;
        private cMileagecats _mileagecats;
        private cEmployeeCorporateCards _cards;
        private List<string> _errorMessages;
        private int _transactionCount = 1;
        
        internal List<string> WarningMessages { get { return _errorMessages; } }
        
        #endregion Properties


        #region Constructor

        public EmployeeRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, emp => emp.Id, emp => emp.UserName)
        {
            _employees = this.ActionContext.Employees;
            _mileagecats = this.ActionContext.MileageCategories;
            _cards = this.ActionContext.EmployeeCorporateCards;
        }

        #endregion Constructor


        #region Public Direct Employee Methods

        public override IList<Employee> GetAll()
        {
            return
                _employees.GetAllEmployeeRequiredInfoAsList().Select(d =>
                    new Employee
                    {
                        Id = d.Id,
                        EmployeeId = d.Id,
                        UserName = d.Username,
                        Title = d.Title,
                        Forename = d.Forename,
                        Surname = d.Surname,
                        Archived = d.Archived
                    }).ToList();
        }

        public override Employee Get(int employeeId)
        {
            var cEmp = _employees.GetEmployeeById(employeeId);
            return cEmp.Cast<Employee>(User, ActionContext);
        }

        /// <summary>
        /// Returns a stripped down version of <see cref="Employee">Employee</see> with just the data required for the passenger search.
        /// </summary>
        /// <param name="employeeId">The employeeId</param>
        /// <returns>A <see cref="Employee">Employee</see> of passenger data</returns>
        public Employee GetPassengerDetails(int employeeId)
        {
            var employee = this._employees.GetEmployeeById(employeeId);

            return new Employee
            {
                Id = employee.EmployeeID,
                Forename = employee.Forename,
                Surname = employee.Surname,
                UserName = employee.Username
            };
        }

        /// <summary>
        /// The get passenger search results for the supplied criteria
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// A list of <see cref="Employee">Employee</see>
        /// </returns>
        public List<Employee> GetPassengerSearchResults(string criteria)
        {
            var filters = new Dictionary<string, JSFieldFilter>
            {
                {
                    "0",
                    new JSFieldFilter
                    {
                        ConditionType = ConditionType.NotLike,
                        FieldID = new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"),
                        ValueOne = "admin%"
                    }
                }
            };

            List<TokenInputResult> results = ActionContext.Employees.SearchEmployees(filters, criteria, User);

            return (from employee in results where Convert.ToInt32(employee.id) > 0 select this.GetPassengerDetails(Convert.ToInt32(employee.id))).ToList();
        }

        public override Employee Add(Employee employee)
        {
            _transactionCount = 1;
            base.Add(employee);

            employee.IsVerified = true;
            employee.IsLocked = false;
            employee.LogonCount = 0;
            employee.LogonRetryCount = 0;
            employee.UserDefinedFields = Helper.NullIf(employee.UserDefinedFields);

            employee.Validate(ActionContext);

            int employeeId;
            var costCentres = new List<cDepCostItem>();
            
            var employeeGeneralDetails = employee.EmployeeDetails;
            if (employeeGeneralDetails != null)
            {
                if (employeeGeneralDetails.WorkDetails != null && employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns != null)
                {
                    costCentres = employee.EmployeeDetails.WorkDetails.CostCentreBreakdowns.Cast<List<cDepCostItem>>();
                }

                
                employeeGeneralDetails.EmailNotifications = Helper.NullIf(employeeGeneralDetails.EmailNotifications);

                var userDefinedFieldsSortedList = new SortedList<int, object>();

                foreach (var userDefinedFieldValue in employee.UserDefinedFields)
                {
                    userDefinedFieldsSortedList.Add(userDefinedFieldValue.Id, userDefinedFieldValue.Value);
                }

                employeeId = _employees.SaveEmployee(employee.Cast<SpendManagementLibrary.Employees.Employee>(User, this.ActionContext),
                    costCentres.ToArray(),
                    employeeGeneralDetails.EmailNotifications,
                    userDefinedFieldsSortedList);
            }
            else
            {
                employeeId = _employees.SaveEmployee(employee.Cast<SpendManagementLibrary.Employees.Employee>(User, this.ActionContext),
                    costCentres.ToArray(), new List<int>(), null);
            }

             


            if (employeeId < 0)
            {
                throw new InvalidDataException("Employee data could not be saved successfully");
            }

            employee.Id = employeeId;

            if (employee.SendPasswordKeyAndWelcomeEmail && employee.IsActive)
            {
                var subaccs = new cAccountSubAccounts(User.AccountID);
                cAccountProperties properties = subaccs.getSubAccountById(subaccs.getFirstSubAccount().SubAccountID).SubAccountProperties;
                int senderId = properties.MainAdministrator > 0 ? properties.MainAdministrator : User.EmployeeID;                
             
                _employees.SendPasswordKey(employeeId, cEmployees.PasswordKeyType.NewEmployee, null, User.CurrentActiveModule);
                _employees.SendWelcomeEmail(senderId, employeeId, this.User, false);
            }


            _errorMessages = new List<string>();

            return this.Get(employeeId);
        }

        public override Employee Delete(int employeeId)
        {
            Employee employee = Get(employeeId);

            if (employee == null)
            {
                throw new ApiException(ApiResources.ApiErrorWrongEmployeeId,
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (!employee.Archived)
            {
                throw new ApiException("Invalid Data", "The employee needs to be archived before deletion");
            }

            int data = employee.Cast<SpendManagementLibrary.Employees.Employee>(User, this.ActionContext).Delete(User);

            switch (data)
            {
                case 1:
                    throw new ApiException("Failure", "The specified employee is assigned to one or more Signoff Groups and cannot be deleted.");
                case 2:
                    throw new ApiException("Failure", "This employee cannot be deleted as they have one or more advances allocated to them.");
                case 3:
                    throw new ApiException("Failure", "This employee is currently set as a budget holder.");
                case 4:
                    throw new ApiException("Failure", "You must archive an employee before it can be deleted.");
                case 5:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are the owner of one or more contracts.");
                case 6:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are on one or more contract audiences as an individual.");
                case 7:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are on one or more attachment audiences as an individual.");
                case 8:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are on one or more contract notification lists.");
                case 9:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are the leader of one or more teams.");
                case 10:
                    throw new ApiException("Failure", "This employee cannot be deleted as it would leave one or more empty teams.");
                case 11:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are associated to one or more contracts history.");
                case 12:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are associated to one or more report folders.");
                case 13:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are associated to one or more audiences.");
                case 14:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are the owner of one or more cost codes.");
                case 15:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are assigned to one or more approval matrices.");
                case -10:
                    throw new ApiException("Failure", "This employee cannot be deleted as they are referenced in either a GreenLight or by a user defined field.");
            }

            employee = this.Get(employeeId);

            if (employee != null)
            {
                throw new ApiException("Failure", "Item could not be deleted");
            }

            return employee;
        }

        /// <summary>
        /// Updates the db with part or all of the employee details provided
        /// </summary>
        /// <param name="employee"></param>
        /// <returns></returns>
        public override Employee Update(Employee employee)
        {
            base.Update(employee);

            Employee existingData = this.Get(employee.Id);

            if (existingData == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            employee.Validate(this.ActionContext);

            Employee mergedData = Employee.Merge(employee, existingData);

            List<cDepCostItem> costCentres = new List<cDepCostItem>();
            if (mergedData.EmployeeDetails != null && mergedData.EmployeeDetails.WorkDetails != null
                && mergedData.EmployeeDetails.WorkDetails.CostCentreBreakdowns != null)
            {
                costCentres = mergedData.EmployeeDetails.WorkDetails.CostCentreBreakdowns.Cast<List<cDepCostItem>>();
            }

            var userDefinedFieldsSortedList = new SortedList<int, object>();

            foreach (var userDefinedFieldValue in mergedData.UserDefinedFields)
            {
                userDefinedFieldsSortedList.Add(userDefinedFieldValue.Id, userDefinedFieldValue.Value);
            }

            int employeeId = _employees.SaveEmployee(mergedData.Cast<SpendManagementLibrary.Employees.Employee>(User, this.ActionContext), costCentres.ToArray(), mergedData.EmployeeDetails.EmailNotifications, userDefinedFieldsSortedList);

            if (employeeId < 0)
            {
                throw new InvalidDataException("Employee Data could not be saved successfully");
            }

            _errorMessages = new List<string>();

            return this.Get(employeeId);
        }

        public override Employee Archive(int id, bool archive)
        {
            var emp = base.Archive(id, archive);
            var dbEmp = _employees.GetEmployeeById(id);
            dbEmp.ChangeArchiveStatus(archive, User);
            return emp;
        }

        /// <summary>
        /// Activates a list of employees by their Id.
        /// </summary>
        /// <param name="ids">The list of employee ids.</param>
        /// <param name="sendWelcomeEmail">Whether to email the employees.</param>
        /// <param name="response">The response to populate with the results.</param>
        /// <returns></returns>
        public EmployeeActivateResponse ActivateEmployees(List<int> ids, bool sendWelcomeEmail, EmployeeActivateResponse response)
        {
            response.AlreadyActivatedEmployees = new List<int>();
            response.ActivatedEmployees = new List<int>();

            // check all Ids first
            var validEmployees = new List<SpendManagementLibrary.Employees.Employee>();

            if (ids.Any(empId => _employees.GetEmployeeById(empId) == null))
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            validEmployees = ids.Select(id => _employees.GetEmployeeById(id)).ToList();

            validEmployees.ForEach(emp =>
            {
                if (emp.Active)
                {
                    response.AlreadyActivatedEmployees.Add(emp.EmployeeID);
                }
                else
                {
                    this._employees.Activate(emp.EmployeeID);
                    if (sendWelcomeEmail)
                    {
                        var subaccs = new cAccountSubAccounts(User.AccountID);
                        cAccountProperties properties = subaccs.getSubAccountById(subaccs.getFirstSubAccount().SubAccountID).SubAccountProperties;
                        int senderId = properties.MainAdministrator > 0 ? properties.MainAdministrator : this.User.EmployeeID;
                        
                        this._employees.SendWelcomeEmail(senderId, emp.EmployeeID, this.User, true);
                        this._employees.SendPasswordKey(emp.EmployeeID, cEmployees.PasswordKeyType.NewEmployee, DateTime.UtcNow, User.CurrentActiveModule);
                    }
                    response.ActivatedEmployees.Add(emp.EmployeeID);
                }
            });



            return response;
        }

        /// <summary>
        /// Changes an Employees password
        /// </summary>
        /// <param name="id">The Id of the Employee to change</param>
        /// <param name="newPassword">The Employee's new password</param>
        /// <param name="response">A ChangePasswordResponse object</param>
        /// <returns>A ChangePasswordResponse object</returns>
        public ChangePasswordResponse ChangePassword(int id, string newPassword, ChangePasswordResponse response)
        {
            var dbEmp = _employees.GetEmployeeById(id);
            response.RequestResponse = dbEmp.ChangePassword(string.Empty, newPassword, false, 0, 0, this.User);
            return response;
        }

        public void UpdateEsrAssignments(List<EsrAssignment> esrAssignments, int employeeId)
        {
            if (esrAssignments == null || esrAssignments.Count == 0)
            {
                return;
            }

            esrAssignments.ForEach(
                esr =>
                {
                    cESRAssignments clsassignments = new cESRAssignments(User.AccountID, employeeId);
                    cESRAssignment assignment;

                    //Updating an existing assignment
                    cESRAssignment oldassignment = null;
                    if (esr.EsrAssignmentId > 0)
                    {
                        oldassignment = clsassignments.getAssignmentById(esr.EsrAssignmentId);
                    }

                    SetSupervisorAssigmentDetails(esr, oldassignment);

                    assignment = esr.Cast<cESRAssignment>(oldassignment);

                    clsassignments.saveESRAssignment(assignment);
                });
        }

        /// <summary>
        /// Determines the Primary currency for the employee
        /// </summary>
        /// <returns>The primary currency Id</returns>
        public int DeterminePrimaryCurrenyId()
        {
            SpendManagementLibrary.Employees.Employee employee = this.ActionContext.Employees.GetEmployeeById(User.EmployeeID);    
            cCurrency currency;
            if (employee.PrimaryCurrency != 0)
            {
                currency = this.ActionContext.Currencies.getCurrencyById(employee.PrimaryCurrency);
            }
            else
            {
                var subAccounts = new cAccountSubAccounts(User.AccountID);
                cAccountProperties properties = subAccounts.getSubAccountById(subAccounts.getFirstSubAccount().SubAccountID).SubAccountProperties;
                currency = this.ActionContext.Currencies.getCurrencyById((int)properties.BaseCurrency);
            }

            return currency.currencyid;
        }

        /// <summary>
        /// The determines primary currency symbol for the current employee.
        /// </summary>
        /// <param name="misc">
        /// An instance of <see cref="cMisc"/>
        /// </param>
        /// <param name="currenciesRepository">
        /// An instance of <see cref="CurrencyRepository"/>
        /// </param>
        /// <returns>
        /// The <see cref="string"/> of the currency symbol.
        /// </returns>
        public string DeterminePrimaryCurrencySymbol(cMisc misc, CurrencyRepository currenciesRepository)
        {
            var employee = this.ActionContext.Employees.GetEmployeeById(this.User.EmployeeID);
            cGlobalProperties properties = misc.GetGlobalProperties(this.User.AccountID);
            int primaryCurrencyId = employee.PrimaryCurrency == 0 ? properties.basecurrency : employee.PrimaryCurrency;
            string primaryCurrencySymbol = currenciesRepository.DetermineCurrencySymbol(primaryCurrencyId);

            return primaryCurrencySymbol;
        }

        #endregion Public Direct Employee Methods

        #region Internal Methods

        internal void ResetPassword(int employeeId)
        {
            Employee employee = this.Get(employeeId);

            if (employee == null)
            {
                throw new InvalidDataException(ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (employee.Archived)
            {
                throw new InvalidDataException(ApiResources.ApiErrorResetPasswordEmployeeArchived);
            }

            if (!employee.IsActive)
            {
                throw new InvalidDataException(ApiResources.ApiErrorResetPasswordEmployeeInactive);
            }

            _employees.SendPasswordKey(employeeId, cEmployees.PasswordKeyType.AdminRequest, null, User.CurrentActiveModule);
        }

        internal int GetTransactionCount()
        {
            return _transactionCount;
        }

        #endregion Internal Methods


        #region Private Methods

        private void SetSupervisorAssigmentDetails(EsrAssignment esr, cESRAssignment oldAssignment)
        {
            if (esr.SupervisorEmployeeId.HasValue)
            {
                SpendManagementLibrary.Employees.Employee supervisor =
                            this.ActionContext.Employees.GetEmployeeById(esr.SupervisorEmployeeId.Value);

                cESRAssignments supervisorAssignments = null;
                Dictionary<int, cESRAssignment> supervisorAssignmentsCollection = new Dictionary<int, cESRAssignment>();
                if (supervisor != null && supervisor.EmployeeID > 0)
                {
                    supervisorAssignments = new cESRAssignments(User.AccountID, supervisor.EmployeeID);
                    supervisorAssignmentsCollection = supervisorAssignments.getAssignmentsAssociated();
                }

                string supervisorAssignmentNumber = string.Empty;
                if (supervisor != null && supervisor.EmployeeID > 0)
                {
                    supervisorAssignmentNumber =
                        supervisorAssignmentsCollection.Values.Where(a => a.primaryassignment)
                        .Select(a => a.assignmentnumber).DefaultIfEmpty(string.Empty).FirstOrDefault();
                    esr.SupervisorAssignmentNumber = supervisorAssignmentNumber;

                    if (!string.IsNullOrEmpty(supervisorAssignmentNumber))
                    {
                        esr.SupervisorAssignmentNumber = supervisorAssignmentNumber;
                        esr.SupervisorEmployeeNumber = supervisor.EmployeeNumber;
                        esr.SupervisorFullName = supervisor.FullName;
                    }
                }

                if (oldAssignment != null && supervisorAssignmentNumber == oldAssignment.assignmentnumber)
                {
                    esr.SupervisorAssignmentNumber = esr.AssignmentNumber;
                }
            }
        }

        #endregion Private Methods

    }
}