namespace ApiCrud.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Objects;
    using System.Data.SqlClient;

    using ApiCrud.DataClasses;
    using ApiCrud.Interfaces;

    /// <summary>
    /// The employee data access class.
    /// </summary>
    public class EmployeeDataAccess : DataAccess, IDataAccess<Employee>
    {
        /// <summary>
        /// The METABASE.
        /// </summary>
        private string metaBase;

        /// <summary>
        /// The account id.
        /// </summary>
        private int accountId;

        /// <summary>
        /// Initialises a new instance of the <see cref="EmployeeDataAccess"/> class.
        /// </summary>
        /// <param name="baseUrl">
        /// The base url.
        /// </param>
        /// <param name="dataConnection">
        /// The data Connection.
        /// </param>
        public EmployeeDataAccess(string baseUrl, IApiDbConnection dataConnection)
            : base(baseUrl, dataConnection)
        {
            this.metaBase = dataConnection.MetaBase;
            this.accountId = dataConnection.AccountId;
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Create(Employee entity)
        {
            return this.SaveEmployee(entity);
        }

        /// <summary>
        /// The read.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Read(int entityId)
        {
            var filter = new DataAccessFilter { new DataAccessFilter.Filter { FieldName = "employeeid", FilterValue = entityId } };
            var result = this.GetEmployees(filter);
            return result.Count > 0 ? result[0] : null;
        }

        /// <summary>
        /// The read all.
        /// </summary>
        /// <param name="filter">
        /// The filter.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>List</cref>
        ///     </see>
        ///     .
        /// </returns>
        public List<Employee> ReadAll(DataAccessFilter filter)
        {
            var result = this.GetEmployees(filter);
            return result;
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Update(Employee entity)
        {
            return this.SaveEmployee(entity);
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entityId">
        /// The entity id.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        public Employee Delete(int entityId)
        {
            var employee = this.Read(entityId);
            return this.DeleteEmployee(employee);
        }

        /// <summary>
        /// The save employee method.
        /// </summary>
        /// <param name="employee">
        /// The employee.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private Employee SaveEmployee(Employee employee)
        {
            int employeeid = 0;
            bool newEmployee = employee.EmployeeId == 0;

            this.expdata.Sqlexecute.Parameters.AddWithValue("@employeeid", employee.EmployeeId);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@title", employee.title);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@title", employee.title);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@firstname", employee.firstname);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@surname", employee.surname);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@address1", employee.address1);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@address2", employee.address2);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@city", employee.city);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@county", employee.county);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@postcode", employee.postcode);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@telno", employee.telno);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@email", employee.email);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@creditor", employee.creditor);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@payroll", employee.payroll);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@position", employee.position);

            if (employee.groupid == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@groupid", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@groupid", employee.groupid);
            }

            if (employee.linemanager == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@linemanager", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@linemanager", employee.linemanager);
            }

            this.expdata.Sqlexecute.Parameters.AddWithValue("@mileagetotal", employee.mileagetotal);
            if (employee.mileagetotaldate.HasValue)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@mileagetotaldate", employee.mileagetotaldate);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@mileagetotaldate", DBNull.Value);
            }

            this.expdata.Sqlexecute.Parameters.AddWithValue("@homeemail", employee.homeemail);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@faxno", employee.faxno);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@pagerno", employee.pagerno);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@mobileno", employee.mobileno);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@extension", employee.extension);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@advancegroup", employee.advancegroupid);



            if (employee.primarycurrency == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@primarycurrency", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@primarycurrency", employee.primarycurrency);
            }
            if (employee.primarycountry == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@primarycountry", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@primarycountry", employee.primarycountry);
            }
            if (employee.licencecheckedby == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licencecheckedby", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licencecheckedby", employee.licencecheckedby);
            }
            if (employee.licencelastchecked == null)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licencelastchecked", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licencelastchecked", employee.licencelastchecked);
            }
            if (employee.licenceexpiry == null)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licenceexpiry", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licenceexpiry", employee.licenceexpiry);
            }
            if (employee.licencenumber == string.Empty)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licencenumber", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@licencenumber", employee.licencenumber);
            }
            if (employee.groupidcc == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@groupidcc", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@groupidcc", employee.groupidcc);
            }
            if (employee.groupidpc == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@groupidpc", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@groupidpc", employee.groupidpc);
            }
            if (employee.ninumber == string.Empty)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@ninumber", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@ninumber", employee.ninumber);
            }

            if (employee.middlenames == string.Empty)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@middlenames", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@middlenames", employee.middlenames);
            }
            if (employee.maidenname == string.Empty)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@maidenname", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@maidenname", employee.maidenname);
            }
            if (employee.gender == string.Empty)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@gender", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@gender", employee.gender);
            }
            if (employee.dateofbirth == null)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@dateofbirth", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@dateofbirth", employee.dateofbirth);
            }
            if (employee.hiredate == null)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@hiredate", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@hiredate", employee.hiredate);
            }
            if (employee.terminationdate == null)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@terminationdate", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@terminationdate", employee.terminationdate);
            }
            if (employee.country == string.Empty)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@country", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@country", employee.country);
            }
            this.expdata.Sqlexecute.Parameters.AddWithValue("@active", employee.active);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@username", employee.username);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@name", employee.name);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@accountnumber", employee.accountnumber);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@accounttype", employee.accounttype);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@sortcode", employee.sortcode);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@reference", employee.reference);
            if (employee.EmployeeId > 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@date", employee.ModifiedOn);
                this.expdata.Sqlexecute.Parameters.AddWithValue("@userid", employee.ModifiedBy);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@date", employee.CreatedOn);
                this.expdata.Sqlexecute.Parameters.AddWithValue("@userid", employee.CreatedBy);
            }
            if (employee.localeID == null || employee.localeID == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@localeID", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@localeID", employee.localeID);
            }

            if (employee.NHSTrustID.HasValue == false || employee.NHSTrustID == 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@NHSTrustID", DBNull.Value);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@NHSTrustID", employee.NHSTrustID.Value);
            }
            if (employee.defaultSubAccountId >= 0)
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@defaultSubAccountId", employee.defaultSubAccountId);
            }
            else
            {
                this.expdata.Sqlexecute.Parameters.AddWithValue("@defaultSubAccountId", DBNull.Value);
            }
            this.expdata.Sqlexecute.Parameters.AddWithValue("@CreationMethod", employee.CreationMethod);

            this.expdata.Sqlexecute.Parameters.AddWithValue("@CUemployeeID", -1);
            this.expdata.Sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);

            this.expdata.Sqlexecute.Parameters.Add("@identity", SqlDbType.Int);
            this.expdata.Sqlexecute.Parameters["@identity"].Direction = ParameterDirection.ReturnValue;
            this.expdata.ExecuteProc("APIsaveEmployee");
            int returnvalue;
            try
            {
                returnvalue = (int)expdata.Sqlexecute.Parameters["@identity"].Value;
                this.expdata.Sqlexecute.Parameters.Clear();
            }
            catch (Exception ex)
            {
                employee.ActionResult = ApiResult.Failure;
                employee.EmployeeId = -3;
                return employee;
            }

            if (returnvalue <= 0)
            {
                employee.EmployeeId = returnvalue;
                employee.ActionResult = ApiResult.Failure;
                return employee;
            }

            employeeid = returnvalue;
            employee.EmployeeId = employeeid;
            //if (newEmployee)
            //{
            //    Guid newPassword = Guid.NewGuid();
            //    changePassword(accountid, employeeid, newPassword.ToString(), newPassword.ToString(), false, 0);
            //}

            //cFields clsFields = new cFields(accountid);
            //cTables clstables = new cTables(accountid);
            //cTable tbl = clstables.GetTableByID(new Guid("618db425-f430-4660-9525-ebab444ed754"));
            //cUserdefinedFields clsuserdefined = new cUserdefinedFields(accountid);
            //clsuserdefined.SaveValues(tbl.UserdefinedTable, employeeid, userDefinedFields, clstables, clsFields, currentUser);

            //int action = 0;
            //if (employee.employeeid > 0)
            //{
            //    action = 2;
            //    //update base currency on single claim
            //    cMisc clsmisc = new cMisc(accountid);
            //    cClaims clsclaims = new cClaims(accountid);
            //    cGlobalProperties clsProperties = clsmisc.GetGlobalProperties(accountid);

            //    if (clsProperties.singleclaim && employee.primarycurrency > 0)
            //    {
            //        cClaim claim = clsclaims.getClaimById(clsclaims.getDefaultClaim(ClaimStage.Current, employee.employeeid));
            //        if (claim != null && claim.noitems == 0)
            //        {
            //            const string strsql = "update claims_base set currencyid = @currencyid where claimid = @claimid";
            //            this.expdata.Sqlexecute.Parameters.AddWithValue("@currencyid", employee.primarycurrency);
            //            this.expdata.Sqlexecute.Parameters.AddWithValue("@claimid", claim.claimid);
            //            this.expdata.ExecuteSQL(strsql);
            //            this.expdata.Sqlexecute.Parameters.Clear();
            //        }
            //    }

            //}
            //InsertCostCodeBreakdown(employeeid, action, costCodeBreakdown);

            employee.ActionResult = ApiResult.Success;
            return employee;
        }

        /// <summary>
        /// Get employees from specified account with optional filter.
        /// </summary>
        /// <param name="filter">
        /// The optional filter.
        /// </param>
        /// <returns>
        /// The <see cref="List{T}"/>.
        /// </returns>
        private List<Employee> GetEmployees(DataAccessFilter filter)
        {
            var result = new List<Employee>();
            
            if (this.expdata.ConnectionStringValid)
            {
                int employeeid = 0;
                string username = string.Empty;
                string password = string.Empty;
                string title = string.Empty;
                string firstname = string.Empty;
                string surname = string.Empty;
                string address1 = string.Empty;
                string address2 = string.Empty;
                string city = string.Empty;
                string county = string.Empty;
                string postcode = string.Empty;
                bool archived;

                string creditor = string.Empty;
                int curclaimno, currefnum;
                int groupid = 0;
                string email = string.Empty;
                DateTime lastchange = DateTime.Today;
                int additems = 0;
                int mileagetotal = 0;
                DateTime? mileagetotaldate = null;
                string payroll = string.Empty;
                string position = string.Empty;
                string telno = string.Empty;
                int linemanager = 0;
                int advancegroup = 0;
                int groupidpc = 0;
                int groupidcc = 0;
                string faxno = string.Empty;
                string homeemail = string.Empty;
                string pagerno = string.Empty;
                string mobileno = string.Empty;
                string country = string.Empty;
                string extension = string.Empty;
                string middlenames = string.Empty;
                string maidenname = string.Empty;
                string gender = string.Empty;
                string name = string.Empty;
                string accountnumber = string.Empty;
                string sortcode = string.Empty;
                string reference = string.Empty;
                string accounttype = string.Empty;
                DateTime? dateofbirth = null;
                DateTime? hiredate = null;
                DateTime? terminationdate = null;
                var createdOn = new DateTime(1900, 01, 01);
                var modifiedOn = new DateTime(1900, 01, 01);
                int createdBy = 0;
                int modifiedBy = 0;
                int? nhsTrustId = null;

                int? localeID = null;
                int primarycurrency = 0;
                int primarycountry = 0;

                bool active, verified;

                DateTime? licenceexpiry = null;
                DateTime? licencelastchecked = null;
                string licencenumber = string.Empty;
                string ninumber = string.Empty;

                int licencecheckedby = 0;
                int logoncount = 0;
                int retrycount = 0;
                bool firstLogon;
                int LicenceAttachID = 0;
                int defaultSubAccountID = -1;

                int CreationMethod = 0;
                int passwordmethod = 0;

                bool adminonly = false;
                bool locked = false;

                this.expdata.Sqlexecute.Parameters.AddWithValue("@Filter", filter);

                using (IDataReader reader = this.expdata.GetStoredProcReader("dbo.APIgetEmployees"))
                {
                    while (reader.Read())
                    {
                        employeeid = reader.GetInt32(reader.GetOrdinal("employeeid"));
                        username = reader.GetString(reader.GetOrdinal("username"));

                        if (reader.IsDBNull(reader.GetOrdinal("password")) == false)
                        {
                            password = reader.GetString(reader.GetOrdinal("password"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("title")) == false)
                        {
                            title = reader.GetString(reader.GetOrdinal("title"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("firstname")) == false)
                        {
                            firstname = reader.GetString(reader.GetOrdinal("firstname"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("surname")) == false)
                        {
                            surname = reader.GetString(reader.GetOrdinal("surname"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("address1")) == false)
                        {
                            address1 = reader.GetString(reader.GetOrdinal("address1"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("address2")) == false)
                        {
                            address2 = reader.GetString(reader.GetOrdinal("address2"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("city")) == false)
                        {
                            city = reader.GetString(reader.GetOrdinal("city"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("county")) == false)
                        {
                            county = reader.GetString(reader.GetOrdinal("county"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("postcode")) == false)
                        {
                            postcode = reader.GetString(reader.GetOrdinal("postcode"));
                        }

                        archived = reader.GetBoolean(reader.GetOrdinal("archived"));

                        if (reader.IsDBNull(reader.GetOrdinal("creditor")) == false)
                        {
                            creditor = reader.GetString(reader.GetOrdinal("creditor"));
                        }

                        curclaimno = reader.GetInt32(reader.GetOrdinal("curclaimno"));

                        currefnum = reader.GetInt32(reader.GetOrdinal("currefnum"));

                        if (reader.IsDBNull(reader.GetOrdinal("email")) == false)
                        {
                            email = reader.GetString(reader.GetOrdinal("email"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("groupid")) == false)
                        {
                            groupid = reader.GetInt32(reader.GetOrdinal("groupid"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("lastchange")) == false)
                        {
                            lastchange = reader.GetDateTime(reader.GetOrdinal("lastchange"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("additems")) == false)
                        {
                            additems = reader.GetInt32(reader.GetOrdinal("additems"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("mileagetotal")) == false)
                        {
                            mileagetotal = reader.GetInt32(reader.GetOrdinal("mileagetotal"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("mileagetotaldate")) == false)
                        {
                            mileagetotaldate = reader.GetDateTime(reader.GetOrdinal("mileagetotaldate"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("payroll")) == false)
                        {
                            payroll = reader.GetString(reader.GetOrdinal("payroll"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("position")) == false)
                        {
                            position = reader.GetString(reader.GetOrdinal("position"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("telno")) == false)
                        {
                            telno = reader.GetString(reader.GetOrdinal("telno"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("extension")) == false)
                        {
                            extension = reader.GetString(reader.GetOrdinal("extension"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("homeemail")) == false)
                        {
                            homeemail = reader.GetString(reader.GetOrdinal("homeemail"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("pagerno")) == false)
                        {
                            pagerno = reader.GetString(reader.GetOrdinal("pagerno"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("mobileno")) == false)
                        {
                            mobileno = reader.GetString(reader.GetOrdinal("mobileno"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("faxno")) == false)
                        {
                            faxno = reader.GetString(reader.GetOrdinal("faxno"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("linemanager")) == false)
                        {
                            linemanager = reader.GetInt32(reader.GetOrdinal("linemanager"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("advancegroupid")) == false)
                        {
                            advancegroup = reader.GetInt32(reader.GetOrdinal("advancegroupid"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("primarycountry")) == false)
                        {
                            primarycountry = reader.GetInt32(reader.GetOrdinal("primarycountry"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("primarycurrency")) == false)
                        {
                            primarycurrency = reader.GetInt32(reader.GetOrdinal("primarycurrency"));
                        }

                        active = reader.GetBoolean(reader.GetOrdinal("active"));

                        verified = reader.GetBoolean(reader.GetOrdinal("verified"));

                        if (reader.IsDBNull(reader.GetOrdinal("licenceexpiry")) == false)
                        {
                            licenceexpiry = reader.GetDateTime(reader.GetOrdinal("licenceexpiry"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("licencelastchecked")) == false)
                        {
                            licencelastchecked = reader.GetDateTime(reader.GetOrdinal("licencelastchecked"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("licencecheckedby")) == false)
                        {
                            licencecheckedby = reader.GetInt32(reader.GetOrdinal("licencecheckedby"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("licencenumber")) == false)
                        {
                            licencenumber = reader.GetString(reader.GetOrdinal("licencenumber"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("groupidcc")) == false)
                        {
                            groupidcc = reader.GetInt32(reader.GetOrdinal("groupidcc"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("groupidpc")) == false)
                        {
                            groupidpc = reader.GetInt32(reader.GetOrdinal("groupidpc"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("ninumber")) == false)
                        {
                            ninumber = reader.GetString(reader.GetOrdinal("ninumber"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("middlenames")) == false)
                        {
                            middlenames = reader.GetString(reader.GetOrdinal("middlenames"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("maidenname")) == false)
                        {
                            maidenname = reader.GetString(reader.GetOrdinal("maidenname"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("gender")) == false)
                        {
                            gender = reader.GetString(reader.GetOrdinal("gender"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("dateofbirth")) == false)
                        {
                            dateofbirth = reader.GetDateTime(reader.GetOrdinal("dateofbirth"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("hiredate")) == false)
                        {
                            hiredate = reader.GetDateTime(reader.GetOrdinal("hiredate"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("terminationdate")) == false)
                        {
                            terminationdate = reader.GetDateTime(reader.GetOrdinal("terminationdate"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("country")) == false)
                        {
                            country = reader.GetString(reader.GetOrdinal("country"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("name")) == false)
                        {
                            name = reader.GetString(reader.GetOrdinal("name"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("accountnumber")) == false)
                        {
                            accountnumber = reader.GetString(reader.GetOrdinal("accountnumber"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("accounttype")) == false)
                        {
                            accounttype = reader.GetString(reader.GetOrdinal("accounttype"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("sortcode")) == false)
                        {
                            sortcode = reader.GetString(reader.GetOrdinal("sortcode"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("reference")) == false)
                        {
                            reference = reader.GetString(reader.GetOrdinal("reference"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("createdon")) == false)
                        {
                            createdOn = reader.GetDateTime(reader.GetOrdinal("createdon"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("createdby")) == false)
                        {
                            createdBy = reader.GetInt32(reader.GetOrdinal("createdby"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("modifiedon")) == false)
                        {
                            modifiedOn = reader.GetDateTime(reader.GetOrdinal("modifiedon"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("modifiedby")) == false)
                        {
                            modifiedBy = reader.GetInt32(reader.GetOrdinal("modifiedby"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("NHSTrustID")) == false)
                        {
                            nhsTrustId = reader.GetInt32(reader.GetOrdinal("NHSTrustID"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("localeID")) == false)
                        {
                            localeID = reader.GetInt32(reader.GetOrdinal("localeID"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("passwordMethod")) == false)
                        {
                            passwordmethod = reader.GetByte(reader.GetOrdinal("passwordMethod"));
                        }

                        logoncount = reader.GetInt32(reader.GetOrdinal("logonCount"));

                        retrycount = reader.GetInt32(reader.GetOrdinal("retryCount"));

                        firstLogon = reader.GetBoolean(reader.GetOrdinal("firstLogon"));

                        if (reader.IsDBNull(reader.GetOrdinal("licenceAttachID")) == false)
                        {
                            LicenceAttachID = reader.GetInt32(reader.GetOrdinal("licenceAttachID"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("defaultSubAccountId")) == false)
                        {
                            defaultSubAccountID = reader.GetInt32(reader.GetOrdinal("defaultSubAccountId"));
                        }

                        if (reader.IsDBNull(reader.GetOrdinal("CreationMethod")) == false)
                        {
                            CreationMethod = reader.GetByte(reader.GetOrdinal("CreationMethod"));
                        }

                        adminonly = reader.GetBoolean(reader.GetOrdinal("adminonly"));

                        locked = reader.GetBoolean(reader.GetOrdinal("Locked"));

                        result.Add(
                            new Employee(
                                employeeid,
                                username,
                                password,
                                passwordmethod,
                                title,
                                firstname,
                                surname,
                                mileagetotal,
                                mileagetotaldate,
                                email,
                                currefnum,
                                curclaimno,
                                address1,
                                address2,
                                city,
                                county,
                                postcode,
                                payroll,
                                position,
                                telno,
                                creditor,
                                archived,
                                groupid,
                                lastchange,
                                additems,
                                faxno,
                                homeemail,
                                extension,
                                pagerno,
                                mobileno,
                                linemanager,
                                advancegroup,
                                primarycountry,
                                primarycurrency,
                                verified,
                                active,
                                licenceexpiry,
                                licencelastchecked,
                                licencecheckedby,
                                licencenumber,
                                groupidcc,
                                groupidpc,
                                ninumber,
                                middlenames,
                                maidenname,
                                gender,
                                dateofbirth,
                                hiredate,
                                terminationdate,
                                country,
                                createdOn,
                                createdBy,
                                modifiedOn,
                                modifiedBy,
                                name,
                                accountnumber,
                                accounttype,
                                sortcode,
                                reference,
                                localeID,
                                nhsTrustId,
                                logoncount,
                                retrycount,
                                firstLogon,
                                LicenceAttachID,
                                defaultSubAccountID,
                                CreationMethod,
                                adminonly,
                                locked,
                                this.FormatUrl(employeeid)));
                    }
                    reader.Close();
                }

                this.expdata.Sqlexecute.Parameters.Clear();
            }

            return result;
        }

        /// <summary>
        /// The delete employee.
        /// </summary>
        /// <param name="employee">
        /// The employee.
        /// </param>
        /// <returns>
        /// The <see cref="Employee"/>.
        /// </returns>
        private Employee DeleteEmployee(Employee employee)
        {
            expdata.Sqlexecute.Parameters.AddWithValue("@CUemployeeID", 0);
            expdata.Sqlexecute.Parameters.AddWithValue("@CUdelegateID", DBNull.Value);
            expdata.Sqlexecute.Parameters.AddWithValue("@employeeid", employee.EmployeeId);
            expdata.Sqlexecute.Parameters.Add("@returnvalue", SqlDbType.Int);
            expdata.Sqlexecute.Parameters["@returnvalue"].Direction = ParameterDirection.ReturnValue;
            expdata.ExecuteProc("APIdeleteEmployee");
            int returnvalue = 0;
            if (expdata.Sqlexecute.Parameters["@returnvalue"].Value != null)
            {
                returnvalue = (int)expdata.Sqlexecute.Parameters["@returnvalue"].Value;
            }
            
            expdata.Sqlexecute.Parameters.Clear();
            employee.EmployeeId = returnvalue;
            employee.ActionResult = returnvalue == 0 ? ApiResult.Deleted : ApiResult.Failure;
            return employee;
        }
    }
}