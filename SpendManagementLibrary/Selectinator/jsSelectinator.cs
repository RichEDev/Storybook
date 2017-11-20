using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpendManagementLibrary.Selectinator
{
    public class JsSelectinator
    {
        
        public JsSelectinator()
        {
            
        }

        public List<JSFieldFilter> Filters = new List<JSFieldFilter>();
        public Guid TableGuid = new Guid();
        public Guid DisplayField = new Guid();
        public List<Guid> MatchFields = new List<Guid>();
        public string Name = string.Empty;

        /// <summary>
        /// List of field Guids set as autocomplete fields in Greenlight
        /// </summary>
        public List<Guid> AutocompleteFields = new List<Guid>();
        
    }
}
