namespace Spend_Management.shared.code.DVLA
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using DutyOfCareAPI.DutyOfCare.LicenceCheck.VehicleLookup;
    using SpendManagementLibrary;
    using SpendManagementLibrary.DVLA;
    using SpendManagementLibrary.Logic_Classes.Fields;
    using SpendManagementLibrary.Logic_Classes.Tables;
    using Spend_Management.shared.code.GreenLight;

    /// <summary>
    /// A class to manage entries in the GreenLight "Tax Document"
    /// </summary>
    public class TaxDocumentRepository
    {
        private readonly cCustomEntities _customEntities;
        private readonly ICurrentUser _currentUser;
        private readonly IFields _fields;
        private readonly ITables _tables;

        /// <summary>
        /// Create a new instanse of <see cref="TaxDocumentRepository"/>
        /// </summary>
        /// <param name="currentUser">An instance of <see cref="ICurrentUser"/></param>
        /// <param name="customEntities">An instance of <see cref="cCustomEntities"/></param>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <param name="tables">An instance of <see cref="ITables"/></param>
        public TaxDocumentRepository(ICurrentUser currentUser, cCustomEntities customEntities,  IFields fields, ITables tables)
        {
            this._customEntities = customEntities;
            this._currentUser = currentUser;
            this._fields = fields;
            this._tables = tables;
        }

        /// <summary>
        /// Add an instance of a Tax document to the current database
        /// </summary>
        /// <param name="taxExpiryDate">The date the <see cref="Vehicle"/>Tax is set to expire</param>
        /// <param name="carId">The ID of the car to add</param>
        /// <returns>An instance of <see cref="CurrentCustomEntityRecord"/> or null</returns>
        public CurrentCustomEntityRecord Add(DateTime taxExpiryDate, int carId)
        {
            CurrentCustomEntityRecord taxDocumentEntityReferenceList = new CurrentCustomEntityRecord();
            if (taxExpiryDate.Year < 1900)
            {
                return taxDocumentEntityReferenceList;
            }

            var entity = this._customEntities.getEntityByTableId(new Guid(TaxDocumentConstants.Entity));
            var clsquery = new cUpdateQuery(this._currentUser.AccountID, cAccounts.getConnectionString(this._currentUser.AccountID), null, entity.table, this._tables as cTables, this._fields as cFields);
            var expiryDate = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.ExpiryDate));
            var status = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.Status));
            var documentType = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.DocumentType));
            var vehicleId = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.VehicleId));
            var sorn = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.Sorn));
            var checkedByVehicleOwner = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.CheckedbyVehicleOwner));
            var documentAttribute = entity.GetAttributeByFieldId(documentType.FieldID);
            var createdOn = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.CreatedOn));
            var createdBy = this._fields.GetFieldByID(new Guid(TaxDocumentConstants.CreatedBy));
            if (documentAttribute is cListAttribute)
            {
                var list = documentAttribute as cListAttribute;
                foreach (cListAttributeElement element in list.items.Values)
                {
                    if (element.elementText == "Tax")
                    {
                        clsquery.addColumn(documentType, element.elementValue);
                        break;
                    }
                }
            }

            var documentStatus = entity.GetAttributeByFieldId(status.FieldID);
            if (documentStatus is cListAttribute)
            {
                var list = documentStatus as cListAttribute;
                foreach (cListAttributeElement element in list.items.Values)
                {
                    if (element.elementText == "Automatic Lookup")
                    {
                        clsquery.addColumn(status, element.elementValue);
                        break;
                    }
                }

            }

            clsquery.addColumn(expiryDate, taxExpiryDate);
            clsquery.addColumn(vehicleId, carId);
            clsquery.addColumn(sorn, 0);
            clsquery.addColumn(checkedByVehicleOwner, 0);
            clsquery.addColumn(createdOn, DateTime.Now.Date);
            clsquery.addColumn(createdBy, this._currentUser.EmployeeID);
            var result = clsquery.executeInsertStatement();
            taxDocumentEntityReferenceList.EntityId = entity.entityid;
            taxDocumentEntityReferenceList.RecordId = result;
            return taxDocumentEntityReferenceList;
        }
    }
}