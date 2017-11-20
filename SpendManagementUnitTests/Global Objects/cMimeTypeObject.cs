using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    /// <summary>
    /// Create the mime type global object
    /// </summary>
    public class cMimeTypeObject
    {
        /// <summary>
        /// Create the global mime type object
        /// </summary>
        /// <returns></returns>
        public static cMimeType CreateMimeType()
        {
            cGlobalMimeType gMime = cGlobalMimeTypeObject.CreateGlobalMimeType();

            cMimeTypes clsMimeTypes = new cMimeTypes(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            int ID = clsMimeTypes.SaveMimeType(gMime.GlobalMimeID);

            cMimeType mime = clsMimeTypes.GetMimeTypeByID(ID);
            return mime;
        }

        /// <summary>
        /// Delete the global mime type
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteMimeType(int ID)
        {
            cMimeTypes clsMimeTypes = new cMimeTypes(cGlobalVariables.AccountID, cGlobalVariables.SubAccountID);
            cMimeType mime = clsMimeTypes.GetMimeTypeByID(ID);

            if (mime != null)
            {
                cGlobalMimeTypeObject.DeleteGlobalMimeType(mime.GlobalMimeID);
                clsMimeTypes.DeleteMimeType(ID);
            }
        }
    }
}
