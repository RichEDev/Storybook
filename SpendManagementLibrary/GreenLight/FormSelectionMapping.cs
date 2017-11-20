using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.GreenLight
{
    /// <summary>
    /// The form selection attribute value that corresponds to a particular form choice
    /// </summary>
    public struct FormSelectionMapping
    {
        /// <summary>
        /// The id of the mapping
        /// </summary>
        public int FormSelectionMappingId;

        /// <summary>
        /// The view that the mappings apply to
        /// </summary>
        public int ViewId;

        /// <summary>
        /// Is the mapping used from the add of the view (edit if not)
        /// </summary>
        public bool IsAdd;

        /// <summary>
        /// The form that will be used when this value is encountered
        /// </summary>
        public int FormId;

        /// <summary>
        /// If the attribute type is a list then this should have a an id for one of its values
        /// </summary>
        public int ListValue;

        /// <summary>
        /// If the attribute type is text then this should have a value
        /// </summary>
        public string TextValue;
    }
}
