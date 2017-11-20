namespace ApiCrud.Interfaces
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.ServiceModel.Web;
    using ApiLibrary.DataObjects.Base;
    using ApiLibrary.DataObjects.ESR;
    using ApiLibrary.DataObjects.Spend_Management;

    /// <summary>
    /// The API Create Read Update Delete interface.
    /// </summary>
    [ServiceContract]
    public interface IApiCrud
    {
        #region Employees

        /// <summary>
        /// Create employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employees">
        /// The employees.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/employee/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Employee> CreateEmployees(string metabase, string accountid, List<Employee> employees);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/employee/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Employee ReadEmployee(string metabase, string accountid, string id);

        /// <summary>
        /// The read employee by user name.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="userName">
        /// The user name.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/employee/username/{username}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Employee ReadEmployeeByUsername(string metaBase, string accountid, string userName);

        /// <summary>
        /// The read all employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/employee/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<Employee> ReadAllEmployees(string metabase, string accountid);

        /// <summary>
        /// The update employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employees">
        /// The employees.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/employee/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Employee> UpdateEmployees(string metabase, string accountid, List<Employee> employees);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/employee/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Employee DeleteEmployees(string metabase, string accountid, string id);

        /// <summary>
        /// The read employee by person.
        /// </summary>
        /// <param name="metabase">
        ///     The METABASE.
        /// </param>
        /// <param name="accountid">
        ///     The account id.
        /// </param>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/employee/?esrid={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Employee> ReadEmployeeByPerson(string metabase, string accountid, string id);
        #endregion

        #region ESR Person

        /// <summary>
        /// The create ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPersons">
        /// The ESR persons.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/esrperson/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPerson> CreateEsrPersons(string metabase, string accountid, List<EsrPerson> esrPersons);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/esrperson/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrPerson ReadEsrPerson(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPerson"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/esrperson/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPerson> ReadAllEsrPerson(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPersons">
        /// The ESR Persons.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/esrperson/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPerson> UpdateEsrPersons(string metabase, string accountid, List<EsrPerson> esrPersons);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/esrperson/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrPerson DeleteEsrPerson(string metabase, string accountid, string id);

        #endregion
        #region ESR Assignment

        /// <summary>
        /// The create ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignments">
        /// The ESR Assignments.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrAssignment/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAssignment> CreateEsrAssignments(string metabase, string accountid, List<EsrAssignment> esrAssignments);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAssignment/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrAssignment ReadEsrAssignment(string metabase, string accountid, string id);

        /// <summary>
        /// The read ESR assignment by ESRPERSONID.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAssignment/?esrid={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAssignment> ReadEsrAssignmentByPerson(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignment"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAssignment/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAssignment> ReadAllEsrAssignment(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignments">
        /// The ESR Assignments.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrAssignment/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAssignment> UpdateEsrAssignments(string metabase, string accountid, List<EsrAssignment> esrAssignments);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrAssignment/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrAssignment DeleteEsrAssignment(string metabase, string accountid, string id);

        #endregion
        #region ESR Vehicle

        /// <summary>
        /// The create ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrVehicles">
        /// The ESR Vehicles.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrVehicle/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrVehicle> CreateEsrVehicles(string metabase, string accountid, List<EsrVehicle> esrVehicles);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrVehicle/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrVehicle ReadEsrVehicle(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrVehicle/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrVehicle> ReadAllEsrVehicle(string metabase, string accountid);

        /// <summary>
        /// The read ESR Vehicle by ESRPERSONID.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrVehicle"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrVehicle/?esrid={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrVehicle> ReadEsrVehicleByPerson(string metabase, string accountid, string id);

        /// <summary>
        /// The update ESR vehicles.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrVehicles">
        /// The ESR Vehicles.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrVehicle/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrVehicle> UpdateEsrVehicles(string metabase, string accountid, List<EsrVehicle> esrVehicles);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrVehicle/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrVehicle DeleteEsrVehicle(string metabase, string accountid, string id);

        #endregion
        #region ESR Phone

        /// <summary>
        /// The create ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPhones">
        /// The ESR persons.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrPhone/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPhone> CreateEsrPhones(string metabase, string accountid, List<EsrPhone> esrPhones);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrPhone/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrPhone ReadEsrPhone(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrPhone/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPhone> ReadAllEsrPhone(string metabase, string accountid);

        /// <summary>
        /// The read ESR Phone by ESRPERSONID.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPhone"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrPhone/?esrid={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPhone> ReadEsrPhoneByPerson(string metabase, string accountid, string id);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPhones">
        /// The ESR Phones.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrPhone/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPhone> UpdateEsrPhones(string metabase, string accountid, List<EsrPhone> esrPhones);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrPhone/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrPhone DeleteEsrPhone(string metabase, string accountid, string id);

        #endregion
        #region ESR Address

        /// <summary>
        /// The create ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAddress">
        /// The ESR Address.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAddress> CreateEsrAddresss(string metabase, string accountid, List<EsrAddress> esrAddress);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAddress/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrAddress ReadEsrAddress(string metabase, string accountid, string id);

        /// <summary>
        /// The read ESR address by person ID.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The ID.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAddress/?esrid={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAddress> ReadEsrAddressByPerson(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAddress"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAddress> ReadAllEsrAddress(string metabase, string accountid);

        /// <summary>
        /// The update ESR Addresss.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAddresss">
        /// The ESR Address.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAddress> UpdateEsrAddresss(string metabase, string accountid, List<EsrAddress> esrAddresss);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrAddress/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrAddress DeleteEsrAddress(string metabase, string accountid, string id);

        #endregion
        #region ESR AssignmentCostings

        /// <summary>
        /// The create ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignmentCostings">
        /// The ESR persons.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrAssignmentCostings/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAssignmentCostings> CreateEsrAssignmentCostingss(string metabase, string accountid, List<EsrAssignmentCostings> esrAssignmentCostings);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentCostings"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAssignmentCostings/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrAssignmentCostings ReadEsrAssignmentCostings(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrAssignmentCostings"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrAssignmentCostings/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAssignmentCostings> ReadAllEsrAssignmentCostings(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrAssignmentCostingss">
        /// The ESR AssignmentCostingss.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrAssignmentCostings/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrAssignmentCostings> UpdateEsrAssignmentCostingss(string metabase, string accountid, List<EsrAssignmentCostings> esrAssignmentCostingss);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrAssignmentCostings/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrAssignmentCostings DeleteEsrAssignmentCostings(string metabase, string accountid, string id);

        #endregion
        #region ESR EsrElementFields

        /// <summary>
        /// The create ESR EsrElementFields.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="EsrElementFieldss">
        /// The ESR EsrElementFields.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrElementFields/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElementFields> CreateEsrElementFieldss(string metabase, string accountid, List<EsrElementFields> EsrElementFieldss);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementFields"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrElementFields/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrElementFields ReadEsrElementFields(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementFields"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrElementFields/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElementFields> ReadAllEsrElementFields(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="EsrElementFieldss">
        /// The ESR EsrElementFieldss.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrElementFields/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElementFields> UpdateEsrElementFieldss(string metabase, string accountid, List<EsrElementFields> EsrElementFieldss);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrElementFields/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrElementFields DeleteEsrElementFields(string metabase, string accountid, string id);

        #endregion
        #region ESR Element

        /// <summary>
        /// The create ESR Element.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElements">
        /// The ESR Element.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrElement/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElement> CreateEsrElements(string metabase, string accountid, List<EsrElement> esrElements);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElement"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrElement/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrElement ReadEsrElement(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElement"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrElement/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElement> ReadAllEsrElement(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElements">
        /// The ESR Elements.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrElement/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElement> UpdateEsrElements(string metabase, string accountid, List<EsrElement> esrElements);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrElement/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrElement DeleteEsrElement(string metabase, string accountid, string id);

        #endregion
        #region ESR Trust

        /// <summary>
        /// The create ESR Trust.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrTrusts">
        /// The ESR Trust.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrTrust/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrTrust> CreateEsrTrusts(string metabase, string accountid, List<EsrTrust> esrTrusts);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrTrust/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrTrust ReadEsrTrust(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrTrust/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrTrust> ReadAllEsrTrust(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrTrusts">
        /// The ESR Trusts.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrTrust/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrTrust> UpdateEsrTrusts(string metabase, string accountid, List<EsrTrust> esrTrusts);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrTrust/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrTrust DeleteEsrTrust(string metabase, string accountid, string id);

        /// <summary>
        /// Find ESR trust method.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="esrTrustId">
        /// The esr Trust Id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrTrust"/>.
        /// </returns>
        [OperationContract]
        [WebGet(RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json,
            UriTemplate = "/{metabase}/{EsrTrustId}")]
        EsrTrust FindEsrTrust(string metaBase, string esrTrustId);

        #endregion
        #region ESR Location

        /// <summary>
        /// The create ESR Location.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrLocations">
        /// The ESR Location.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrLocation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrLocation> CreateEsrLocations(string metabase, string accountid, List<EsrLocation> esrLocations);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrLocation/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrLocation ReadEsrLocation(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrLocation"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrLocation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrLocation> ReadAllEsrLocation(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrLocations">
        /// The ESR Locations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrLocation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrLocation> UpdateEsrLocations(string metabase, string accountid, List<EsrLocation> esrLocations);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrLocation/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrLocation DeleteEsrLocation(string metabase, string accountid, string id);

        #endregion
        #region ESR Organisation

        /// <summary>
        /// The create ESR Organisation.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrOrganisations">
        /// The ESR Organisation.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrOrganisation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrOrganisation> CreateEsrOrganisations(string metabase, string accountid, List<EsrOrganisation> esrOrganisations);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrOrganisation/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrOrganisation ReadEsrOrganisation(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrOrganisation"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrOrganisation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrOrganisation> ReadAllEsrOrganisation(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrOrganisations">
        /// The ESR Organisations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrOrganisation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrOrganisation> UpdateEsrOrganisations(string metabase, string accountid, List<EsrOrganisation> esrOrganisations);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrOrganisation/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrOrganisation DeleteEsrOrganisation(string metabase, string accountid, string id);

        #endregion
        #region ESR Position

        /// <summary>
        /// The create ESR Position.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPositions">
        /// The ESR Position.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrPosition/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPosition> CreateEsrPositions(string metabase, string accountid, List<EsrPosition> esrPositions);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrPosition/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrPosition ReadEsrPosition(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrPosition"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrPosition/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPosition> ReadAllEsrPosition(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrPositions">
        /// The ESR Positions.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrPosition/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrPosition> UpdateEsrPositions(string metabase, string accountid, List<EsrPosition> esrPositions);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrPosition/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrPosition DeleteEsrPosition(string metabase, string accountid, string id);

        #endregion
        #region ESR ElementSubCat

        /// <summary>
        /// The create ESR ElementSubCat.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElementSubCats">
        /// The ESR ElementSubCat.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EsrElementSubCat/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElementSubCat> CreateEsrElementSubCats(string metabase, string accountid, List<EsrElementSubCat> esrElementSubCats);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementSubCat"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrElementSubCat/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrElementSubCat ReadEsrElementSubCat(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="EsrElementSubCat"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EsrElementSubCat/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElementSubCat> ReadAllEsrElementSubCat(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="esrElementSubCats">
        /// The ESR ElementSubCats.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EsrElementSubCat/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EsrElementSubCat> UpdateEsrElementSubCats(string metabase, string accountid, List<EsrElementSubCat> esrElementSubCats);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EsrElementSubCat/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EsrElementSubCat DeleteEsrElementSubCat(string metabase, string accountid, string id);

        #endregion
        #region Car

        /// <summary>
        /// The create Car.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="cars">
        /// The Car.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/Car/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Car> CreateCars(string metabase, string accountid, List<Car> cars);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Car"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Car/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Car ReadCar(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="Car"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Car/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Car> ReadAllCar(string metabase, string accountid);

        /// <summary>
        /// The read car by ESR id.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Car/?esrid={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Car> ReadCarByEsrId(string metabase, string accountid, string id);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="Cars">
        /// The cars.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/Car/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Car> UpdateCars(string metabase, string accountid, List<Car> Cars);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/Car/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Car DeleteCar(string metabase, string accountid, string id);

        #endregion
        #region templateMappings
        /// <summary>
        /// The create TemplateMapping.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="templateMappings">
        /// The TemplateMapping.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/TemplateMapping/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<TemplateMapping> CreateTemplateMappings(string metabase, string accountid, List<TemplateMapping> templateMappings);

        /// <summary>
        /// The read ESR person.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/TemplateMapping/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        TemplateMapping ReadTemplateMapping(string metabase, string accountid, string id);

        /// <summary>
        /// The read all ESR person records.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="TemplateMapping"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/TemplateMapping/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<TemplateMapping> ReadAllTemplateMapping(string metabase, string accountid);

        /// <summary>
        /// The update ESR persons.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="templateMappings">
        /// The templateMappings.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/TemplateMapping/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<TemplateMapping> UpdateTemplateMappings(string metabase, string accountid, List<TemplateMapping> templateMappings);

        /// <summary>
        /// The delete employees.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/TemplateMapping/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        TemplateMapping DeleteTemplateMapping(string metabase, string accountid, string id);

        #endregion
        #region Fields

        /// <summary>
        /// The read all fields.
        /// </summary>
        /// <param name="metaBase">
        /// The meta Base.
        /// </param>
        /// <param name="accountID">
        /// The account Id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Field/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Field> ReadAllFields(string metaBase, string accountID);
        #endregion
        #region UDF

        /// <summary>
        /// The read all User defined fields.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/UserDefinedField/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<UserDefinedField> ReadAllUdf(string metaBase, string accountId);

        /// <summary>
        /// The create UDF.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(UriTemplate = "/{metabase}/{accountid}/UserDefinedField/", Method = "POST", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<UserDefinedField> CreateUdfValue(string metaBase, string accountId, List<UserDefinedField> entity);

        #endregion
        #region Account Properties

        /// <summary>
        /// The read all account properties.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/AccountProperty/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<AccountProperty> ReadAllAccountProperties(string metaBase, string accountId);
        #endregion
        #region Lookup

        /// <summary>
        /// The read lookup value.
        /// </summary>
        /// <param name="metaBase">
        /// The meta base.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
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
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Lookup/{tableid}/{fieldid}/{keyvalue}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Lookup ReadLookupValue(string metaBase, string accountId, string tableid, string fieldid, string keyvalue);
        #endregion
        #region Addresses

        /// <summary>
        /// Create Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="addresses">
        /// The Addresses.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/Address/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Address> CreateAddresses(string metabase, string accountid, List<Address> addresses);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Address/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Address ReadAddress(string metabase, string accountid, string id);

        /// <summary>
        /// The read all Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Address/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<Address> ReadAllAddresses(string metabase, string accountid);

        /// <summary>
        /// The update Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="addresses">
        /// The Addresses.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/Address/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Address> UpdateAddresses(string metabase, string accountid, List<Address> Addresses);

        /// <summary>
        /// The delete Addresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="Address"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/Address/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        Address DeleteAddresses(string metabase, string accountid, string id);

        /// <summary>
        /// The read Address by ESR.
        /// </summary>
        /// <param name="metabase">
        ///     The METABASE.
        /// </param>
        /// <param name="accountid">
        ///     The account id.
        /// </param>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/Address/?esrid={id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<Address> ReadAddressByEsr(string metabase, string accountid, string id);
        
        #endregion
        #region employeeHomeAddresses

        /// <summary>
        /// Create EmployeeHomeAddress.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeHomeAddresses">
        /// The EmployeeHomeAddresses.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EmployeeHomeAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeHomeAddress> CreateEmployeeHomeAddresses(string metabase, string accountid, List<EmployeeHomeAddress> employeeHomeAddresses);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeHomeAddress/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeHomeAddress ReadEmployeeHomeAddress(string metabase, string accountid, string id);

        /// <summary>
        /// The read all EmployeeHomeAddresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeHomeAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<EmployeeHomeAddress> ReadAllEmployeeHomeAddresses(string metabase, string accountid);

        /// <summary>
        /// The update EmployeeHomeAddresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeHomeAddresses">
        /// The EmployeeHomeAddresses.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EmployeeHomeAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeHomeAddress> UpdateEmployeeHomeAddresses(string metabase, string accountid, List<EmployeeHomeAddress> employeeHomeAddresses);

        /// <summary>
        /// The delete EmployeeHomeAddresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeHomeAddress"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EmployeeHomeAddress/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeHomeAddress DeleteEmployeeHomeAddresses(string metabase, string accountid, string id);
        #endregion
        #region employeeWorkAddresses

        /// <summary>
        /// Create employeeWorkAddresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeWorkAddresses">
        /// The employeeWorkAddresses.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EmployeeWorkAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeWorkAddress> CreateEmployeeWorkAddresses(string metabase, string accountid, List<EmployeeWorkAddress> employeeWorkAddresses);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeWorkAddress/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeWorkAddress ReadEmployeeWorkAddress(string metabase, string accountid, string id);

        /// <summary>
        /// The read all employeeWorkAddresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeWorkAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<EmployeeWorkAddress> ReadAllEmployeeWorkAddresses(string metabase, string accountid);

        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeWorkAddress/EmployeeWorkAddress/{assignmentnumber}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeWorkAddress> ReadEmployeeWorkAddressesByAssignment(string metaBase, string accountid, string assignmentnumber);

        /// <summary>
        /// The update employeeWorkAddresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeWorkAddresses">
        /// The employeeWorkAddresses.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EmployeeWorkAddress/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeWorkAddress> UpdateEmployeeWorkAddresses(string metabase, string accountid, List<EmployeeWorkAddress> employeeWorkAddresses);

        /// <summary>
        /// The delete employeeWorkAddresses.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="employeeWorkAddress"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EmployeeWorkAddress/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeWorkAddress DeleteEmployeeWorkAddresses(string metabase, string accountid, string id);
        #endregion
        #region CostCodes

        /// <summary>
        /// Create CostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="costCodes">
        /// The CostCodes.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/CostCode/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CostCode> CreateCostCodes(string metabase, string accountid, List<CostCode> costCodes);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/CostCode/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CostCode ReadCostCode(string metabase, string accountid, string id);

        /// <summary>
        /// The read all CostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/CostCode/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<CostCode> ReadAllCostCodes(string metabase, string accountid);

        /// <summary>
        /// The update CostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="costCodes">
        /// The CostCodes.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/CostCode/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CostCode> UpdateCostCodes(string metabase, string accountid, List<CostCode> costCodes);

        /// <summary>
        /// The delete CostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CostCode"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/CostCode/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CostCode DeleteCostCodes(string metabase, string accountid, string id);
        #endregion
        #region EmployeeCostCodes

        /// <summary>
        /// Create EmployeeCostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="EmployeeCostCodes">
        /// The EmployeeCostCodes.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/EmployeeCostCode/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeCostCode> CreateEmployeeCostCodes(string metabase, string accountid, List<EmployeeCostCode> EmployeeCostCodes);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeCostCode/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeCostCode ReadEmployeeCostCode(string metabase, string accountid, string id);

        /// <summary>
        /// The read all EmployeeCostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeCostCode/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<EmployeeCostCode> ReadAllEmployeeCostCodes(string metabase, string accountid);

        /// <summary>
        /// The read employee cost codes by employee.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/EmployeeCostCode/employee/{employeeid}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<EmployeeCostCode> ReadEmployeeCostCodesByEmployee(string metabase, string accountid, string employeeId);

        /// <summary>
        /// The update EmployeeCostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="EmployeeCostCodes">
        /// The EmployeeCostCodes.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/EmployeeCostCode/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeCostCode> UpdateEmployeeCostCodes(string metabase, string accountid, List<EmployeeCostCode> EmployeeCostCodes);

        /// <summary>
        /// The delete EmployeeCostCodes.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="EmployeeCostCode"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/EmployeeCostCode/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        EmployeeCostCode DeleteEmployeeCostCodes(string metabase, string accountid, string id);
        #endregion
        #region CarAssignmentNumberAllocations

        /// <summary>
        /// Create CarAssignmentNumberAllocations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="CarAssignmentNumberAllocations">
        /// The CarAssignmentNumberAllocations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/CarAssignmentNumberAllocation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CarAssignmentNumberAllocation> CreateCarAssignmentNumberAllocations(string metabase, string accountid, List<CarAssignmentNumberAllocation> CarAssignmentNumberAllocations);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/CarAssignmentNumberAllocation/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CarAssignmentNumberAllocation ReadCarAssignmentNumberAllocation(string metabase, string accountid, string id);

        /// <summary>
        /// The read all CarAssignmentNumberAllocations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/CarAssignmentNumberAllocation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<CarAssignmentNumberAllocation> ReadAllCarAssignmentNumberAllocations(string metabase, string accountid);

        /// <summary>
        /// The read employee cost codes by employee.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/CarAssignmentNumberAllocation/employee/{employeeid}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<CarAssignmentNumberAllocation> ReadCarAssignmentNumberAllocationsByEmployee(string metabase, string accountid, string employeeId);

        /// <summary>
        /// The update CarAssignmentNumberAllocations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="CarAssignmentNumberAllocations">
        /// The CarAssignmentNumberAllocations.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/CarAssignmentNumberAllocation/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CarAssignmentNumberAllocation> UpdateCarAssignmentNumberAllocations(string metabase, string accountid, List<CarAssignmentNumberAllocation> CarAssignmentNumberAllocations);

        /// <summary>
        /// The delete CarAssignmentNumberAllocations.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CarAssignmentNumberAllocation"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/CarAssignmentNumberAllocation/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CarAssignmentNumberAllocation DeleteCarAssignmentNumberAllocations(string metabase, string accountid, string id);
        #endregion
        #region ImportLogs

        /// <summary>
        /// Create ImportLogs.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportLogs">
        /// The ImportLogs.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/ImportLog/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ImportLog> CreateImportLogs(string metabase, string accountid, List<ImportLog> ImportLogs);

        /// <summary>
        /// The update ImportLogs.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportLogs">
        /// The ImportLogs.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/ImportLog/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ImportLog> UpdateImportLogs(string metabase, string accountid, List<ImportLog> ImportLogs);

        #endregion
        #region ImportHistorys

        /// <summary>
        /// Create ImportHistorys.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportHistorys">
        /// The ImportHistorys.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/ImportHistory/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ImportHistory> CreateImportHistorys(string metabase, string accountid, List<ImportHistory> ImportHistorys);

        /// <summary>
        /// The update ImportHistorys.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="ImportHistorys">
        /// The ImportHistorys.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/ImportHistory/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<ImportHistory> UpdateImportHistorys(string metabase, string accountid, List<ImportHistory> ImportHistorys);

        #endregion
        #region UserDefinedMatchField

        /// <summary>
        /// The read user defined match field.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/UserDefinedMatchField/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<UserDefinedMatchField> ReadUserDefinedMatchField(string metabase, string accountid, string id);
        #endregion
        #region employeeRoles

        /// <summary>
        /// The create employee roles.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeRoles">
        /// The employee roles.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/employeeRole/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeRole> CreateEmployeeRoles(string metabase, string accountid, List<EmployeeRole> employeeRoles);

        #endregion
        #region employeeAccessRoles

        /// <summary>
        /// The create employee roles.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="employeeAccessRoles">
        /// The employee access roles.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/employeeAccessRole/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<EmployeeAccessRole> CreateEmployeeAccessRoles(string metabase, string accountid, List<EmployeeAccessRole> employeeAccessRoles);
        #endregion
        #region Vehicle Journey Rates

        /// <summary>
        /// The read all vehicle journey rates.
        /// </summary>
        /// <param name="metabase">
        /// The metabase.
        /// </param>
        /// <param name="accountid">
        /// The accountid.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/VehicleJourneyRate/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<VehicleJourneyRate> ReadAllVehicleJourneyRates(string metabase, string accountid);

        #endregion
        #region Car Vehicle Journey Rates

        /// <summary>
        /// The create car vehicle journey rates.
        /// </summary>
        /// <param name="metabase">
        /// The meta base.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="CarVehicleJourneyRates">
        /// The car vehicle journey rates.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/{metabase}/{accountid}/CarVehicleJourneyRate/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CarVehicleJourneyRate> CreateCarVehicleJourneyRates(string metabase, string accountid, List<CarVehicleJourneyRate> CarVehicleJourneyRates);

        /// <summary>
        /// Read an Entity record.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account ID.
        /// </param>
        /// <param name="id">
        /// The ID of the entity to read.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/CarVehicleJourneyRate/{id}/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CarVehicleJourneyRate> ReadCarVehicleJourneyRate(string metabase, string accountid, string id);

        /// <summary>
        /// The read all CarVehicleJourneyRates.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebGet(UriTemplate = "/{metabase}/{accountid}/CarVehicleJourneyRate/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(ApiException))]
        List<CarVehicleJourneyRate> ReadAllCarVehicleJourneyRates(string metabase, string accountid);

        /// <summary>
        /// The update CarVehicleJourneyRates.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="CarVehicleJourneyRates">
        /// The CarVehicleJourneyRates.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/{metabase}/{accountid}/CarVehicleJourneyRate/", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        List<CarVehicleJourneyRate> UpdateCarVehicleJourneyRates(string metabase, string accountid, List<CarVehicleJourneyRate> CarVehicleJourneyRates);

        /// <summary>
        /// The delete CarVehicleJourneyRates.
        /// </summary>
        /// <param name="metabase">
        /// The METABASE.
        /// </param>
        /// <param name="accountid">
        /// The account id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <returns>
        /// The <see cref="CarVehicleJourneyRate"/>.
        /// </returns>
        [OperationContract]
        [WebInvoke(Method = "DELETE", UriTemplate = "/{metabase}/{accountid}/CarVehicleJourneyRate/{id}", RequestFormat = WebMessageFormat.Json, ResponseFormat = WebMessageFormat.Json)]
        CarVehicleJourneyRate DeleteCarVehicleJourneyRates(string metabase, string accountid, string id);
        #endregion
    }
}
