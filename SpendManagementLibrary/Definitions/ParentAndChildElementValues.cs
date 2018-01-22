using System.Collections.Generic;
using SpendManagementLibrary.Definitions;

namespace SpendManagementLibrary
{
    /// <summary>
    /// The parent and child element values.
    /// </summary>
    public class ParentAndChildElementValues
    {
        /// <summary>
        /// Gets or sets the child control.
        /// </summary>
        public string ChildControl { get; set; }

        /// <summary>
        /// Gets or sets a List of <see cref="ParentElement"/>
        /// </summary>
        public List<ParentElement> ParentControls { get; set; }

    }
}