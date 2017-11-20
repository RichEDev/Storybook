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
    /// Summary description for svcSuppliers
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcSuppliers : System.Web.Services.WebService
    {
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public cSupplier retrieveSupplierRecord(string suppliername)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            cSuppliers suppliers = new cSuppliers(curUser.AccountID, 0);
            cSupplier curSupplier = suppliers.getSupplierByName(suppliername); // will need to retrieve current location id moving forward

            return curSupplier;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public object[] retrieveContact(int contactid, int supplierid)
        {
            object[] arrContact = new object[2];
            CurrentUser curUser = cMisc.GetCurrentUser();
            cTables tables = new cTables(curUser.AccountID);
            cTable table = tables.GetTableByName("supplier_contacts");
            cSupplierContact retContact = null;
            cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
            cSupplierContacts contacts = new cSupplierContacts(curUser.AccountID, supplierid);
            retContact = contacts.getContactById(contactid);
            cSupplierContact tmpContact = (cSupplierContact)retContact.Clone();
            arrContact[0] = tmpContact;
            arrContact[1] = ufields.getUserdefinedValuesForClient(retContact.UserDefined, table.GetUserdefinedTable());
            tmpContact.UserDefined = null;
            return arrContact;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public int deleteSupplierContact(int contactid, string supplierid)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            if (!curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SupplierContacts, true))
            {
                return 0;
            }
            cSupplierContacts contacts = new cSupplierContacts(curUser.AccountID, int.Parse(supplierid));
            contacts.DeleteContact(contactid);

            return contactid;
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public cSupplierContact updateSupplierContact(cSupplierContact contact, List<object> userdefined)
        {
            CurrentUser curUser = cMisc.GetCurrentUser();
            AccessRoleType requiredPermission = (contact.ContactId == 0 ? AccessRoleType.Add : AccessRoleType.Edit);
            if (curUser.CheckAccessRole(requiredPermission, SpendManagementElement.SupplierContacts, true))
            {
                SortedList<int, object> ufields = new SortedList<int, object>();

                foreach (object o in userdefined)
                {
                    ufields.Add(Convert.ToInt32(((object[])o)[0]), ((object[])o)[1]);
                }
                
                cSupplierContacts contacts = new cSupplierContacts(curUser.AccountID, contact.SupplierId);
                cSupplierContact newContact = new cSupplierContact(contact.ContactId, contact.SupplierId, contact.Name, contact.Position, contact.Mobile, contact.Email, contact.BusinessAddress, contact.HomeAddress, contact.Comments, contact.MainContact, contact.CreatedDate, contact.CreatedById, contact.ModifiedDate, contact.ModifiedById, ufields);
                int newId = contacts.UpdateContact(newContact);

                contact.ContactId = newId;
            }
            return contact;
        }

        [WebMethod(EnableSession=true)]
        [ScriptMethod]
        public object[] getBlankContact(int supplierId)
        {
            object[] arrContact = new object[2];
            CurrentUser curUser = cMisc.GetCurrentUser();
            cTables tables = new cTables(curUser.AccountID);
            cTable table = tables.GetTableByName("supplier_contacts");

            cAddress businessAddress = new cAddress(0,"","","","","","",0,"","",false,DateTime.Now,curUser.EmployeeID,null,null);
            cAddress homeAddress = new cAddress(0, "", "", "", "", "", "", 0, "", "", true, DateTime.Now, curUser.EmployeeID, null, null);
            cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);

            cSupplierContact newContact = new cSupplierContact(0, supplierId, "", "", "", "", businessAddress, homeAddress, "", false, DateTime.Now, curUser.EmployeeID, null, null, null);
            arrContact[0] = newContact;
            arrContact[1] = ufields.getUserdefinedValuesForClient(new SortedList<int, object>(), table.GetUserdefinedTable());
            return arrContact;
        }

        [WebMethod(EnableSession = true)]
		[ScriptMethod]
		public int[] deleteSupplier(int supplierid)
		{
			CurrentUser curUser = cMisc.GetCurrentUser();
            List<int> retVals = new List<int>();
            retVals.Add(supplierid);

            if (curUser.CheckAccessRole(AccessRoleType.Delete, SpendManagementElement.SupplierDetails, true))
            {
                cSuppliers suppliers = new cSuppliers(curUser.AccountID, curUser.CurrentSubAccountId);

                int retcode = suppliers.DeleteSupplier(supplierid);
                retVals.Add(retcode);
            }
            return retVals.ToArray();
		}
    }
}
