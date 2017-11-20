using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SpendManagementLibrary;
using Spend_Management;

namespace SpendManagementUnitTests.Global_Objects
{
    public class cGlobalMimeTypeObject
    {
        /// <summary>
        /// Create the global mime type object
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType CreateGlobalMimeType()
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);
            cGlobalMimeType gMime = GetGlobalMimeType();
            clsGlobalMimeTypes.SaveCustomMimeHeader(gMime);
            gMime = clsGlobalMimeTypes.getMimeTypeByExtension(gMime.FileExtension);

            return gMime;
        }
         
        /// <summary>
        /// A global mime type object with all values set
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType GetGlobalMimeType()
        {
            cGlobalMimeType tempMime = new cGlobalMimeType(Guid.Empty, "test" + DateTime.Now.Ticks.ToString().Substring(0, 16), "test", "testdesc");
            return tempMime;
        }

        /// <summary>
        /// A global mime type object with all values set to null or Nothing that can be
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType GetGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe()
        {
            cGlobalMimeType tempMime = new cGlobalMimeType(Guid.Empty, "test" + DateTime.Now.Ticks.ToString().Substring(0, 16), "test", "");
            return tempMime;
        }

        /// <summary>
        /// Create the global mime type object with all values set to null or Nothing that can be
        /// </summary>
        /// <returns></returns>
        public static cGlobalMimeType CreateGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe()
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);
            cGlobalMimeType gMime = GetGlobalMimeTypeWithValuesSetToNullOrNothingThatCanBe();
            clsGlobalMimeTypes.SaveCustomMimeHeader(gMime);
            gMime = clsGlobalMimeTypes.getMimeTypeByExtension(gMime.FileExtension);

            return gMime;
        }

        /// <summary>
        /// Delete the global mime type object used for the unit test
        /// </summary>
        /// <param name="ID"></param>
        public static void DeleteGlobalMimeType(Guid ID)
        {
            cGlobalMimeTypes clsGlobalMimeTypes = new cGlobalMimeTypes(cGlobalVariables.AccountID);
            clsGlobalMimeTypes.DeleteCustomMimeHeader(ID);
        }
    }
}
