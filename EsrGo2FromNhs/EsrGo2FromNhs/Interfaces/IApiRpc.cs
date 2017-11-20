namespace EsrGo2FromNhs.Interfaces
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Spend_Management;

    using EsrLocationRecord = global::EsrGo2FromNhs.ESR.EsrLocationRecord;
    using EsrOrganisationRecord = global::EsrGo2FromNhs.ESR.EsrOrganisationRecord;
    using EsrPersonRecord = global::EsrGo2FromNhs.ESR.EsrPersonRecord;
    using EsrPositionRecord = global::EsrGo2FromNhs.ESR.EsrPositionRecord;

    /// <summary>
    /// The APIRPC interface.
    /// </summary>
    [ServiceContract(Namespace = "http://software-europe.com/ApiRpc/2013/02")]
    public interface IApiRpc
    {
        #region EsrPersonRecord

        /// <summary>
        /// The save ESR persons.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrPersonRecords">
        /// The ESR Person Records.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<EsrPersonRecord> UpdateEsrPersonRecords(int accountId, string trustVpd, List<EsrPersonRecord> esrPersonRecords);

        /// <summary>
        /// The save ESR person.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPersonRecord"/>.
        /// </returns>
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<EsrPersonRecord> GetAllPersons(int accountId);

        #endregion

        #region Employees

        /// <summary>
        /// The save Employee.
        /// </summary>
        /// <param name="accountId">
        /// The account Id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrLocationRecords">
        /// The ESR Location Records.
        /// </param>
        /// <returns>
        /// The <see />.
        /// </returns>
        /// <summary>
        /// The save ESR locations.
        /// </summary>
        /// <returns>
        /// The <see cref="EsrLocationRecord"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrLocationRecord> UpdateEsrLocationRecords(int accountId, string trustVpd, List<EsrLocationRecord> esrLocationRecords);


        /// <summary>
        /// Read ESR location.
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
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrLocationRecord GetEsrLocationRecord(int accountId, int esrLocationRecordId);

        #endregion

        #region EsrOrganisation

        /// <summary>
        /// The save ESR Organisations.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrOrganisation">
        /// The ESR Organisation.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrOrganisationRecord> UpdateEsrOrganisations(int accountId, string trustVpd, List<EsrOrganisationRecord> esrOrganisation);

        /// <summary>
        /// Read ESR Organisation.
        /// </summary>
        /// <param name="accountId">
        ///     The account id.
        /// </param>
        /// <param name="esrOrganisationId">
        ///     The ESR Organisation Id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrOrganisationRecord GetEsrOrganisation(int accountId, int esrOrganisationId);

        #endregion

        #region EsrPosition

        /// <summary>
        /// The save ESR Positions.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="trustVpd">
        /// The trust VPD.
        /// </param>
        /// <param name="esrPositionRecords">
        /// The ESR Position.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPositionRecord> UpdateEsrPositions(int accountId, string trustVpd, List<EsrPositionRecord> esrPositionRecords);

        /// <summary>
        /// Read ESR Position.
        /// </summary>
        /// <param name="accountId">
        ///     The account id.
        /// </param>
        /// <param name="esrPositionId">
        ///     The ESR Position Id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrPositionRecord GetEsrPosition(int accountId, int esrPositionId);

        #endregion

        #region EsrTrust

        /// <summary>
        /// The get account from VPD.
        /// </summary>
        /// <param name="vpd">
        ///     The VPD.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrTrust GetAccountFromVpd(string vpd);

        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        bool UpdateEsrTrust(EsrTrust trust);

        #endregion

        #region ImportLog

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
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<ImportLog> CreateImportLog(int accountId, List<ImportLog> importLogs);

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
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<ImportLog> UpdateImportLog(int accountId, List<ImportLog> importLogs);
        #endregion

        #region ImportHistory

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
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<ImportHistory> CreateImportHistory(int accountId, List<ImportHistory> importHistories);

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
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        List<ImportHistory> UpdateImportHistory(int accountId, List<ImportHistory> importHistories);
        #endregion

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
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json, RequestFormat = WebMessageFormat.Json)]
        DataClassBase DeleteEntity(int accountId, DataClassBase entity);
        #endregion
    }
}
