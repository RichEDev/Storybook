namespace ApiRpc
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using ApiLibrary.ApiObjects.Base;
    using ApiLibrary.ApiObjects.ESR;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    using Utilities.DistributedCaching;

    using global::ApiRpc.Classes.ApiCrud;
    using global::ApiRpc.Classes.Code;
    using global::ApiRpc.Classes.Crud;
    using global::ApiRpc.Interfaces;

    using Action = ApiLibrary.DataObjects.Base.Action;

    /// <summary>
    /// The API RPC web service.
    /// </summary>
    public class ApiRpc : IApiRpc
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string metaBase = "expenses";

        /// <summary>
        /// The API CRUD.
        /// </summary>
        private readonly ApiCrudList apiCrud;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log logger;

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiRpc"/> class.  Used for Unit Tests.
        /// </summary>
        /// <param name="apicrud">
        /// The APICRUD.
        /// </param>
        /// <param name="logConnection">
        /// The logConnection for logging.
        /// </param>
        public ApiRpc(ApiCrudList apicrud, ApiLibrary.Interfaces.IApiDbConnection logConnection)
        {
            this.apiCrud = apicrud;
            this.metaBase = ConfigurationManager.AppSettings["metabase"];
            this.logger = new Log(logConnection);
            this.logger.WriteExtra(string.Empty, 0, 0, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} Starting {1}", "Api Rpc Service", DateTime.Now), "ApiRpc");
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiRpc"/> class.
        /// </summary>
        public ApiRpc()
        {
            this.apiCrud = null;
            this.metaBase = ConfigurationManager.AppSettings["metabase"];
            this.logger = new Log();
            this.logger.WriteExtra(string.Empty, 0, 0, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} Starting {1}", "Api Rpc Service", DateTime.Now), "ApiRpc");
        }

        #region Esr Person

        /// <summary>
        /// The save ESR person records.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrPersonRecords">
        /// The ESR person records.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPersonRecord> UpdateEsrPersonRecords(int accountId, string trustVpd, List<EsrPersonRecord> esrPersonRecords)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrPersonRecords", DateTime.Now), "ApiRpc");
            var esrPersons = new EsrPersonsCrud(GetUrl(), this.metaBase, accountId, this.apiCrud)
                                 {
                                     TrustVpd = trustVpd
                                 };
            var updated = new List<EsrPersonRecord>();
            var deleted = new List<EsrPersonRecord>();
            var result = new List<EsrPersonRecord>();
            foreach (EsrPersonRecord esrPersonRecord in esrPersonRecords)
            {
                if (esrPersonRecord.IsValid())
                {
                    if (esrPersonRecord.Action == Action.Delete)
                    {
                        deleted.Add(esrPersonRecord);
                    }
                    else
                    {
                        updated.Add(esrPersonRecord);
                    }
                }
                else
                {
                    result.Add(esrPersonRecord);
                }
            }

            var updateResult = esrPersons.Create(updated);
            var esrCarJourneyRates = new CarVehicleJourneyRateCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
            
            updateResult.AddRange(deleted.Select(esrPersonRecord => esrPersons.Delete(esrPersonRecord.ESRPersonId)));
            updateResult.RemoveAll(esrPersonRecord => esrPersonRecord == null);

            result.AddRange(updateResult);
            result = esrCarJourneyRates.UpdateMileageRates(result, trustVpd);

            this.ClearEsrPersonsEmployeeCache(accountId, result.Select(x => x.EmployeeId).Where(x => x > 0).ToList());

            return result;
        }

        /// <summary>
        /// The get all people.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPersonRecord> GetAllPersons(int accountId)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetAllPersons", DateTime.Now), "ApiRpc");
            var esrPersons = new EsrPersonsCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
            return esrPersons.ReadAll();
        }

        /// <summary>
        /// Clear the ancillary caches for employees
        /// </summary>
        /// <param name="accountId">The account identity</param>
        /// <param name="employeeIdentities">A list of employees to check</param>
        /// <returns></returns>
        public bool ClearEsrPersonsEmployeeCache(int accountId, List<int> employeeIdentities)
        {
            Cache cache = new Cache();
            foreach (var employeeIdentity in employeeIdentities)
            {
                cache.Delete(accountId, "employeeHomeAddresses", employeeIdentity.ToString(CultureInfo.InvariantCulture));
                cache.Delete(accountId, "employeeWorkAddresses", employeeIdentity.ToString(CultureInfo.InvariantCulture));
                cache.Delete(accountId, "employeeAccessRoles", employeeIdentity.ToString(CultureInfo.InvariantCulture));
                cache.Delete(accountId, "employeeCars", employeeIdentity.ToString(CultureInfo.InvariantCulture));
                cache.Delete(accountId, "esrAssignments", employeeIdentity.ToString(CultureInfo.InvariantCulture));
            }

            return true;
        }

        #endregion

        #region Esr Location

        /// <summary>
        /// The save ESR locations.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrLocationRecords">
        /// The ESR Location Record.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrLocationRecord> UpdateEsrLocationRecords(int accountId, string trustVpd, List<EsrLocationRecord> esrLocationRecords)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrLocationRecords", DateTime.Now), "ApiRpc");
            var esrLocation = new EsrLocationCrud(GetUrl(), this.metaBase, accountId, this.apiCrud)
                                  {
                                      TrustVpd =
                                          trustVpd
                                  };
            var esrLocations = esrLocationRecords.Select(locationRecord => DataClassBase.GetDataClassFromRecord(locationRecord, typeof(EsrLocation)) as EsrLocation).ToList();
            var updated = new List<EsrLocation>();
            var deleted = new List<EsrLocation>();
            var result = new List<EsrLocationRecord>();
            foreach (EsrLocation location in esrLocations)
            {
                if (location.IsValid())
                {
                    if (location.Action == Action.Delete)
                    {
                        deleted.Add(location);
                    }
                    else
                    {
                        updated.Add(location);
                    }
                }
                else
                {
                    result.Add(DataClassBase.GetDataClassFromRecord(location, typeof(EsrLocationRecord)) as EsrLocationRecord);
                }
            }

            var locations = esrLocation.Create(updated);
            locations.AddRange(deleted.Select(location => esrLocation.Delete(location.ESRLocationId)));

            result.AddRange(locations.Select(location => DataClassBase.GetRecordFromDataClass(location, typeof(EsrLocationRecord)) as EsrLocationRecord).ToList());
            return result;
        }

        /// <summary>
        /// The read ESR location.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrLocationRecordId">
        /// The ESR Location Record Id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocationRecord"/>.
        /// </returns>
        public EsrLocationRecord GetEsrLocationRecord(int accountId, int esrLocationRecordId)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetEsrLocationRecord", DateTime.Now), "ApiRpc");
            var esrLocation = new EsrLocationCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
            return DataClassBase.GetRecordFromDataClass(esrLocation.Read(esrLocationRecordId), typeof(EsrLocationRecord)) as EsrLocationRecord;
        }

        #endregion

        #region Esr Organisation

        /// <summary>
        /// The save ESR organisations.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrOrganisation">
        /// The ESR organisation.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrOrganisationRecord> UpdateEsrOrganisations(int accountId, string trustVpd, List<EsrOrganisationRecord> esrOrganisation)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrOrganisations", DateTime.Now), "ApiRpc");
            var esrOrganisations = new EsrOrganisationCrud(GetUrl(), this.metaBase, accountId, this.apiCrud)
            {
                TrustVpd =
                    trustVpd
            };
            var updated = new List<EsrOrganisation>();
            var deleted = new List<EsrOrganisationRecord>();
            var result = new List<EsrOrganisationRecord>();
            foreach (EsrOrganisationRecord esrOrganisationRecord in esrOrganisation)
            {
                if (esrOrganisationRecord.IsValid())
                {
                    if (esrOrganisationRecord.Action == Action.Delete)
                    {
                        deleted.Add(esrOrganisationRecord);
                    }
                    else
                    {
                        updated.Add(esrOrganisationRecord);
                    }
                }
                else
                {
                    result.Add(esrOrganisationRecord);
                }
            }

            var updateResult = esrOrganisations.Create(updated);
            if (deleted.Count > 0)
            {
                updateResult.AddRange(deleted.Select(esrOrganisationRecord => esrOrganisations.Delete(esrOrganisationRecord.ESROrganisationId)));    
            }
            
            result.AddRange(updateResult.Select(organisation => DataClassBase.GetRecordFromDataClass(organisation, typeof(EsrOrganisationRecord))).Cast<EsrOrganisationRecord>());
            return result;
        }

        /// <summary>
        /// The read ESR organisation.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrOrganisationId">
        /// The ESR organisation id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        public EsrOrganisationRecord GetEsrOrganisation(int accountId, int esrOrganisationId)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetEsrOrganisations", DateTime.Now), "ApiRpc");
            var esrOrganisations = new EsrOrganisationCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
            return DataClassBase.GetRecordFromDataClass(esrOrganisations.Read(esrOrganisationId), typeof(EsrOrganisationRecord)) as EsrOrganisationRecord;
        }

        #endregion

        #region Esr Position

        /// <summary>
        /// The save ESR positions.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrPositionRecords">
        /// The ESR position.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<EsrPositionRecord> UpdateEsrPositions(int accountId, string trustVpd, List<EsrPositionRecord> esrPositionRecords)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrPositions", DateTime.Now), "ApiRpc");
            var esrPositions = new EsrPositionCrud(GetUrl(), this.metaBase, accountId, this.apiCrud)
            {
                TrustVpd =
                    trustVpd
            };
            var positions = esrPositionRecords.Select(positionRecord => DataClassBase.GetDataClassFromRecord(positionRecord, typeof(EsrPosition)) as EsrPosition).ToList();
            var updated = new List<EsrPosition>();
            var deleted = new List<EsrPosition>();
            var result = new List<EsrPositionRecord>();
            foreach (EsrPosition position in positions)
            {
                if (position.IsValid())
                {
                    if (position.Action == Action.Delete)
                    {
                        deleted.Add(position);
                    }
                    else
                    {
                        updated.Add(position);
                    }
                }
                else
                {
                    result.Add(DataClassBase.GetDataClassFromRecord(position, typeof(EsrPositionRecord)) as EsrPositionRecord);
                }
            }

            var updatedPositions = esrPositions.Create(updated);
            updatedPositions.AddRange(deleted.Select(position => esrPositions.Delete(position.ESRPositionId)));

            result.AddRange(updatedPositions.Select(location => DataClassBase.GetRecordFromDataClass(location, typeof(EsrPositionRecord)) as EsrPositionRecord).ToList());
            return result;
        }

        /// <summary>
        /// The read ESR position.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="esrPositionId">
        /// The ESR position id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        public EsrPositionRecord GetEsrPosition(int accountId, int esrPositionId)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetEsrPosition", DateTime.Now), "ApiRpc");
            var esrPositions = new EsrPositionCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
            return DataClassBase.GetRecordFromDataClass(esrPositions.Read(esrPositionId), typeof(EsrPositionRecord)) as EsrPositionRecord;
        }

        #endregion

        #region Esr Account Lookup
        /// <summary>
        /// The get account from VPD.
        /// </summary>
        /// <param name="vpd">
        /// The VPD.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public EsrTrust GetAccountFromVpd(string vpd)
        {
            int vpdId;
            int.TryParse(vpd, out vpdId);
            this.logger.WriteDebug(string.Empty, vpdId, 0, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetAccountFromVpd", DateTime.Now), "ApiRpc");
            var esrTrusts = new TrustApi(this.metaBase, 0);
            var result = esrTrusts.ReadSpecial(vpd).FirstOrDefault();
            

            return result;
        }

        #endregion Esr Account Lookup

        #region Esr Import Log

        /// <summary>
        /// The create import log.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="importLogs">
        /// The import logs.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<ImportLog> CreateImportLog(int accountId, List<ImportLog> importLogs)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateImportLog", DateTime.Now), "ApiRpc");
            var crud = new ImportLogCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);

            List<ImportLog> result = crud.Create(importLogs);
            
            return result;
        }

        /// <summary>
        /// The update import log.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="importLogs">
        /// The import logs.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<ImportLog> UpdateImportLog(int accountId, List<ImportLog> importLogs)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateImportLog", DateTime.Now), "ApiRpc");
            var crud = new ImportLogCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);

            List<ImportLog> result = crud.Create(importLogs);

            return result;
        }

        #endregion Esr Import Log

        #region Esr Import History

        /// <summary>
        /// The create import history.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="importHistories">
        /// The import histories.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<ImportHistory> CreateImportHistory(int accountId, List<ImportHistory> importHistories)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateImportHistory", DateTime.Now), "ApiRpc");
            var crud = new ImportHistoryCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);

            List<ImportHistory> result = crud.Create(importHistories);

            return result;
        }

        /// <summary>
        /// The update import history.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="importHistories">
        /// The import histories.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<ImportHistory> UpdateImportHistory(int accountId, List<ImportHistory> importHistories)
        {
            this.logger.WriteDebug(string.Empty, 0, accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateImportHistory", DateTime.Now), "ApiRpc");
            var crud = new ImportHistoryCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);

            List<ImportHistory> result = crud.Create(importHistories);

            return result;
        }

        #endregion Esr Import History

        #region Esr Trust

        /// <summary>
        /// The update ESR trust.
        /// </summary>
        /// <param name="trust">
        /// The trust.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool UpdateEsrTrust(EsrTrust trust)
        {
            this.logger.WriteDebug(string.Empty, 0, trust.AccountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrTrust", DateTime.Now), "ApiRpc");
            var esrTrust = new EsrTrustCrud(GetUrl(), this.metaBase, trust.AccountId, this.apiCrud)
            {
                TrustVpd = trust.trustVPD
            };

            List<EsrTrust> savedTrust = esrTrust.Update(new List<EsrTrust> { trust });

            if (savedTrust.Count == 0)
            {
                return false;
            }

            return savedTrust.First().currentOutboundSequence == trust.currentOutboundSequence;
        }

        #endregion Esr Trust

        #region Delete

        /// <summary>
        /// Delete entity.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public DataClassBase DeleteEntity(int accountId, DataClassBase entity)
        {
            var result = new DataClassBase();
            if (entity is EsrVehicleRecord)
            {
                var vehiclecrud = new EsrVehiclesCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
                result = vehiclecrud.Delete(entity as EsrVehicle);
                if (result != null)
                {
                    var employeesCrud = new EmployeesCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
                    List<Employee> employee = employeesCrud.ReadByEsrId(((EsrVehicle)result).ESRPersonId);
                    this.ClearEsrPersonsEmployeeCache(accountId, employee.Select(x => x.employeeid).ToList());
                }
            }

            if (entity is EsrAddressRecord)
            {
                var addresscrud = new EsrAddressesCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
                result = addresscrud.Delete(entity as EsrAddress);
            }

            if (entity is EsrPhoneRecord)
            {
                var phonecrud = new EsrPhonesCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
                result = phonecrud.Delete(entity as EsrPhone);
            }

            if (entity is EsrAssignmentRecord)
            {
                var assignmentcrud = new EsrAssignmentsCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
                result = assignmentcrud.Delete(entity as EsrAssignment);
            }

            if (entity is EsrAssignmentCostingRecord)
            {
                var assignmentcostingcrud = new EsrAssignmentCostingCrud(GetUrl(), this.metaBase, accountId, this.apiCrud);
                result = assignmentcostingcrud.Delete(entity as EsrAssignmentCostings);
            }

            if (result == null)
            {
                result = new DataClassBase { ActionResult = new ApiResult {Result = ApiActionResult.Failure}, Action = Action.Delete };
            }

            return new DataClassBase { ActionResult = result.ActionResult };
        }

        #endregion
        /// <summary>
        /// Get the API Crud url.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetUrl()
        {
            return String.Empty;
        }

    }
}
