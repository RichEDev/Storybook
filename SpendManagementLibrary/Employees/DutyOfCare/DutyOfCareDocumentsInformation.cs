namespace SpendManagementLibrary.Employees.DutyOfCare
{
    using Helpers;
    using Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data;
    /// <summary>
    /// gets the information regarding duty of care documents from the database
    /// </summary>
    public class DutyOfCareDocumentsInformation
    {
        /// <summary>
        /// gets the list of duty of care documents which can expire and are related to carId
        /// </summary>
        /// <param name="accountId">account id of the current user</param>
        /// <param name="carId">carId</param>
        /// <param name="expenseItemDate">date of expense item</param>
        /// <returns>List of Vehicle DoC documents for which the DoC checks fails</returns>
        public List<IDutyOfCareDocument> GetCarExpiryInformation(int accountId, int carId, DateTime expenseItemDate)
        {
            using (IDBConnection connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@carid", carId);
                connection.sqlexecute.Parameters.AddWithValue("@expenseItemDate", Convert.ToDateTime(expenseItemDate.Date));
                using (IDataReader expiryInformation = connection.GetReader("GetCarDocumentsExpiryInformation", CommandType.StoredProcedure))
                {
                    return this.PopulateDutyOfCareDocuments(expiryInformation);
                }
            }
        }

        /// <summary>
        /// Gets the list of duty of care driving licence documents which are due to expire
        /// </summary>
        /// <param name="accountId">Account Id of the current user</param>
        /// <param name="employeeId">Employee Id</param>
        /// <param name="expenseItemDate">Date of expense item</param>
        /// <returns>List of Duty of care documents for which the DoC checks fails</returns>
        public List<IDutyOfCareDocument> GetLicenceExpiryInformation(int accountId, int employeeId, DateTime expenseItemDate, bool hasDvlaLookupKeyAndDvlaConnectLicenceElement)
        {
            using (IDBConnection connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                connection.sqlexecute.Parameters.AddWithValue("@expenseItemDate", Convert.ToDateTime(expenseItemDate.Date));
                connection.sqlexecute.Parameters.AddWithValue("@hasDvlaLookupKeyAndDvlaConnectLicenceElement", Convert.ToBoolean(hasDvlaLookupKeyAndDvlaConnectLicenceElement));
                using (IDataReader expiryInformation = connection.GetReader("GetDrivingLicenceExpiryInformation", CommandType.StoredProcedure))
                {
                    return this.PopulateDutyOfCareLicenceDocuments(expiryInformation, hasDvlaLookupKeyAndDvlaConnectLicenceElement);
                }
            }
        }

        /// <summary>
        /// gets the class 1 business information related to carId
        /// </summary>
        /// <param name="accountId">account id of the current user</param>
        /// <param name="carId">carId</param>
        /// <param name="expenseItemDate">date of the expense item</param>
        /// <returns>Vehicle registration number for which the class 1 business check fails</returns>
        public string GetClass1BusinessInformation(int accountId, int carId, DateTime expenseItemDate)
        {
            using (IDBConnection connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@carid", carId);
                connection.sqlexecute.Parameters.AddWithValue("@expenseItemDate", Convert.ToDateTime(expenseItemDate.Date));
                using (IDataReader class1Information = connection.GetReader("GetClass1BusinessInformation", CommandType.StoredProcedure))
                {
                    if (class1Information == null)
                    {
                        return string.Empty;
                    }

                    var registrationOrd = class1Information.GetOrdinal("Registration");

                    while (class1Information.Read())
                    {
                        if (!class1Information.IsDBNull(registrationOrd))
                        {
                            return class1Information.GetString(registrationOrd);
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Get the entity and view id using system guid
        /// </summary>
        /// <param name="entityGuid">sytem guid for entity</param>
        /// <param name="viewGuid">system guid for view</param>
        /// <param name="accountId">accountid</param>
        /// <returns>entity and view id</returns>
        public string GetDocEntityAndViewIdByGuid(string entityGuid, string viewGuid, int accountId)
        {
            string returnvalue = null;
            using (var connection = new DatabaseConnection(cAccounts.getConnectionString(accountId)))
            {
                connection.sqlexecute.Parameters.AddWithValue("@SystemCustomEntityID", entityGuid);
                connection.sqlexecute.Parameters.AddWithValue("@fullName", 0);
                connection.sqlexecute.Parameters.Add("@returnValue1", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                using (connection.GetReader("getentityid", CommandType.StoredProcedure))
                {
                    returnvalue = connection.sqlexecute.Parameters["@returnValue1"].Value.ToString();
                }

                connection.sqlexecute.Parameters.Clear();
                connection.sqlexecute.Parameters.AddWithValue("@ViewSystemGuid", viewGuid);
                connection.sqlexecute.Parameters.Add("@returnValue", SqlDbType.Int).Direction = ParameterDirection.ReturnValue;
                using (connection.GetReader("GetViewId", CommandType.StoredProcedure))
                {
                    returnvalue = string.Format("{0},{1}", returnvalue, connection.sqlexecute.Parameters["@returnValue"].Value.ToString());
                }
            }
            return returnvalue;
        }

        /// <summary>
        /// gets the list of objects from reader
        /// </summary>
        /// <param name="reader">reader which has information about expiry date of all documents of the car</param>
        /// <returns></returns>
        private List<IDutyOfCareDocument> PopulateDutyOfCareDocuments(IDataReader reader)
        {
            List<IDutyOfCareDocument> docDocumentList = new List<IDutyOfCareDocument>();
            if (reader == null || reader.FieldCount == 0)
            {
                return docDocumentList;
            }

            var registrationOrd = reader.GetOrdinal("Registration");
            var taxDocumentExpiryDateOrd = reader.GetOrdinal("TaxDocumentExpiryDate");
            var blockTaxOrd = reader.GetOrdinal("BlockTax");
            var taxReviewedOrd = reader.GetOrdinal("TaxReviewed");
            var insuranceDocumentExpiryDateOrd = reader.GetOrdinal("InsuranceDocumentExpiryDate");
            var blockInsuranceOrd = reader.GetOrdinal("BlockInsurance");
            var insuranceReviewedOrd = reader.GetOrdinal("InsuranceReviewed");
            var motDocumentExpiryDateOrd = reader.GetOrdinal("MotDocumentExpiryDate");
            var blockMotOrd = reader.GetOrdinal("BlockMOT");
            var motReviewedOrd = reader.GetOrdinal("MOTReviewed");

            var blockBreakdownCoverOrd = reader.GetOrdinal("BlockBreakdownCover");
            var breakdownCoverReviewedOrd = reader.GetOrdinal("BreakdownCoverReviewed");
            var breakdownCoverExpiryDateOrd = reader.GetOrdinal("BreakdownCoverDocumentExpiryDate");

            while (reader.Read())
            {
                var registration = string.Empty;
                if (!reader.IsDBNull(registrationOrd))
                {
                    registration = reader.GetString(registrationOrd);
                }
                bool isBlocked, isReviewed = false;
                DateTime expiryDate;

                expiryDate = reader.IsDBNull(taxDocumentExpiryDateOrd) == false ? reader.GetDateTime(taxDocumentExpiryDateOrd) : expiryDate = DateTime.MinValue;
                isBlocked = reader.IsDBNull(blockTaxOrd) == false && reader.GetBoolean(blockTaxOrd);
                isReviewed = reader.IsDBNull(taxReviewedOrd) == false && reader.GetBoolean(taxReviewedOrd);

                docDocumentList.Add(new TaxDocument(registration, expiryDate, isBlocked, isReviewed));

                expiryDate = reader.IsDBNull(insuranceDocumentExpiryDateOrd) == false ? reader.GetDateTime(insuranceDocumentExpiryDateOrd) : expiryDate = DateTime.MinValue;
                isBlocked = reader.IsDBNull(blockInsuranceOrd) == false && reader.GetBoolean(blockInsuranceOrd);
                isReviewed = reader.IsDBNull(insuranceReviewedOrd) == false && reader.GetBoolean(insuranceReviewedOrd);

                docDocumentList.Add(new InsuranceDocument(registration, expiryDate, isBlocked, isReviewed));

                expiryDate = reader.IsDBNull(motDocumentExpiryDateOrd) == false ? reader.GetDateTime(motDocumentExpiryDateOrd) : expiryDate = DateTime.MinValue;
                isBlocked = reader.IsDBNull(blockMotOrd) == false && reader.GetBoolean(blockMotOrd);
                isReviewed = reader.IsDBNull(motReviewedOrd) == false && reader.GetBoolean(motReviewedOrd);

                docDocumentList.Add(new MOTDocument(registration, expiryDate, isBlocked, isReviewed));

                expiryDate = reader.IsDBNull(breakdownCoverExpiryDateOrd) == false ? reader.GetDateTime(breakdownCoverExpiryDateOrd) : expiryDate = DateTime.MinValue;
                isBlocked = reader.IsDBNull(blockBreakdownCoverOrd) == false && reader.GetBoolean(blockBreakdownCoverOrd);
                isReviewed = reader.IsDBNull(breakdownCoverReviewedOrd) == false && reader.GetBoolean(breakdownCoverReviewedOrd);

                docDocumentList.Add(new BreakdownCoverDocument(registration, expiryDate, isBlocked, isReviewed));

            }
            return docDocumentList;
        }
        private List<IDutyOfCareDocument> PopulateDutyOfCareLicenceDocuments(IDataReader reader, bool hasDvlaLookupKeyAndDvlaConnectLicenceElement)
        {
            List<IDutyOfCareDocument> docDocumentList = new List<IDutyOfCareDocument>();
            if (reader == null || reader.FieldCount == 0)
            {
                return docDocumentList;
            }

            var paperDrivingLicenceDocumentStartDateOrd = reader.GetOrdinal("PaperDrivingLicenceDocumentValidFromDate");
            var nonGbDrivingLicenceStartDateOrd = reader.GetOrdinal("NonGbDrivingLicenceStartDate");
            var photocardDrivingLicenceExpiryDateOrd = reader.GetOrdinal("PhotocardDrivingLicenceExpiryDate");
            var nonGbDrivingLicenceExpiryDateOrd = reader.GetOrdinal("NonGbDrivingLicenceExpiryDate");
            var blockDrivingLicenceOrd = reader.GetOrdinal("BlockDrivingLicence");
            var photocardExistsOrd = reader.GetOrdinal("PhotocardExists");
            var nonGbDrivingLicenceExistsOrd = reader.GetOrdinal("NonGbDrivingLicenceExists");
            var dvlaLookupEnabledOrd = reader.GetOrdinal("DvlaLookupEnabled");
            var dvlaLicenceValidOrd = reader.GetOrdinal("ValidDvlaLicence");
            var dvlaLicenceExpiryOrd = reader.GetOrdinal("DvlaLicenceExpiry");
            var reviewStatusOrd = reader.GetOrdinal("ReviewStatus");
            var validDrivingLicenceOrd = reader.GetOrdinal("IsValidLicence");
            var isValidManualLicence = reader.GetOrdinal("IsValidManualLicence");

            while (reader.Read())
            {
                var registration = string.Empty;
                bool isBlocked, isReviewed = false;
                DateTime expiryDate, startDate;
                bool isPhotocard = reader.IsDBNull(photocardExistsOrd) == false && reader.GetBoolean(photocardExistsOrd);
                bool isNonGbDrivingLicence = reader.IsDBNull(nonGbDrivingLicenceExistsOrd) == false && reader.GetBoolean(nonGbDrivingLicenceExistsOrd);
                isBlocked = reader.IsDBNull(blockDrivingLicenceOrd) == false && reader.GetBoolean(blockDrivingLicenceOrd);
                bool isDvlaLookupEnabledOrd = reader.IsDBNull(dvlaLookupEnabledOrd) == false && reader.GetBoolean(dvlaLookupEnabledOrd);
                //isValid will be true if the DVLA lookup is disabled but the there is a valid drving licence(licence type DVLA lookup automatic)  
                var isValid = reader.IsDBNull(dvlaLicenceValidOrd) == false && reader.GetBoolean(dvlaLicenceValidOrd);
                string reviewStatus = reader.IsDBNull(reviewStatusOrd) == false ? reader.GetString(reviewStatusOrd) : string.Empty;
                var validDrivingLicence = reader.IsDBNull(validDrivingLicenceOrd) == false && reader.GetBoolean(validDrivingLicenceOrd);
                var isValidManualLicencePresent = reader.GetBoolean(isValidManualLicence);
                if ((isDvlaLookupEnabledOrd && hasDvlaLookupKeyAndDvlaConnectLicenceElement) || isValid)
                {
                    expiryDate = reader.IsDBNull(dvlaLicenceExpiryOrd) == false ? reader.GetDateTime(dvlaLicenceExpiryOrd) : DateTime.MinValue;
                    docDocumentList.Add(new DvlaAutoLookupDrivingLicence(registration, expiryDate, isBlocked, isValid, validDrivingLicence, isValidManualLicencePresent));
                }
                else if (!isPhotocard && !isNonGbDrivingLicence)
                {
                    startDate = reader.IsDBNull(paperDrivingLicenceDocumentStartDateOrd) == false ? reader.GetDateTime(paperDrivingLicenceDocumentStartDateOrd) : DateTime.MinValue;
                    docDocumentList.Add(new PaperDrivingLicence(registration, startDate, isBlocked, reviewStatus, validDrivingLicence, isValidManualLicencePresent));
                }
                else if (isPhotocard)
                {
                    expiryDate = reader.IsDBNull(photocardDrivingLicenceExpiryDateOrd) == false ? reader.GetDateTime(photocardDrivingLicenceExpiryDateOrd) : expiryDate = DateTime.MinValue;
                    docDocumentList.Add(new PhotoCardDrivingLicence(registration, expiryDate, isBlocked, reviewStatus, validDrivingLicence, isValidManualLicencePresent));
                }
                else
                {
                    startDate = reader.IsDBNull(nonGbDrivingLicenceStartDateOrd) == false ? reader.GetDateTime(nonGbDrivingLicenceStartDateOrd) : startDate = DateTime.MinValue;
                    expiryDate = reader.IsDBNull(nonGbDrivingLicenceExpiryDateOrd) == false ? reader.GetDateTime(nonGbDrivingLicenceExpiryDateOrd) : expiryDate = DateTime.MinValue;
                    docDocumentList.Add(new NonGbDrivingLicence(registration, startDate, expiryDate, isBlocked, reviewStatus, validDrivingLicence, isValidManualLicencePresent));
                }
            }

            return docDocumentList;
        }
    }
}