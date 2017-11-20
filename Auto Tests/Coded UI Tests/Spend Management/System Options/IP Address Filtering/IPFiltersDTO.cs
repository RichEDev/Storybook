using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests
{

    public class IPFiltersDTO
    {
        internal string ipAddress { get; set; }
        internal string description { get; set; }
        internal bool active { get; set; }

        internal IPFiltersDTO() { }

        internal IPFiltersDTO(string ipAddress, string description, bool active)
        {
            this.ipAddress = ipAddress;
            this.description = description;
            this.active = active;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (obj == this) { return true; }

            IPFiltersDTO dto = obj as IPFiltersDTO;
            if (dto == null) { return false; }

            if (this.ipAddress != dto.ipAddress)
            {
                return false;
            }
            if (this.description != dto.description)
            {
                return false;
            }
            if (this.active != dto.active)
            {
                return false;
            }
            return true;
        }
    }
}
