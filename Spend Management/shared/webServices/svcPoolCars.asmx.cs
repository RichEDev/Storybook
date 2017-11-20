using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using SpendManagementLibrary;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcPoolCars
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    [System.Web.Script.Services.ScriptService]
    public class svcPoolCars : System.Web.Services.WebService
    {
        /// <summary>
        /// Displays the standard pool car grid
        /// </summary>
        /// <returns>Grid HTML string</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreatePoolCarsGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);

            return clsPoolCars.CreatePoolCarsGrid();
        }

        /// <summary>
        /// Delete a pool car from the database
        /// </summary>
        /// <param name="carid">ID of the Pool Car to delete</param>
        /// <returns>cGridNew string</returns>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public CarReturnVal DeletePoolCar(int carID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);

            clsPoolCars.DeletePoolCarUsers(carID);
            CarReturnVal retVal = clsPoolCars.DeleteCar(carID);

            return retVal;
        }
        
        /// <summary>
        /// Delete a pool car user
        /// </summary>
        /// <param name="carid">ID of the Pool Car to delete</param>
        /// <returns>cGridNew string</returns>
        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public string[] DeletePoolCarUser(int carID, int employeeID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();

            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);
            clsPoolCars.DeleteUserFromPoolCar(employeeID, carID);

            return clsPoolCars.CreatePoolCarsUsersGrid(carID);
        }

        /// <summary>
        /// Displays the standard pool car grid
        /// </summary>
        /// <returns>Grid HTML string</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreatePoolCarsUsersGrid(int carID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);

            return clsPoolCars.CreatePoolCarsUsersGrid(carID);
        }

        /// <summary>
        /// Displays the employees to add to a pool car
        /// </summary>
        /// <returns>Grid HTML string</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] CreatePoolCarEmployeeGrid()
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);

            return clsPoolCars.CreatePoolCarEmployeeGrid();
        }

        
        /// <summary>
        /// Save a selection of employeeids to a pool car
        /// </summary>
        /// <param name="carID">carid from page</param>
        /// <param name="employeesList">Javascript Array of employeeids</param>
        /// <returns>cGridNew string</returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string[] SaveUsersToCar(int carID, object[] employeesList)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cEmployees clsEmployees = new cEmployees(currentUser.AccountID);
            cPoolCars clsPoolCars = new cPoolCars(currentUser.AccountID);

            List<int> lstEmployees = new List<int>();

            foreach (int val in employeesList)
            {
                lstEmployees.Add(val);
            }
            if (lstEmployees.Count > 0)
            {
                for (int i = 0; i < lstEmployees.Count; i++)
                {
                    clsPoolCars.AddPoolCarUser(carID, lstEmployees[i]);
                }
            }
            return clsPoolCars.CreatePoolCarsUsersGrid(carID);
        }
    }
}
