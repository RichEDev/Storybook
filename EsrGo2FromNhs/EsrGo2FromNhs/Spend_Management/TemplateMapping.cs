namespace EsrGo2FromNhs.Spend_Management
{
    using System;
    using System.Runtime.Serialization;

    using EsrGo2FromNhs.Base;

    /// <summary>
    /// The template mapping.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    public class TemplateMapping : DataClassBase
    {
        /// <summary>
        /// The template mapping id.
        /// </summary>
        [DataMember(IsRequired = true)]
        [DataClass(IsKeyField = true)]
        public int templateMappingID;

        /// <summary>
        /// The template id.
        /// </summary>
        [DataMember]
        public int templateID;

        /// <summary>
        /// The field id.
        /// </summary>
        [DataMember]
        public Guid fieldID;

        /// <summary>
        /// The destination field.
        /// </summary>
        [DataMember]
        public string destinationField;

        /// <summary>
        /// The column ref.
        /// </summary>
        [DataMember]
        public int? columnRef;

        /// <summary>
        /// The import element type.
        /// </summary>
        [DataMember]
        public ImportElementType importElementType;

        /// <summary>
        /// The mandatory.
        /// </summary>
        [DataMember]
        public bool mandatory;

        /// <summary>
        /// The data type.
        /// </summary>
        [DataMember]
        public byte dataType;

        /// <summary>
        /// The lookup table.
        /// </summary>
        [DataMember]
        public Guid lookupTable;

        /// <summary>
        /// The match field.
        /// </summary>
        [DataMember]
        public Guid matchField;

        /// <summary>
        /// The override primary key.
        /// </summary>
        [DataMember]
        public bool overridePrimaryKey;

        /// <summary>
        /// The import field.
        /// </summary>
        [DataMember]
        public bool importField;

        /// <summary>
        /// The trust VPD.
        /// </summary>
        [DataMember]
        public string trustVPD;

        /// <summary>
        /// The table id.
        /// </summary>
        [DataMember]
        public Guid tableid;

        /// <summary>
        /// The import element type.
        /// </summary>
        public enum ImportElementType
        {
            /// <summary>
            /// The none.
            /// </summary>
            None = 0,

            /// <summary>
            /// The employee.
            /// </summary>
            Employee = 1,

            /// <summary>
            /// The assignment.
            /// </summary>
            Assignment = 2,

            /// <summary>
            /// The location.
            /// </summary>
            Location = 3,

            /// <summary>
            /// The organisation.
            /// </summary>
            Organisation = 4,

            /// <summary>
            /// The position.
            /// </summary>
            Position = 5,

            /// <summary>
            /// The phone.
            /// </summary>
            Phone = 6,

            /// <summary>
            /// The address.
            /// </summary>
            Address = 7,

            /// <summary>
            /// The vehicle.
            /// </summary>
            Vehicle = 8,

            /// <summary>
            /// The costing.
            /// </summary>
            Costing = 9,

            /// <summary>
            /// The person.
            /// </summary>
            Person = 10,

            /// <summary>
            /// The car.
            /// </summary>
            Car = 8
        }
    }
}
