using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Auto_Tests
{
    public class UserDefinedFieldsDTO
    {
        internal int userDefineid { get; set; }
        internal string displayName { get; set; }
        internal string description { get; set; }
        internal int fieldType { get; set; }
        internal bool isMandatory { get; set; }
        internal string appliesTo { get; set; }

        internal UserDefinedFieldsDTO() { }

        internal UserDefinedFieldsDTO(int userDefineid, string displayName, string description, int fieldType, bool isMandatory, string appliesTo)
        {
            //this.userDefineid = userDefineid;
            this.displayName = displayName;
            this.description = description;
            this.fieldType = fieldType;
            this.isMandatory = isMandatory;
            this.appliesTo = appliesTo;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (obj == this) { return true; }

            UserDefinedFieldsDTO dto = obj as UserDefinedFieldsDTO;
            if (dto == null) { return false; }

            if (this.displayName != dto.displayName)
            {
                return false;
            }
            if (this.appliesTo != dto.appliesTo)
            {
                return false;
            }
            if (this.description != dto.description)
            {
                return false;
            }
            /*if (this.fieldType != dto.fieldType)
            {
                return false;
            }*/
            if (this.isMandatory != dto.isMandatory)
            {
                return false;
            }
            return true;
        }
    }
}
