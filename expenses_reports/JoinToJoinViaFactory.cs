namespace Expenses_Reports
{
    using System;
    using System.Collections.Generic;
    using System.Xml;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Definitions.JoinVia;
    using SpendManagementLibrary.Logic_Classes.Fields;
    
    /// <summary>
    /// A class to create missing Join Vias and join vias from table to table dynamically.
    /// </summary>
    public class JoinToJoinViaFactory
    {
        /// <summary>
        /// an instance of <see cref="JoinVias"/>
        /// </summary>
        private readonly JoinVias _joinVias;

        /// <summary>
        /// An instance of <see cref="IFields"/>
        /// </summary>
        private readonly IFields _fields;

        /// <summary>
        /// An instance of <see cref="cTables"/>
        /// </summary>
        private readonly cTables _tables;

        /// <summary>
        /// An instance of <see cref="cJoins"/>
        /// </summary>
        private readonly cJoins _joins;

        /// <summary>
        /// An instance of <see cref="DebugLogger"/>
        /// </summary>
        private readonly DebugLogger _debugLogger;

        /// <summary>
        /// The path of the default joins XML file, derived from the applicaton install path
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// The <see cref="XmlDocument"/> read from the path file.
        /// </summary>
        private readonly XmlDocument _document;

        /// <summary>
        /// Initializes a new instance of the <see cref="JoinToJoinViaFactory"/> class.
        /// </summary>
        /// <param name="joinVias">An instance of <see cref="JoinVias"/></param>
        /// <param name="fields">An instance of <see cref="IFields"/></param>
        /// <param name="tables">An instance of <see cref="cTables"/></param>
        /// <param name="joins">An instance of <see cref="cJoins"/></param>
        /// <param name="debugLogger">An instance of <see cref="DebugLogger"/></param>
        public JoinToJoinViaFactory(JoinVias joinVias, IFields fields, cTables tables, cJoins joins, DebugLogger debugLogger)
        {
            this._joinVias = joinVias;
            this._fields = fields;
            this._tables = tables;
            this._joins = joins;
            this._debugLogger = debugLogger;

            try
            {
                this._document = new XmlDocument();
                this._document.InnerXml = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<Joins>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"691CC01F-FEE7-4B24-BCE1-49290E272284\" Description=\"Savedexpenses to AddressesTo\">\r\n    <JoinStep RelatedId=\"ADD75CEF-D0CE-4685-8915-AB54D6A0FE14\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"DCCE990D-7E7C-4669-BF59-6B830DABFE66\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"93E97A44-0F8E-43E2-B7BA-1772AD80003B\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"46F1778E-4AB9-4B0E-92EC-A713A79FC333\" Description=\"Savedexpenses to AddressesFrom\">\r\n    <JoinStep RelatedId=\"ADD75CEF-D0CE-4685-8915-AB54D6A0FE14\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"09D7E6D1-CB2E-4D7E-BDFA-F5D46E7ECEE6\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"45167B8B-281B-4F2A-B728-9AE90B72A43A\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"82F57980-92C9-4A4B-B76D-2E8485C0BB41\" Description=\"Savedexpenses to Subucat udf\">\r\n    <JoinStep RelatedId=\"8F61ABE2-96DE-4D3F-9E91-FDF2D47800CB\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"F83EEEF7-B365-4467-A9D8-A62E2C2D1977\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"BF9AA39A-82D6-4960-BFEF-C5943BC0542D\" Description=\"Savedexpenses to esr assignments\">\r\n    <JoinStep RelatedId=\"D1124F25-7F80-4D64-98AE-DFF161CA161D\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"02009E21-AA1D-4E0D-908A-4E9D73DDFBDF\" Description=\"Employees to Cost Code allocation\">\r\n    <JoinStep RelatedId=\"BEC89567-9FF1-4922-A453-1AD5090E8CFB\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"2FA3AD65-B3F2-4658-94D7-08394B6EB43E\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"E1EF483C-7870-42CE-BE54-ECC5C1D5FB34\" Description=\"Savedexpenses to Project codes\">\r\n    <JoinStep RelatedId=\"B2CC184D-8BAF-4C09-8080-68B687957AD2\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"EEC7B776-74C9-46B5-ABDD-4B5BE2AD4EFF\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"12DED231-B220-4ACB-A51D-896C52FF8979\" Description=\"Employees to Access roles\">\r\n    <JoinStep RelatedId=\"ADF6A4CB-399B-403C-8652-44087949F3E9\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"008C4487-9634-4280-9F45-772CDAA7EA4D\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"5A83AEAF-86C8-48FB-AA2B-E7AB05A74A0B\" Description=\"Employees to Vehicle journey rates\">\r\n    <JoinStep RelatedId=\"5DDBF0EF-FA06-4E7C-A45A-54E50E33307E\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"109C0FC4-31B4-45B7-B633-007C9EFD320E\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"86C3EBB4-9823-40E0-83FB-88E384200BC5\" RelatedType=\"0\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"BCD8272E-3D91-42F1-B6D2-FB79F27176DB\" Table=\"A008C314-6CEB-4602-B757-FB5EFCA1E530\" Description=\"Supplier Contacts to Supplier contact Summary\">\r\n    <JoinStep RelatedId=\"EB5F9ECC-8846-486B-8A17-F9B3BE432836\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"477B2E12-6817-41B5-9949-467610D7DF68\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"BCD8272E-3D91-42F1-B6D2-FB79F27176DB\" Table=\"A008C314-6CEB-4602-B757-FB5EFCA1E530\" Description=\"Supplier Contacts to Supplier contact Summary_view\">\r\n    <JoinStep RelatedId=\"EB5F9ECC-8846-486B-8A17-F9B3BE432836\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"82EA2128-A19A-434B-8167-F15DAD6E73D2\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"956DBA2C-EA66-454E-BA53-D5FA1EEB82B7\" Description=\"Employees to Odometer Readings\">\r\n    <JoinStep RelatedId=\"5DDBF0EF-FA06-4E7C-A45A-54E50E33307E\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"193F25E6-ADB2-4E6A-8FF4-F0AC6F38263B\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"C7009E76-4093-41EA-AD86-823876A95B5C\" Description=\"SavedExpenses to Esr Trusts\">\r\n    <JoinStep RelatedId=\"34012174-7CE8-4F67-8B91-6C44AC1A4845\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"2501BE3D-AA94-437D-98BB-A28788A35DC4\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"9573ED0B-814B-4CB0-916A-8CE25893617D\" RelatedType=\"0\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"A0F31CB0-16BB-4ACE-AAEA-69A7189D9599\" Description=\"Employees to Departments\">\r\n    <JoinStep RelatedId=\"BEC89567-9FF1-4922-A453-1AD5090E8CFB\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"CAC12A35-EFC7-47E0-B870-B58803307DA8\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"02009E21-AA1D-4E0D-908A-4E9D73DDFBDF\" Description=\"Employees to Costcodes\">\r\n    <JoinStep RelatedId=\"BEC89567-9FF1-4922-A453-1AD5090E8CFB\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"2FA3AD65-B3F2-4658-94D7-08394B6EB43E\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"E1EF483C-7870-42CE-BE54-ECC5C1D5FB34\" Description=\"Employees to Project codes\">\r\n    <JoinStep RelatedId=\"BEC89567-9FF1-4922-A453-1AD5090E8CFB\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"1AD6F5E8-B30C-4A61-84F1-ACB764A027ED\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"A0F31CB0-16BB-4ACE-AAEA-69A7189D9599\" Description=\"Employees to Departments\">\r\n    <JoinStep RelatedId=\"BEC89567-9FF1-4922-A453-1AD5090E8CFB\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"CAC12A35-EFC7-47E0-B870-B58803307DA8\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"B1E9111B-AA3C-4CC0-AD99-6C9E4B3E70FC\" Description=\"Saved Expenses to Global Countries\">\r\n    <JoinStep RelatedId=\"6E94DFE0-CBC6-40D5-B701-C1A2EC55E05B\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"E55B1F6D-7042-43E5-8ADE-E39D4EF4DE15\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"4C948C64-F145-4BF4-BE56-8014E9004F3C\" Description=\"Saved Expenses to Global Currencies\">\r\n    <JoinStep RelatedId=\"309C358F-1B90-4365-B443-C78485127C33\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"DC36FD7C-F0DC-4527-BB0F-5E9C52BC5E94\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"12DED231-B220-4ACB-A51D-896C52FF8979\" Description=\"Saved Expenses to Access Role\">\r\n    <JoinStep RelatedId=\"34012174-7CE8-4F67-8B91-6C44AC1A4845\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"2501BE3D-AA94-437D-98BB-A28788A35DC4\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"ADF6A4CB-399B-403C-8652-44087949F3E9\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n    <JoinStep RelatedId=\"008C4487-9634-4280-9F45-772CDAA7EA4D\" RelatedType=\"0\" Order=\"3\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"C8DBF7C8-6ED7-4872-BE06-5B3AF3B02C5A\" Description=\"Savedexpenses to Claim history\">\r\n    <JoinStep RelatedId=\"34012174-7CE8-4F67-8B91-6C44AC1A4845\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"CD923664-2808-4945-BBE5-7D3E1F980482\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"FA495951-4D06-49AD-9F85-D67F9EFF4A27\" Table=\"618DB425-F430-4660-9525-EBAB444ED754\" Description=\"Teams to Employees\">\r\n    <JoinStep RelatedId=\"6B1EF8D5-12EE-4E4A-9815-142AA739F508\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"6DD9864E-DF3A-4503-8999-5AD4B65B6C07\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"7E9E6BEE-F8CA-45D8-B914-1A9B105E47B2\" Description=\"Savedexpenses to cars udf\">\r\n    <JoinStep RelatedId=\"BCEBD759-DCC2-4C0A-96D7-7B64DD752CCC\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"7E380EDF-688E-4A00-8F34-DDFADB277D07\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"0DC9CA2B-74C7-4C9B-AD1C-A66AE55F979D\" Table=\"401B44D7-D6D8-497B-8720-7FFCC07D635D\" Description=\"Countries to Subcats\">\r\n    <JoinStep RelatedId=\"8B8FCD27-1E83-4828-B412-FAE83143902F\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"E2BB424B-B9D8-4099-BCE5-D093A7C68826\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"E4CCA1BA-A065-4116-860B-ABAA1E7BB2EF\" Description=\"Savedexpenses to Cost codes udf\">\r\n    <JoinStep RelatedId=\"B2CC184D-8BAF-4C09-8080-68B687957AD2\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"9B3E8A13-97BB-4386-9D98-CCC3EB713D5F\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"EDD93D78-306E-47A3-B6AB-CC4573B7DBDE\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"CE235F78-82C6-4BA1-8845-034A015C5DCA\" Description=\"Savedexpenses to Project codes udf\">\r\n    <JoinStep RelatedId=\"B2CC184D-8BAF-4C09-8080-68B687957AD2\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"EEC7B776-74C9-46B5-ABDD-4B5BE2AD4EFF\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"58AADFB6-EF24-4D09-B02C-2E4FAADB8C17\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"CE235F78-82C6-4BA1-8845-034A015C5DCA\" Description=\"Savedexpenses to Departments\">\r\n    <JoinStep RelatedId=\"B2CC184D-8BAF-4C09-8080-68B687957AD2\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"0783DA85-1260-4DB3-81F0-E414A9E51435\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"2DF90D15-237F-4BC5-834A-90E69FE7568B\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"972AC42D-6646-4EFC-9323-35C2C9F95B62\" Description=\"Savedexpenses to Employees udf\">\r\n    <JoinStep RelatedId=\"34012174-7CE8-4F67-8B91-6C44AC1A4845\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"2501BE3D-AA94-437D-98BB-A28788A35DC4\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"FC73B3BB-0737-4D99-9A3B-48DEE8F9E0F8\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"956DBA2C-EA66-454E-BA53-D5FA1EEB82B7\" Description=\"Savedexpenses to Odometer readings\">\r\n    <JoinStep RelatedId=\"BCEBD759-DCC2-4C0A-96D7-7B64DD752CCC\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"193F25E6-ADB2-4E6A-8FF4-F0AC6F38263B\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"91F98F84-98DB-4080-A0E9-E5482F3A53B8\" Description=\"Savedexpenses to Custom_1 (expenses_1476)\">\r\n    <JoinStep RelatedId=\"34012174-7CE8-4F67-8B91-6C44AC1A4845\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"2501BE3D-AA94-437D-98BB-A28788A35DC4\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"F9A23634-CB99-4272-983C-1661C86AA9E1\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"05BCE98E-B6EC-44BF-A93D-A5039D838A11\" Description=\"Savedexpenses to Hotels View\">\r\n    <JoinStep RelatedId=\"DB743A2B-1CBC-4A7C-8E74-D0B8A554CA21\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"3C273833-FB08-4870-8184-DF8197D82221\" Description=\"Savedexpenses to card transactions\">\r\n    <JoinStep RelatedId=\"6DF91C86-1027-43B6-8F57-6C927F83127C\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"998E51FA-2C23-467E-B90F-75C44D1838BC\" Table=\"527C558C-42F3-4CDB-9C28-442685AD2E66\" Description=\"contract_details to product_notes_view\">\r\n    <JoinStep RelatedId=\"B7A2A17B-4350-405C-A9EA-514A36D4A08F\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"953CCA00-6ABF-4330-B25D-3191248F1CAB\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"B1770216-5672-4EFC-A749-2F7CE2C729CE\" RelatedType=\"2\" Order=\"1\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"998E51FA-2C23-467E-B90F-75C44D1838BC\" Table=\"7D3465EC-5101-4BF8-9C09-CD9568BB1224\" Description=\"contract_details to ContractNotes\">\r\n    <JoinStep RelatedId=\"AE07032D-2B4C-42B7-B7B4-D241262C6DE6\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"998E51FA-2C23-467E-B90F-75C44D1838BC\" Table=\"5D69FA65-D521-45C0-8DBB-221B70C0C79E\" Description=\"contract_details to ContractNotes_view\">\r\n    <JoinStep RelatedId=\"9C602F20-2CF2-4F4C-B03B-95A76A6063A6\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"998E51FA-2C23-467E-B90F-75C44D1838BC\" Table=\"32D1E316-D501-47B2-9BB2-AA70646E3231\" Description=\"Contract Details to Product Licences\">\r\n    <JoinStep RelatedId=\"B7A2A17B-4350-405C-A9EA-514A36D4A08F\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"953CCA00-6ABF-4330-B25D-3191248F1CAB\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"31722D97-2F47-46B8-8440-0D99981D1F68\" RelatedType=\"2\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"3602B88E-4EDA-4FCC-BB65-45078C6DB21B\" Table=\"173dcfd7-41c1-47d9-bd8f-63417b71b4a0\" Description=\"Care Home Report Form to Care Home Report (FW)\">\r\n    <JoinStep RelatedId=\"A716E45E-5515-433E-8F47-06A548927A20\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"BCD8272E-3D91-42F1-B6D2-FB79F27176DB\" Table=\"B1E9111B-AA3C-4CC0-AD99-6C9E4B3E70FC\" Description=\"Supplier Contacts to Global Countries\">\r\n    <JoinStep RelatedId=\"94C8F0E7-F3D5-44C8-A352-7D6A0C6AB81D\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"71A04B4F-898A-43EE-8D5B-BFB694A684FA\" RelatedType=\"0\" Order=\"1\"></JoinStep>\r\n    <JoinStep RelatedId=\"E55B1F6D-7042-43E5-8ADE-E39D4EF4DE15\" RelatedType=\"0\" Order=\"2\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"26BDBF5E-E311-4F75-A5FF-66059E836609\" Table=\"ADB3B94D-093E-4F94-80C2-C5983C5A9F2B\" Description=\"Monthly Timesheets to Daily Sheets (saintsfc)\">\r\n    <JoinStep RelatedId=\"38CF2FB7-21FA-4777-B0C2-A62F1A4086D0\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"D70D9E5F-37E2-4025-9492-3BCF6AA746A8\" Table=\"F8039BB3-D79A-4CEF-854D-AB6F32B5D354\" Description=\"Saved expenses to saved expenses_split items\">\r\n    <JoinStep RelatedId=\"FB0313F3-5BF1-4EC5-8D32-610209C684CD\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"618DB425-F430-4660-9525-EBAB444ED754\" Table=\"972AC42D-6646-4EFC-9323-35C2C9F95B62\" Description=\"Employees to Employees udf\">\r\n    <JoinStep RelatedId=\"FC73B3BB-0737-4D99-9A3B-48DEE8F9E0F8\" RelatedType=\"2\" Order=\"0\"></JoinStep>\r\n  </Join>\r\n  <Join BaseTable=\"0EFA50B5-DA7B-49C7-A9AA-1017D5F741D0\" Table=\"972AC42D-6646-4EFC-9323-35C2C9F95B62\" Description=\"Claims to Employees udf\">\r\n    <JoinStep RelatedId=\"2501BE3D-AA94-437D-98BB-A28788A35DC4\" RelatedType=\"0\" Order=\"0\"></JoinStep>\r\n    <JoinStep RelatedId=\"FC73B3BB-0737-4D99-9A3B-48DEE8F9E0F8\" RelatedType=\"2\" Order=\"1\"></JoinStep>  \r\n  </Join>\r\n</Joins>\r\n";
            }
            catch (Exception e)
            {
                this._debugLogger.Log("JoinToJoinViaFactory", e.Message, e.StackTrace);
                throw;
            }
            
        }

        /// <summary>
        /// Convert Join Tables join to a single <see cref="JoinVia"/>
        /// </summary>
        /// <param name="tableId">The <see cref="Guid"/>ID of the target table</param>
        /// <param name="baseTableId">The <see cref="Guid"/>ID of the starting table</param>
        /// <returns>Anin stance of <see cref="JoinVia"/>contains the join information.</returns>
        public JoinVia Convert(Guid tableId, Guid baseTableId)
        {
            var result = this.UseDefaultJoinVia(tableId, baseTableId);
            if (result != null)
            {
                return result;
            }

            return this.CreateJoinViaFromJoinTable(tableId, baseTableId);
        }

        /// <summary>
        /// Read the "default" join via for this join and use it if possible.
        /// </summary>
        /// <param name="tableId"></param>
        /// <param name="baseTableId"></param>
        /// <returns></returns>
        private JoinVia UseDefaultJoinVia(Guid tableId, Guid baseTableId)
        {
            JoinVia joinVia = null;
            try
            {
                var root = this._document.DocumentElement;
                if (root != null)
                    foreach (XmlNode join in root.ChildNodes)
                    {
                        if (@join.Attributes != null)
                        {
                            var baseTable = @join.Attributes["BaseTable"];
                            var table = join.Attributes["Table"];
                            var description = join.Attributes["Description"];
                            if (new Guid(baseTable.Value) == baseTableId && new Guid(table.Value) == tableId)
                            {
                                joinVia = new JoinVia(0, description.Value, Guid.NewGuid(), this.ExtractJoinSteps(join));
                                var result = this._joinVias.SaveJoinVia(joinVia);
                                return this._joinVias.GetJoinViaByID(result);
                            }
                        }
                    }
            }
            catch (Exception e)
            {
                this._debugLogger.Log("JoinToJoinViaFactory", e.Message, e.StackTrace);
                throw;
            }


            return null;
        }

        /// <summary>
        /// Extract the join steps from the <see cref="XmlDocument"/> containing the default join
        /// </summary>
        /// <param name="join">An instance of <see cref="XmlNode"/>Containing the Join information.</param>
        /// <returns>A <see cref="SortedList{TKey,TValue}"/>containg the Order and <seealso cref="JoinViaPart"/></returns>
        private SortedList<int, JoinViaPart> ExtractJoinSteps(XmlNode @join)
        {
            var result = new SortedList<int, JoinViaPart>();
            var order = 0;
            foreach (XmlNode joinStep in join.ChildNodes)
            {
                if (joinStep.Attributes != null)
                {
                    Guid related;
                    int idType =  0;
                    if (Guid.TryParse(joinStep.Attributes["RelatedId"].Value, out related) && int.TryParse(joinStep.Attributes["RelatedType"].Value, out idType))
                    {
                        var step = new JoinViaPart(related, (JoinViaPart.IDType) idType, JoinViaPart.JoinType.LEFT);                        
                        result.Add(order, step);
                        order++;
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// Create/get a join via based on the old style jointables.
        /// </summary>
        /// <param name="tableId">The current table ID</param>
        /// <param name="baseTableId">The base table ID</param>
        /// 
        /// <returns>The new / curernt join via for the join between the two given tables.</returns>
        private JoinVia CreateJoinViaFromJoinTable(Guid tableId, Guid baseTableId)
        {
            var join = this._joins.GetJoin(baseTableId, tableId);
            if (join == null)
            {
                throw new Exception($"Join missing from '{baseTableId}' to '{tableId}'");
            }

            var baseIsCustomEntity = this._tables.GetTableByID(baseTableId).TableSource == cTable.TableSourceType.CustomEntites;

            var joinVia = new JoinVia(0, @join.description, Guid.NewGuid());
            var index = 0;
            foreach (cJoinStep joinStep in @join.Steps.Steps.Values)
            {
                Guid destinationKey = joinStep.destinationkey;
                if (destinationKey == Guid.Empty && joinStep.joinkey != Guid.Empty)
                {
                    destinationKey = joinStep.joinkey;
                }

               
                var joinType = JoinViaPart.IDType.Field;
               
                if (!baseIsCustomEntity)
                {
                    var destinationField = this._fields.GetFieldByTableAndFieldName(joinStep.destinationtableid, this._fields.GetFieldByID(destinationKey).FieldName);
                    if (destinationField == null)
                    {
                        destinationField = this._fields.GetFieldByTableAndFieldName(joinStep.destinationtableid,
                            this._fields.GetFieldByID(joinStep.joinkey).FieldName);
                    }
                    var sourceKey = this._fields.GetFieldByTableAndFieldName(joinStep.sourcetableid,  this._fields.GetFieldByID(joinStep.joinkey).FieldName);
                    if (sourceKey == null)
                    {
                        sourceKey = this._fields.GetFieldByTableAndFieldName(joinStep.sourcetableid, this._fields.GetFieldByID(joinStep.destinationkey).FieldName);
                    }
                    if (destinationField == null)
                    {
                        continue;
                    }

                    destinationKey = destinationField.FieldID;
                    if (sourceKey.IsForeignKey && (!destinationField.IsForeignKey || destinationField.RelatedTableID == Guid.Empty))
                    {
                        destinationKey = sourceKey.FieldID;
                    }
                }
                var destinationtable = this._tables.GetTableByID(joinStep.destinationtableid);
                var joinTypeForTable = destinationtable.JoinType == 1 ? JoinViaPart.JoinType.INNER : JoinViaPart.JoinType.LEFT;
                var joinViaPart = new JoinViaPart(destinationKey, joinType, joinTypeForTable);
                joinVia.JoinViaList.Add(index, joinViaPart);
                index++;
            }

            var result = this._joinVias.SaveJoinVia(joinVia);
            return this._joinVias.GetJoinViaByID(result);
        }

    }
}
