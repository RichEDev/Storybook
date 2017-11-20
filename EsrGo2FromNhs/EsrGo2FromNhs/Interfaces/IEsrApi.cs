namespace EsrGo2FromNhs.Interfaces
{
    using System.Collections.Generic;
    using System.Net;

    using EsrGo2FromNhs.Base;
    using EsrGo2FromNhs.Enum;
    using EsrGo2FromNhs.ESR;
    using EsrGo2FromNhs.Spend_Management;

    public interface IEsrApi
    {
        /// <summary>
        /// Generic Api call returning a List of the specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accessMethod"></param>
        /// <param name="identifier"></param>
        /// <param name="entities"></param>
        /// <param name="returnDefault"></param>
        /// <returns></returns>
        List<T> Execute<T>(DataAccessMethod accessMethod, string identifier = "", List<T> entities = null, DataAccessMethodReturnDefault returnDefault = DataAccessMethodReturnDefault.NewObject) where T : DataClassBase, new();

        /// <summary>
        /// Generic Api call returning a specified type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="identifier"></param>
        /// <param name="accessMethod"></param>
        /// <param name="returnDefault"></param>
        /// <returns></returns>
        T Execute<T>(string identifier, DataAccessMethod accessMethod, DataAccessMethodReturnDefault returnDefault = DataAccessMethodReturnDefault.NewObject) where T : DataClassBase, new();

        /// <summary>
        /// Generic Api call returning a specified type, handling deleting of a generic type entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="accessMethod"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Execute<T>(DataAccessMethod accessMethod, T entity) where T : DataClassBase, new();

        /// <summary>
        /// Write to the logger
        /// </summary>
        /// <param name="accessType"></param>
        void LoggerWrite(string accessType);

        /// <summary>
        /// Get templates where VPD = reference
        /// </summary>
        /// <param name="reference">The VPD to read.</param>
        /// <returns></returns>
        List<TemplateMapping> TemplateMappingReadSpecial(string reference);

        /// <summary>
        /// The find ESR trust in a given meta base by id.
        /// </summary>
        /// <param name="esrTrust">
        /// The ESR Trust.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// Account that contains the NHS VPD
        /// </returns>
        EsrTrust FindEsrTrust(string esrTrust);

        /// <summary>
        /// The read lookup value.
        /// </summary>
        /// <param name="tableid">
        /// The table id.
        /// </param>
        /// <param name="fieldid">
        /// The field id.
        /// </param>
        /// <param name="keyvalue">
        /// The key value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        Lookup ReadLookupValue(string tableid, string fieldid, string keyvalue);

        /// <summary>
        /// Convert string parameters to integers.
        /// </summary>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ConvertParams(string accountid, string id = "");

        /// <summary>
        /// The check meta base.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <exception cref="WebException">
        /// METABASE must be valid
        /// </exception>
        void CheckMetaBase(string metaBase);

        /// <summary>
        /// The close connection.
        /// </summary>
        void CloseConnection();

        /// <summary>
        /// The clear employee cache.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        void ClearCache(IEnumerable<Employee> result);

        void UpdateEsrAssignmentSupervisors();
    }
}