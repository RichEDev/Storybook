namespace ApiLibrary.DataObjects.Spend_Management
{
    using System.Runtime.Serialization;
    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// The employee roles.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.None)]
    public class EmployeeRole : DataClassBase
    {
        /// <summary>
        /// The employee id.
        /// </summary>
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int employeeid;

        /// <summary>
        /// The item role id.
        /// </summary>
        [DataMember]
        public int itemroleid;

        /// <summary>
        /// The order.
        /// </summary>
        [DataMember]
        public int order;
    }
}
