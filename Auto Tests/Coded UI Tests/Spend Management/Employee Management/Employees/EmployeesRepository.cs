namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Text;
    /// <summary>
    /// The employees repository.
    /// </summary>
    public class EmployeesRepository
    {
        /// <summary>
        /// The create employee.
        /// </summary>
        /// <param name="employeeToCreate">
        /// The employee to create.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int CreateEmployee(Employees employeeToCreate, ProductType executingProduct)
        {
            employeeToCreate.employeeID = 0;
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeToCreate.employeeID);
            expdata.sqlexecute.Parameters.AddWithValue("@title", employeeToCreate.Title);
            expdata.sqlexecute.Parameters.AddWithValue("@firstname", employeeToCreate.FirstName);
            expdata.sqlexecute.Parameters.AddWithValue("@surname", employeeToCreate.Surname);
            // no longer used
            // expdata.sqlexecute.Parameters.AddWithValue("@address1", DBNull.Value);
            // expdata.sqlexecute.Parameters.AddWithValue("@address2", DBNull.Value);
            // expdata.sqlexecute.Parameters.AddWithValue("@city", DBNull.Value);
            // expdata.sqlexecute.Parameters.AddWithValue("@county", DBNull.Value);
            // expdata.sqlexecute.Parameters.AddWithValue("@postcode", DBNull.Value);
            expdata.sqlexecute.Parameters.AddWithValue("@telno", employeeToCreate.telephoneNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@email", employeeToCreate.emailAddress);
            expdata.sqlexecute.Parameters.AddWithValue("@creditor", employeeToCreate.creditAccount);
            expdata.sqlexecute.Parameters.AddWithValue("@payroll", employeeToCreate.payrollNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@position", employeeToCreate.position);

            if (employeeToCreate.signoffGroup == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupid", DBNull.Value);
            }
            else
            {
                // set to employeeToCreate.groupid when required in future
                expdata.sqlexecute.Parameters.AddWithValue("@groupid", DBNull.Value);
            }

            if (employeeToCreate.lineManager == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@linemanager", DBNull.Value);
            }
            else
            {
                // set to employeeToCreate.lineManager when required in future
                expdata.sqlexecute.Parameters.AddWithValue("@linemanager", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@mileagetotal", employeeToCreate.currentMileage);
            if (employeeToCreate.startingMileageDate.HasValue)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileagetotaldate", employeeToCreate.startingMileageDate);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@mileagetotaldate", DBNull.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@homeemail", employeeToCreate.pEmailAddress);
            expdata.sqlexecute.Parameters.AddWithValue("@faxno", employeeToCreate.faxNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@pagerno", employeeToCreate.PagerNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@mobileno", employeeToCreate.mobileNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@extension", employeeToCreate.extensionNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@advancegroup", employeeToCreate.asignoffGroup);
            if (employeeToCreate.primaryCurrency == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycurrency", DBNull.Value);
            }
            else
            {
                // set to employeeToCreate.primaryCurrency when required in future
                expdata.sqlexecute.Parameters.AddWithValue("@primarycurrency", DBNull.Value);
            }

            if (employeeToCreate.primaryCountry == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@primarycountry", DBNull.Value);
            }
            else
            {
                // set to employeeToCreate.primaryCountry when required in future
                expdata.sqlexecute.Parameters.AddWithValue("@primarycountry", DBNull.Value);
            }

            if (employeeToCreate.checkedBy == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencecheckedby", DBNull.Value);
            }
            else
            {
                // set to employeeToCreate.checkedBy when required in future
                expdata.sqlexecute.Parameters.AddWithValue("@licencecheckedby", DBNull.Value);
            }

            if (employeeToCreate.lastChecked == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencelastchecked", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencelastchecked", employeeToCreate.lastChecked);
            }

            if (employeeToCreate.licenceExpiryDate == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licenceexpiry", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licenceexpiry", employeeToCreate.licenceExpiryDate);
            }

            if (employeeToCreate.licenceNumber == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencenumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@licencenumber", employeeToCreate.licenceNumber);
            }

            if (employeeToCreate.ccsignoffGroup == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidcc", employeeToCreate.ccsignoffGroup);
            }

            if (employeeToCreate.pcsignoffGroup == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@groupidpc", employeeToCreate.pcsignoffGroup);
            }

            if (employeeToCreate.nationalInsuranceNumner == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ninumber", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@ninumber", employeeToCreate.nationalInsuranceNumner);
            }

            if (employeeToCreate.middleName == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@middlenames", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@middlenames", employeeToCreate.middleName);
            }

            if (employeeToCreate.maidenName == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@maidenname", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@maidenname", employeeToCreate.maidenName);
            }

            if (employeeToCreate.gender == string.Empty)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@gender", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@gender", employeeToCreate.gender);
            }

            if (employeeToCreate.dateOfBirth == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@dateofbirth", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@dateofbirth", employeeToCreate.dateOfBirth);
            }

            if (employeeToCreate.hireDate == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hiredate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@hiredate", employeeToCreate.hireDate);
            }

            if (employeeToCreate.terminationDate == null)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@terminationdate", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@terminationdate", employeeToCreate.terminationDate);
            }

            // expdata.sqlexecute.Parameters.AddWithValue("@country", DBNull.Value);

            expdata.sqlexecute.Parameters.AddWithValue("@active", employeeToCreate.active);
            expdata.sqlexecute.Parameters.AddWithValue("@username", employeeToCreate.UserName);
            expdata.sqlexecute.Parameters.AddWithValue("@name", employeeToCreate.accountHolderName);
            expdata.sqlexecute.Parameters.AddWithValue("@accountnumber", employeeToCreate.accountNumber);
            expdata.sqlexecute.Parameters.AddWithValue("@accounttype", employeeToCreate.accountType);
            expdata.sqlexecute.Parameters.AddWithValue("@sortcode", employeeToCreate.sortCode);
            expdata.sqlexecute.Parameters.AddWithValue("@reference", employeeToCreate.accountReference);
            if (employeeToCreate.employeeID > 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeToCreate.modifiedBy);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@date", DateTime.Now);
                expdata.sqlexecute.Parameters.AddWithValue("@userid", employeeToCreate.createdBy);
            }

            if (employeeToCreate.localeID.HasValue == false || employeeToCreate.localeID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@localeID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@localeID", employeeToCreate.localeID.Value);
            }

            if (employeeToCreate.trustID.HasValue == false || employeeToCreate.trustID == 0)
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", DBNull.Value);
            }
            else
            {
                expdata.sqlexecute.Parameters.AddWithValue("@NHSTrustID", employeeToCreate.trustID.Value);
            }

            expdata.sqlexecute.Parameters.AddWithValue("@employeeNumber", employeeToCreate.employeeNumner);
            expdata.sqlexecute.Parameters.AddWithValue("@nhsuniqueId", employeeToCreate.nhsUniqueID);
            expdata.sqlexecute.Parameters.AddWithValue("@preferredName", employeeToCreate.preferredName);
            int subAccountId = AutoTools.GetSubAccountId(executingProduct);
            expdata.sqlexecute.Parameters.AddWithValue("@defaultSubAccountId", subAccountId);
            expdata.sqlexecute.Parameters.AddWithValue("@CreationMethod", 99);

            expdata.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeToCreate.createdBy);
            expdata.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);

            expdata.sqlexecute.Parameters.AddWithValue("@verified", true);
            expdata.sqlexecute.Parameters.AddWithValue("@locked", false);
            expdata.sqlexecute.Parameters.AddWithValue("@logonRetryCount", 5);
            expdata.sqlexecute.Parameters.AddWithValue("@password", "vlUnCTgO1bFJXxfib/suGQ==");
            expdata.sqlexecute.Parameters.AddWithValue("@passwordMethod", 4);
            expdata.sqlexecute.Parameters.AddWithValue("@archived", false);
            expdata.sqlexecute.Parameters.AddWithValue("@currclaimno", 0);
            expdata.sqlexecute.Parameters.AddWithValue("@lastchange", DateTime.Now);

            expdata.sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            expdata.sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("saveEmployee");
            int returnvalue;
            try
            {
                returnvalue = (int)expdata.sqlexecute.Parameters["@identity"].Value;
                expdata.sqlexecute.Parameters.Clear();
            }
            catch (Exception ex)
            {
                return -3;
            }

            if (returnvalue <= 0)
            {
                return returnvalue;
            }

            employeeToCreate.employeeID = returnvalue;

            SetEmployeeAccessRole(employeeToCreate.employeeID, 545, subAccountId, executingProduct);

            return employeeToCreate.employeeID;
        }

        /// <summary>
        /// The delete employee.
        /// </summary>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int DeleteEmployee(int employeeId, ProductType executingProduct)
        {
            DBConnection db = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));

            db.sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            db.sqlexecute.Parameters.AddWithValue("@CUemployeeID", employeeId);
            db.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
            db.sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
            db.sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;

            ChangeStatus(employeeId, executingProduct);
            db.ExecuteProc("deleteEmployee");
            int returnvalue = (int)db.sqlexecute.Parameters["@returnvalue"].Value;
            db.sqlexecute.Parameters.Clear();

            return returnvalue;
        }

        /// <summary>
        /// The change status.
        /// </summary>
        /// <param name="employeeId">
        /// The employeeId.
        /// </param>
        /// <param name="executingProduct">
        /// The executing product.
        /// </param>
        /// <param name="archive">
        /// The archive.
        /// </param>
        public static void ChangeStatus(int employeeId, ProductType executingProduct, bool archive = true)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
            string strsql = archive ? "update employees set archived = 1 where employeeId = @employeeId" : "update employees set archived = 0, retryCount = 0 where employeeId = @employeeId";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        public static void SetEmployeeAccessRole(int employeeId, int accessRoleId, int subAccountId, ProductType executingProduct)
        {
            DBConnection expdata = new DBConnection(cGlobalVariables.dbConnectionString(executingProduct));
            expdata.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
            expdata.sqlexecute.Parameters.AddWithValue("@accessRoleID", accessRoleId);
            expdata.sqlexecute.Parameters.AddWithValue("@subAccountID", subAccountId);

            string strsql = "insert into employeeAccessRoles values (@employeeID, @accessRoleID, @subAccountID)";

            expdata.ExecuteSQL(strsql);
            expdata.sqlexecute.Parameters.Clear();
        }

        //public static string SqlToExecute()
        //{
        //    string strSQL = "SELECT employeeId, username, title, firstname, surname, mileagetotal, email, payroll, position, telno, creditor, groupid, cardnum, extension, pagerno, mobileno, faxno, homeemail, linemanager, advancegroupid, mileage, primarycountry, primarycurrency, licenceexpiry, licencelastchecked,licencenumber, groupidcc, groupidpc, country, ninumber, maidenname, middlenames, gender, dateofbirth, hiredate, terminationdate, name, accountnumber, accounttype, sortcode, reference, localeID, NHSTrustID, mileagetotaldate, PreferredName, EmployeeNumber, NHSUniqueId from Employees";
        //    return strSQL;
        //}

        /// <summary>
        /// The populate employee.
        /// </summary>
        /// <param name="sqlToExecute">
        /// The Sql To Execute.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        public static List<Employees> PopulateEmployee(int? employeeId = null, string sqlToExecute = "")
        {
            cDatabaseConnection db = new cDatabaseConnection(ConfigurationManager.ConnectionStrings["DatasourceDatabase"].ToString());

            string strSQL = "SELECT employeeId, username, title, firstname, surname, mileagetotal, email, payroll, position, telno, creditor, groupid, cardnum, extension, pagerno, mobileno, faxno, homeemail, linemanager, advancegroupid, mileage, primarycountry, primarycurrency, licenceexpiry, licencelastchecked,licencenumber, groupidcc, groupidpc, country, ninumber, maidenname, middlenames, gender, dateofbirth, hiredate, terminationdate, name, accountnumber, accounttype, sortcode, reference, localeID, NHSTrustID, mileagetotaldate, PreferredName, EmployeeNumber, NHSUniqueId, active from Employees";

            if (sqlToExecute == string.Empty)
            {
                sqlToExecute = strSQL;
            }
            else
            {
                db.sqlexecute.Parameters.AddWithValue("@employeeid", employeeId);
            }
            List<Employees> employees = new List<Employees>();
            using (SqlDataReader reader = db.GetReader(sqlToExecute))
            {
                #region set database columns
                int employeeidOrdinal = reader.GetOrdinal("employeeId");
                int usernameOrdinal = reader.GetOrdinal("username");
                int titleOrdinal = reader.GetOrdinal("title");
                int firstnameOrdinal = reader.GetOrdinal("firstname");
                int surnameOrdinal = reader.GetOrdinal("surname");
                int mileagetotalOrdinal = reader.GetOrdinal("mileagetotal");
                int emailOrdinal = reader.GetOrdinal("email");
                int payrollOrdinal = reader.GetOrdinal("payroll");
                int positionOrdinal = reader.GetOrdinal("position");
                int telnoOrdinal = reader.GetOrdinal("telno");
                int creditorOrdinal = reader.GetOrdinal("creditor");
                int groupidOrdinal = reader.GetOrdinal("groupid");
                int cardnumOrdinal = reader.GetOrdinal("cardnum");
                int extensionOrdinal = reader.GetOrdinal("extension");
                int pagernoOrdinal = reader.GetOrdinal("pagerno");
                int mobilenoOrdinal = reader.GetOrdinal("mobileno");
                int faxnoOrdinal = reader.GetOrdinal("faxno");
                int homeemailOrdinal = reader.GetOrdinal("homeemail");
                int linemanagerOrdinal = reader.GetOrdinal("linemanager");
                int advancegroupidOrdinal = reader.GetOrdinal("advancegroupid");
                int mileageOrdinal = reader.GetOrdinal("mileage");
                int primarycountryOrdinal = reader.GetOrdinal("primarycountry");
                int primarycurrencyOrdinal = reader.GetOrdinal("primarycurrency");
                int licenceexpiryOrdinal = reader.GetOrdinal("licenceexpiry");
                int licencelastcheckedOrdinal = reader.GetOrdinal("licencelastchecked");
                int licencenumberOrdinal = reader.GetOrdinal("licencenumber");
                int groupidccOrdinal = reader.GetOrdinal("groupidcc");
                int groupidpcOrdinal = reader.GetOrdinal("groupidpc");
                int countryOrdinal = reader.GetOrdinal("country");
                int ninumberOrdinal = reader.GetOrdinal("ninumber");
                int maidennameOrdinal = reader.GetOrdinal("maidenname");
                int middlenamesOrdinal = reader.GetOrdinal("middlenames");
                int genderOrdinal = reader.GetOrdinal("gender");
                int dateofbirthOrdinal = reader.GetOrdinal("dateofbirth");
                int hiredateOrdinal = reader.GetOrdinal("hiredate");
                int terminationdatecOrdinal = reader.GetOrdinal("terminationdate");
                int accountholdernameOrdinal = reader.GetOrdinal("name");
                int accountnumberOrdinal = reader.GetOrdinal("accountnumber");
                int accounttypeOrdinal = reader.GetOrdinal("accounttype");
                int sortcodeOrdinal = reader.GetOrdinal("sortcode");
                int referenceOrdinal = reader.GetOrdinal("reference");
                int localeIDOrdinal = reader.GetOrdinal("localeID");
                int NHSTrustIDOrdinal = reader.GetOrdinal("NHSTrustID");
                int mileagetotaldateOrdinal = reader.GetOrdinal("mileagetotaldate");
                int NHSUniqueIDOrdinal = reader.GetOrdinal("NHSUniqueId");
                int preferredNameOrdinal = reader.GetOrdinal("PreferredName");
                int EmployeeNumberOrdinal = reader.GetOrdinal("EmployeeNumber");
                int activeOrdinal = reader.GetOrdinal("active");
                #endregion

                while (reader.Read())
                {
                    var id = reader.GetInt32(employeeidOrdinal);
                    var userName = reader.GetString(usernameOrdinal);
                    var firstName = reader.GetString(firstnameOrdinal);
                    var surname = reader.GetString(surnameOrdinal);
                    var title = reader.IsDBNull(titleOrdinal) ? null : reader.GetString(titleOrdinal);

                    Employees employee = new Employees(userName, title, firstName, surname, id);
                    #region set values
                    employee.currentMileage = reader.GetInt32(mileagetotalOrdinal);
                    employee.emailAddress = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal);
                    employee.payrollNumber = reader.IsDBNull(payrollOrdinal) ? null : reader.GetString(payrollOrdinal);
                    employee.position = reader.IsDBNull(positionOrdinal) ? null : reader.GetString(positionOrdinal);
                    employee.telephoneNumber = reader.IsDBNull(telnoOrdinal) ? null : reader.GetString(telnoOrdinal);
                    employee.signoffGroup = reader.IsDBNull(groupidOrdinal) ? null : (int?)reader.GetInt32(groupidOrdinal);
                    employee.extensionNumber = reader.IsDBNull(extensionOrdinal) ? null : reader.GetString(extensionOrdinal);
                    employee.PagerNumber = reader.IsDBNull(pagernoOrdinal) ? null : reader.GetString(pagernoOrdinal);
                    employee.mobileNumber = reader.IsDBNull(mobilenoOrdinal) ? null : reader.GetString(mobilenoOrdinal);
                    employee.faxNumber = reader.IsDBNull(faxnoOrdinal) ? null : reader.GetString(faxnoOrdinal);
                    employee.pEmailAddress = reader.IsDBNull(homeemailOrdinal) ? null : reader.GetString(homeemailOrdinal);
                    employee.lineManager = reader.IsDBNull(linemanagerOrdinal) ? null : (int?)reader.GetInt32(linemanagerOrdinal);
                    employee.asignoffGroup = reader.IsDBNull(advancegroupidOrdinal) ? null : (int?)reader.GetInt32(advancegroupidOrdinal);
                    employee.startingMileage = reader.GetInt32(mileageOrdinal);
                    employee.primaryCountry = reader.IsDBNull(primarycountryOrdinal) ? null : (int?)reader.GetInt32(primarycountryOrdinal);
                    employee.primaryCountry = reader.IsDBNull(primarycurrencyOrdinal) ? null : (int?)reader.GetInt32(primarycurrencyOrdinal);
                    employee.licenceExpiryDate = reader.IsDBNull(licenceexpiryOrdinal) ? null : (DateTime?)reader.GetDateTime(licenceexpiryOrdinal);
                    employee.lastChecked = reader.IsDBNull(licencelastcheckedOrdinal) ? null : (DateTime?)reader.GetDateTime(licencelastcheckedOrdinal);
                    employee.creditAccount = reader.IsDBNull(creditorOrdinal) ? null : reader.GetString(creditorOrdinal);
                    employee.licenceNumber = reader.IsDBNull(licencenumberOrdinal) ? null : reader.GetString(licencenumberOrdinal);
                    employee.ccsignoffGroup = reader.IsDBNull(groupidccOrdinal) ? null : (int?)reader.GetInt32(groupidccOrdinal);
                    employee.pcsignoffGroup = reader.IsDBNull(groupidpcOrdinal) ? null : (int?)reader.GetInt32(groupidpcOrdinal);

                    employee.nationalInsuranceNumner = reader.IsDBNull(ninumberOrdinal) ? null : reader.GetString(ninumberOrdinal);
                    employee.maidenName = reader.IsDBNull(maidennameOrdinal) ? null : reader.GetString(maidennameOrdinal);
                    employee.middleName = reader.IsDBNull(middlenamesOrdinal) ? null : reader.GetString(middlenamesOrdinal);
                    employee.gender = reader.IsDBNull(genderOrdinal) ? null : reader.GetString(genderOrdinal);
                    employee.dateOfBirth = reader.IsDBNull(dateofbirthOrdinal) ? null : (DateTime?)reader.GetDateTime(dateofbirthOrdinal);
                    employee.hireDate = reader.IsDBNull(hiredateOrdinal) ? null : (DateTime?)reader.GetDateTime(hiredateOrdinal);
                    employee.terminationDate = reader.IsDBNull(terminationdatecOrdinal) ? null : (DateTime?)reader.GetDateTime(terminationdatecOrdinal);
                    employee.accountHolderName = reader.IsDBNull(accountholdernameOrdinal) ? null : reader.GetString(accountholdernameOrdinal);
                    employee.accountNumber = reader.IsDBNull(accountnumberOrdinal) ? null : reader.GetString(accountnumberOrdinal);
                    employee.accountType = reader.IsDBNull(accounttypeOrdinal) ? null : reader.GetString(accounttypeOrdinal);
                    employee.sortCode = reader.IsDBNull(sortcodeOrdinal) ? null : reader.GetString(sortcodeOrdinal);
                    employee.accountReference = reader.IsDBNull(referenceOrdinal) ? null : reader.GetString(referenceOrdinal);
                    employee.localeID = reader.IsDBNull(localeIDOrdinal) ? null : (int?)reader.GetInt32(localeIDOrdinal);
                    employee.trustID = reader.IsDBNull(NHSTrustIDOrdinal) ? null : (int?)reader.GetInt32(NHSTrustIDOrdinal);
                    employee.startingMileageDate = reader.IsDBNull(mileagetotaldateOrdinal) ? null : (DateTime?)reader.GetDateTime(mileagetotaldateOrdinal);
                    employee.employeeNumner = reader.IsDBNull(EmployeeNumberOrdinal) ? null : reader.GetString(EmployeeNumberOrdinal);
                    employee.nhsUniqueID = reader.IsDBNull(NHSUniqueIDOrdinal) ? null : reader.GetString(NHSUniqueIDOrdinal);
                    employee.preferredName = reader.IsDBNull(preferredNameOrdinal) ? null : reader.GetString(preferredNameOrdinal);

                    employee.active = reader.GetBoolean(activeOrdinal);
                    #endregion
                    employees.Add(employee);
                }

                reader.Close();
                db.sqlexecute.Parameters.Clear();
            }

            return employees;
        }
    }
}
