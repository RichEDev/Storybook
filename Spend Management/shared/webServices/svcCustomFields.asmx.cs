using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Linq;
using System.Web.Script.Services;
using SpendManagementLibrary;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Spend_Management
{
    /// <summary>
    /// Summary description for svcCustomFields
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class svcCustomFields : System.Web.Services.WebService
    {
        private readonly Regex regGuid = new Regex("[a-z0-9]{8}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{4}-[a-z0-9]{12}", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Create the grid for the alias and functional fields
        /// </summary>
        /// <param name="entityid"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        public string[] getFieldGrid()
        {
            CurrentUser user = cMisc.GetCurrentUser();

            cCustomFields clsCustFields = new cCustomFields(user.AccountID);
            return clsCustFields.CreateCustomFieldGrid();
        }

        /// <summary>
        /// Returns a list of fields that are related to tableID
        /// </summary>
        /// <param name="tableID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public List<sCustomField> GetTableFields(string tableID)
        {
            if (string.IsNullOrEmpty(tableID) == true || regGuid.IsMatch(tableID) == false)
            {
                throw new FormatException("tableID can not be converted to a Guid");
            }

            Guid gTableID = new Guid(tableID);
            CurrentUser currentUser = cMisc.GetCurrentUser();
            List<cField> lstFields = new List<cField>();
            cFields clsFields = new cFields(currentUser.AccountID);
            lstFields = clsFields.GetFieldsByTableID(gTableID);

            List<sCustomField> tmp = new List<sCustomField>();
            sCustomField custField;

            foreach (cField field in lstFields)
            {
                if (field.FieldCategory == FieldCategory.ViewField)
                {
                    custField = new sCustomField();
                    custField.FieldID = field.FieldID.ToString();
                    custField.Description = field.Description;
                    custField.DataType = field.FieldType;
                    custField.FieldName = field.FieldName;
                    custField.TableID = field.TableID.ToString();
                    custField.FieldCat = field.FieldCategory;

                    tmp.Add(custField);
                }
            }

            return tmp;
        }

        /// <summary>
        /// Get the custom field for editing
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public sCustomField getCustomField(string ID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cFields clsFields = new cFields(currentUser.AccountID);

            Guid FieldID = new Guid(ID);
            cField field = clsFields.GetFieldByID(FieldID);
            sCustomField custField = new sCustomField();

            if (field != null)
            {
                custField = new sCustomField();
                custField.FieldID = field.FieldID.ToString();
                custField.Description = field.Description;
                custField.DataType = field.FieldType;
                custField.FieldName = field.FieldName;
                custField.TableID = field.TableID.ToString();
                custField.FieldCat = field.FieldCategory;

                List<cField> lstFields = new List<cField>();
                lstFields = clsFields.GetFieldsByTableID(field.TableID);

                if (field.FieldCategory == FieldCategory.AliasField)
                {
                    foreach (cField fld in lstFields)
                    {
                        if (fld.FieldCategory == FieldCategory.ViewField)
                        {
                            if (fld.FieldName == custField.FieldName)
                            {
                                custField.RelatedFieldID = fld.FieldID.ToString();
                                break;
                            }
                        }
                    }
                }

            }

            return custField;
        }

        /// <summary>
        /// Save the custom field to the database
        /// </summary>
        /// <param name="custField"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public string SaveCustomField(sCustomField custField)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomFields clsCustFields = new cCustomFields(currentUser.AccountID);

            Guid FieldID = clsCustFields.SaveCustomField(custField);

            return FieldID.ToString();
        }

        /// <summary>
        /// Delete the custom field from the databse. A check is made to make sure the field is not associated to 
        /// a workflow condition or a custom entity view, if it is then the field is not delete and the status returned
        /// </summary>
        /// <param name="FieldID"></param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        [ScriptMethod]
        public ReturnValues DeleteCustomField(string FieldID)
        {
            CurrentUser currentUser = cMisc.GetCurrentUser();
            cCustomFields clsCustFields = new cCustomFields(currentUser.AccountID);

            ReturnValues returnCode = clsCustFields.DeleteCustomField(FieldID);

            return returnCode;
        }
    }
}
