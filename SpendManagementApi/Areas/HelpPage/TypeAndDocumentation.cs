using System;
using System.Collections.Generic;

namespace SpendManagementApi.Areas.HelpPage
{
    /// <summary>Contains all of the types used in the API, along with their documentation.</summary>
    public class TypeLibrary
    {
        /// <summary>All the enum types.</summary>
        public List<KeyValuePair<string, TypeDocumentation>> EnumTypes { get; set; }
        
        /// <summary>All the request types.</summary>
        public List<KeyValuePair<string, TypeDocumentation>> RequestTypes { get; set; }

        /// <summary>All the reference types.</summary>
        public List<KeyValuePair<string, TypeDocumentation>> ReferenceTypes { get; set; }
    }

    /// <summary>Represents all the documentation for a particular type.</summary>
    public class TypeDocumentation
    {
        /// <summary>The actual type.</summary>
        public Type Type { get; set; }

        /// <summary>The documentation that appears on this type, not its members.</summary>
        public string Documentation { get; set; }
        
        /// <summary>The documentation for each member of this type.</summary>
        public Dictionary<string, MemberDocumentation> MemberDocumentation { get; set; }
    }

    /// <summary>
    /// Represents a documentation node for a type.
    /// The properties are optional, as not all the info is provided per member.
    /// </summary>
    public class MemberDocumentation
    {
        /// <summary>The Type of the member.</summary>
        public string Type { get; set; }

        /// <summary>The contents of the summary node in the xml comments.</summary>
        public string Summary { get; set; }

        /// <summary>The contents of the return node in the xml comments.</summary>
        public string Return { get; set; }

        /// <summary>Any rules that apply to this property, in the form of DataAnnotations or filter Attributes.</summary>
        public List<string> Rules { get; set; }

        /// <summary>The contents of the params nodes in the xml comments.</summary>
        public Dictionary<string, string> Parameters { get; set; }
    }
}