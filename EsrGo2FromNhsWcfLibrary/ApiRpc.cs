namespace EsrGo2FromNhsWcfLibrary
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Crud;
    using EsrGo2FromNhsWcfLibrary.ESR;
    using EsrGo2FromNhsWcfLibrary.Interfaces;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Utilities.DistributedCaching;

    using Action = EsrGo2FromNhsWcfLibrary.Base.Action;

    /// <summary>
    /// The API RPC web service.
    /// </summary>
    public class ApiRpc : IApiRpc
    {
        /// <summary>
        /// The meta base.
        /// </summary>
        private readonly string _metaBase = "expenses";

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly Log _logger;

        private readonly IEsrApi _esrApiHandler;

        /// <summary>
        /// Initialises a new instance of the <see cref="ApiRpc"/> class.  Used for Unit Tests.
        /// </summary>
        /// <param name="logConnection">
        /// The logConnection for logging.
        /// </param>
        /// <param name="esrApiHandler"></param>
        public ApiRpc(IApiDbConnection logConnection = null, IEsrApi esrApiHandler = null)
        {
            this._metaBase = ConfigurationManager.AppSettings["metabase"];
            
            if (esrApiHandler != null)
            {
                this._esrApiHandler = esrApiHandler;
            }

            this._logger = logConnection != null ? new Log(logConnection) : new Log();
            this._logger.WriteExtra(string.Empty, "0", 0, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.None, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} Starting {1}", "Api Rpc Service", DateTime.Now), "ApiRpc");
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrPersonRecords", DateTime.Now), "ApiRpc");
            var esrPersons = new EsrPersonsCrud(this._metaBase, accountId, this._esrApiHandler)
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
            var esrCarJourneyRates = new CarVehicleJourneyRateCrud(this._metaBase, accountId, this._esrApiHandler);

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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetAllPersons", DateTime.Now), "ApiRpc");
            var esrPersons = new EsrPersonsCrud(this._metaBase, accountId, this._esrApiHandler);
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
            var cache = new Cache();
            foreach (var employeeIdentity in employeeIdentities)
            {
                cache.Delete(accountId, "employeeHomeAddresses", employeeIdentity.ToString(CultureInfo.InvariantCulture));
                cache.Delete(accountId, "employeeWorkAddresses", employeeIdentity.ToString(CultureInfo.InvariantCulture));
                cache.Delete(accountId, "employeeAccessRoles", employeeIdentity.ToString(CultureInfo.InvariantCulture));
                cache.Delete(accountId, "employeeCars", employeeIdentity.ToString(CultureInfo.InvariantCulture));
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrLocationRecords", DateTime.Now), "ApiRpc");
            var esrLocation = new EsrLocationCrud(this._metaBase, accountId, this._esrApiHandler)
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetEsrLocationRecord", DateTime.Now), "ApiRpc");
            var esrLocation = new EsrLocationCrud(this._metaBase, accountId, this._esrApiHandler);
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrOrganisations", DateTime.Now), "ApiRpc");
            var esrOrganisations = new EsrOrganisationCrud(this._metaBase, accountId, this._esrApiHandler)
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetEsrOrganisations", DateTime.Now), "ApiRpc");
            var esrOrganisations = new EsrOrganisationCrud(this._metaBase, accountId, this._esrApiHandler);
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrPositions", DateTime.Now), "ApiRpc");
            var esrPositions = new EsrPositionCrud(this._metaBase, accountId, this._esrApiHandler)
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetEsrPosition", DateTime.Now), "ApiRpc");
            var esrPositions = new EsrPositionCrud(this._metaBase, accountId, this._esrApiHandler);
            return DataClassBase.GetRecordFromDataClass(esrPositions.Read(esrPositionId), typeof(EsrPositionRecord)) as EsrPositionRecord;
        }

        EsrTrust IApiRpc.GetAccountFromVpd(string vpd)
        {
            return this.GetAccountFromVpd(vpd);
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
            this._logger.WriteDebug(string.Empty, vpd, 0, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "GetAccountFromVpd", DateTime.Now), "ApiRpc");
            var esrTrusts = new EsrTrustCrud(this._metaBase, 0, this._esrApiHandler, this._logger)
                            {
                                TrustVpd = vpd
                            };
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateImportLog", DateTime.Now), "ApiRpc");
            var crud = new ImportLogCrud(this._metaBase, accountId, this._esrApiHandler);
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateImportLog", DateTime.Now), "ApiRpc");
            var crud = new ImportLogCrud(this._metaBase, accountId, this._esrApiHandler);
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "CreateImportHistory", DateTime.Now), "ApiRpc");
            var crud = new ImportHistoryCrud(this._metaBase, accountId, this._esrApiHandler);
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
            this._logger.WriteDebug(string.Empty, "0", accountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateImportHistory", DateTime.Now), "ApiRpc");
            var crud = new ImportHistoryCrud(this._metaBase, accountId, this._esrApiHandler); 
            return crud.Update(importHistories);
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
            this._logger.WriteDebug(string.Empty, "0", trust.AccountId, LogRecord.LogItemTypes.None, LogRecord.TransferTypes.EsrOutbound, 0, string.Empty, LogRecord.LogReasonType.None, string.Format("{0} - {1}", "UpdateEsrTrust", DateTime.Now), "ApiRpc");
            var esrTrust = new EsrTrustCrud(this._metaBase, trust.AccountId, this._esrApiHandler, this._logger)
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
        /// The <see />.
        /// </returns>
        public DataClassBase DeleteEntity(int accountId, DataClassBase entity)
        {
            var result = new DataClassBase();
            if (entity is EsrVehicleRecord)
            {
                var vehiclecrud = new EsrVehiclesCrud(this._metaBase, accountId, this._esrApiHandler);
                result = vehiclecrud.Delete(entity as EsrVehicle);
                if (result != null)
                {
                    var employeesCrud = new EmployeesCrud(this._metaBase, accountId, this._esrApiHandler);
                    List<Employee> employee = employeesCrud.ReadByEsrId(((EsrVehicle)result).ESRPersonId);
                    this.ClearEsrPersonsEmployeeCache(accountId, employee.Select(x => x.employeeid).ToList());
                }
            }

            if (entity is EsrAddressRecord)
            {
                var addresscrud = new EsrAddressesCrud(this._metaBase, accountId, this._esrApiHandler);
                result = addresscrud.Delete(entity as EsrAddress);
            }

            if (entity is EsrPhoneRecord)
            {
                var phonecrud = new EsrPhonesCrud(this._metaBase, accountId, this._esrApiHandler);
                result = phonecrud.Delete(entity as EsrPhone);
            }

            if (entity is EsrAssignmentRecord)
            {
                var assignmentcrud = new EsrAssignmentsCrud(this._metaBase, accountId, this._esrApiHandler);
                result = assignmentcrud.Delete(entity as EsrAssignment);
            }

            if (entity is EsrAssignmentCostingRecord)
            {
                var assignmentcostingcrud = new EsrAssignmentCostingCrud(this._metaBase, accountId, this._esrApiHandler);
                result = assignmentcostingcrud.Delete(entity as EsrAssignmentCostings);
            }

            if (result == null)
            {
                result = new DataClassBase { ActionResult = new ApiResult { Result = ApiActionResult.PartialSuccess, Message = "No record to delete." }, Action = Action.Delete };
            }

            return new DataClassBase { ActionResult = result.ActionResult };
        }

        #endregion
    }
}
