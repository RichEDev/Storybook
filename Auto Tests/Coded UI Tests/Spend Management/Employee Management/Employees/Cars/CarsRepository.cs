using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees.Cars
{
    /// <summary>
    /// The Car Repository
    /// </summary>
    public class CarsRepository
    {
        /// <summary>
        /// The Create Car
        /// </summary>
        /// <param name="carToCreate"></param>
        /// <param name="executingProduct"></param>
        /// <returns></returns>
        public static int createCar(Cars carToCreate, ProductType executingProduct)
        {
            carToCreate.carID = 0;
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carToCreate.carID);
            if (carToCreate.employeeID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@employeeid", carToCreate.employeeID);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@make", carToCreate.make);
            expdata.sqlexecute.Parameters.AddWithValue("@model", carToCreate.model);
            expdata.sqlexecute.Parameters.AddWithValue("@registration", carToCreate.regNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultunit", Convert.ToByte(carToCreate.unitOfMeasure));
            expdata.sqlexecute.Parameters.AddWithValue("@cartypeid", carToCreate.carType);
            expdata.sqlexecute.Parameters.AddWithValue("@active", Convert.ToByte(carToCreate.active));
            expdata.sqlexecute.Parameters.AddWithValue("@odometer", carToCreate.odometer);
            expdata.sqlexecute.Parameters.AddWithValue("@fuelcard", Convert.ToByte(carToCreate.fuelCard));
            expdata.sqlexecute.Parameters.AddWithValue("@endodometer", carToCreate.endOdometer);
            expdata.sqlexecute.Parameters.AddWithValue("@engineSize", carToCreate.engineSize);
            expdata.sqlexecute.Parameters.AddWithValue("@approved", carToCreate.approved);


            if (carToCreate.startDate == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@startdate", DBNull.Value);

            }
            else
            {

                expdata.sqlexecute.Parameters.AddWithValue("@startdate", carToCreate.startDate);

            }

            if (carToCreate.endDate == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@enddate", DBNull.Value);

            }
            else
            {

                expdata.sqlexecute.Parameters.AddWithValue("@enddate", carToCreate.endDate);

            }


            if (carToCreate.taxExpiry == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxexpiry", carToCreate.taxExpiry);
            }
            if (carToCreate.taxLastChecked == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxlastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxlastchecked", carToCreate.taxLastChecked);
            }
            if (carToCreate.motExpiry == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motexpiry", carToCreate.motExpiry);
            }
            if (carToCreate.motLastChecked == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motlastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motlastchecked", carToCreate.motLastChecked);
            }
            if (carToCreate.insuranceExpiry == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insuranceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insuranceexpiry", carToCreate.insuranceExpiry);
            }
            if (carToCreate.insuranceLastChecked == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancelastchecked", carToCreate.insuranceLastChecked);
            }
            if (carToCreate.serviceExpiry == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@serviceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@serviceexpiry", carToCreate.serviceExpiry);
            }
            if (carToCreate.serviceLastChecked == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicelastchecked", carToCreate.serviceLastChecked);
            }
            if (carToCreate.taxCheckedBy == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxcheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@taxcheckedby", carToCreate.taxCheckedBy);
            }
            if (carToCreate.motCheckedBy == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motcheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@motcheckedby", carToCreate.motCheckedBy);
            }
            if (carToCreate.insuranceCheckedBy == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancecheckedby", carToCreate.insuranceCheckedBy);
            }
            if (carToCreate.serviceCheckedby == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicecheckedby", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@servicecheckedby", carToCreate.serviceCheckedby);
            }
            if (carToCreate.motTestNumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mottestnumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mottestnumber", carToCreate.motTestNumber);
            }
            if (carToCreate.insuranceNumber == "")
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancenumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@insurancenumber", carToCreate.insuranceNumber);
            }
            expdata.sqlexecute.Parameters.AddWithValue("@exemptFromHomeToOffice", Convert.ToByte(carToCreate.exemptFromHomeToOffice));

            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", carToCreate.createdBy);

            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);

            expdata.sqlexecute.Parameters.Add("@identity", System.Data.SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = System.Data.ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveCar");
            carToCreate.carID = (int)expdata.sqlexecute.Parameters["@identity"].Value;

            expdata.sqlexecute.Parameters.Clear();

            return carToCreate.carID;
        }
 
        /// <summary>
        /// The populate car.
        /// </summary>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        internal static List<Cars> PopulateCar(int? employeeID = null, string sqlToExecute = "")
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT carid, employeeid, startdate, enddate, make, model, registration, mileageid, cartypeid, active, odometer, fuelcard, endodometer, taxexpiry, taxlastchecked, taxcheckedby, mottestnumber, motlastchecked, motcheckedby, motexpiry, insurancenumber, insuranceexpiry, insurancelastchecked, insurancecheckedby, serviceexpiry, servicelastchecked, servicecheckedby, default_unit, enginesize, approved, exemptFromHomeToOffice, taxAttachID, MOTAttachID, insuranceAttachID, serviceAttachID FROM cars";
            
            if (sqlToExecute == string.Empty)
            {
                sqlToExecute = strSQL;
            }
            else
            {
                db.sqlexecute.Parameters.Add("@employeeid", employeeID);
            }

            List<Cars> cars = new List<Cars>();
            using (SqlDataReader reader = db.GetReader(sqlToExecute))
            {
                #region set database columns
                int employeeIdOrdinal = reader.GetOrdinal("employeeid");
                int startDateOrdinal = reader.GetOrdinal("startdate");
                int endDateOrdinal = reader.GetOrdinal("enddate");
                int makeOrdinal = reader.GetOrdinal("make");
                int modelOrdinal = reader.GetOrdinal("model");
                int registerationOrdinal = reader.GetOrdinal("registration");
                int unitOfMeasureOrdinal = reader.GetOrdinal("default_unit");
                int engineTypeOrdinal = reader.GetOrdinal("cartypeid");
                int engineSizeOrdinal = reader.GetOrdinal("enginesize");
                int activeOrdinal = reader.GetOrdinal("active");
                int exemptFromHomeToOfficeOrdinal = reader.GetOrdinal("exemptFromHomeToOffice");

                int taxExpiryOrdinal = reader.GetOrdinal("taxexpiry");
                int taxLastCheckedOrdinal = reader.GetOrdinal("taxlastchecked");
                int taxCheckedByOrdinal = reader.GetOrdinal("taxcheckedby");
                int motTestNumberOrdinal = reader.GetOrdinal("mottestnumber");
                int motLastCheckedOrdinal = reader.GetOrdinal("motlastchecked");
                int motCheckedByOrdinal = reader.GetOrdinal("motcheckedby");
                int motExpiryOrdinal = reader.GetOrdinal("motexpiry");
                int insuranceNumberOrdinal = reader.GetOrdinal("insurancenumber");
                int insuranceExpiryOrdinal = reader.GetOrdinal("insuranceexpiry");
                int insuranceLastCheckedOrdinal = reader.GetOrdinal("insurancelastchecked");
                int insuranceCheckedByOrdinal = reader.GetOrdinal("insurancecheckedby");
                int serviceExpiryOrdinal = reader.GetOrdinal("serviceexpiry");
                int serviceLastCheckedOrdinal = reader.GetOrdinal("servicelastchecked");
                int serviceCheckedByOrdinal = reader.GetOrdinal("servicecheckedby");

                int fuelCardOrdinal = reader.GetOrdinal("fuelcard");
                int startOdometerReadingOrdinal = reader.GetOrdinal("odometer");
                int endOdometerReadingOrdinal = reader.GetOrdinal("endodometer");
                int vehicleTypeOrdinal = reader.GetOrdinal("vehicletypeid");

                #endregion
                while (reader.Read())
                {
                    var car = new Cars
                                  {
                                      carID = 0,
                                      make = reader.GetString(makeOrdinal),
                                      model = reader.GetString(modelOrdinal),
                                      regNumber = reader.GetString(registerationOrdinal),
                                      unitOfMeasure = reader.GetByte(unitOfMeasureOrdinal),
                                      carType = reader.GetByte(engineTypeOrdinal),
                                      engineSize = reader.GetInt32(engineSizeOrdinal),
                                      active = reader.GetBoolean(activeOrdinal),
                                      exemptFromHomeToOffice = reader.GetBoolean(exemptFromHomeToOfficeOrdinal),
                                      startDate =
                                          reader.IsDBNull(startDateOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(startDateOrdinal),
                                      endDate =
                                          reader.IsDBNull(endDateOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(endDateOrdinal),
                                      taxExpiry =
                                          reader.IsDBNull(taxExpiryOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(taxExpiryOrdinal),
                                      taxLastChecked =
                                          reader.IsDBNull(taxLastCheckedOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(taxLastCheckedOrdinal),
                                      taxCheckedBy =
                                          reader.IsDBNull(taxCheckedByOrdinal)
                                              ? null
                                              : (int?)reader.GetInt32(taxCheckedByOrdinal),
                                      motTestNumber =
                                          reader.IsDBNull(motTestNumberOrdinal)
                                              ? null
                                              : reader.GetString(motTestNumberOrdinal),
                                      motExpiry =
                                          reader.IsDBNull(motExpiryOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(motExpiryOrdinal),
                                      motLastChecked =
                                          reader.IsDBNull(motLastCheckedOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(motLastCheckedOrdinal),
                                      motCheckedBy =
                                          reader.IsDBNull(motCheckedByOrdinal)
                                              ? null
                                              : (int?)reader.GetInt32(motCheckedByOrdinal),
                                      insuranceNumber =
                                          reader.IsDBNull(insuranceNumberOrdinal)
                                              ? null
                                              : reader.GetString(insuranceNumberOrdinal),
                                      insuranceExpiry =
                                          reader.IsDBNull(insuranceExpiryOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(insuranceExpiryOrdinal),
                                      insuranceLastChecked =
                                          reader.IsDBNull(insuranceLastCheckedOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(insuranceLastCheckedOrdinal),
                                      insuranceCheckedBy =
                                          reader.IsDBNull(insuranceCheckedByOrdinal)
                                              ? null
                                              : (int?)reader.GetInt32(insuranceCheckedByOrdinal),
                                      serviceExpiry =
                                          reader.IsDBNull(serviceExpiryOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(serviceExpiryOrdinal),
                                      serviceLastChecked =
                                          reader.IsDBNull(serviceLastCheckedOrdinal)
                                              ? null
                                              : (DateTime?)reader.GetDateTime(serviceLastCheckedOrdinal),
                                      serviceCheckedby =
                                          reader.IsDBNull(serviceCheckedByOrdinal)
                                              ? null
                                              : (int?)reader.GetInt32(serviceCheckedByOrdinal),
                                      fuelCard = reader.GetBoolean(fuelCardOrdinal),
                                      odometer =
                                          reader.IsDBNull(startOdometerReadingOrdinal)
                                              ? null
                                              : (int?)reader.GetInt64(startOdometerReadingOrdinal),
                                      endOdometer =
                                          reader.IsDBNull(endOdometerReadingOrdinal)
                                              ? null
                                              : (int?)reader.GetInt32(endOdometerReadingOrdinal)
                                  };
                    
                    #region set values
                    
                    cars.Add(car);
                    #endregion
                }
                reader.Close();
                db.sqlexecute.Parameters.Clear();
            }
            return cars;
        }

        /// <summary>
        /// The add pool car to user
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="employeeID"></param>
        /// <param name="executingProduct"></param>
        internal static void AddPoolCarUser(int carID, int employeeID, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@createdby", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@createdon", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.ExecuteProc("addPoolCarUser");
            expdata.sqlexecute.Parameters.Clear();
        }

        /// <summary>
        /// The delete pool car to user
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="employeeID"></param>
        /// <param name="executingProduct"></param>
        internal static void DeletePoolCarUser(int carID, int employeeID, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
            expdata.sqlexecute.Parameters.AddWithValue("@employeeid", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.ExecuteProc("deleteUserFromPoolCar");
            expdata.sqlexecute.Parameters.Clear();
        }


        /// <summary>
        /// The delete car
        /// </summary>
        /// <param name="carID"></param>
        /// <param name="userID"></param>
        /// <param name="employeeID"></param>
        /// <param name="executingProduct"></param>
        internal static void DeleteCar(int carID, int userID, int employeeID, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@carid", carID);
            expdata.sqlexecute.Parameters.AddWithValue("@userid", userID);
            expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.ExecuteProc("deleteCar");
            expdata.sqlexecute.Parameters.Clear();
        }
    }
}
