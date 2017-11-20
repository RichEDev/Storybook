using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnitTest2012Ultimate
{
    using System.Diagnostics;
    using Spend_Management;
    using SpendManagementLibrary;

    internal class UserdefinedObject
    {
        public static cUserDefinedField New(cUserDefinedField udf = null, ICurrentUser currentUser = null)
        {
            currentUser = currentUser ?? Moqs.CurrentUser();
            udf = udf ?? Template();

            cUserdefinedFields clsuserdefined = null;
            int udfId = -1;

            try
            {
                clsuserdefined = new cUserdefinedFields(currentUser.AccountID);
                udfId = clsuserdefined.SaveUserDefinedField(udf);
                cUserdefinedFields clsuserdefined2 = new cUserdefinedFields(currentUser.AccountID);
                udf = clsuserdefined2.GetUserDefinedById(udfId);
            }
            catch (Exception e)
            {
                try
                {
                    #region Cleanup
                    if (udfId > 0 && clsuserdefined != null)
                    {
                        int success = clsuserdefined.DeleteUserDefined(udfId);
                        if (success != 1)
                        {
                            throw new Exception(string.Format("Attempting to cleanup userdefined field following error returned code {0}", success), e);
                        }
                    }
                    #endregion Cleanup
                }
                finally
                {
                    throw new Exception("Error during setup of unit test dummy object of type <" + typeof(cCustomEntityViewObject).ToString() + ">", e);
                }
            }

            return udf;
        }

        public static bool TearDown(int udfId)
        {
            if (udfId > 0)
            {
                try
                {
                    ICurrentUser currentUser = Moqs.CurrentUser();
                    cUserdefinedFields clsuserdefined = new cUserdefinedFields(currentUser.AccountID);
                    int successVal = clsuserdefined.DeleteUserDefined(udfId);

                    return successVal == 1 ? true : false;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            return false;
        }

        public static cUserDefinedField Template(int udfId = 0, cTable udfTable = null, int order = 1, List<int> subcats = null, cAttribute udfAttribute = null, cUserdefinedFieldGrouping udfGrouping = null, bool archived = false, bool itemspecific = false, bool allowsearch = false, bool allowemployeetopopulate = false, DateTime createdOn = new DateTime(), int createdBy = -1, DateTime? modifiedOn = null, int? modifiedBy = null)
        {
            if (udfAttribute == null)
            {
                throw new Exception("Attribute parameter cannot be passed as null");
            }
            string dt = DateTime.UtcNow.Ticks.ToString();

            createdBy = (createdBy == -1) ? Moqs.CurrentUser().EmployeeID : createdBy;
            createdOn = (createdOn.ToLongDateString() == new DateTime().ToLongDateString()) ? DateTime.UtcNow : createdOn;

            return new cUserDefinedField(udfId, udfTable, order, subcats, createdOn, createdBy, modifiedOn, modifiedBy, udfAttribute, udfGrouping, archived, itemspecific, allowsearch, allowemployeetopopulate);
        }
    }
}
