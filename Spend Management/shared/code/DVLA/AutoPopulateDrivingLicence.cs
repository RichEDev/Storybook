namespace Spend_Management.shared.code.DVLA
{
    using System;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using SpendManagementLibrary;
    using Bootstrap;
    using System.Collections.Generic;
    using DutyOfCareAPI.DutyOfCareLicenceCheckApi;
    using SpendManagementLibrary.DVLA;
    using SpendManagementLibrary.Employees;
    using SpendManagementLibrary.Interfaces;
    using System.Configuration;
    using System.Web;

    using BusinessLogic.Identity;

    using global::expenses;

    using Spend_Management.expenses.code;

    using DrivingLicenceDetailsResponse = SpendManagementLibrary.DVLA.DrivingLicenceDetailsResponse;
    using EndorsementDetails = SpendManagementLibrary.DVLA.EndorsementDetails;
    using EntitlementDetails = SpendManagementLibrary.DVLA.EntitlementDetails;
    using GreenLight;
    using SpendManagementLibrary.Employees.DutyOfCare;

    /// <summary>
    /// Adds driving licence details
    /// </summary>
    public class AutoPopulatedDrivingLicence
    {
        /// <summary>
        ///  Indicator of which directive applies to licence 
        /// </summary>
        private const string DirectiveIndicator0 = "Pre-harmonised licence issued prior to 31st December 1985.";
        private const string DirectiveIndicator1 = "Pre-harmonised licence issued between 1st January 1986 and 31st May 1990.";
        private const string DirectiveIndicator2 = "Harmonised licence issued between 1st June 1990 and 31st December 1996.";
        private const string DirectiveIndicator4 = "3rd Directive licence issued since 19th January 2013.";
        private const string DirectiveIndicator3 = "2nd Directive licence issued between 1st January 1997 and 18th January 2013.";

        /// <summary>
        ///  Indicates if entitlement is Provisional (P), Full (F), or Unclaimed Test Pass (U)
        /// </summary>
        private const string EntitlementTypeFull = "Full (F)";
        private const string EntitlementTypeProvisional = "Provisional (P)";
        private const string EntitlementTypeUnclaimed = "Unclaimed Test Pass (U)";

        /// <summary>
        /// Code for DVLA status
        /// </summary>
        private const string Fc = "Current Full Licence";
        private const string Pc = "Current Provisional Licence";
        private const string Dq = "Disqualified";
        private const string Dx = "Disqualified for life";
        private const string Dd = "Disqualified until a given date";
        private const string Dt = "Disqualified until a test is passed";
        private const string Ds = "Disqualified until sentenced";
        private const string Fe = "Expired Full Licence";
        private const string Pe = "Expired Provisional Licence";
        private const string Fl = "Foreign Licence Holder";
        private const string Ne = "No current GB entitlement held";
        private const string Rv = "Revoked until test passed";
        private const string Fs = "Surrendered Full Licence";
        private const string Ps = "Surrendered Provisional Licence";

        /// <summary>
        /// Default list items for auto populated driving licence
        /// </summary>
        private const string DrivingLicenceListItem ="Driving Licence (Automatic DVLA Check)";
        private const string NoneListItem="[None]";
        private const string DvlaListItem="DVLA";

        /// <summary>
        /// The default source for manually inserted documents.
        /// </summary>
        private const string DvlaManualListItem = "Manual";

        /// <summary>
        /// The default review status for manual insert.
        /// </summary>
        private const string ReviewGoodListItem = "Reviewed - OK";

        /// <summary>
        /// Save driving licence information
        /// </summary>
        /// <param name="employees">
        /// Instance of employees class
        /// </param>
        /// <param name="accountId">
        /// Account details to save driving licence
        /// </param>
        /// <param name="drivingLicenceDetailsResponse">
        /// Driving licence details
        /// </param>
        /// <param name="isManualCheck">
        /// Initial lookup if manual check 
        /// </param>
        /// <param name="isLookupDateUpdated">Employees lookupdate has updated with reviewdate when enabling the general option or from any previous lookup</param>
        /// <param name="connection">
        /// Connection to database.
        /// </param>
        /// <returns>
        /// The <see cref="CurrentCustomEntityRecord"/> saved record object.
        /// </returns>
        public CurrentCustomEntityRecord AddDrivingLicenceInformation(cEmployees employees,int accountId, DrivingLicenceDetailsResponse drivingLicenceDetailsResponse, bool isManualCheck, bool isLookupDateUpdated, IDBConnection connection)
        {
            if (HttpContext.Current.User.Identity.Name == string.Empty)
            {
                HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(accountId, drivingLicenceDetailsResponse.EmployeeId));
            }

            var curUser = cMisc.GetCurrentUser(accountId + "," + drivingLicenceDetailsResponse.EmployeeId);
            var employeeDetails = employees.GetEmployeeById(drivingLicenceDetailsResponse.EmployeeId);
            CurrentCustomEntityRecord drivingLicenceEntityReferenceList = null;
            var id=0;
            if (string.IsNullOrWhiteSpace(drivingLicenceDetailsResponse.ResponseCode))
            {
                #region driving licence
                var properties = new cAccountSubAccounts(curUser.AccountID).getFirstSubAccount().SubAccountProperties;
                var notifications = new NotificationTemplates(curUser);
                var teams = new cTeams(accountId);
                var customentities = new cCustomEntities(curUser);
                var entity = customentities.getEntityByTableId(new Guid(DrivingLicenceConstants.Entity));
                var viewId = entity.Views.Values.Where(t => t.SystemCustomEntityViewId == new Guid(DrivingLicenceConstants.DrivingLicenceViewForAllDocuments)).Select(view => view.viewid).First();
                var formId = entity.Forms.Values.First(form => form.SystemCustomEntityFormId == new Guid(DrivingLicenceConstants.DrivingLicenceForm)).formid;
                var tables = new cTables(accountId);
                var fields = new cFields(accountId);
                var clsquery = new cUpdateQuery(accountId, cAccounts.getConnectionString(accountId), null, entity.table, tables, fields);
                string licenceSource, licenceStatus, licenceTypeCategory;
                var licencenumber = fields.GetFieldByID(new Guid(DrivingLicenceConstants.LicenceNumber));
                var issuenumber = fields.GetFieldByID(new Guid(DrivingLicenceConstants.IssueNumber));
                var expirydate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ExpiryDate));
                var validfrom = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ValidFrom));
                var employee = fields.GetFieldByID(new Guid(DrivingLicenceConstants.Employee));
                var issuingauthority = fields.GetFieldByID(new Guid(DrivingLicenceConstants.IssuingAuthority));
                var lookupdate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.LookupDate));
                var dvlaStatus = fields.GetFieldByID(new Guid(DrivingLicenceConstants.DvlaStatus));
                var directiveIdentifier = fields.GetFieldByID(new Guid(DrivingLicenceConstants.DirectiveIdentifier));
                var licenceType = fields.GetFieldByID(new Guid(DrivingLicenceConstants.LicenceType));
                var status = fields.GetFieldByID(new Guid(DrivingLicenceConstants.Status));
                var source = fields.GetFieldByID(new Guid(DrivingLicenceConstants.Source));
                var createdOn = fields.GetFieldByID(new Guid(DrivingLicenceConstants.CreatedOn));
                var ModifiedOn = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ModifiedOn));
                clsquery.addColumn(licencenumber, drivingLicenceDetailsResponse.DrivingLicenceNumber.ToUpper());
                clsquery.addColumn(issuenumber, drivingLicenceDetailsResponse.IssueNumber);

                if (this.IsValidDate(drivingLicenceDetailsResponse.ValidTo))
                {
                    clsquery.addColumn(expirydate, DateTime.SpecifyKind(drivingLicenceDetailsResponse.ValidTo.ToLocalTime(), DateTimeKind.Utc));
                }

                if (this.IsValidDate(drivingLicenceDetailsResponse.ValidFrom))
                {
                    clsquery.addColumn(validfrom, DateTime.SpecifyKind(drivingLicenceDetailsResponse.ValidFrom.ToLocalTime(), DateTimeKind.Utc));
                }

                clsquery.addColumn(employee, drivingLicenceDetailsResponse.EmployeeId);
                clsquery.addColumn(issuingauthority, issuingauthority.ListItems.Keys[issuingauthority.ListItems.IndexOfValue(DvlaListItem)]);
                if (!isManualCheck || employeeDetails.DvlaLookUpDate == null || isLookupDateUpdated)
                {
                    clsquery.addColumn(lookupdate, DateTime.Now.Date);
                }

                switch (drivingLicenceDetailsResponse.DvlaStatus)
                {
                    case "FC":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Fc)]);
                        break;
                    case "PC":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Pc)]);
                        break;
                    case "DQ":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Dq)]);
                        break;
                    case "DX":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Dx)]);
                        break;
                    case "DD":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Dd)]);
                        break;
                    case "DT":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Dt)]);
                        break;
                    case "DS":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Ds)]);
                        break;
                    case "FE":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Fe)]);
                        break;
                    case "PE":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Pe)]);
                        break;
                    case "FL":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Fl)]);
                        break;
                    case "NE":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Ne)]);
                        break;
                    case "RV":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Rv)]);
                        break;
                    case "FS":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Fs)]);
                        break;
                    case "PS":
                        clsquery.addColumn(dvlaStatus, dvlaStatus.ListItems.Keys[dvlaStatus.ListItems.IndexOfValue(Ps)]);
                        break;
                }

                switch (drivingLicenceDetailsResponse.DirectiveIdentifier)
                {
                    case "0":
                        clsquery.addColumn(directiveIdentifier, directiveIdentifier.ListItems.Keys[directiveIdentifier.ListItems.IndexOfValue(DirectiveIndicator0)]);
                        break;
                    case "1":
                        clsquery.addColumn(directiveIdentifier, directiveIdentifier.ListItems.Keys[directiveIdentifier.ListItems.IndexOfValue(DirectiveIndicator1)]);
                        break;
                    case "2":
                        clsquery.addColumn(directiveIdentifier, directiveIdentifier.ListItems.Keys[directiveIdentifier.ListItems.IndexOfValue(DirectiveIndicator2)]);
                        break;
                    case "3":
                        clsquery.addColumn(directiveIdentifier, directiveIdentifier.ListItems.Keys[directiveIdentifier.ListItems.IndexOfValue(DirectiveIndicator3)]);
                        break;
                    case "4":
                        clsquery.addColumn(directiveIdentifier, directiveIdentifier.ListItems.Keys[directiveIdentifier.ListItems.IndexOfValue(DirectiveIndicator4)]);
                        break;
                }

                if (drivingLicenceDetailsResponse.Source == DvlaManualListItem)
                {
                    licenceSource = DvlaManualListItem;
                    licenceStatus = ReviewGoodListItem;
                    licenceTypeCategory = drivingLicenceDetailsResponse.LicenceType;
                }
                else
                {
                    licenceSource = DvlaListItem;
                    licenceStatus = NoneListItem;
                    licenceTypeCategory = DrivingLicenceListItem;
                }

                clsquery.addColumn(licenceType, licenceType.ListItems.Keys[licenceType.ListItems.IndexOfValue(licenceTypeCategory)]);
                clsquery.addColumn(source, source.ListItems.Keys[source.ListItems.IndexOfValue(licenceSource)]);
                clsquery.addColumn(status, status.ListItems.Keys[status.ListItems.IndexOfValue(licenceStatus)]);
                int recordId = CheckDrivingLicenceInformationUpdated(accountId, entity, customentities, drivingLicenceDetailsResponse, fields);

                if (recordId == 0)
                {
                    clsquery.addColumn(createdOn, DateTime.Now);
                    id = clsquery.executeInsertStatement();
                    if (id == 0) return drivingLicenceEntityReferenceList;
                    Dictionary<string, EndorsementDetails> endorsments = new Dictionary<string, EndorsementDetails>();

                    if (drivingLicenceDetailsResponse.Endorsements.Count > 0)
                    {
                        endorsments = drivingLicenceDetailsResponse.Endorsements.ToDictionary(e => e.Code, StringComparer.OrdinalIgnoreCase);
                    }
                    InsertEndorsement(accountId, endorsments, id, customentities, tables, fields);
                    InsertEntitlement(accountId, drivingLicenceDetailsResponse, id, customentities, tables, fields, connection);

                    if (drivingLicenceDetailsResponse.DvlaStatus != "FC" && drivingLicenceDetailsResponse.DvlaStatus != "PC" && drivingLicenceDetailsResponse.DvlaStatus != "FL")
                    {
                        this.NotificationOnInvalidDrivingLicence(curUser, properties, notifications, teams);
                    }

                    drivingLicenceEntityReferenceList = new CurrentCustomEntityRecord() { EntityId = entity.entityid, ViewId = viewId, RecordId = id, FormId = formId };
                }
                else
                {
                    clsquery.addColumn(ModifiedOn, DateTime.Now);
                    clsquery.addFilter(entity.table.GetPrimaryKey(), ConditionType.Equals, new object[] { recordId }, null, ConditionJoiner.None, null);
                    clsquery.executeUpdateStatement();

                    drivingLicenceEntityReferenceList = new CurrentCustomEntityRecord() { EntityId = entity.entityid, ViewId = viewId, RecordId = recordId, FormId = formId };

                }

                #endregion
            }

            employees.SaveDvlaLookupInformation(drivingLicenceDetailsResponse.EmployeeId, drivingLicenceDetailsResponse.ResponseCode, (isManualCheck && employeeDetails.DvlaLookUpDate != null && !isLookupDateUpdated) ? employeeDetails.DvlaLookUpDate : DateTime.UtcNow);
            if (!string.IsNullOrWhiteSpace(drivingLicenceDetailsResponse.ResponseCode))
            {
                NotificationTemplates notifications = new NotificationTemplates(curUser, ConfigurationManager.AppSettings["SupportEmailAddress"]);
                DvlaServiceEventLogAndEmailNotification.DvlaLogEntry(drivingLicenceDetailsResponse.ResponseCode, drivingLicenceDetailsResponse.ErrorStatus, curUser.Account.companyid, "Autolookup", notifications, employeeDetails);
            }

            return drivingLicenceEntityReferenceList;
        }

        /// <summary>
        /// Get directive identifier id and group id by its code 
        /// </summary>
        /// <param name="directiveIdentifier">Directive identifier of the vehicle</param>
        /// <param name="group">Group code of vehicle which it belongs </param>
        /// <param name="connection">database connection </param>
        /// <returns>directive identifier and group id </returns>
        public string GetDirectiveIdentifierAndGroup(string directiveIdentifier, string group, IDBConnection connection)
        {
            using (connection)
            {
                connection.sqlexecute.Parameters.AddWithValue("@directiveIdentifier", directiveIdentifier);
                connection.sqlexecute.Parameters.AddWithValue("@group", group);
                var status = new SqlParameter("@return", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                connection.sqlexecute.Parameters.Add(status);
                connection.ExecuteProc("GetDirectiveIdentifierAndGroup");
                connection.sqlexecute.Parameters.Clear();
                return status.Value?.ToString() ?? "0,0";
            }
        }


        /// <summary>
        /// The save driving licence review method.
        /// </summary>
        /// <param name="drivingLicenceReviewRequest">
        /// The driving licence review request.
        /// </param>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <returns>
        /// The <see cref="CurrentCustomEntityRecord"/>.
        /// </returns>
        public CurrentCustomEntityRecord SaveDrivingLicenceReview(DrivingLicenceReviewDetails drivingLicenceReviewRequest, int accountId, int employeeId)
        {
            CurrentCustomEntityRecord drivingLicenceEntityReferenceList = null;
            var currentUser = cMisc.GetCurrentUser(accountId + "," + employeeId);
            var customentities = new cCustomEntities(currentUser);
            var entity = customentities.getEntityByTableId(new Guid(DrivingLicenceConstants.ReviewEntity));
            var tables = new cTables(accountId);
            var fields = new cFields(accountId);
            var clsquery = new cUpdateQuery(accountId, cAccounts.getConnectionString(accountId), null, entity.table, tables, fields);
            var viewId = entity.Views.Values.Where(t => t.SystemCustomEntityViewId == new Guid(DrivingLicenceConstants.DrivingLicenceViewId)).Select(view => view.viewid).First();
            var checkCode = fields.GetFieldByID(new Guid(DrivingLicenceConstants.CheckCode));
            var checkCodeExpiry = fields.GetFieldByID(new Guid(DrivingLicenceConstants.CheckCodeExpiry));
            var reviewerNotes = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ReviewerNotes));
            var createdOn = fields.GetFieldByID(new Guid(DrivingLicenceConstants.CreatedOn));
            var reviewDate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ReviewDate));
            var status = fields.GetFieldByID(new Guid(DrivingLicenceConstants.StatusOfReview));
            var createdBy = fields.GetFieldByID(new Guid(DrivingLicenceConstants.CreatedByOfReview));
            var reviewedBy = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ReviewedBy));
            var drivingLicenceId = fields.GetFieldByID(new Guid(DrivingLicenceConstants.DrivingLicenceId));
            clsquery.addColumn(checkCode, drivingLicenceReviewRequest.CheckCode);
            clsquery.addColumn(checkCodeExpiry, drivingLicenceReviewRequest.CheckCodeExpiry);
            clsquery.addColumn(reviewerNotes, drivingLicenceReviewRequest.ReviewerNotes);
            clsquery.addColumn(createdOn, DateTime.Now.Date);
            clsquery.addColumn(reviewDate, drivingLicenceReviewRequest.ReviewDate);
            clsquery.addColumn(status, status.ListItems.Keys[status.ListItems.IndexOfValue(drivingLicenceReviewRequest.Status)]);
            clsquery.addColumn(createdBy, drivingLicenceReviewRequest.CreatedBy);
            clsquery.addColumn(reviewedBy, drivingLicenceReviewRequest.ReviewedBy);
            clsquery.addColumn(drivingLicenceId, drivingLicenceReviewRequest.DrivingLicenceNumber);
            var id = clsquery.executeInsertStatement();
            if (id > 0)
            {
                drivingLicenceEntityReferenceList = new CurrentCustomEntityRecord() { EntityId = entity.entityid, ViewId = viewId, RecordId = id };
            }

            return drivingLicenceEntityReferenceList;
        }

        /// <summary>
        /// Insert the entitlement records against driving licence number
        /// </summary>
        /// <param name="accountId">Account id of customer</param>
        /// <param name="drivingLicenceDetailsResponse">Driving Licence Details from Dvla lookup</param>
        /// <param name="drivingLicenceEntityId">Record Id of the driving licence which is already existing in the system.This will be zero if the record is new(the licence details are updated in dvla)</param>
        /// <param name="customentities">The <see cref="cCustomEntities">Custom Entities</see></param>
        /// <param name="tables">The <see cref="cTables"></param>
        /// <param name="fields">The <see cref="cFields"></param>
        private void InsertEntitlement(int accountId, DrivingLicenceDetailsResponse drivingLicenceDetailsResponse, int drivingLicenceEntityId, cCustomEntities customentities, cTables tables, cFields fields, IDBConnection connection)
        {
            #region Entitlement          
            var entitlementEntity = customentities.getEntityByTableId(new Guid(DrivingLicenceConstants.EntitlementEntity));
            var entitlementCode = fields.GetFieldByID(new Guid(DrivingLicenceConstants.EntitlementCode));
            var validFrom = fields.GetFieldByID(new Guid(DrivingLicenceConstants.EntitlementValidFrom));
            var validTo = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ValidTo));
            var type = fields.GetFieldByID(new Guid(DrivingLicenceConstants.Type));
            var entitlementvalue = fields.GetFieldByID(new Guid(DrivingLicenceConstants.EntitlementValue));
            var autoPopulatedEntitlement = fields.GetFieldByID(new Guid(DrivingLicenceConstants.AutoPopulatedEntitlement));
            var createdOn = fields.GetFieldByID(new Guid(DrivingLicenceConstants.EntitlementCreatedOn));
            var dateOfIssue = fields.GetFieldByID(new Guid(DrivingLicenceConstants.DateOfIssue));

            foreach (var entitlementdetails in drivingLicenceDetailsResponse.Entitlements)
            {
                var entitlementQuery = new cUpdateQuery(accountId, cAccounts.getConnectionString(accountId), null, entitlementEntity.table, tables, fields);
                var identifierAndGroup = this.GetDirectiveIdentifierAndGroup(drivingLicenceDetailsResponse.DirectiveIdentifier, entitlementdetails.Code, connection).Split(',');
                entitlementQuery.addColumn(dateOfIssue, identifierAndGroup[0]);
                entitlementQuery.addColumn(entitlementCode, identifierAndGroup[1]);

                if (this.IsValidDate(entitlementdetails.ValidFrom))
                {
                    entitlementQuery.addColumn(validFrom, DateTime.SpecifyKind(entitlementdetails.ValidFrom.ToLocalTime(), DateTimeKind.Utc));
                }

                if (this.IsValidDate(entitlementdetails.ValidTo))
                {
                    entitlementQuery.addColumn(validTo, DateTime.SpecifyKind(entitlementdetails.ValidTo.ToLocalTime(), DateTimeKind.Utc));
                }

                switch (entitlementdetails.Type)
                {
                    case "F":
                        entitlementQuery.addColumn(type, type.ListItems.Keys[type.ListItems.IndexOfValue(EntitlementTypeFull)]);
                        break;
                    case "P":
                        entitlementQuery.addColumn(type, type.ListItems.Keys[type.ListItems.IndexOfValue(EntitlementTypeProvisional)]);
                        break;
                    case "U":
                        entitlementQuery.addColumn(type, type.ListItems.Keys[type.ListItems.IndexOfValue(EntitlementTypeUnclaimed)]);
                        break;
                }
                entitlementQuery.addColumn(createdOn, DateTime.Now);
                entitlementQuery.addColumn(entitlementvalue, drivingLicenceEntityId);
                entitlementQuery.addColumn(autoPopulatedEntitlement, drivingLicenceEntityId);
                entitlementQuery.executeInsertStatement();
            }

            #endregion
        }

        /// <summary>
        /// Insert or update the endorsement records against driving licence number
        /// </summary>
        /// <param name="accountId">Account id of customer</param>
        /// <param name="endorsementList">Endorsement details from Dvla lookup</param>
        /// <param name="drivingLicenceEntityId">Record Id of the driving licence which is already existing in the system.This will be zero if the record is new(the licence details are updated in dvla)</param>
        /// <param name="customentities">The <see cref="cCustomEntities">Custom Entities</see></param>
        /// <param name="tables">The <see cref="cTables"></param>
        /// <param name="fields">The <see cref="cFields"></param>
        private void InsertEndorsement(int accountId, Dictionary<string, EndorsementDetails> endorsementList, int drivingLicenceEntityId, cCustomEntities customentities, cTables tables, cFields fields)
        {
            #region Endorsement

            var endorsementEntity = customentities.getEntityByTableId(new Guid(DrivingLicenceConstants.EndorsementEntity));
            var code = fields.GetFieldByID(new Guid(DrivingLicenceConstants.Code));
            var numberOfPoints = fields.GetFieldByID(new Guid(DrivingLicenceConstants.NumberOfPoints));
            var disqualificationEndDate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.DisqualificationEndDate));
            var convictionDate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ConvictionDate));
            var offenceDate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.OffenceDate));
            var isDisqualified = fields.GetFieldByID(new Guid(DrivingLicenceConstants.IsDisqualified));
            var endorsement = fields.GetFieldByID(new Guid(DrivingLicenceConstants.Endorsement));
            var autoPopulatedEndorsement = fields.GetFieldByID(new Guid(DrivingLicenceConstants.AutoPopulatedEndorsement));
            var createdOn = fields.GetFieldByID(new Guid(DrivingLicenceConstants.EndorsementCreatedOn));
            var modifiedOn = fields.GetFieldByID(new Guid(DrivingLicenceConstants.EndorsementModifiedOn));

            foreach (KeyValuePair<string, EndorsementDetails> endorsementdetailsitem in endorsementList)
            {
                EndorsementDetails endorsementdetails = endorsementdetailsitem.Value;
                var endorsementQuery = new cUpdateQuery(accountId, cAccounts.getConnectionString(accountId), null, endorsementEntity.table, tables, fields);
                endorsementQuery.addColumn(code, endorsementdetails.Code);
                endorsementQuery.addColumn(numberOfPoints, endorsementdetails.NumberOfPoints);
                if (this.IsValidDate(endorsementdetails.DisqualificationEndDate))
                {
                    endorsementQuery.addColumn(disqualificationEndDate, DateTime.SpecifyKind(endorsementdetails.DisqualificationEndDate.ToLocalTime(), DateTimeKind.Utc));
                }

                if (this.IsValidDate(endorsementdetails.ConvictionDate))
                {
                    endorsementQuery.addColumn(convictionDate, DateTime.SpecifyKind(endorsementdetails.ConvictionDate.ToLocalTime(), DateTimeKind.Utc));
                }

                endorsementQuery.addColumn(isDisqualified, endorsementdetails.IsDisqualified);

                if (this.IsValidDate(endorsementdetails.OffenceDate))
                {
                    endorsementQuery.addColumn(offenceDate, DateTime.SpecifyKind(endorsementdetails.OffenceDate.ToLocalTime(), DateTimeKind.Utc));
                }

                endorsementQuery.addColumn(endorsement, drivingLicenceEntityId);
                endorsementQuery.addColumn(autoPopulatedEndorsement, drivingLicenceEntityId);

                if (endorsementdetailsitem.Key == endorsementdetails.Code)
                {
                    endorsementQuery.addColumn(createdOn, DateTime.Now);
                    endorsementQuery.executeInsertStatement();
                }
                else
                {
                    endorsementQuery.addColumn(modifiedOn, DateTime.Now);
                    endorsementQuery.addFilter(endorsementEntity.table.GetPrimaryKey(), ConditionType.Equals, new object[] { endorsementdetailsitem.Key }, null, ConditionJoiner.None, null);
                    endorsementQuery.executeUpdateStatement();
                }

            }
            #endregion
        }

        /// <summary>
        /// Delete the endorsement from greenlight if they are no longer available in DVLA
        /// </summary>
        /// <param name="accountId">Account id of customer</param>
        /// <param name="endorsementList">Endorsement details from Dvla lookup</param>       
        /// <param name="tables">The <see cref="cTables"></param>
        /// <param name="fields">The <see cref="cFields"></param>
        /// <param name="endorsementEntity">The <see cref="cCustomEntity">Custom Entity</see></param>
        private void DeleteEndorsementQuery(int accountId, Dictionary<string, EndorsementDetails> endorsementList, cTables tables, cFields fields, cCustomEntity endorsementEntity)
        {
            foreach (KeyValuePair<string, EndorsementDetails> deleteEndorsements in endorsementList)
            {
                EndorsementDetails deleteEndorsement = deleteEndorsements.Value;
                var deleteEndorsementQuery = new cDeleteQuery(accountId, cAccounts.getConnectionString(accountId), null, endorsementEntity.table, tables, fields);
                deleteEndorsementQuery.addFilter(endorsementEntity.table.GetPrimaryKey(), ConditionType.Equals, new object[] { deleteEndorsements.Key}, null, ConditionJoiner.None, null);
                deleteEndorsementQuery.ExecuteDeleteStatement();
            }
        }

        /// <summary>
        /// Compare the existing driving licence information with the new response from the lookup 
        /// </summary>
        /// <param name="accountId">Account id of customer</param>
        /// <param name="drivingLicenceDetailsResponse">Driving Licence Details from Dvla lookup</param>
        /// <param name="entity">Greenlight entity for driving licence</param>
        /// <param name="customentities">The <see cref="cCustomEntities">Custom Entities</see></param>
        /// <param name="fields">The <see cref="cFields"/></param>
        /// <returns>
        /// The green light record id to update the details .Id will be zero if the new drivign licence number needs to insert
        /// </returns>
        private int CheckDrivingLicenceInformationUpdated(int accountId,cCustomEntity entity, cCustomEntities customentities, DrivingLicenceDetailsResponse drivingLicenceDetailsResponse,cFields fields)
        {
            int recordId = 0;
            var employees = new cEmployees(accountId);
            recordId = employees.GetLatestDrivingLicenceRecordForEmployee(drivingLicenceDetailsResponse.EmployeeId, drivingLicenceDetailsResponse.DrivingLicenceNumber);
            cCustomEntityForm autoDrivingLicenceForm = entity.GetFormByGuid(new Guid(DrivingLicenceConstants.DrivingLicenceForm));
            cTables tables = new cTables(accountId);
            Dictionary<string, EndorsementDetails> allEndorsements = new Dictionary<string, EndorsementDetails>();
            Dictionary<string, EndorsementDetails> deletedEndorsements = new Dictionary<string, EndorsementDetails>();

            if (recordId > 0)
            {
                SortedList<int, object> drivingLicenceRecord = customentities.getEntityRecord(entity, recordId, autoDrivingLicenceForm);

                //If Issue number is modified then new DL is inserted
                if (Convert.ToInt32(drivingLicenceDetailsResponse.IssueNumber) > Convert.ToInt32(drivingLicenceRecord[customentities.getAttributeByFieldId(new Guid(DrivingLicenceConstants.IssueNumber)).attributeid]))
                {
                    return 0;
                }

                //If issue number is not modified then compare the endorsements
                var endorsementEntity = customentities.getEntityByTableId(new Guid(DrivingLicenceConstants.EndorsementEntity));
                Dictionary<int, cOneToManyRelationship> endorsementOtm = new Dictionary<int, cOneToManyRelationship>();
                // get available relationships attributes for the entities              
                customentities.getRelationshipAttributes(entity.entityid, endorsementEntity.entityid, ref endorsementOtm);
                var code = fields.GetFieldByID(new Guid(DrivingLicenceConstants.Code));
                var numberOfPoints = fields.GetFieldByID(new Guid(DrivingLicenceConstants.NumberOfPoints));
                var disqualificationEndDate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.DisqualificationEndDate));
                var convictionDate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.ConvictionDate));
                var offenceDate = fields.GetFieldByID(new Guid(DrivingLicenceConstants.OffenceDate));
                var isDisqualified = fields.GetFieldByID(new Guid(DrivingLicenceConstants.IsDisqualified));
                List<EndorsementDetails> existingListEndorsement = new List<EndorsementDetails>();
                foreach (KeyValuePair<int, cOneToManyRelationship> kvp in endorsementOtm)
                {
                    // Check for autolookup endorsement relationship attribute
                    if ((cOneToManyRelationship)kvp.Value == customentities.getAttributeByFieldId(new Guid(DrivingLicenceConstants.AutoPopulatedEndorsement)))
                    {
                        cQueryBuilder endorsementQuery = new cQueryBuilder(accountId, cAccounts.getConnectionString(accountId), System.Configuration.ConfigurationManager.ConnectionStrings["metabase"].ConnectionString, endorsementEntity.table, tables, fields);
                        endorsementQuery.addColumn(code);
                        endorsementQuery.addColumn(numberOfPoints);
                        endorsementQuery.addColumn(disqualificationEndDate);
                        endorsementQuery.addColumn(convictionDate);
                        endorsementQuery.addColumn(offenceDate);
                        endorsementQuery.addColumn(isDisqualified);
                        endorsementQuery.addColumn(fields.GetFieldByID(endorsementEntity.getKeyField().fieldid));
                        endorsementQuery.addFilter(new cQueryFilter(fields.GetFieldByID(new Guid(DrivingLicenceConstants.Endorsement)), ConditionType.Equals, new List<object>() { recordId }, null, ConditionJoiner.None, null)); // null as on the query bt ?   !!!!!!

                        using (IDataReader drOutput = endorsementQuery.getReader())
                        {
                            while (drOutput.Read())
                            {
                                if (!drOutput.IsDBNull(0))
                                {
                                    EndorsementDetails existingEndorsement = new EndorsementDetails(
                                        drOutput.GetString(drOutput.GetOrdinal(code.FieldName)),
                                        drOutput.GetDateTime(drOutput.GetOrdinal(convictionDate.FieldName)),
                                        drOutput.GetBoolean(drOutput.GetOrdinal(isDisqualified.FieldName)),
                                        drOutput.GetDateTime(drOutput.GetOrdinal(disqualificationEndDate.FieldName)),
                                        drOutput.GetInt32(drOutput.GetOrdinal(numberOfPoints.FieldName)),
                                        drOutput.GetDateTime(drOutput.GetOrdinal(offenceDate.FieldName)));
                                    existingListEndorsement.Add(existingEndorsement);

                                    if (!drivingLicenceDetailsResponse.Endorsements.Any(s => s.Code == existingEndorsement.Code))
                                    {
                                        deletedEndorsements.Add(drOutput[drOutput.GetOrdinal(fields.GetFieldByID(endorsementEntity.getKeyField().fieldid).FieldName)].ToString(), existingEndorsement);
                                    }
                                    else
                                    {
                                        EndorsementDetails endorsmentResponse = drivingLicenceDetailsResponse.Endorsements.First(s => s.Code == existingEndorsement.Code);

                                        if ((endorsmentResponse.NumberOfPoints != existingEndorsement.NumberOfPoints) ||
                                            (endorsmentResponse.DisqualificationEndDate.Date != existingEndorsement.DisqualificationEndDate.Date) ||
                                            (endorsmentResponse.OffenceDate.Date != existingEndorsement.OffenceDate.Date) ||
                                            (endorsmentResponse.IsDisqualified != existingEndorsement.IsDisqualified) ||
                                            (endorsmentResponse.ConvictionDate.Date != existingEndorsement.DisqualificationEndDate.Date))
                                        {
                                            allEndorsements.Add(drOutput[drOutput.GetOrdinal(fields.GetFieldByID(endorsementEntity.getKeyField().fieldid).FieldName)].ToString(), endorsmentResponse);
                                        }

                                    }
                                }
                            }
                            drOutput.Close();
                        }
                    }
                }

                List<EndorsementDetails> newEndorsmentCodes = drivingLicenceDetailsResponse.Endorsements.Where(x => !existingListEndorsement.Any(y => y.Code == x.Code)).ToList();
                //For each new endorsment in response code add the key as "-1".
                foreach (var newEndorsmentCode in newEndorsmentCodes)
                {
                    allEndorsements.Add(newEndorsmentCode.Code, newEndorsmentCode);
                }

                //Delete endorsements that are not available on DVLA response
                if (deletedEndorsements.Count > 0)
                {
                    DeleteEndorsementQuery(accountId, deletedEndorsements, tables, fields, endorsementEntity);
                }
            }

            //if there is no change in DL information but endorsment is modified then need to update the endorsement details and update the DL lookupdate
            if (allEndorsements.Count>0)
            {
                InsertEndorsement(accountId, allEndorsements, recordId, customentities, tables, fields);
            }
            return recordId;
        }

        /// <summary>
        /// Fetch driving licence details.
        /// </summary>
        /// <param name="employeeList">
        /// Employee details list for auto populate driving licence from DVLA portal
        /// </param>
        /// <param name="accountId">
        /// Account id of customer
        /// </param>
        /// <param name="isManualCheck">Initial lookup if manual check </param>
        /// <returns>
        /// The <see cref="DrivingLicenceList"/>.
        /// List of Driving licence details.
        /// </returns>
        public DrivingLicenceList PopulateDrivingLicences(EmployeesToPopulateDrivingLicence employeeList, int accountId, bool isManualCheck)
        {
            var dvlaApi = BootstrapDvla.CreateNew();
            var result = new DrivingLicenceList();
            foreach (var employee in employeeList.EmployeeList)
            {
                if (HttpContext.Current.User.Identity.Name == string.Empty)
                {
                    HttpContext.Current.User = new TemporaryWebPrincipal(new UserIdentity(accountId, employee.EmployeeId));
                }

                int recordId = 0;
                var employees = new cEmployees(accountId);
                recordId = employees.GetLatestDrivingLicenceRecordForEmployee(employee.EmployeeId, employee.DrivingLicenceNumber);
                bool isInitialLookup = employee.DvlaLookUpDate == null ? true : recordId == 0 ? true : false;
                cAccounts accounts = new cAccounts();
                var account = accounts.GetAccountByID(accountId);
                var response = dvlaApi.GetDrivingLicence(employee.DrivingLicenceNumber, employee.DrivingLicenceFirstname, employee.DrivingLicenceSurname, DateTime.SpecifyKind(employee.DrivingLicenceDateOfBirth.ToLocalTime(), DateTimeKind.Utc), employee.DrivingLicenceSex == 1 ? Sex.Male : Sex.Female, employee.DriverId, employee.EmployeeId, isManualCheck || isInitialLookup, account.DvlaLookUpLicenceKey);



                if (response.ValidFrom != DateTime.MinValue && response.ValidTo != DateTime.MinValue && !string.IsNullOrWhiteSpace(response.IssueNumber))
                {
                    var endorsementList = response.Endorsements.Select(endorsementdetails => new EndorsementDetails(endorsementdetails.Code, endorsementdetails.ConvictionDate, endorsementdetails.IsDisqualified, endorsementdetails.DisqualificationEndDate, endorsementdetails.NumberOfPoints, endorsementdetails.OffenceDate)).ToList();
                    var entitlementList = response.Entitlements.Select(entitlement => new EntitlementDetails(entitlement.Code, entitlement.Type, entitlement.ValidFrom, entitlement.ValidTo)).ToList();
                    result.DrivingLicenceInformationList.Add(new DrivingLicenceDetailsResponse(response.DrivingLicenceNumber, response.ValidFrom, response.ValidTo, response.DirectiveIdentifier, response.IssueNumber, response.DvlaStatus, response.ErrorStatus, endorsementList, entitlementList, employee.EmployeeId, response.ResponseCode));
                }
                else
                {
                    result.DrivingLicenceInformationList.Add(new DrivingLicenceDetailsResponse(employee.EmployeeId, response.ErrorStatus, response.ResponseCode));
                    if (!string.IsNullOrEmpty(response.ResponseCode))
                    {
                        if (response.ResponseCode == "504" || response.ResponseCode == "501")
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// The get driving licence status.
        /// </summary>
        /// <param name="accountId">
        /// The account id.
        /// </param>
        /// <param name="employeeId">
        /// The employee id.
        /// </param>
        /// <param name="connection">
        /// The connection.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// Status of driving licence
        /// </returns>
        public static string GetDrivingLicenceStatus(int accountId,int employeeId, IDBConnection connection)
        {
            using (connection)
            {
                connection.sqlexecute.Parameters.AddWithValue("@employeeId", employeeId);
                var status = new SqlParameter("@status", SqlDbType.VarChar, 100) { Direction = ParameterDirection.Output };
                connection.sqlexecute.Parameters.Add(status);
                connection.ExecuteProc("GetDrivingLicenceStatus");
                return status.Value.ToString();
            }
        }

        /// <summary>
        /// The notification on invalid driving licence.
        /// </summary>
        /// <param name="user">
        /// Claimant to sent email
        /// The user.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        /// <param name="notifications">
        /// The notification Templates.
        /// </param>
        /// <param name="teams">
        /// The teams which employee related to .
        /// </param>
        private void NotificationOnInvalidDrivingLicence(CurrentUser user, cAccountProperties properties, NotificationTemplates notifications, cTeams teams)
        {
            var employee = Employee.Get(user.EmployeeID, user.AccountID);
            var recipientsId = properties.DutyOfCareApprover.ToLower() == "line manager"
                ? new[] { employee.LineManager }
                : teams.GetTeamMembers(Convert.ToInt32(properties.DutyOfCareTeamAsApprover)).ToArray();
            var approverTemplateName = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToApproverWhenClaimantDrivingLicenceIsInvalid);
            var claimantTemplateName = SendMessageEnum.GetEnumDescription(SendMessageDescription.SentToClaimantForInvalidDrivingLicence);
            var senderId = user.EmployeeID;
            notifications.SendMessage(approverTemplateName, senderId, recipientsId, defaultSender: "admin@sel-expenses.com");
            notifications.SendMessage(claimantTemplateName, senderId, new int[] { senderId }, defaultSender: "admin@sel-expenses.com");
        }

        /// <summary>
        /// Checks for valid date.
        /// </summary>
        /// <param name="date">
        /// The date.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// Bool value to state whether date is valid or not
        /// </returns>
        private bool IsValidDate(DateTime date)
        {
            return date != DateTime.MinValue && date != new DateTime(1900, 01, 01);
        }
    }
}
