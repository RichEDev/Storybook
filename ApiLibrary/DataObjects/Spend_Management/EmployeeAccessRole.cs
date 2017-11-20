namespace ApiLibrary.DataObjects.Spend_Management
{
    using System.Runtime.Serialization;

    using ApiLibrary.DataObjects.Base;

    /// <summary>
    /// The employee access role.
    /// </summary>
    [DataContract(Namespace = "http://software-europe.com/API/2013/02")]
    [DataClass(ElementType = TemplateMapping.ImportElementType.None)]
    public class EmployeeAccessRole : DataClassBase
    {
        /// <summary>
        /// The employee id.
        /// </summary>
        [DataMember]
        [DataClass(IsKeyField = true)]
        public int employeeID;

        /// <summary>
        /// The access role id.
        /// </summary>
        [DataMember]
        public int accessRoleID;

        /// <summary>
        /// The aub account id.
        /// </summary>
        [DataMember]
        public int subAccountID;
    }
}
