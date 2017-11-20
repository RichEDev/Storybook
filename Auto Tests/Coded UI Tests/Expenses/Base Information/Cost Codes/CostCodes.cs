using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests.Coded_UI_Tests.Expenses.Base_Information.Cost_Codes
{
    /// <summary>
    /// The Cost Codes Class
    /// </summary>
    public class CostCodes
    {
        /// <summary>
        /// Cost code id
        /// </summary>
        public int CostCodeId { get; set; }

        /// <summary>
        /// Cost code Name
        /// </summary>
        public string CostCodeName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Owner
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// custom cunstructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="owner"></param>
        public CostCodes(int id, string name, string description/*, string owner*/) 
        {
            CostCodeId = id;
            CostCodeName = name;
            Description = description;
            // Owner = owner;
        }
        
        /// <summary>
        /// Costcode grid values
        /// </summary>
        public List<string> CostCodeGridValues
        { 
            get 
            {
                return new List<string>
                           {
                               this.CostCodeName, 
                               this.Description
                           };
            } 
        }
    }
}
