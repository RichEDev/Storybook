namespace expenses.webServices
{
    using System;
    using System.Web.Services;
    using System.Web.Script.Services;

    using BusinessLogic.DataConnections;
    using BusinessLogic.GeneralOptions;

    using Spend_Management;

    /// <summary>
    /// Summary description for svcDutyOfCare
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [ScriptService]
    public class svcDutyOfCare : WebService
    {
        private const string VehicleErrorMessage= "You currently have no active vehicles to claim mileage against, please contact your administrator or Add a vehicle using the link.";
        private const string VehicleErrorMessageWithApproval= "You currently have no active vehicles to claim mileage against, please contact your administrator or Add a vehicle using the link and wait for your administrator to approve it.";

        /// <summary>
        /// An instance of <see cref="IDataFactory{IGeneralOptions,Int32}"/> to get a <see cref="IGeneralOptions"/>
        /// </summary>
        public IDataFactory<IGeneralOptions, int> GeneralOptionsFactory = expenses.Global.container.GetInstance<IDataFactory<IGeneralOptions, int>>();

        /// <summary>
        /// getDocComment webmethod create DOC Validation messages for the car selected.
        /// Session removed to fix response time
        /// </summary>
        /// <param name="id"></param>
        /// <param name="accountid"></param>
        /// <param name="employeeid"></param>
        /// <param name="carid"></param>
        /// <param name="subcatid"></param>
        /// <param name="date"></param>
        /// <param name="claimSubmitted"></param>
        /// <param name="isDelegate">A value indicating  whether or not user is delegate</param>
        /// <returns></returns>
        [WebMethod, ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string[] GetDutyOfCareComments(string id, int accountid, int employeeid, int carid, int subcatid, DateTime date, bool claimSubmitted, bool isDelegate)
        {
            cItemBuilder itemBuilder = new cItemBuilder(accountid, employeeid, date, this.GeneralOptionsFactory);
            string[] data = new string[2];
            data[0] = id;
            data[1] = itemBuilder.CreateDutyOfCareExpiryMessages(subcatid, carid, claimSubmitted, date, isDelegate);
            if (!string.IsNullOrWhiteSpace(data[1])) return data;
            var user = cMisc.GetCurrentUser();

            var generalOptions =
                this.GeneralOptionsFactory[user.CurrentSubAccountId].WithCar();

            var employeeCars = new cEmployeeCars(accountid, employeeid).GetActiveCars(date);
            if (employeeCars.Count != 0) return data;
            data[1] = generalOptions.Car.ActivateCarOnUserAdd ? VehicleErrorMessage : VehicleErrorMessageWithApproval;
            return data;
        }
    }
}
