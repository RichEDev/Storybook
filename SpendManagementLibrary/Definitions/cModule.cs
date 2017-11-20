using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    /// <summary>
    /// A spend management related module
    /// </summary>
    public class cModule
    {
        private int nModuleID;
        private string sModuleName;
        private string sBrandNameHTML;
        private string sBrandNamePlainText;
        private string sDescription;
        private List<cElement> lstElements = null;

        /// <summary>
        /// Constructor for cModule
        /// </summary>
        /// <param name="moduleID"></param>
        /// <param name="moduleName"></param>
        /// <param name="description"></param>
        /// <param name="brandNameHTML">The module's brand name formatted for HTML</param>
        /// <param name="brandNamePlainText">Plain text version of the brand name</param>
        public cModule(int moduleID, string moduleName, string description, string brandNameHTML, string brandNamePlainText)
        {
            nModuleID = moduleID;
            sModuleName = moduleName;
            sBrandNameHTML = brandNameHTML;
            sBrandNamePlainText = brandNamePlainText;
            sDescription = description;
        }

        /// <summary>
        /// Gets the moduleID for the module, should correspond to Modules enum
        /// </summary>
        public int ModuleID { get { return nModuleID; } }

        /// <summary>
        /// Gets the name for the module
        /// </summary>
        public string ModuleName { get { return sModuleName; } }

        /// <summary>
        /// Gets the module brand name as formatted HTML
        /// </summary>
        public string BrandNameHTML { get { return sBrandNameHTML; } }

        /// <summary>
        /// Gets the module brand name as plain text
        /// </summary>
        public string BrandNamePlainText { get { return sBrandNamePlainText; } }

        /// <summary>
        /// Gets the description for the module
        /// </summary>
        public string Description { get { return sDescription; } }

        /// <summary>
        /// Gets or Sets the elements that work with this module
        /// </summary>
        public List<cElement> Elements
        {
            get { return lstElements; }
            set { lstElements = value; }
        }
    }
}
