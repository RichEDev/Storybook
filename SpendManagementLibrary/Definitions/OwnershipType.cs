using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Definitions
{
    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// Class to represent an owner where the owner could be a 
    /// </summary>
    public class OwnershipType<T> where T : IOwnership
    {
        public string Description
        {
            get
            {
                return OwnerObject.ItemDefinition();
            }
        }

        public SpendManagementElement OwnerType;

        public T OwnerObject;
    }
}
