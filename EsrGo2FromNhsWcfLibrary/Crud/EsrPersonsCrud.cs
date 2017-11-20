namespace EsrGo2FromNhsWcfLibrary.Crud
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Runtime.Serialization;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Enum;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Action = EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// The ESR persons helper class.  Contains methods to split ESR person records into ESR person, assignment etc.
    /// </summary>
    public class EsrPersonsCrud : EntityBase, IDataAccess<EsrPersonRecord>
    {
        #region Public Methods

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPersonsCrud"/> class.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrApiHandler">An interfaced version of the esr api for unit tests (optional)</param>
        /// <param name="logger"></param>
        public EsrPersonsCrud(string metabase, int accountId, IEsrApi esrApiHandler = null, Log logger = null)
            : base(metabase, accountId, esrApiHandler, logger)
        {
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entities">
        /// The ESR person records.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPersonRecord> Create(List<EsrPersonRecord> entities)
        {
            var methodStart = DateTime.Now;
            var result = new List<EsrPersonRecord>();
            var persons = new List<EsrPerson>();
            var allAssignments = new List<EsrAssignment>();
            var allAddresses = new List<Address>();
            var allEsrAddresses = new List<EsrAddress>();
            var allPhones = new List<EsrPhone>();
            var allVehicles = new List<EsrVehicle>();
            var allAssignmentCostings = new List<EsrAssignmentCostings>();

            foreach (EsrPersonRecord personRecord in entities)
            {
                if (personRecord.ActionResult == null)
                {
                    personRecord.ActionResult = new ApiResult();
                }

                //// Make a list of ESRPERSON to pass to create method
                //// has person got a valid employee record?
                //// if not create an employee otherwise update existing employee
                var singlePerson = DataClassBase.GetDataClassFromRecord(personRecord, typeof(EsrPerson)) as EsrPerson;
                if (singlePerson != null)
                {
                    persons.Add(singlePerson);
                }
            }

            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Data Class From Record Elapsed Time {0}", DateTime.Now - methodStart),
                "EsrPersonsCrud");

            persons = this.UpdateEsrPersonsMapping(persons);

            var subMethodStart = DateTime.Now;
            persons = this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, persons);

            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create Persons Time {0}", DateTime.Now - subMethodStart),
                "EsrPersonsCrud");


            var employees = new List<Employee>();
            for (int i = 0; i < persons.Count; i++)
            {
                var originalPerson = entities[i];
                var updatedPerson = DataClassBase.GetRecordFromDataClass(persons[i], typeof(EsrPersonRecord)) as EsrPersonRecord;
                if (updatedPerson != null && updatedPerson.ActionResult.Result == ApiActionResult.Success)
                {
                    var currentEmployees = this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadByEsrId, updatedPerson.ESRPersonId.ToString(CultureInfo.InvariantCulture), null, DataAccessMethodReturnDefault.Null);
                    updatedPerson.EsrAssignments = originalPerson.EsrAssignments;
                    if (this.ImportElementType(TemplateMapping.ImportElementType.Employee, this.TrustVpd))
                    {
                        var newEmployee = this.UpdateEmployeeMapping(updatedPerson, currentEmployees.Count == 0 ? null : currentEmployees[0]);
                        employees.Add(newEmployee);
                    }
                }
            }

            subMethodStart = DateTime.Now;
            employees = this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, employees);

            var employeeIdsRoles = new List<EmployeeRole>();
            var employeeIdAccessRoles = new List<EmployeeAccessRole>();
            int itemRoleId = int.Parse(this.GetAccountProperty("defaultItemRole"));
            int accessRoleId = int.Parse(this.GetAccountProperty("defaultRole"));

            foreach (Employee processedEmployee in employees.Where(employee => employee.employeeid != -1))
            {
                if (itemRoleId > 0)
                {
                    if (this.EsrApiHandler.Execute<EmployeeRole>(processedEmployee.employeeid.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read, DataAccessMethodReturnDefault.Null) == null)
                    {

                        employeeIdsRoles.Add(
                            new EmployeeRole
                            {
                                employeeid = processedEmployee.employeeid,
                                itemroleid =
                                    int.Parse(itemRoleId.ToString(CultureInfo.InvariantCulture)),
                                order = 0,
                                Action = Action.Create
                            });
                    }
                }

                if (accessRoleId > 0)
                {
                    if (this.EsrApiHandler.Execute<EmployeeAccessRole>(processedEmployee.employeeid.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read, DataAccessMethodReturnDefault.Null) == null)
                    {

                        employeeIdAccessRoles.Add(new EmployeeAccessRole
                        {
                            employeeID = processedEmployee.employeeid,
                            accessRoleID = int.Parse(accessRoleId.ToString(CultureInfo.InvariantCulture)),
                            Action = Action.Create
                        });
                    }
                }
            }

            this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, employeeIdsRoles);
            this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, employeeIdAccessRoles);


            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create {0} Employees Elapsed Time {1}", employees.Count, DateTime.Now - subMethodStart),
                "EsrPersonsCrud");

            Template template = null;

            for (int i = 0; i < persons.Count; i++)
            {
                var originalPerson = entities[i];
                var updatedPerson = DataClassBase.GetRecordFromDataClass(persons[i], typeof(EsrPersonRecord)) as EsrPersonRecord;

                if (updatedPerson != null && updatedPerson.ActionResult.Result == ApiActionResult.Success)
                {
                    if (this.ImportElementType(TemplateMapping.ImportElementType.Employee, this.TrustVpd)
                        && employees.Count == persons.Count)
                    {
                        var newEmployee = employees[i];
                        var udfs = newEmployee.UserDefinedFields;
                        newEmployee.UserDefinedFields = udfs;
                        if (newEmployee.KeyValue == -1)
                        {
                            var existingUser = this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadSpecial, newEmployee.username);
                            if (existingUser != null && existingUser.Count > 0)
                            {
                                existingUser[0].ESRPersonId = originalPerson.ESRPersonId;
                                newEmployee.employeeid = existingUser[0].employeeid;
                                existingUser[0].Action = Action.Update;
                                this.EsrApiHandler.Execute(DataAccessMethod.Update, string.Empty, existingUser);
                            }
                            else
                            {
                                newEmployee = this.UpdateDuplicateUser(newEmployee);
                                if (newEmployee.ActionResult.Result != ApiActionResult.Success)
                                {
                                    newEmployee.ActionResult.Result = ApiActionResult.Failure;
                                    newEmployee.ActionResult.Message = "username not unique";
                                }
                            }
                        }

                        if (newEmployee.ActionResult.Result == ApiActionResult.Failure)
                        {
                            updatedPerson.ActionResult.Result = ApiActionResult.Failure;
                            updatedPerson.ActionResult.Message = string.Format(
                                "Employee Update/Insert failed - {0}", newEmployee.ActionResult.Message);
                        }

                        if (newEmployee.UserDefinedFields != null && newEmployee.UserDefinedFields.Fields != null)
                        {
                            foreach (UserDefinedField udf in newEmployee.UserDefinedFields.Fields.Where(udf => udf.Value != null))
                            {
                                udf.recordId = newEmployee.employeeid;
                                udf.Action = Action.Update;
                                this.UpdateUserDefinedFieldValue(udf);
                            }
                        }

                        updatedPerson.EmployeeId = newEmployee.employeeid;
                        updatedPerson.ActionResult = this.CheckSubObjectResults("Employee", updatedPerson.ActionResult, new List<DataClassBase> { newEmployee });
                        newEmployee.ActionResult.Message = string.Empty;
                    }
                    else
                    {
                        this.Logger.WriteDebug(
                            this.MetaBase,
                            "0",
                            this.AccountId,
                            LogRecord.LogItemTypes.None,
                            LogRecord.TransferTypes.None,
                            0,
                            string.Empty,
                            LogRecord.LogReasonType.None,
                            string.Format("Employee Import Skipped {0}", DateTime.Now),
                            "EsrPersonsCrud");
                    }

                    int currentEmployeeId = updatedPerson.EmployeeId;
                    if (this.ImportElementType(TemplateMapping.ImportElementType.Assignment, this.TrustVpd))
                    {
                        //// create update assignments
                        if (originalPerson.EsrAssignments != null)
                        {
                            foreach (EsrAssignment esrAssignment in originalPerson.EsrAssignments)
                            {
                                esrAssignment.employeeid = currentEmployeeId;
                            }
                        }

                        updatedPerson.EsrAssignments = this.UpdateAssignmentMapping(originalPerson, out template);
                        if (this.ImportElementType(TemplateMapping.ImportElementType.Location, this.TrustVpd))
                        {
                            allAddresses.AddRange(this.UpdateWorkAddressMapping(updatedPerson));
                        }
                    }
                    else
                    {
                        this.Logger.WriteDebug(
                            this.MetaBase,
                            "0",
                            this.AccountId,
                            LogRecord.LogItemTypes.None,
                            LogRecord.TransferTypes.None,
                            0,
                            string.Empty,
                            LogRecord.LogReasonType.None,
                            string.Format("Assignment Import Skipped {0}", DateTime.Now),
                            "EsrPersonsCrud");
                    }
                    //// create update phones
                    updatedPerson.Phones = originalPerson.Phones;
                    updatedPerson.Addresses = originalPerson.Addresses;

                    if (this.ImportElementType(TemplateMapping.ImportElementType.Vehicle, this.TrustVpd))
                    {
                        //// create update Vehicles
                        updatedPerson.Vehicles = originalPerson.Vehicles;
                    }
                    else
                    {
                        this.Logger.WriteDebug(
                            this.MetaBase,
                            "0",
                            this.AccountId,
                            LogRecord.LogItemTypes.None,
                            LogRecord.TransferTypes.None,
                            0,
                            string.Empty,
                            LogRecord.LogReasonType.None,
                            string.Format("Vehicles Import Skipped {0}", DateTime.Now),
                            "EsrPersonsCrud");
                    }

                    if (this.ImportElementType(TemplateMapping.ImportElementType.Costing, this.TrustVpd))
                    {
                        //// create update AssignmentCostings
                        updatedPerson.AssignmentCostings = originalPerson.AssignmentCostings;
                    }
                    else
                    {
                        this.Logger.WriteDebug(
                            this.MetaBase,
                            "0",
                            this.AccountId,
                            LogRecord.LogItemTypes.None,
                            LogRecord.TransferTypes.None,
                            0,
                            string.Empty,
                            LogRecord.LogReasonType.None,
                            string.Format("Assignment Costings Import Skipped {0}", DateTime.Now),
                            "EsrPersonsCrud");
                    }
                }

                result.Add(updatedPerson);

                if (updatedPerson != null)
                {
                    if (updatedPerson.EsrAssignments != null)
                    {
                        allAssignments.AddRange(updatedPerson.EsrAssignments);
                    }

                    if (updatedPerson.Addresses != null)
                    {
                        allEsrAddresses.AddRange(updatedPerson.Addresses);
                    }

                    if (updatedPerson.Phones != null)
                    {
                        allPhones.AddRange(updatedPerson.Phones);
                    }

                    if (updatedPerson.Vehicles != null)
                    {
                        allVehicles.AddRange(updatedPerson.Vehicles);
                    }

                    if (updatedPerson.AssignmentCostings != null)
                    {
                        allAssignmentCostings.AddRange(updatedPerson.AssignmentCostings);
                    }
                }
            }

            subMethodStart = DateTime.Now;
            var assignmentResult = Mapper<EsrAssignment>.SubmitList(allAssignments, this.EsrApiHandler);

       if (this.ImportElementType(TemplateMapping.ImportElementType.Location, this.TrustVpd))
            {
                new EsrAssignmentLocationCrud(this.MetaBase, this.AccountId, this.EsrApiHandler, this.Logger)
                    .CreateFromAssignments(assignmentResult);
            }

            this.LogResults(Mapper<EsrAssignment>.EsrToDataClass(assignmentResult));
            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create Assignments Elapsed Time {0}", DateTime.Now - subMethodStart),
                "EsrPersonsCrud");

            foreach (var assignment in allAssignments)
            {
                if (template != null && template.SignOffOwnerFieldId != Guid.Empty)
                {
                    // set the Sign-Off Owner to the configured column (e.g. supervisorEmployeeId, departmentManagerEmployeeId)
                    var signOffOwnerEmployeeId = this.GetEmployeeId(template.SignOffOwnerFieldId, assignment);
                    assignment.SignOffOwner = (signOffOwnerEmployeeId == null ? null : "25," + signOffOwnerEmployeeId); // 25 = Employee
                }
            }

            foreach (var person in entities)
            {
                if (this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd) != null && person.EsrAssignments != null)
                {
                    var employee = this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadByEsrId, person.ESRPersonId.ToString(CultureInfo.InvariantCulture)).FirstOrDefault();

                    if (template != null && template.LineManagerFieldId != Guid.Empty)
                    {
                        // set the Line Manager to the configured column (e.g. supervisorEmployeeId, departmentManagerEmployeeId)
                        employee.linemanager = this.GetEmployeeId(template.LineManagerFieldId, person.PrimaryAssignment);
                        employee.Action = Action.Update;
                        this.EsrApiHandler.Execute(DataAccessMethod.Update, string.Empty, new List<Employee> { employee });
                    }
                }
            }

            assignmentResult = Mapper<EsrAssignment>.SubmitList(allAssignments, this.EsrApiHandler);

            this.LogResults(Mapper<EsrAssignment>.EsrToDataClass(assignmentResult));
            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Update Assignments Elapsed Time {0}", DateTime.Now - subMethodStart),
                "EsrPersonsCrud");


            var phoneResult = new List<EsrPhone>();
            if (this.ImportElementType(TemplateMapping.ImportElementType.Phone, this.TrustVpd))
            {
                subMethodStart = DateTime.Now;
                phoneResult = Mapper<EsrPhone>.SubmitList(allPhones, this.EsrApiHandler);
                this.LogResults(Mapper<EsrPhone>.EsrToDataClass(phoneResult));
                this.Logger.WriteExtra(
                    this.MetaBase,
                    "0",
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("Create Phone Elapsed Time {0}", DateTime.Now - subMethodStart),
                    "EsrPersonsCrud");
            }

            var addressResult = new List<EsrAddress>();
            if (this.ImportElementType(TemplateMapping.ImportElementType.Address, this.TrustVpd))
            {
                subMethodStart = DateTime.Now;
                addressResult = Mapper<EsrAddress>.SubmitList(allEsrAddresses, this.EsrApiHandler);
                this.LogResults(Mapper<EsrAddress>.EsrToDataClass(addressResult));
                this.Logger.WriteExtra(
                    this.MetaBase,
                    "0",
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("Create Address Elapsed Time {0}", DateTime.Now - subMethodStart),
                    "EsrPersonsCrud");
            }

            var vehicleResult = new List<EsrVehicle>();
            if (this.ImportElementType(TemplateMapping.ImportElementType.Vehicle, this.TrustVpd))
            {
                subMethodStart = DateTime.Now;
                allVehicles = this.UpdateVehiclesMapping(result);
                vehicleResult = Mapper<EsrVehicle>.SubmitList(allVehicles, this.EsrApiHandler);
                this.LogResults(Mapper<EsrVehicle>.EsrToDataClass(vehicleResult));
                this.Logger.WriteExtra(
                    this.MetaBase,
                    "0",
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("Create Vehicles Elapsed Time {0}", DateTime.Now - subMethodStart),
                    "EsrPersonsCrud");
            }

            subMethodStart = DateTime.Now;
            var assignmentCostingResult = Mapper<EsrAssignmentCostings>.SubmitList(allAssignmentCostings, this.EsrApiHandler);
            this.LogResults(Mapper<EsrAssignmentCostings>.EsrToDataClass(assignmentCostingResult));
            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create Assignment Costing Elapsed Time {0}", DateTime.Now - subMethodStart),
                "EsrPersonsCrud");
            subMethodStart = DateTime.Now;

            foreach (EsrPersonRecord person in result)
            {
                var currentPerson = person;
                if (assignmentResult != null)
                {
                    var currentAssignments = assignmentResult.Where(assignment => assignment.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.EsrAssignments = currentAssignments.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults("Assignment", currentPerson.ActionResult, currentPerson.EsrAssignments);

                    foreach (EsrAssignment assignment in currentPerson.EsrAssignments)
                    {
                        assignment.ActionResult.Message = string.Empty;
                    }
                }

                if (phoneResult != null)
                {
                    var currentPhones = phoneResult.Where(phone => phone.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.Phones = currentPhones.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults("Phone", currentPerson.ActionResult, currentPerson.Phones);

                    foreach (EsrPhone phone in currentPerson.Phones)
                    {
                        phone.ActionResult.Message = string.Empty;
                    }
                }

                if (addressResult != null)
                {
                    var currentAddress = addressResult.Where(address => address.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.Addresses = currentAddress.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults("Address", currentPerson.ActionResult, currentPerson.Addresses);

                    foreach (EsrAddress address in currentPerson.Addresses)
                    {
                        address.ActionResult.Message = string.Empty;
                    }

                    if (this.ImportElementType(TemplateMapping.ImportElementType.Address, this.TrustVpd))
                    {
                        this.CreateUpdateHomeAddresses(currentPerson);
                    }
                }

                if (vehicleResult != null && this.ImportElementType(TemplateMapping.ImportElementType.Vehicle, this.TrustVpd))
                {
                    var currentVehicle = vehicleResult.Where(vehicle => vehicle.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.Vehicles = currentVehicle.ToList();
                    var carresult = this.CreateUpdateCars(currentPerson);
                    currentPerson.ActionResult = this.CheckSubObjectResults("Vehicle", currentPerson.ActionResult, carresult);
                    foreach (EsrVehicle vehicle in currentPerson.Vehicles)
                    {
                        vehicle.ActionResult.Message = string.Empty;
                    }
                }

                if (assignmentCostingResult != null)
                {
                    var currentCostings = assignmentCostingResult.Where(costing => costing.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.AssignmentCostings = currentCostings.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults("AssignmentCosting", currentPerson.ActionResult, currentPerson.AssignmentCostings);
                    foreach (EsrAssignmentCostings costingRecord in currentPerson.AssignmentCostings)
                    {
                        costingRecord.ActionResult.Message = string.Empty;
                    }
                }
            }

            this.EsrApiHandler.UpdateEsrAssignmentSupervisors();

            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create Mappings Elapsed Time {0}", DateTime.Now - subMethodStart),
                "EsrPersonsCrud");

            this.Logger.WriteExtra(
                this.MetaBase,
                "0",
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create Elapsed Time {0}", DateTime.Now - methodStart),
                "EsrPersonsCrud");

            return result;
        }

        private static IEnumerable<Address> UniqueAddresses(IEnumerable<Address> allAddresses)
        {
            var addressIDs = new Dictionary<int, long>();
            var uniqueAddresses = new List<Address>();
            foreach (Address address in allAddresses)
            {
                if (address.AddressID == 0 || !addressIDs.ContainsKey(address.AddressID))
                {
                    uniqueAddresses.Add(address);
                    if (address.AddressID != 0)
                    {
                        addressIDs.Add(address.AddressID, 1);
                    }
                }
            }
            return uniqueAddresses;
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPersonRecord Read(int entityId)
        {
            var esrPerson = this.EsrApiHandler.Execute<EsrPerson>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
            return this.GetExtraPersonData(esrPerson);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPersonRecord Read(long entityId)
        {
            var esrPerson = this.EsrApiHandler.Execute<EsrPerson>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
            return this.GetExtraPersonData(esrPerson);
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
        public List<EsrPersonRecord> ReadAll()
        {
            var allPersons = this.EsrApiHandler.Execute<EsrPerson>(DataAccessMethod.ReadAll);
            return allPersons.Select(this.GetExtraPersonData).ToList();
        }

        /// <summary>
        /// The read by esr id.
        /// </summary>
        /// <param name="esrId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPersonRecord> ReadByEsrId(long esrId)
        {
            var esrPersons = this.EsrApiHandler.Execute<EsrPerson>(DataAccessMethod.ReadByEsrId, esrId.ToString(CultureInfo.InvariantCulture));
            return esrPersons.Select(this.GetExtraPersonData).ToList();
        }

        /// <summary>
        /// The read special - Not implemented.
        /// </summary>
        /// <param name="reference">
        /// The reference.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        public List<EsrPersonRecord> ReadSpecial(string reference)
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
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPersonRecord> Update(List<EsrPersonRecord> entity)
        {
            return this.Create(entity);
        }

        /// <summary>
        /// The delete person record.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPersonRecord Delete(int entityId)
        {
            return new EsrPersonRecord();
        }

        /// <summary>
        /// The delete person record.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        public EsrPersonRecord Delete(long entityId)
        {
            var result = this.GetExtraPersonData(this.EsrApiHandler.Execute<EsrPerson>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read));
            var employeeToDelete = this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadByEsrId, entityId.ToString(CultureInfo.InvariantCulture));
            List<Employee> employeesToDelete = employeeToDelete ?? new List<Employee>();
            if (employeesToDelete.Count == 1)
            {
                employeesToDelete[0].archived = true;
                employeesToDelete[0].Action = Action.Update;
                var employeeResult = this.EsrApiHandler.Execute(DataAccessMethod.Update, string.Empty, employeesToDelete);
                if (employeeResult[0].ActionResult.Result == ApiActionResult.Success)
                {
                    result.ActionResult = employeeResult[0].ActionResult;
                    if (result.Vehicles != null)
                    {
                        foreach (EsrVehicle vehicle in result.Vehicles)
                        {
                            var currentCar = this.EsrApiHandler.Execute<Car>(DataAccessMethod.ReadByEsrId, employeeResult[0].ESRPersonId.ToString());
                            foreach (Car car in currentCar)
                            {
                                car.active = false;
                                car.Action = Action.Update;
                                this.EsrApiHandler.Execute<CarAssignmentNumberAllocation>(vehicle.ESRVehicleAllocationId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
                            }

                            this.EsrApiHandler.Execute(DataAccessMethod.Update, string.Empty, currentCar);
                            var vehicleResult = this.EsrApiHandler.Execute<EsrVehicle>(vehicle.KeyValue.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
                            if (vehicleResult != null && vehicleResult.ActionResult.Result != ApiActionResult.Deleted)
                            {
                                result.ActionResult.Message += this.FormatFailure(
                                    "Vehicle",
                                    vehicle.ESRVehicleAllocationId,
                                    vehicleResult.ActionResult.Message,
                                    vehicleResult.ActionResult.Result);
                                result.ActionResult.Result = ApiActionResult.PartialSuccess;
                            }
                        }
                    }

                    if (result.AssignmentCostings != null)
                    {
                        foreach (EsrAssignmentCostings assignmentCosting in result.AssignmentCostings)
                        {
                            if (assignmentCosting != null)
                            {
                                var assignmentCostingResult =
                                    this.EsrApiHandler.Execute<EsrAssignmentCostings>(assignmentCosting.KeyValue.ToString(CultureInfo.InvariantCulture),
                                                                                      DataAccessMethod.Delete,
                                                                                      DataAccessMethodReturnDefault.Null);
                                if (assignmentCostingResult != null
                                    && assignmentCostingResult.ActionResult.Result != ApiActionResult.Deleted)
                                {
                                    result.ActionResult.Result = ApiActionResult.PartialSuccess;
                                    result.ActionResult.Message += this.FormatFailure(
                                        "AssignmentCosting",
                                        assignmentCosting.ESRCostingAllocationId,
                                        assignmentCostingResult.ActionResult.Message,
                                        assignmentCosting.ActionResult.Result);
                                }
                            }
                        }
                    }

                    if (result.EsrAssignments != null)
                    {
                        foreach (EsrAssignment assignment in result.EsrAssignments)
                        {
                            var assignmentResult = this.EsrApiHandler.Execute<EsrAssignment>(assignment.KeyValue.ToString(CultureInfo.InvariantCulture),
                                                                                             DataAccessMethod.Delete,
                                                                                             DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
                            if (assignmentResult != null
                                && assignmentResult.ActionResult.Result != ApiActionResult.Deleted)
                            {
                                result.ActionResult.Result = ApiActionResult.PartialSuccess;
                                result.ActionResult.Message += this.FormatFailure(
                                    "Assignment",
                                    assignment.esrAssignID,
                                    assignmentResult.ActionResult.Message,
                                    assignmentResult.ActionResult.Result);
                            }
                        }
                    }

                    if (result.Addresses != null)
                    {
                        foreach (EsrAddress esrAddress in result.Addresses)
                        {
                            if (esrAddress != null)
                            {
                                var currentAddresses = this.EsrApiHandler.Execute<Address>(DataAccessMethod.ReadByEsrId, esrAddress.KeyValue.ToString(CultureInfo.InvariantCulture));
                                if (currentAddresses != null)
                                {
                                    foreach (Address address in currentAddresses)
                                    {
                                        address.Archived = true;
                                        address.EsrAddressID = null;
                                        address.Action = Action.Update;
                                    }
                                }

                                this.EsrApiHandler.Execute(DataAccessMethod.Update, string.Empty, currentAddresses);
                            }

                            if (esrAddress != null)
                            {
                                var addressResult = this.EsrApiHandler.Execute<Address>(esrAddress.KeyValue.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
                                if (addressResult != null
                                    && addressResult.ActionResult.Result != ApiActionResult.Deleted)
                                {
                                    result.ActionResult.Result = ApiActionResult.PartialSuccess;
                                    result.ActionResult.Message += this.FormatFailure(
                                        "Address",
                                        esrAddress.ESRAddressId,
                                        addressResult.ActionResult.Message,
                                        addressResult.ActionResult.Result);
                                }
                            }
                        }
                    }

                    if (result.Phones != null)
                    {
                        foreach (EsrPhone phone in result.Phones)
                        {
                            if (phone != null)
                            {
                                var phoneResult = this.EsrApiHandler.Execute<EsrPhone>(phone.KeyValue.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete, DataAccessMethodReturnDefault.Null);
                                if (phoneResult != null && phoneResult.ActionResult.Result != ApiActionResult.Deleted)
                                {
                                    result.ActionResult.Result = ApiActionResult.PartialSuccess;
                                    result.ActionResult.Message += this.FormatFailure(
                                        "Phone",
                                        phone.ESRPhoneId,
                                        phoneResult.ActionResult.Message,
                                        phoneResult.ActionResult.Result);
                                }
                            }
                        }
                    }

                    var personresult = this.EsrApiHandler.Execute<EsrPerson>(entityId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Delete);
                    if (personresult.ActionResult.Result != ApiActionResult.Deleted)
                    {
                        result.ActionResult.Result = ApiActionResult.PartialSuccess;
                        result.ActionResult.Message += this.FormatFailure(
                            "Vehicle",
                            personresult.ESRPersonId,
                            personresult.ActionResult.Message,
                            personresult.ActionResult.Result);
                    }
                }
                else
                {
                    result.ActionResult = employeeResult[0].ActionResult;
                }
            }

            return this.GetExtraPersonData(result);
        }

        public EsrPersonRecord Delete(EsrPersonRecord entity)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Methods

        #region Create Update Data

        /// <summary>
        /// The create update assignment COSTINGS.
        /// </summary>
        /// <param name="person">
        /// The person.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        private List<EsrAssignmentCostings> CreateUpdateCostCodes(EsrPersonRecord person)
        {
            var employeeCostCodes = new List<EsrAssignmentCostings>();
            if (person.AssignmentCostings != null)
            {
                foreach (EsrAssignmentCostings costing in person.AssignmentCostings)
                {
                    Lookup lookupResult = this.EsrApiHandler.ReadLookupValue(
                        "BF9AA39A-82D6-4960-BFEF-C5943BC0542D",
                        "7C0D8EAB-D9AF-415F-9BB7-D1BE01F69E2F",
                        costing.ESRAssignmentId.ToString(CultureInfo.InvariantCulture));
                    if (lookupResult != null && lookupResult.FirstColumnValue != null)
                    {
                        costing.ESRAssignId = (int?)lookupResult.FirstColumnValue;
                    }

                    employeeCostCodes.Add(costing);
                }
            }

            return employeeCostCodes;
        }

        /// <summary>
        /// Create update cars from the ESR vehicle records.
        /// </summary>
        /// <param name="personRecord">
        /// The person Record.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        private IEnumerable<Car> CreateUpdateCars(EsrPersonRecord personRecord)
        {
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);
            if (mapping != null)
            {
                var newCars = new List<Car>();
                var currentVehicleList = new List<EsrVehicle>();
                foreach (EsrVehicle vehicle in personRecord.Vehicles)
                {
                    var currentCarList = this.EsrApiHandler.Execute<Car>(DataAccessMethod.ReadByEsrId, personRecord.ESRPersonId.ToString(CultureInfo.InvariantCulture));
                    var currentCarAssignments = new List<CarAssignmentNumberAllocation>();
                    foreach (Car car in currentCarList)
                    {
                        currentCarAssignments.AddRange(this.EsrApiHandler.Execute<CarAssignmentNumberAllocation>(DataAccessMethod.ReadSpecial, car.carid.ToString(CultureInfo.InvariantCulture)));
                    }

                    var currentCar = new Car();
                    var baseAttribute = currentCar.ClassAttribute.ElementType;
                    currentCar = this.FindCarByRegistration(vehicle.RegistrationNumber, currentCarList);
                    EsrVehicle currentVehicle = vehicle;
                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        foreach (FieldInfo field in currentCar.ClassFields())
                        {
                            var attribute = field.GetCustomAttribute<DataClassAttribute>();
                            if (attribute != null && attribute.FieldId != null)
                            {
                                if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                {
                                    // found a mapping field
                                    if (baseAttribute == templateMapping.importElementType)
                                    {
                                        string fieldValue;
                                        if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                        {
                                            currentCar =
                                                (Car)
                                                this.LookupField(
                                                    templateMapping, vehicle, currentCar, out fieldValue, field);
                                        }
                                        else
                                        {
                                            currentCar = Mapper<Car>.SetMappingField(
                                                templateMapping.columnRef,
                                                vehicle,
                                                templateMapping.fieldID,
                                                currentCar,
                                                out fieldValue,
                                                field);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (personRecord.EmployeeId > 0)
                    {
                        if (currentCar.Allocations == null)
                        {
                            currentCar.Allocations = new List<CarAssignmentNumberAllocation>();
                        }

                        var allocation = new CarAssignmentNumberAllocation
                        {
                            CarId = currentCar.carid,
                            Action = Action.Create,
                            ESRVehicleAllocationId =
                                vehicle.ESRVehicleAllocationId
                        };

                        if (vehicle.ESRAssignID != null)
                        {
                            allocation.ESRAssignId = (int)vehicle.ESRAssignID;
                        }
                        else
                        {
                            var vehicleAssignment = personRecord.EsrAssignments.FirstOrDefault(ass => ass.AssignmentID == vehicle.ESRAssignmentId);
                            if (vehicleAssignment != null)
                            {
                                allocation.ESRAssignId = vehicleAssignment.esrAssignID;
                            }
                        }

                        currentCar.Action = Action.Create;
                        currentCar.employeeid = personRecord.EmployeeId;

                        var engine = this.EsrApiHandler.Execute<VehicleEngineType>(DataAccessMethod.ReadSpecial, vehicle.FuelType).FirstOrDefault();
                        if (engine == null)
                        {
                            currentCar.ActionResult.Message = string.Format("Could not find Vehicle Engine Type Code '{0}' for vehicle {1}", vehicle.FuelType, vehicle.ESRVehicleAllocationId);
                            currentCar.ActionResult.Result = ApiActionResult.PartialSuccess;

                            // User Story 72848 will replace this eventually
                            engine = new VehicleEngineType();
                        }

                        currentCar.cartypeid = (byte)engine.VehicleEngineTypeId;
                        var carResult = this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, new List<Car> { currentCar });
                        if (carResult != null && carResult.Count == 1)
                        {
                            currentCar = carResult[0];
                            allocation.CarId = currentCar.carid;
                            currentCar.Allocations.Add(allocation);
                            newCars.Add(currentCar);
                        }

                    }

                    currentVehicleList.Add(currentVehicle);
                }

                personRecord.Vehicles = currentVehicleList;
                var allAllocations = newCars.Where(car => car.Allocations != null).SelectMany(car => car.Allocations).ToList();

                this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, allAllocations);
                return newCars;
            }

            return new List<Car>();
        }

        /// <summary>
        /// The create update home addresses.
        /// </summary>
        /// <param name="updatedPerson">
        /// The updated person.
        /// </param>
        private void CreateUpdateHomeAddresses(EsrPersonRecord updatedPerson)
        {
            var homeAddressDictionary = new Dictionary<long, EmployeeHomeAddress>();
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);
            if (mapping != null)
            {
                var addressVal = this.GetAccountProperty("ImportHomeAddressFormat");
                var addresses = new List<Address>();

                foreach (EsrAddress address in updatedPerson.Addresses.Where(address => String.Equals(address.PrimaryFlag, "Yes", StringComparison.OrdinalIgnoreCase) || GetAccountProperty("esrPrimaryAddressOnly") == "0"))
                {
                    var newAddress = new Address();
                    var baseAttribute = address.ClassAttribute.ElementType;

                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        if (templateMapping.importElementType == baseAttribute)
                        {
                            string fieldValue = string.Empty;

                            foreach (FieldInfo field in newAddress.ClassFields())
                            {
                                var attribute = field.GetCustomAttribute<DataClassAttribute>();

                                if (attribute != null && attribute.FieldId != null)
                                {
                                    if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                    {
                                        if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                        {
                                            if (templateMapping.destinationField.Contains("Country"))
                                            {
                                                var targetValue = string.Format("'{0}'", "GB");
                                                Lookup result =
                                                    this.EsrApiHandler.ReadLookupValue(
                                                        "B1E9111B-AA3C-4CC0-AD99-6C9E4B3E70FC",
                                                        targetValue.Length > 4 ? "0053315D-3492-442F-97D9-B6E3015DFF5C" : "078DD298-7CA6-4E52-8D57-E85444C8797A",
                                                        targetValue);
                                                if (result != null)
                                                {
                                                    newAddress.Country = (int?)result.FirstColumnValue;
                                                }
                                            }
                                            else
                                            {
                                                newAddress =
                                                    (Address)
                                                        this.LookupField(
                                                        templateMapping, address, newAddress, out fieldValue, field);
                                            }
                                        }
                                        else
                                        {
                                            newAddress = Mapper<Address>.SetMappingField(
                                                    templateMapping.columnRef,
                                                    address,
                                                    templateMapping.fieldID,
                                                newAddress,
                                                    out fieldValue,
                                                    field);
                                        }
                                    }
                                }

                                addressVal = Mapper<EsrAddress>.SetStringFormatValues(
                                    addressVal, field.Name, fieldValue);
                            }
                        }
                    }

                    newAddress.EsrAddressID = address.ESRAddressId;
                    newAddress.Action = Action.Create;
                    newAddress.CreationMethod = AddressCreationMethod.EsrOutbound;
                    if (string.IsNullOrEmpty(newAddress.AddressName))
                    {
                        newAddress.AddressName = "";
                    }

                    addresses.Add(newAddress);

                    if (updatedPerson.EmployeeId > 0)
                    {
                        homeAddressDictionary.Add(address.ESRAddressId,
                            new EmployeeHomeAddress
                            {
                                Action = Action.Create,
                                EmployeeId = updatedPerson.EmployeeId,
                                AddressId = newAddress.AddressID,
                                StartDate = address.EffectiveStartDate,
                                EndDate = address.EffectiveEndDate,
                                CreatedBy = updatedPerson.EmployeeId
                            });
                    }
                }

                var addressResult = this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, addresses);

                var homeAddressList = new List<EmployeeHomeAddress>();
                foreach (Address address in addressResult)
                {
                    if (address.AddressID > 0 && updatedPerson.EmployeeId > 0 && address.EsrAddressID.HasValue && homeAddressDictionary.ContainsKey(address.EsrAddressID.Value))
                    {
                        EmployeeHomeAddress homeAddress = homeAddressDictionary[address.EsrAddressID.Value];
                        homeAddress.AddressId = address.AddressID;

                        homeAddressList.Add(homeAddress);
                    }
                }

                this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, homeAddressList);
            }
        }

        /// <summary>
        /// The update user defined field value.
        /// </summary>
        /// <param name="udf">
        /// The User Defined Field.
        /// </param>
        private void UpdateUserDefinedFieldValue(UserDefinedField udf)
        {
            this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, new List<UserDefinedField> { udf });
        }

        #endregion

        #region UpdateMappings

        /// <summary>
        /// The create update assignments.
        /// </summary>
        /// <param name="esrPerson">
        /// The ESR Person.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        internal List<EsrAssignment> UpdateAssignmentMapping(EsrPersonRecord esrPerson, out Template template)
        {
            var udfList = this.GetUdf(this.AccountId);
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);
            template = null;

            if (mapping != null && esrPerson.EsrAssignments != null)
            {
                var newResult = esrPerson.EsrAssignments.Count > 0 ? esrPerson.EsrAssignments[0].ActionResult : new ApiResult();
                var assignments = new List<EsrAssignment>();
                var employee = this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadByEsrId, esrPerson.ESRPersonId.ToString(CultureInfo.InvariantCulture)).FirstOrDefault();
                
                foreach (EsrAssignment assignment in esrPerson.EsrAssignments)
                {
                    EsrAssignment existingAssignment = null;
                    if (assignment.ESRPersonId != null)
                    {
                        var currentAssignment = this.EsrApiHandler.Execute<EsrAssignment>(DataAccessMethod.ReadByEsrId, assignment.ESRPersonId.ToString(), null, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
                        if (currentAssignment != null && currentAssignment.Count > 0)
                        {
                            EsrAssignment assignment1 = assignment;
                            foreach (EsrAssignment esrAssignment in
                                currentAssignment.Where(esrAssignment => assignment1 != null && esrAssignment.AssignmentNumber == assignment1.AssignmentNumber && esrAssignment.EffectiveStartDate == assignment1.EffectiveStartDate))
                            {
                                existingAssignment = assignment;
                                existingAssignment.esrAssignID = esrAssignment.esrAssignID;
                                existingAssignment.SignOffOwner = esrAssignment.SignOffOwner;
                                existingAssignment.SupervisorEsrAssignID = esrAssignment.SupervisorEsrAssignID;
                                existingAssignment.Action = Action.Update;
                                existingAssignment.Active = this.SetActiveOnAssignment(existingAssignment);
                                break;
                            }
                        }
                    }

                    assignment.Active = this.SetActiveOnAssignment(assignment);

                    foreach (FieldInfo field in assignment.ClassFields())
                    {
                        var mapped = false;
                        foreach (TemplateMapping templateMapping in mapping)
                        {
                            if (template == null)
                            {
                                template = this.EsrApiHandler.Execute<Template>(templateMapping.templateID.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
                            }

                            var attribute = field.GetCustomAttribute<DataClassAttribute>();
                            if (attribute != null && attribute.FieldId != null)
                            {
                                if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                {
                                    mapped = true;
                                    if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                    {
                                        string fieldValue;
                                        if (existingAssignment != null)
                                        {
                                            existingAssignment = (EsrAssignment)this.LookupField(templateMapping, assignment, existingAssignment, out fieldValue, field, Mapper<EsrAssignment>.EsrToDataClass(esrPerson.EsrAssignments));
                                        }
                                        else
                                        {
                                            existingAssignment = (EsrAssignment)this.LookupField(templateMapping, assignment, assignment, out fieldValue, field, Mapper<EsrAssignment>.EsrToDataClass(esrPerson.EsrAssignments));
                                        }
                                    }
                                }
                            }
                        }

                        if (!mapped)
                        {
                            var dataAttribute = field.GetCustomAttribute<DataMemberAttribute>();
                            if (dataAttribute == null || (!dataAttribute.IsRequired && dataAttribute.Order != 99))
                            {
                                field.SetValue(assignment, null);
                            }
                        }
                    }

                    var theAssignment = (existingAssignment ?? assignment);

                    assignments.Add(theAssignment);
                }

                var primaryAssignment = esrPerson.PrimaryAssignment;
                if (primaryAssignment != null)
                {
                    var valueSet = false;
                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        if (templateMapping.importElementType != TemplateMapping.ImportElementType.Assignment)
                        {
                            continue;
                        }

                        if (employee != null)
                        {
                            foreach (FieldInfo field in employee.ClassFields())
                            {
                                var attribute = field.GetCustomAttribute<DataClassAttribute>();
                                if (attribute != null && attribute.FieldId != null)
                                {
                                    if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                    {
                                        // found a mapping field
                                        if (attribute.ColumnRef != templateMapping.columnRef
                                            || TemplateMapping.ImportElementType.Assignment != templateMapping.importElementType)
                                        {
                                            // find field and map data
                                            switch (templateMapping.importElementType)
                                            {
                                                case TemplateMapping.ImportElementType.Assignment:
                                                case TemplateMapping.ImportElementType.Employee:
                                                    string fieldValue;
                                                    if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                                    {
                                                        employee =
                                                            (Employee)
                                                            this.LookupField(
                                                                templateMapping, primaryAssignment, employee, out fieldValue, field);
                                                        valueSet = true;
                                                    }
                                                    else
                                                    {
                                                        employee = Mapper<Employee>.SetMappingField(
                                                            templateMapping.columnRef,
                                                            primaryAssignment,
                                                            templateMapping.fieldID,
                                                            employee,
                                                            out fieldValue,
                                                            field);
                                                        valueSet = true;
                                                    }

                                                    break;
                                            }
                                        }
                                    }
                                }

                                if (valueSet)
                                {
                                    employee.Action = Action.Update;
                                    this.EsrApiHandler.Execute(DataAccessMethod.Update, string.Empty, new List<Employee> { employee });
                                    break;
                                }
                            }
                        }
                    }

                    if (template != null && template.LineManagerFieldId != Guid.Empty)
                    {
                        // set the Line Manager to the configured column (e.g. supervisorEmployeeId, departmentManagerEmployeeId)
                        employee.linemanager = this.GetEmployeeId(template.LineManagerFieldId, primaryAssignment);
                        employee.Action = Action.Update;
                        this.EsrApiHandler.Execute(DataAccessMethod.Update, String.Empty, new List<Employee> { employee });
                    }

                }

                assignments[0].ActionResult = newResult;
                return assignments;
            }

            return new List<EsrAssignment>();
        }

        /// <summary>
        /// The set active on assignment record.
        /// </summary>
        /// <param name="assignment">
        /// The assignment.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool SetActiveOnAssignment(EsrAssignment assignment)
        {
            var today = DateTime.Now;

            if (today >= assignment.EarliestAssignmentStartDate)
            {
                if (assignment.EffectiveEndDate == null || assignment.EffectiveEndDate > today)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// The create update work addresses from person / Assignments.
        /// </summary>
        /// <param name="person">
        /// The current person record.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        private IEnumerable<Address> UpdateWorkAddressMapping(EsrPersonRecord person)
        {
            var addressVal = this.GetAccountProperty("ImportWorkAddressFormat");

            var addresses = new List<Address>();
            foreach (EsrAssignment assignment in person.EsrAssignments)
            {
                EsrLocation location = null;
                if (assignment.EsrLocationId != null)
                {
                    location = this.EsrApiHandler.Execute<EsrLocation>(assignment.EsrLocationId.ToString(), DataAccessMethod.Read);
                }
                else
                {
                    if (assignment.EsrOrganisationId != null)
                    {
                        var organisation = this.EsrApiHandler.Execute<EsrOrganisation>(assignment.EsrOrganisationId.ToString(), DataAccessMethod.Read);
                        if (organisation != null && organisation.ESRLocationId != null)
                        {
                            location = this.EsrApiHandler.Execute<EsrLocation>(organisation.ESRLocationId.ToString(), DataAccessMethod.Read);
                        }
                    }
                }

                if (location != null)
                {
                    var currentAddress = this.EsrApiHandler.Execute<Address>(DataAccessMethod.ReadByEsrId, location.ESRLocationId.ToString(CultureInfo.InvariantCulture));
                    if (currentAddress.Count == 0)
                    {
                        var targetValue = string.Format("'{0}'", "GB");
                        Lookup result = this.EsrApiHandler.ReadLookupValue(
                            "B1E9111B-AA3C-4CC0-AD99-6C9E4B3E70FC",
                            targetValue.Length > 4 ? "0053315D-3492-442F-97D9-B6E3015DFF5C" : "078DD298-7CA6-4E52-8D57-E85444C8797A",
                            targetValue) ?? new Lookup();

                        var address = new Address
                        {
                            AddressName = location.Description,
                            Line1 = location.AddressLine1,
                            Line2 = location.AddressLine2,
                            Line3 = location.AddressLine3,
                            Postcode = location.Postcode,
                            County = location.County,
                            City = location.Town,
                            CreatedOn = DateTime.Now,
                            Action = Action.Create,
                            EsrLocationID = location.ESRLocationId,
                            CreationMethod = AddressCreationMethod.EsrOutbound,
                            Country = (int?)result.FirstColumnValue
                        };
                        foreach (FieldInfo field in address.ClassFields())
                        {
                            var fieldValue = field.GetValue(address);
                            if (fieldValue != null)
                            {
                                addressVal = Mapper<Address>.SetStringFormatValues(addressVal, field.Name, fieldValue.ToString());
                            }
                        }

                        addresses.AddRange(this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, new List<Address> { address }));
                    }
                    else
                    {
                        addresses.Add(currentAddress[0]);
                    }
                }
            }

            return addresses;
        }

        /// <summary>
        /// The create update ESR persons.
        /// </summary>
        /// <param name="persons">
        /// The persons.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        private List<EsrPerson> UpdateEsrPersonsMapping(List<EsrPerson> persons)
        {
            var methodStart = DateTime.Now;
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);
            if (mapping != null)
            {
                var newPersons = new List<EsrPerson>();
                foreach (EsrPerson person in persons)
                {
                    EsrPerson newPerson = null;
                    foreach (TemplateMapping templateMapping in mapping)
                    {
                        foreach (FieldInfo field in person.ClassFields())
                        {
                            var attribute = field.GetCustomAttribute<DataClassAttribute>();
                            if (attribute != null && attribute.FieldId != null)
                            {
                                if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                {
                                    if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                    {
                                        string fieldValue;
                                        newPerson = (EsrPerson)this.LookupField(templateMapping, person, newPerson, out fieldValue, field, Mapper<EsrPerson>.EsrToDataClass(persons));
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    newPersons.Add(newPerson ?? person);
                }

                this.Logger.WriteExtra(
                    this.MetaBase,
                    "0",
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("CreateUpdatePersons Elapsed Time {0}", DateTime.Now - methodStart),
                    "EsrPersonsCrud");
                return newPersons;
            }

            return new List<EsrPerson>();
        }

        /// <summary>
        /// The create update employee method.
        /// </summary>
        /// <param name="personRecord">
        /// The person record.
        /// </param>
        /// <param name="employee">
        /// The employee.
        /// </param>
        /// <returns>
        /// Created Employee Id or zero if failed.
        /// </returns>
        private Employee UpdateEmployeeMapping(EsrPersonRecord personRecord, Employee employee)
        {
            var userdefinedFieldList = this.GetUdf(this.AccountId);
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);
            var usernameVal = this.GetAccountProperty("importUsernameFormat");
            if (mapping != null)
            {
                if (employee == null)
                {
                    employee = new Employee { Action = Action.Create };
                }

                var baseAttribute = employee.ClassAttribute.ElementType;
                var newUdfList = new List<UserDefinedField>();
                employee.firstname = personRecord.FirstName;
                employee.surname = personRecord.LastName;
                employee.GetUserDefined(this.AccountId, userdefinedFieldList);
                Template template = null;

                foreach (TemplateMapping templateMapping in mapping)
                {
                    if (template == null)
                    {
                        template = this.EsrApiHandler.Execute<Template>(templateMapping.templateID.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
                    }

                    if (templateMapping.importElementType != TemplateMapping.ImportElementType.Employee && templateMapping.importElementType != TemplateMapping.ImportElementType.Assignment && templateMapping.importElementType != TemplateMapping.ImportElementType.Person)
                    {
                        continue;
                    }

                    var supervisorAssignmentNumberMapped = this.IsSupervisorAssignmentNumberMapped(mapping);

                    string fieldValue = string.Empty;
                    foreach (FieldInfo field in employee.ClassFields())
                    {
                        var attribute = field.GetCustomAttribute<DataClassAttribute>();
                        if (attribute != null && attribute.FieldId != null)
                        {
                            if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                            {
                                // found a mapping field
                                if (attribute.ColumnRef != templateMapping.columnRef
                                    || baseAttribute != templateMapping.importElementType)
                                {
                                    // find field and map data
                                    switch (templateMapping.importElementType)
                                    {
                                        case TemplateMapping.ImportElementType.Assignment:
                                            var primaryAssignment = personRecord.PrimaryAssignment;
                                            if (primaryAssignment == null)
                                            {
                                                var currentPerson =
                                                    DataClassBase.GetRecordFromDataClass(this.EsrApiHandler.Execute<EsrPerson>(personRecord.ESRPersonId.ToString(CultureInfo.InvariantCulture),
                                                                                           DataAccessMethod.Read),
                                                                                         typeof(EsrPersonRecord)) as EsrPersonRecord;
                                                if (currentPerson != null)
                                                {
                                                    primaryAssignment = currentPerson.PrimaryAssignment;
                                                }
                                            }

                                            if (primaryAssignment != null)
                                            {
                                                if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                                {
                                                    employee =
                                                        (Employee)
                                                        this.LookupField(
                                                            templateMapping,
                                                            primaryAssignment,
                                                            employee,
                                                            out fieldValue,
                                                            field);
                                                }
                                                else
                                                {
                                                    employee =
                                                        Mapper<Employee>.SetMappingField(
                                                            templateMapping.columnRef,
                                                            primaryAssignment,
                                                            templateMapping.fieldID,
                                                            employee,
                                                            out fieldValue,
                                                            field);
                                                }

                                                if ((primaryAssignment.SupervisorPersonId != null) && !supervisorAssignmentNumberMapped)
                                                {
                                                    var supervisorEmployees = this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadByEsrId, primaryAssignment.SupervisorPersonId.ToString());
                                                    if (supervisorEmployees.Count == 1)
                                                    {
                                                        employee.linemanager = supervisorEmployees[0].employeeid;
                                                    }
                                                }
                                            }

                                            break;
                                        case TemplateMapping.ImportElementType.Employee:
                                            if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                            {
                                                employee =
                                                    (Employee)
                                                    this.LookupField(
                                                        templateMapping, personRecord, employee, out fieldValue, field);
                                            }
                                            else
                                            {
                                                employee = Mapper<Employee>.SetMappingField(
                                                    templateMapping.columnRef,
                                                    personRecord,
                                                    templateMapping.fieldID,
                                                    employee,
                                                    out fieldValue,
                                                    field);
                                            }

                                            break;
                                    }
                                }
                            }
                        }
                    }

                    usernameVal = Mapper<Employee>.SetStringFormatValues(
                        usernameVal, templateMapping.destinationField, fieldValue);

                    foreach (UserDefinedField udf in employee.UserDefinedFields.Fields)
                    {
                        bool udfValueSet = false;
                        UserDefinedField newUdf = null;

                        if (udf.fieldid == templateMapping.fieldID)
                        {
                            switch (templateMapping.importElementType)
                            {
                                case TemplateMapping.ImportElementType.Employee:
                                case TemplateMapping.ImportElementType.Person:
                                    newUdf = Mapper<Employee>.SetUdf(templateMapping, personRecord, employee, this.EsrApiHandler);
                                    udfValueSet = true;
                                    break;
                                case TemplateMapping.ImportElementType.Assignment:
                                    var primaryAssignment = personRecord.PrimaryAssignment;
                                    if (primaryAssignment != null)
                                    {
                                        newUdf = Mapper<Employee>.SetUdf(templateMapping, primaryAssignment, employee, this.EsrApiHandler);
                                        udfValueSet = true;
                                    }

                                    break;
                            }
                        }

                        if (udfValueSet)
                        {
                            newUdfList.Add(newUdf);
                            break;
                        }
                    }
                }

                employee.UserDefinedFields.Fields = newUdfList;
                if (string.IsNullOrEmpty(employee.username))
                {
                    if (!string.IsNullOrEmpty(usernameVal))
                    {
                        employee.username = usernameVal;
                    }
                    else
                    {
                        // If no format set then set below as the default
                        employee.username = employee.firstname + "." + employee.surname;
                    }

                    employee.Action = Action.Create;
                }
                else
                {
                    employee.Action = Action.Update;
                }

                employee.ESRPersonId = personRecord.ESRPersonId;
                employee.CreationMethod = EmployeeCreationMethod.EsrOutbound;

                employee.primarycountry = int.Parse(this.GetAccountProperty("homeCountry"));
                employee.primarycurrency = int.Parse(this.GetAccountProperty("baseCurrency"));
                employee.NHSTrustID = this.GetTrustId();
                return employee;
            }

            return new Employee();
        }

        private int? GetEmployeeId(Guid fieldId, EsrAssignment assignment)
        {
            switch (fieldId.ToString())
            {
                case "d68db9d1-0a73-4a76-aae9-f0de5f19f9ff":
                    // Supervisor
                    if (assignment != null)
                    {
                        long? personId = assignment.SafeSupervisorPersonId;

                        if (personId != null)
                        {
                            var employees = this.EsrApiHandler.Execute<Employee>(DataAccessMethod.ReadByEsrId, personId.Value.ToString(CultureInfo.InvariantCulture));
                            if (employees.Count == 1)
                            {
                                return employees[0].employeeid;
                            }
                        }
                    }
                    break;

                case "081837aa-a9ef-4316-bf97-15507b7febfe":
                    // Dept Manager
                    if (assignment != null)
                    {
                        long? assignmentId = assignment.DepartmentManagerAssignmentId;

                        if (assignmentId != null)
                        {
                            var assignments = this.EsrApiHandler.Execute<EsrAssignment>(DataAccessMethod.ReadByAssignmentId, assignmentId.Value.ToString(CultureInfo.InvariantCulture));
                            if (assignments.Count >= 1)
                            {
                                return assignments[0].employeeid;
                            }
                        }
                    }
                    break;
            }
            return null;

        }

        /// <summary>
        /// The is supervisor assignment number mapped.
        /// </summary>
        /// <param name="mapping">
        /// The mapping.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsSupervisorAssignmentNumberMapped(IEnumerable<TemplateMapping> mapping)
        {
            return !mapping.Any(templateMapping => templateMapping.destinationField == "Supervisor Assignment Number" && templateMapping.fieldID == new Guid("ED828976-F992-4ADC-B461-27EE83EBFDC8"));
        }

        /// <summary>
        /// The create update vehicles.
        /// </summary>
        /// <param name="personRecords">
        /// The person Records.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        private List<EsrVehicle> UpdateVehiclesMapping(IEnumerable<EsrPersonRecord> personRecords)
        {
            var mapping = this.EsrApiHandler.TemplateMappingReadSpecial(this.TrustVpd);
            if (mapping != null)
            {
                var currentVehicleList = new List<EsrVehicle>();
                foreach (EsrPersonRecord personRecord in personRecords)
                {
                    if (personRecord.Vehicles != null)
                    {
                        foreach (EsrVehicle vehicle in personRecord.Vehicles)
                        {
                            EsrVehicle currentVehicle = vehicle;
                            var baseAttribute = vehicle.ClassAttribute.ElementType;
                            foreach (TemplateMapping templateMapping in mapping)
                            {
                                foreach (FieldInfo field in vehicle.ClassFields())
                                {
                                    var attribute = field.GetCustomAttribute<DataClassAttribute>();
                                    if (attribute != null && attribute.FieldId != null)
                                    {
                                        if (attribute.FieldId == templateMapping.fieldID.ToString().ToUpper())
                                        {
                                            // found a mapping field
                                            if (baseAttribute == templateMapping.importElementType)
                                            {
                                                if (templateMapping.dataType == 10 || templateMapping.dataType == 12)
                                                {
                                                    string fieldValue;
                                                    currentVehicle =
                                                        (EsrVehicle)
                                                        this.LookupField(
                                                            templateMapping, vehicle, vehicle, out fieldValue, field);
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (currentVehicle != null)
                            {
                                if (currentVehicle.ESRAssignID != null)
                                {
                                    var assignment = this.EsrApiHandler.Execute<EsrAssignment>(currentVehicle.ESRAssignID.ToString(), DataAccessMethod.Read, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
                                    if (assignment == null)
                                    {
                                        currentVehicle.ActionResult.Result = ApiActionResult.ForeignKeyFail;
                                        currentVehicle.ActionResult.Message = this.FormatFailure(
                                            "Esr Vehicle",
                                            currentVehicle.ESRVehicleAllocationId,
                                            string.Format("Lookup for Assignment {0} failed", currentVehicle.ESRAssignID),
                                            currentVehicle.ActionResult.Result);
                                        currentVehicle.ESRAssignID = null;
                                    }
                                }

                                currentVehicleList.Add(currentVehicle);
                            }
                        }
                    }
                }

                return currentVehicleList;
            }

            return new List<EsrVehicle>();
        }

        #endregion

        /// <summary>
        /// The check sub object results.
        /// </summary>
        /// <param name="objectName">
        /// The object Name.
        /// </param>
        /// <param name="currentResult">
        /// The current result.
        /// </param>
        /// <param name="dataClassList">
        /// The data class list.
        /// </param>
        /// <returns>
        /// The <see cref="ApiResult"/>.
        /// </returns>
        private ApiResult CheckSubObjectResults(
            string objectName, ApiResult currentResult, IEnumerable<DataClassBase> dataClassList)
        {
            foreach (DataClassBase classBase in dataClassList)
            {
                if (classBase.ActionResult == null)
                {
                    classBase.ActionResult = new ApiResult();
                }

                if (classBase.ActionResult.Result != ApiActionResult.Success)
                {
                    currentResult.Result = ApiActionResult.PartialSuccess;
                    currentResult.Message += this.FormatFailure(objectName, classBase.KeyValue, classBase.ActionResult.Message, classBase.ActionResult.Result);
                    if (!classBase.ActionResult.LookupUpdateFailure && currentResult.LookupUpdateFailure)
                    {
                        classBase.ActionResult.LookupUpdateFailure = true;
                    }

                    classBase.ActionResult.Message = string.Empty;
                }
            }

            return currentResult;
        }

        /// <summary>
        /// The find person by location id.
        /// </summary>
        /// <param name="persons">
        /// The persons.
        /// </param>
        /// <param name="locationId">
        /// The location id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        private IEnumerable<EsrPersonRecord> FindPersonByLocationId(IEnumerable<EsrPersonRecord> persons, long? locationId)
        {
            var result = new List<EsrPersonRecord>();
            foreach (EsrPersonRecord person in persons.Where(person => person.EsrAssignments != null))
            {
                result.AddRange(from assignment in person.EsrAssignments where assignment.EsrLocationId == locationId select person);
            }

            return result;
        }

        /// <summary>
        /// The get address import format.
        /// </summary>
        /// <param name="propertyKey">
        /// The format Item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetAccountProperty(string propertyKey)
        {
            var returnValue = MemoryCache.Default.Get(string.Format("{0}_{1}", propertyKey, this.TrustVpd)) as string;
            if (returnValue == null)
            {
                var accountProperties = this.EsrApiHandler.Execute<AccountProperty>(DataAccessMethod.ReadAll);
                if (accountProperties != null)
                {
                    foreach (AccountProperty accountProperty in accountProperties)
                    {
                        if (accountProperty.StringKey == propertyKey)
                        {
                            returnValue = accountProperty.StringValue;
                            MemoryCache.Default.Add(
                                string.Format("{0}_{1}", propertyKey, this.TrustVpd),
                                returnValue,
                                new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
                            break;
                        }
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// For a given ESR person occurrence, create a new ESRPERSON and insert the assignment, phone, vehicle etc..
        /// </summary>
        /// <param name="esrPerson">
        /// The ESR person.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        private EsrPersonRecord GetExtraPersonData(EsrPerson esrPerson)
        {
            var result = DataClassBase.GetDataClassFromRecord(esrPerson, typeof(EsrPersonRecord)) as EsrPersonRecord;
            if (result != null)
            {
                result.EsrAssignments = this.EsrApiHandler.Execute<EsrAssignment>(DataAccessMethod.ReadByEsrId, esrPerson.ESRPersonId.ToString(CultureInfo.InvariantCulture), null, DataAccessMethodReturnDefault.NewObjectWithFailureFlag);
                result.Addresses = this.EsrApiHandler.Execute<EsrAddress>(DataAccessMethod.ReadByEsrId, esrPerson.ESRPersonId.ToString(CultureInfo.InvariantCulture));
                result.Phones = this.EsrApiHandler.Execute<EsrPhone>(DataAccessMethod.ReadByEsrId, esrPerson.ESRPersonId.ToString(CultureInfo.InvariantCulture));
                result.Vehicles = this.EsrApiHandler.Execute<EsrVehicle>(DataAccessMethod.ReadByEsrId, esrPerson.ESRPersonId.ToString(CultureInfo.InvariantCulture));
                var assignmentCosting = this.EsrApiHandler.Execute<EsrAssignmentCostings>(esrPerson.ESRPersonId.ToString(CultureInfo.InvariantCulture), DataAccessMethod.Read);
                result.AssignmentCostings = assignmentCosting == null ? new List<EsrAssignmentCostings>() : new List<EsrAssignmentCostings> { assignmentCosting };
            }

            return result;
        }

        /// <summary>
        /// Find car by registration from list provided.
        /// </summary>
        /// <param name="registration">
        /// The registration.
        /// </param>
        /// <param name="cars">
        /// The cars.
        /// </param>
        /// <param name="carAssignments">
        /// The car Assignments.
        /// </param>
        /// <param name="vehicleAllocationId">
        /// The vehicle Allocation Id.
        /// </param>
        /// <returns>
        /// The <see cref="Car"/>.
        /// </returns>
        private Car FindCarByRegistration(string registration, IEnumerable<Car> cars)
        {
            return cars.FirstOrDefault(car => car.registration == registration) ?? new Car();
        }

        /// <summary>
        /// Find the current car assignment.
        /// </summary>
        /// <param name="carid">
        /// The car id.
        /// </param>
        /// <param name="carAssignments">
        /// The car assignments.
        /// </param>
        /// <returns>
        /// The <see cref="CarAssignmentNumberAllocation"/>.
        /// </returns>
        private List<CarAssignmentNumberAllocation> FindCarAssignment(int carid, IEnumerable<CarAssignmentNumberAllocation> carAssignments)
        {
            return carAssignments.Where(allocation => allocation.CarId == carid).ToList();
        }

        /// <summary>
        /// The log results from updates.
        /// </summary>
        /// <param name="entities">
        /// The entities.
        /// </param>
        private void LogResults(IEnumerable<DataClassBase> entities)
        {
            foreach (DataClassBase entity in entities)
            {
                this.Logger.WriteDebug(
                    this.MetaBase,
                    "0",
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("Update {0} result = {1} -- Key Value {2}", entity.ClassName(), entity.ActionResult.Result, entity.KeyValue),
                    "EsrPersonsCrud");
            }
        }

        /// <summary>
        /// Update duplicate users.
        /// </summary>
        /// <param name="employee">
        /// The employee.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        private Employee UpdateDuplicateUser(Employee employee)
        {
            Employee result = null;
            if (employee.employeeid == -1)
            {
                employee.username = string.Format("{0}.{1}", employee.username, employee.ESRPersonId);
                employee.employeeid = 0;
                employee.ActionResult = new ApiResult();
                employee.Action = Action.Update;
                result = this.EsrApiHandler.Execute(DataAccessMethod.Update, null, new List<Employee> { employee }).FirstOrDefault();
                if (result != null && result.Action == Action.Update)
                {
                    this.Logger.WriteExtra(
                        this.MetaBase,
                        "0",
                        this.AccountId,
                        LogRecord.LogItemTypes.None,
                        LogRecord.TransferTypes.None,
                        0,
                        string.Empty,
                        LogRecord.LogReasonType.None,
                        string.Format(
                            "Update duplicate Employee {0} employeeid {1}", employee.username, employee.employeeid),
                        "EsrPersonsCrud");

                    var defaultItemRole = int.Parse(this.GetAccountProperty("defaultItemRole"));
                    if (defaultItemRole > 0)
                    {
                        var employeeRole = new EmployeeRole
                        {
                            employeeid = result.employeeid,
                            itemroleid = defaultItemRole,
                            order = 0,
                            Action = Action.Create
                        };
                        this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, new List<EmployeeRole> { employeeRole });
                    }

                    var defaultRole = int.Parse(this.GetAccountProperty("defaultRole"));
                    if (defaultRole > 0)
                    {
                        var employeeAccessRole = new EmployeeAccessRole
                        {
                            employeeID = result.employeeid,
                            accessRoleID = defaultRole,
                            subAccountID = 1, // TODO: Only one subaccount?
                            Action = Action.Create
                        };
                        this.EsrApiHandler.Execute(DataAccessMethod.Create, string.Empty, new List<EmployeeAccessRole> { employeeAccessRole });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get trust id from cache.
        /// </summary>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private int? GetTrustId()
        {
            var trustApi = new EsrTrustCrud(this.MetaBase, this.AccountId, this.EsrApiHandler, this.Logger);
            var result = trustApi.ReadSpecial(this.TrustVpd);

            return result.Count > 0 ? result[0].trustID : (int?)null;
        }

        private List<UserDefinedField> GetUdf(int accountId)
        {
            List<UserDefinedField> result;
            if (MemoryCache.Default.Contains(string.Format("UDF_{0}", accountId)))
            {
                result = MemoryCache.Default.Get(string.Format("UDF_{0}", accountId)) as List<UserDefinedField>;
            }
            else
            {
                var udfCrud = new UdfCrud(this.MetaBase, accountId);
                result = udfCrud.ReadAll();

                var udfMatchCrud = new UdfMatchCrud(this.MetaBase, this.AccountId);
                foreach (UserDefinedField userDefinedField in result)
                {
                    userDefinedField.UserDefinedMatchFields = udfMatchCrud.ReadSpecial(userDefinedField.userdefineid.ToString(CultureInfo.InvariantCulture));
                }

                MemoryCache.Default.Add(
                        string.Format("UDF_{0}", accountId),
                        result,
                        new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
            }

            return result;
        }
        #endregion
    }
}