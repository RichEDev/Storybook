using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary
{
    public class cElementCategory
    {
        private int nElementCategoryID;
        private string sElementCategoryName;
        private string sElementDescription;
        
        /// <summary>
        /// Constructor for cElementCategory
        /// </summary>
        /// <param name="elementCategoryID"></param>
        /// <param name="elementCategoryName"></param>
        /// <param name="elementDescription"></param>
        public cElementCategory(int elementCategoryID, string elementCategoryName, string elementDescription)
        {
            nElementCategoryID = elementCategoryID;
            sElementCategoryName = elementCategoryName;
            sElementDescription = elementDescription;
        }

        /// <summary>
        /// Gets the element category ID
        /// </summary>
        public int ElementCategoryID { get { return nElementCategoryID; } }

        /// <summary>
        /// Gets the element category name
        /// </summary>
        public string ElementCategoryName { get { return sElementCategoryName; } }

        /// <summary>
        /// Gets the element category description
        /// </summary>
        public string ElementDescription { get { return sElementDescription; } }
    }
}
