namespace ApiRpc.Classes.Code
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Caching;
    using System.Runtime.Serialization;

    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// The ESR persons helper class.  Contains methods to split ESR person records into ESR person, assignment etc.
    /// </summary>
    public class EsrPersonsCrud : EntityBase, IDataAccess<EsrPersonRecord>
    {
        #region Public Methods

        /// <summary>
        /// Initialises a new instance of the <see cref="EsrPersonsCrud"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="apiCollection">
        /// The API Collection.
        /// </param>
        public EsrPersonsCrud(string baseUrl, string metabase, int accountid, ApiCrudList apiCollection)
            : base(baseUrl, metabase, accountid, apiCollection)
        {
            // newApi was : new ApiCrudBase<EsrPerson>(baseUrl, metabase, accountid);
            if (this.DataSources.EmployeeApi == null)
            {
                this.DataSources.EmployeeApi = new EmployeeApi(metabase, accountid);
            }

            if (this.DataSources.EsrPersonApi == null)
            {
                this.DataSources.EsrPersonApi = new PersonApi(metabase, accountid);
            }

            if (this.DataSources.EsrAssignmentApi == null)
            {
                this.DataSources.EsrAssignmentApi = new AssignmentApi(metabase, accountid);
            }

            if (this.DataSources.EsrAddressApi == null)
            {
                this.DataSources.EsrAddressApi = new EsrAddressApi(metabase, accountid);
            }

            if (this.DataSources.EsrPhoneApi == null)
            {
                this.DataSources.EsrPhoneApi = new PhoneApi(metabase, accountid);
            }

            if (this.DataSources.EsrVehicleApi == null)
            {
                this.DataSources.EsrVehicleApi = new VehicleApi(metabase, accountid);
            }

            if (this.DataSources.UdfApi == null)
            {
                this.DataSources.UdfApi = new UdfApi(metabase, accountid);
            }

            if (this.DataSources.AccountPropertyApi == null)
            {
                this.DataSources.AccountPropertyApi = new AccountPropertyApi(metabase, accountid);
            }

            if (this.DataSources.EsrAssignmentCostingsApi == null)
            {
                this.DataSources.EsrAssignmentCostingsApi = new AssignmentCostingsApi(metabase, accountid);
            }

            if (this.DataSources.CarApi == null)
            {
                this.DataSources.CarApi = new CarApi(metabase, accountid);
            }

            if (this.DataSources.AddressApi == null)
            {
                this.DataSources.AddressApi = new AddressApi(metabase, accountid);
            }

            if (this.DataSources.EmployeeHomeAddressApi == null)
            {
                this.DataSources.EmployeeHomeAddressApi = new EmployeeHomeAddressApi(metabase, accountid);
            }

            if (this.DataSources.EmployeeWorkAddressApi == null)
            {
                this.DataSources.EmployeeWorkAddressApi = new EmployeeWorkAddressApi(metabase, accountid);
            }

            if (this.DataSources.EsrLocationApi == null)
            {
                this.DataSources.EsrLocationApi = new LocationApi(metabase, accountid);
            }

            if (this.DataSources.EsrOrganisationApi == null)
            {
                this.DataSources.EsrOrganisationApi = new EsrOrganisationApi(metabase, accountid);
            }

            if (this.DataSources.CostCodeApi == null)
            {
                this.DataSources.CostCodeApi = new CostCodeApi(metabase, accountid);
            }

            if (this.DataSources.EmployeeCostCodeApi == null)
            {
                this.DataSources.EmployeeCostCodeApi = new EmployeeCostCodeApi(metabase, accountid);
            }

            if (this.DataSources.CarAssignmentNumberApi == null)
            {
                this.DataSources.CarAssignmentNumberApi = new CarAssignmentNumberApi(metabase, accountid);
            }

            if (this.DataSources.EmployeeAccessRoleApi == null)
            {
                this.DataSources.EmployeeAccessRoleApi = new EmployeeAccessRoleApi(metabase, accountid);
            }

            if (this.DataSources.EmployeeRoleApi == null)
            {
                this.DataSources.EmployeeRoleApi = new EmployeeRoleApi(metabase, accountid);
            }

            if (this.DataSources.TemplateApi == null)
            {
                this.DataSources.TemplateApi = new TemplateApi(metabase, accountid);
            }

            if (this.DataSources.TemplateMappingApi == null)
            {
                this.DataSources.TemplateMappingApi = new TemplateMappingApi(metabase, accountid);
            }
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

            Logger.WriteExtra(
                this.MetaBase,
                0,
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
            if (this.DataSources.EsrPersonApi != null)
            {
                persons = this.DataSources.EsrPersonApi.Create(persons);
                Logger.WriteExtra(
                    this.MetaBase,
                    0,
                    this.AccountId,
                    LogRecord.LogItemTypes.None,
                    LogRecord.TransferTypes.None,
                    0,
                    string.Empty,
                    LogRecord.LogReasonType.None,
                    string.Format("Create Persons Time {0}", DateTime.Now - subMethodStart),
                    "EsrPersonsCrud");
            }

            var employees = new List<Employee>();
            for (int i = 0; i < persons.Count; i++)
            {
                var originalPerson = entities[i];
                var updatedPerson =
                    DataClassBase.GetRecordFromDataClass(persons[i], typeof(EsrPersonRecord)) as EsrPersonRecord;
                if (updatedPerson != null && updatedPerson.ActionResult.Result == ApiActionResult.Success)
                {
                    var currentEmployees = this.DataSources.EmployeeApi.ReadByEsrId(updatedPerson.ESRPersonId);
                    updatedPerson.EsrAssignments = originalPerson.EsrAssignments;
                    if (this.ImportElementType(TemplateMapping.ImportElementType.Employee, this.TrustVpd))
                    {
                        var newEmployee = this.UpdateEmployeeMapping(
                            updatedPerson, currentEmployees.Count == 0 ? null : currentEmployees[0]);
                        employees.Add(newEmployee);
                    }
                }
            }

            subMethodStart = DateTime.Now;
            List<Employee> newEmployees = this.DataSources.EmployeeApi.Create(employees);

            foreach (Employee newEmployee in newEmployees)
            {
                int originalEmployeeIdx = (from x in employees where x.ESRPersonId == newEmployee.ESRPersonId select employees.IndexOf(x)).DefaultIfEmpty(-1).FirstOrDefault();

                if (originalEmployeeIdx != -1)
                {
                    employees[originalEmployeeIdx].employeeid = newEmployee.employeeid;
                }

                if (newEmployee.employeeid != -1)
                {
                    if (this.DataSources.EmployeeRoleApi.Read(newEmployee.employeeid) == null)
                    {
                        this.DataSources.EmployeeRoleApi.Create(new List<EmployeeRole>
                        {
                            new EmployeeRole
                            {
                                employeeid = newEmployee.employeeid,
                                itemroleid = int.Parse(this.GetImportFormat("defaultItemRole")),
                                order = 0,
                                Action = Action.Create
                            }
                        });
                    }

                    if (this.DataSources.EmployeeAccessRoleApi.Read(newEmployee.employeeid) == null)
                    {
                        this.DataSources.EmployeeAccessRoleApi.Create(new List<EmployeeAccessRole>
                        {
                            new EmployeeAccessRole
                            {
                                employeeID = newEmployee.employeeid,
                                accessRoleID = int.Parse(this.GetImportFormat("defaultRole")),
                                //subAccountID is populated automatically in the APIsaveEmployeeAccessRole stored procedure
                                Action = Action.Create
                            }
                        });
                    }
                }
            }

            Logger.WriteExtra(
                this.MetaBase,
                0,
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create {0} Employees Elapsed Time {1}", employees.Count, DateTime.Now - subMethodStart),
                "EsrPersonsCrud");

            for (int i = 0; i < persons.Count; i++)
            {
                var originalPerson = entities[i];
                var updatedPerson =
                    DataClassBase.GetRecordFromDataClass(persons[i], typeof(EsrPersonRecord)) as EsrPersonRecord;
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
                            var existingUser =
                                this.DataSources.EmployeeApi.ReadSpecial(
                                    string.Format("username/{0}", newEmployee.username));
                            if (existingUser != null && existingUser.Count > 0)
                            {
                                existingUser[0].ESRPersonId = originalPerson.ESRPersonId;
                                newEmployee.employeeid = existingUser[0].employeeid;
                                existingUser[0].Action = Action.Update;
                                this.DataSources.EmployeeApi.Update(existingUser);
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
                            foreach (UserDefinedField udf in newEmployee.UserDefinedFields.Fields)
                            {
                                if (udf.Value != null)
                                {
                                    udf.recordId = newEmployee.employeeid;
                                    udf.Action = Action.Update;
                                    this.UpdateUserDefinedFieldValue(udf);
                                }
                            }
                        }

                        updatedPerson.EmployeeId = newEmployee.employeeid;
                        updatedPerson.ActionResult = this.CheckSubObjectResults(
                            "Employee", updatedPerson.ActionResult, new List<DataClassBase> { newEmployee });
                        newEmployee.ActionResult.Message = string.Empty;
                    }
                    else
                    {
                        Logger.WriteDebug(
                            this.MetaBase,
                            0,
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

                        updatedPerson.EsrAssignments = this.UpdateAssignmentMapping(originalPerson);
                        if (this.ImportElementType(TemplateMapping.ImportElementType.Location, this.TrustVpd))
                        {
                            allAddresses.AddRange(this.UpdateWorkAddressMapping(updatedPerson));
                        }
                    }
                    else
                    {
                        Logger.WriteDebug(
                            this.MetaBase,
                            0,
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
                        Logger.WriteDebug(
                            this.MetaBase,
                            0,
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
                        updatedPerson.AssignmentCostings = this.CreateUpdateCostCodes(originalPerson);
                    }
                    else
                    {
                        Logger.WriteDebug(
                            this.MetaBase,
                            0,
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
            var assignmentResult = Mapper<EsrAssignment>.SubmitList(allAssignments, this.DataSources.EsrAssignmentApi);
            this.LogResults(Mapper<EsrAssignment>.EsrToDataClass(assignmentResult));
            Logger.WriteExtra(
                this.MetaBase,
                0,
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create Assignments Elapsed Time {0}", DateTime.Now - subMethodStart),
                "EsrPersonsCrud");

            var workAddresses = new List<EmployeeWorkAddress>();
            List<EmployeeWorkAddress> allExistingWorkAddresses = this.DataSources.EmployeeWorkAddressApi.ReadAll();
            
            List<Address> uniqueAddresses = UniqueAddresses(allAddresses);

            foreach (Address address in uniqueAddresses)
            {
                var person = this.FindPersonByLocationId(result, address.EsrLocationID);
                foreach (EsrPersonRecord personRecord in person)
                {
                    if (personRecord != null && address.AddressID > 0 && personRecord.EmployeeId > 0)
                    {
                        var assignments = personRecord.EsrAssignments.Where(x => x.EsrLocationId == address.EsrLocationID);

                        foreach (EsrAssignment assignment in assignments)
                        {
                            EmployeeWorkAddress assignmentWorkAddress = allExistingWorkAddresses.FirstOrDefault(
                                x => x.EmployeeId == personRecord.EmployeeId &&
                                        x.AddressId == address.AddressID &&
                                        (assignment == null || x.StartDate == assignment.EffectiveStartDate));
                            if (assignmentWorkAddress == null)
                            {
                                workAddresses.Add(
                                    new EmployeeWorkAddress
                                    {
                                        EmployeeWorkAddressId = 0,
                                        Action = Action.Create,
                                        EmployeeId = personRecord.EmployeeId,
                                        AddressId = address.AddressID,
                                        StartDate = assignment != null ? assignment.EffectiveStartDate : DateTime.Now,
                                        EndDate = assignment != null ? assignment.EffectiveEndDate : null,
                                        CreatedBy = personRecord.EmployeeId,
                                        Active = true,
                                        CreatedOn = DateTime.Now
                                    }
                                );
                            }
                            else
                            {
                                assignmentWorkAddress.Action = Action.Update;
                                assignmentWorkAddress.EndDate = assignment != null ? assignment.EffectiveEndDate : null;
                                assignmentWorkAddress.ModifiedBy = personRecord.EmployeeId;
                                assignmentWorkAddress.ModifiedOn = DateTime.Now;
                                workAddresses.Add(assignmentWorkAddress);
                            }
                        }
                    }
                }
            }

            if (this.ImportElementType(TemplateMapping.ImportElementType.Location, this.TrustVpd))
            {
                var workResult = Mapper<EmployeeWorkAddress>.SubmitList(
                    workAddresses, this.DataSources.EmployeeWorkAddressApi);
                this.LogResults(Mapper<EmployeeWorkAddress>.EsrToDataClass(workResult));
            }

            var phoneResult = new List<EsrPhone>();
            if (this.ImportElementType(TemplateMapping.ImportElementType.Phone, this.TrustVpd))
            {
                subMethodStart = DateTime.Now;
                phoneResult = Mapper<EsrPhone>.SubmitList(allPhones, this.DataSources.EsrPhoneApi);
                this.LogResults(Mapper<EsrPhone>.EsrToDataClass(phoneResult));
                Logger.WriteExtra(
                    this.MetaBase,
                    0,
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
                addressResult = Mapper<EsrAddress>.SubmitList(allEsrAddresses, this.DataSources.EsrAddressApi);
                this.LogResults(Mapper<EsrAddress>.EsrToDataClass(addressResult));
                Logger.WriteExtra(
                    this.MetaBase,
                    0,
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
                vehicleResult = Mapper<EsrVehicle>.SubmitList(allVehicles, this.DataSources.EsrVehicleApi);
                this.LogResults(Mapper<EsrVehicle>.EsrToDataClass(vehicleResult));
                Logger.WriteExtra(
                    this.MetaBase,
                    0,
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
            var assignmentCostingResult = Mapper<EsrAssignmentCostings>.SubmitList(
                allAssignmentCostings, this.DataSources.EsrAssignmentCostingsApi);
            this.LogResults(Mapper<EsrAssignmentCostings>.EsrToDataClass(assignmentCostingResult));
            Logger.WriteExtra(
                this.MetaBase,
                0,
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
                    var currentAssignments =
                        assignmentResult.Where(assignment => assignment.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.EsrAssignments = currentAssignments.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults(
                        "Assignment", currentPerson.ActionResult, currentPerson.EsrAssignments);
                    foreach (EsrAssignment assignment in currentPerson.EsrAssignments)
                    {
                        assignment.ActionResult.Message = string.Empty;
                    }
                }

                if (phoneResult != null)
                {
                    var currentPhones = phoneResult.Where(phone => phone.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.Phones = currentPhones.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults(
                        "Phone", currentPerson.ActionResult, currentPerson.Phones);
                    foreach (EsrPhone phone in currentPerson.Phones)
                    {
                        phone.ActionResult.Message = string.Empty;
                    }
                }

                if (addressResult != null)
                {
                    var currentAddress = addressResult.Where(
                        address => address.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.Addresses = currentAddress.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults(
                        "Address", currentPerson.ActionResult, currentPerson.Addresses);
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
                    var currentVehicle = vehicleResult.Where(
                        vehicle => vehicle.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.Vehicles = currentVehicle.ToList();
                    var carresult = this.CreateUpdateCars(currentPerson);
                    currentPerson.ActionResult = this.CheckSubObjectResults(
                        "Vehicle", currentPerson.ActionResult, carresult);
                    foreach (EsrVehicle vehicle in currentPerson.Vehicles)
                    {
                        vehicle.ActionResult.Message = string.Empty;
                    }
                }

                if (assignmentCostingResult != null)
                {
                    var currentCostings =
                        assignmentCostingResult.Where(costing => costing.ESRPersonId == currentPerson.ESRPersonId);
                    currentPerson.AssignmentCostings = currentCostings.ToList();
                    currentPerson.ActionResult = this.CheckSubObjectResults(
                        "AssignmentCosting", currentPerson.ActionResult, currentPerson.AssignmentCostings);
                    foreach (EsrAssignmentCostings costingRecord in currentPerson.AssignmentCostings)
                    {
                        costingRecord.ActionResult.Message = string.Empty;
                    }
                    if (this.ImportElementType(TemplateMapping.ImportElementType.Costing, this.TrustVpd))
                    {
                        this.CreateUpdateCostCodes(currentPerson);
                    }
                }
            }

            Logger.WriteExtra(
                this.MetaBase,
                0,
                this.AccountId,
                LogRecord.LogItemTypes.None,
                LogRecord.TransferTypes.None,
                0,
                string.Empty,
                LogRecord.LogReasonType.None,
                string.Format("Create Mappings Elapsed Time {0}", DateTime.Now - subMethodStart),
                "EsrPersonsCrud");

            Logger.WriteExtra(
                this.MetaBase,
                0,
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

        private static List<Address> UniqueAddresses(List<Address> allAddresses)
        {
            var addressIDs = new Dictionary<int, long>();
            var uniqueAddresses = new List<Address>();
            foreach (Address address in allAddresses)
            {
                if (address.AddressID == 0 || !addressIDs.ContainsKey(address.AddressID))
                {
                    uniqueAddresses.Add(address);
                    addressIDs.Add(address.AddressID, 1);
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
            var esrPerson = this.DataSources.EsrPersonApi.Read(entityId);
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
            var esrPerson = this.DataSources.EsrPersonApi.Read(entityId);
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
            var allPersons = this.DataSources.EsrPersonApi.ReadAll();
            return allPersons.Select(esrPerson => this.GetExtraPersonData(esrPerson)).ToList();
        }

        /// <summary>
        /// The read by person id.
        /// </summary>
        /// <param name="personId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPersonRecord> ReadByEsrId(long personId)
        {
            var esrPersons = this.DataSources.EsrPersonApi.ReadByEsrId(personId);
            return esrPersons.Select(esrPerson => this.GetExtraPersonData(esrPerson)).ToList();
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
            var result = this.GetExtraPersonData(this.DataSources.EsrPersonApi.Read(entityId));
            var employeeToDelete = this.DataSources.EmployeeApi.ReadByEsrId(entityId);
            if (employeeToDelete != null && employeeToDelete.Count == 1)
            {
                employeeToDelete[0].archived = true;
                employeeToDelete[0].Action = Action.Update;
                var employeeResult = this.DataSources.EmployeeApi.Update(employeeToDelete);
                if (employeeResult[0].ActionResult.Result == ApiActionResult.Success)
                {
                    result.ActionResult = employeeResult[0].ActionResult;
                    if (result.Vehicles != null)
                    {
                        foreach (EsrVehicle vehicle in result.Vehicles)
                        {
                            var currentCar = this.DataSources.CarApi.ReadByEsrId(employeeResult[0].employeeid);
                            foreach (Car car in currentCar)
                            {
                                car.active = false;
                                car.Action = Action.Update;
                                this.DataSources.CarAssignmentNumberApi.Delete(vehicle.ESRVehicleAllocationId);
                            }

                            this.DataSources.CarApi.Update(currentCar);
                            var vehicleResult = this.DataSources.EsrVehicleApi.Delete(vehicle.KeyValue);
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
                                    this.DataSources.EsrAssignmentCostingsApi.Delete(assignmentCosting.KeyValue);
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
                            var assignmentResult = this.DataSources.EsrAssignmentApi.Delete(assignment.KeyValue);
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
                                var currentAddress = this.DataSources.AddressApi.ReadByEsrId(esrAddress.KeyValue);
                                if (currentAddress != null)
                                {
                                    foreach (Address address in currentAddress)
                                    {
                                        address.Archived = true;
                                        address.EsrAddressID = null;
                                        address.Action = Action.Update;
                                    }
                                }

                                this.DataSources.AddressApi.Update(currentAddress);
                            }

                            if (esrAddress != null)
                            {
                                var addressResult = this.DataSources.EsrAddressApi.Delete(esrAddress.KeyValue);
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
                                var phoneResult = this.DataSources.EsrPhoneApi.Delete(phone.KeyValue);
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

                    var personresult = this.DataSources.EsrPersonApi.Delete(entityId);
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
                    Lookup lookupResult = this.ApiCrudLookupValue.Read(
                        "BF9AA39A-82D6-4960-BFEF-C5943BC0542D",
                        "7C0D8EAB-D9AF-415F-9BB7-D1BE01F69E2F",
                        costing.ESRAssignmentId.ToString());
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
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);
            if (mapping != null)
            {
                var newCars = new List<Car>();
                var currentVehicleList = new List<EsrVehicle>();
                foreach (EsrVehicle vehicle in personRecord.Vehicles)
                {
                    var currentCarList = this.DataSources.CarApi.ReadByEsrId(personRecord.EmployeeId);
                    var currentCarAssignments = new List<CarAssignmentNumberAllocation>();
                    foreach (Car car in currentCarList)
                    {
                        currentCarAssignments.AddRange(this.DataSources.CarAssignmentNumberApi.ReadByEsrId(car.carid));
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

                    if (personRecord.EmployeeId > 0 && vehicle.ESRAssignID > 0)
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

                        currentCar.Action = Action.Create;
                        currentCar.employeeid = personRecord.EmployeeId;
                        var lookupFailed = false;

                        var lookupResult = Mapper<Car>.StaticFieldMapping(vehicle.FuelType, "cartypeid");
                        if (!string.IsNullOrEmpty(lookupResult))
                        {
                            currentCar.cartypeid = byte.Parse(lookupResult);
                        }
                        else
                        {
                            lookupFailed = true;
                        }

                        var carResult = this.DataSources.CarApi.Create(new List<Car> { currentCar });
                        if (lookupFailed)
                        {
                            currentCar.ActionResult.Message = string.Format("Could not find Mapping for Fuel Type '{0}' on vehicle {1}", vehicle.FuelType, vehicle.ESRVehicleAllocationId);
                            currentCar.ActionResult.Result = ApiActionResult.PartialSuccess;
                        }

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

                this.DataSources.CarAssignmentNumberApi.Create(allAllocations);
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
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);
            if (mapping != null)
            {
                var addressVal = this.GetImportFormat("ImportHomeAddressFormat");
                var addresses = new List<Address>();

                foreach (EsrAddress address in updatedPerson.Addresses)
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
                                                    this.ApiCrudLookupValue.Read(
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
                    //if (string.IsNullOrEmpty(addressVal))
                    //{
                    //    company.company = string.Format("{0}{1}", company.address1, company.postcode);
                    //}
                    //else
                    //{
                    //    company.company = addressVal;
                    //}

                    newAddress.CreationMethod = AddressCreationMethod.EsrOutbound;
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

                var addressResult = this.DataSources.AddressApi.Create(addresses);

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

                this.DataSources.EmployeeHomeAddressApi.Create(homeAddressList);
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
            this.DataSources.UdfApi.Create(new List<UserDefinedField> { udf });
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
        internal List<EsrAssignment> UpdateAssignmentMapping(EsrPersonRecord esrPerson)
        {
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var udfList = this.GetUdf(this.AccountId);
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);
            if (mapping != null && esrPerson.EsrAssignments != null)
            {
                var newResult = esrPerson.EsrAssignments.Count > 0 ? esrPerson.EsrAssignments[0].ActionResult : new ApiResult();
                var assignments = new List<EsrAssignment>();
                var employee = this.DataSources.EmployeeApi.ReadByEsrId(esrPerson.ESRPersonId).FirstOrDefault();
                Template template = null;

                foreach (EsrAssignment assignment in esrPerson.EsrAssignments)
                {
                    EsrAssignment existingAssignment = null;
                    if (assignment.ESRPersonId != null)
                    {
                        var currentAssignment = this.DataSources.EsrAssignmentApi.ReadByEsrId((long)assignment.ESRPersonId);
                        if (currentAssignment != null && currentAssignment.Count > 0)
                        {
                            foreach (EsrAssignment esrAssignment in
                                currentAssignment.Where(esrAssignment => assignment != null && esrAssignment.AssignmentID == assignment.AssignmentID && esrAssignment.EffectiveStartDate == assignment.EffectiveStartDate))
                            {
                                existingAssignment = assignment;
                                existingAssignment.esrAssignID = esrAssignment.esrAssignID;
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
                                template = this.DataSources.TemplateApi.Read(templateMapping.templateID);
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
                            if (dataAttribute == null || !dataAttribute.IsRequired)
                            {
                                field.SetValue(assignment, null);    
                    }
                        }
                    }

                    if (existingAssignment != null && existingAssignment.SupervisorAssignmentId != null)
                    {
                        var result = this.ApiCrudLookupValue.Read("BF9AA39A-82D6-4960-BFEF-C5943BC0542D", "7C0D8EAB-D9AF-415F-9BB7-D1BE01F69E2F", existingAssignment.SupervisorAssignmentId.ToString());

                        if (result != null && result.FirstColumnValue != null)
                        {
                            existingAssignment.SupervisorEsrAssignID = (int?)result.FirstColumnValue;
                        }
                    }

                    var theAssignment = (existingAssignment ?? assignment);

                    if (template != null && theAssignment.SignOffOwner == null)
                    {
                        // set the Sign-Off Owner to the configured column (e.g. supervisorEmployeeId, departmentManagerEmployeeId)
                        var signOffOwnerEmployeeId = this.GetEmployeeId(template.SignOffOwnerFieldId, theAssignment);
                        theAssignment.SignOffOwner = (signOffOwnerEmployeeId == null ? null : "25," + signOffOwnerEmployeeId); // 25 = Employee
                    }

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
                                    this.DataSources.EmployeeApi.Update(new List<Employee> { employee });
                                    break;
                                }
                            }

                            employee.GetUserDefined(this.AccountId, udfList);
                            foreach (UserDefinedField udf in employee.UserDefinedFields.Fields)
                            {
                                bool udfValueSet = false;
                                UserDefinedField newUdf = null;

                                if (udf.fieldid == templateMapping.fieldID)
                                {
                                    switch (templateMapping.importElementType)
                                    {
                                        case TemplateMapping.ImportElementType.Assignment:
                                            newUdf = Mapper<Employee>.SetUdf(templateMapping, primaryAssignment, employee, this.ApiCrudLookupValue);
                                            udfValueSet = true;
                                            break;
                                    }
                                }

                                if (udfValueSet)
                                {
                                    newUdf.Action = Action.Update;
                                    newUdf.recordId = employee.employeeid;
                                    var udfResult = this.DataSources.UdfApi.Update(new List<UserDefinedField> { newUdf });
                                    newResult = this.CheckSubObjectResults(
                                        "UserDefinedField", newResult, udfResult);
                                    if (newResult.Result == ApiActionResult.PartialSuccess)
                                    {
                                        newResult.Result = ApiActionResult.ForeignKeyFail;
                                    }

                                    break;
                                }
                            }
                        }
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
            var addressVal = this.GetImportFormat("ImportWorkAddressFormat");

            var addresses = new List<Address>();
            foreach (EsrAssignment assignment in person.EsrAssignments)
            {
                EsrLocation location = null;
                if (assignment.EsrLocationId != null)
                {
                    location = this.DataSources.EsrLocationApi.Read((long)assignment.EsrLocationId);
                }
                else
                {
                    if (assignment.EsrOrganisationId != null)
                    {
                        var organisation = this.DataSources.EsrOrganisationApi.Read((long)assignment.EsrOrganisationId);
                        if (organisation != null && organisation.ESRLocationId != null)
                        {
                            location = this.DataSources.EsrLocationApi.Read((long)organisation.ESRLocationId);
                        }
                    }
                }

                if (location != null)
                {
                    var currentAddress = this.DataSources.AddressApi.ReadByEsrId(location.ESRLocationId);
                    if (currentAddress.Count == 0)
                    {
                        var targetValue = string.Format("'{0}'", "GB");
                        Lookup result = this.ApiCrudLookupValue.Read(
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

                        //company.company = string.IsNullOrEmpty(addressVal)
                        //                      ? string.Format("{0}{1}", location.AddressLine1, location.Postcode)
                        //                      : addressVal;
                        addresses.AddRange(this.DataSources.AddressApi.Create(new List<Address> { address }));
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
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);
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
                    0,
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
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);
            var usernameVal = this.GetImportFormat("importUsernameFormat");
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
                        template = this.DataSources.TemplateApi.Read(templateMapping.templateID);
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
                                                    DataClassBase.GetRecordFromDataClass(
                                                        this.DataSources.EsrPersonApi.Read(personRecord.ESRPersonId),
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
                                                    var supervisorEmployee = this.DataSources.EmployeeApi.ReadByEsrId((long)primaryAssignment.SupervisorPersonId);
                                                    if (supervisorEmployee.Count == 1)
                                                    {
                                                        employee.linemanager = supervisorEmployee[0].employeeid;
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
                                    newUdf = Mapper<Employee>.SetUdf(templateMapping, personRecord, employee, this.ApiCrudLookupValue);
                                    udfValueSet = true;
                                    break;
                                case TemplateMapping.ImportElementType.Assignment:
                                    var primaryAssignment = personRecord.PrimaryAssignment;
                                    if (primaryAssignment != null)
                                    {
                                        newUdf = Mapper<Employee>.SetUdf(templateMapping, primaryAssignment, employee, this.ApiCrudLookupValue);
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

                if (employee.linemanager == null && template != null && personRecord.PrimaryAssignment != null)
                {
                    // set the Line Manager to the configured column (e.g. supervisorEmployeeId, departmentManagerEmployeeId)
                    employee.linemanager = this.GetEmployeeId(template.LineManagerFieldId, personRecord.PrimaryAssignment);
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

                employee.primarycountry = int.Parse(this.GetImportFormat("homeCountry"));
                employee.primarycurrency = int.Parse(this.GetImportFormat("baseCurrency"));
                employee.NHSTrustID = this.GetTrustID();
                return employee;
            }

            return new Employee();
        }

        private int? GetEmployeeId(Guid fieldId, EsrAssignment assignment)
        {
            long? personId = null;

            switch (fieldId.ToString())
            {
                case "d68db9d1-0a73-4a76-aae9-f0de5f19f9ff":
                    // Supervisor
                    if (assignment != null && assignment.SafeSupervisorPersonId != null)
                    {
                        personId = assignment.SupervisorPersonId;
                    }
                    break;

                case "081837aa-a9ef-4316-bf97-15507b7febfe":
                    // Dept Manager
                    if (assignment != null)
                    {
                        personId = assignment.DepartmentManagerPersonId;
                    }
                    break;
            }

            if (personId != null)
            {
                var employees = this.DataSources.EmployeeApi.ReadByEsrId(personId.Value);
                if (employees.Count == 1)
                {
                    return employees[0].employeeid;
                }
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
            var templateMappingApi = this.DataSources.TemplateMappingApi;
            var mapping = templateMappingApi.ReadSpecial(this.TrustVpd);
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
                                    var assignment = this.DataSources.EsrAssignmentApi.Read((int)currentVehicle.ESRAssignID);
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
            foreach (EsrPersonRecord person in persons)
            {
                if (person.EsrAssignments != null)
                {
                    foreach (EsrAssignment assignment in person.EsrAssignments)
                    {
                        if (assignment.EsrLocationId == locationId)
                        {
                            result.Add(person);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get address import format.
        /// </summary>
        /// <param name="formatItem">
        /// The format Item.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetImportFormat(string formatItem)
        {
            var addressVal = MemoryCache.Default.Get(string.Format("{0}_{1}", formatItem, this.TrustVpd)) as string;
            if (addressVal == null)
            {
                var accountProperties = this.DataSources.AccountPropertyApi.ReadAll();
                if (accountProperties != null)
                {
                    foreach (AccountProperty accountProperty in accountProperties)
                    {
                        if (accountProperty.stringKey == formatItem)
                        {
                            addressVal = accountProperty.StringValue;
                            MemoryCache.Default.Add(
                                string.Format("{0}_{1}", formatItem, this.TrustVpd),
                                addressVal,
                                new CacheItemPolicy { SlidingExpiration = TimeSpan.FromMinutes(EsrFile.CacheExpiryMinutes) });
                        }
                    }
                }
            }

            return addressVal;
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
                result.EsrAssignments = this.DataSources.EsrAssignmentApi.ReadByEsrId(esrPerson.ESRPersonId);
                result.Addresses = this.DataSources.EsrAddressApi.ReadByEsrId(esrPerson.ESRPersonId);
                result.Phones = this.DataSources.EsrPhoneApi.ReadByEsrId(esrPerson.ESRPersonId);
                result.Vehicles = this.DataSources.EsrVehicleApi.ReadByEsrId(esrPerson.ESRPersonId);
                result.AssignmentCostings = this.DataSources.EsrAssignmentCostingsApi.ReadByEsrId(esrPerson.ESRPersonId);
            }

            return result;
        }

        /// <summary>
        /// Find car by registration from list provided.
        /// </summary>
        /// <param name="registration">
        /// The registration of the car you're searching for.
        /// </param>
        /// <param name="cars">
        /// The list of cars to search within.
        /// </param>
        /// <returns>
        /// The <see cref="Car"/>, or a new Car if a match was not found.
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
        private void LogResults(List<DataClassBase> entities)
        {
            foreach (DataClassBase entity in entities)
            {
                Logger.WriteDebug(
                    this.MetaBase,
                    0,
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
                result = this.DataSources.EmployeeApi.Update(new List<Employee> { employee }).FirstOrDefault();
                if (result != null && result.Action == Action.Update)
                {
                    Logger.WriteExtra(
                        this.MetaBase,
                        0,
                        this.AccountId,
                        LogRecord.LogItemTypes.None,
                        LogRecord.TransferTypes.None,
                        0,
                        string.Empty,
                        LogRecord.LogReasonType.None,
                        string.Format(
                            "Update duplicate Employee {0} employeeid {1}", employee.username, employee.employeeid),
                        "EsrPersonsCrud");
                    var employeeRole = new EmployeeRole
                    {
                        employeeid = result.employeeid,
                        itemroleid =
                            int.Parse(this.GetImportFormat("defaultItemRole")),
                        order = 0,
                        Action = Action.Create
                    };
                    this.DataSources.EmployeeRoleApi.Create(new List<EmployeeRole> { employeeRole });
                    var employeeAccessRole = new EmployeeAccessRole
                    {
                        employeeID = result.employeeid,
                        accessRoleID =
                            int.Parse(this.GetImportFormat("defaultRole")),
                        subAccountID = 1, // TODO: Only one subaccount?
                        Action = Action.Create
                    };
                    this.DataSources.EmployeeAccessRoleApi.Create(new List<EmployeeAccessRole> { employeeAccessRole });
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
        private int? GetTrustID()
        {
            var trustApi = new TrustApi(this.MetaBase, this.AccountId);
            var result = trustApi.ReadSpecial(this.TrustVpd);

            return result.Count > 0 ? result[0].trustID : (int?)null;
        }

        private List<UserDefinedField> GetUdf(int accountId)
        {
            var result = new List<UserDefinedField>();
            if (MemoryCache.Default.Contains(string.Format("UDF_{0}", accountId)))
            {
                result = MemoryCache.Default.Get(string.Format("UDF_{0}", accountId)) as List<UserDefinedField>;
            }
            else
            {
                var udfCrud = new UdfApi(this.MetaBase, accountId);
                result = udfCrud.ReadAll();

                var udfMatchCrud = new UserDefinedMatchApi(this.MetaBase, this.AccountId);
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