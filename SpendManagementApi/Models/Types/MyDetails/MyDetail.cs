namespace SpendManagementApi.Models.Types.MyDetails
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using SpendManagementApi.Common;
    using SpendManagementApi.Common.Enums;
    using SpendManagementApi.Interfaces;
    using SpendManagementApi.Models.Requests;
    using SpendManagementApi.Models.Types.Employees;
    using SpendManagementApi.Repositories;

    using SpendManagementLibrary;

    using Spend_Management;

    using BankAccount = SpendManagementApi.Models.Types.BankAccount;

    /// <summary>
    /// Represents an Employee details within the system. The employee is the base of Spend Management.
    /// </summary>
    public class MyDetail : BaseExternalType
    {
        /// <summary>
        /// Gets the unique Id of this Employee.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the title of the employee. Mr, Ms etc.
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// Gets the user name of this Employee.
        /// </summary>
        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the forename of the employee.
        /// </summary>
        [Required]
        public string Forename { get; set; }

        /// <summary>
        /// Gets or sets the surname of the employee.
        /// </summary>
        [Required]
        public string Surname { get; set; }
        
        /// <summary>
        /// Gets the Bank details of the employee.
        /// </summary>
        public List<BankAccount> BankAccounts { get; set; }

        /// <summary>
        /// Gets or sets the payroll number for this employee.
        /// </summary>
        public string PayrollNumber { get; set; }

        /// <summary>
        /// Gets or sets the position title for this employee.
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// Gets or sets the mileage total.
        /// </summary>
        public decimal CurrentMileage { get; set; }

        /// <summary>
        /// Gets or sets the personal miles.
        /// </summary>
        public int PersonalMiles { get; set; }

        /// <summary>
        /// Gets or sets the SignOff Group
        /// </summary>
        public SignOffGroup SignOffGroup { get; set; }
        
        /// <summary>
        /// Gets or sets the telephone extension number for this employee.
        /// </summary>
        public string TelephoneExtensionNumber { get; set; }

        /// <summary>
        /// Gets or sets the telephone number for this employee.
        /// </summary>
        public string TelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the mobilr number for this employee.
        /// </summary>
        public string MobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the pager number for this employee.
        /// </summary>
        public string PagerNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address for this employee.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the personal email address for this employee.
        /// </summary>
        public string PersonalEmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the creditor for this employee.
        /// </summary>
        public string Creditor { get; set; }

        /// <summary>
        /// Gets the Employee Vehicle Detail
        /// </summary>
        public List<VehicleDetail> VehicleDetails { get; set; }

        /// <summary>
        /// Gets the Cost Centre Breakdown
        /// </summary>
        public List<CostCentreBreakdown> CostCentreBreakdowns { get; set; }

        /// <summary>
        /// Gets the Home details of the employee.
        /// </summary>
        public List<HomeAddress> HomeAddresses { get; set; }

        /// <summary>
        /// Gets the Work details of the employee.
        /// </summary>
        public List<WorkAddress> WorkAddresses { get; set; }

        /// <summary>
        /// Gets or sets the esr assignments.
        /// </summary>
        public List<ESRAssignment> ESRAssignments { get; set; }
    }

    /// <summary>
    /// The mydetail conversion.
    /// </summary>
    public static class MyDetailConversion
    {
        internal static TResult CastEmploye<TResult>(this SpendManagementLibrary.Employees.Employee employee, ICurrentUser user, IActionContext actionContext)
     where TResult : MyDetail, new()
        {
            if (employee == null)
            {
                return null;
            }
            var employees = actionContext.Employees;
            var esrAssignments = actionContext.EsrAssignments;

           List<SpendManagementLibrary.Account.BankAccount> employeeBankAccounts = actionContext.BankAccounts.GetAccountAsListByEmployeeId(user.EmployeeID);

            var currencies = new cCurrencies(user.AccountID, user.CurrentSubAccountId);
            var globalCurrencies = new cGlobalCurrencies();
            var bankAccounts = new List<BankAccount>();

            foreach (var account in employeeBankAccounts)
            {
                var bankAccount = new BankAccount().From(account, actionContext);             
                var globalCurrencyId = currencies.getCurrencyById(bankAccount.CurrencyId).globalcurrencyid;      
                bankAccount.CurrencyName = globalCurrencies.getGlobalCurrencyById(globalCurrencyId).label;
        
                bankAccounts.Add(bankAccount);
            }

            var employeeVehicles = GetEmployeeVehicles(user);

            var result = new TResult
            {
                Id = employee.EmployeeID,
                Title = employee.Title,
                UserName = employee.Username,
                Forename = employee.Forename,
                Surname = employee.Surname,
                TelephoneExtensionNumber = employee.TelephoneExtensionNumber,
                MobileNumber = employee.MobileTelephoneNumber,
                TelephoneNumber = employee.TelephoneNumber,
                PagerNumber = employee.PagerNumber,
                EmailAddress = employee.EmailAddress,
                PersonalEmailAddress = employee.HomeEmailAddress,
                PayrollNumber = employee.PayrollNumber,
                Creditor = employee.Creditor,
                Position = employee.Position,
                CurrentMileage = employees.getMileageTotal(user.EmployeeID, DateTime.Today),
                PersonalMiles = employees.getPersonalMiles(user.EmployeeID),
                SignOffGroup = new SignOffGroupsRepository(user, actionContext).Get(employee.SignOffGroupID),
                BankAccounts = bankAccounts,
                CostCentreBreakdowns = employees.GetEmployeeCostCodeFromDatabase(user.EmployeeID, user.AccountID).Select(cCostCentreBreakdown => new CostCentreBreakdown().From(cCostCentreBreakdown, actionContext)).ToList(),
                ESRAssignments = esrAssignments.getAssignmentsAssociated().Select(cEsrAssignment => new ESRAssignment().From(cEsrAssignment.Value, actionContext)).ToList(),
                HomeAddresses = employees.GetEmployeeHomeAddressFromDatabase(user.EmployeeID, user.AccountID).Select(empHomeaddress => new HomeAddress().From(empHomeaddress, actionContext)).ToList(),
                WorkAddresses = employees.GetEmployeeWorkAddressFromDatabase(user.EmployeeID, user.AccountID).Select(empHomeaddress => new WorkAddress().From(empHomeaddress, actionContext)).ToList(),
                VehicleDetails = employeeVehicles
            };
            return result;
        }

        /// <summary>
        /// Gets the employee's vehicles
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <returns>
        /// The a list of <see cref="VehicleDetail">VehicleDetail</see>
        /// </returns>
        private static List<VehicleDetail> GetEmployeeVehicles(ICurrentUser user)
        {
            var employeeCars = new cEmployeeCars(user.AccountID, user.EmployeeID);

            var poolCars = new cPoolCars(user.AccountID);
            var employeeVehicles = new List<VehicleDetail>();

            foreach (var car in employeeCars.Cars)
            {
                //check car is not a pool car
                if (poolCars.GetCarByID(car.carid) == null)            
                {
                    var vehicleDetail = new VehicleDetail();
                    vehicleDetail.Approved = car.active;
                    vehicleDetail.DefaultUnit = car.defaultuom.ToString();
                    vehicleDetail.EngineSize = car.EngineSize;
                    vehicleDetail.RegistrationNumber = car.registration;

                    SpendManagementApi.Common.Enums.VehicleEngineType vehicleEngineType = (SpendManagementApi.Common.Enums.VehicleEngineType)car.VehicleEngineTypeId;
                    vehicleDetail.VehicleEngineType = vehicleEngineType.ToString();

                    vehicleDetail.VehicleMake = car.make;
                    vehicleDetail.VehicleModel = car.model;
                    vehicleDetail.VehicleStatus = car.active;

                    var vehicleType = (VehicleType)car.VehicleTypeID;
                    vehicleDetail.VehicleTypeDescription = vehicleType.ToString();

                    employeeVehicles.Add(vehicleDetail);
                }
            }

            return employeeVehicles;
        }
    }
}