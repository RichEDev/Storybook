namespace UnitTest2012Ultimate
{
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementLibrary.UserDefinedFields;
    using Spend_Management;
    using SpendManagementLibrary;

    [TestClass]
    public class UserdefinedFieldsTests
    {
        List<UdfAuditingDetails> output = null;
        const string dateTimeValue = "01/01/2015 13:00:00";
        const string dateValue = "01/01/2015 00:00:00";
        const string timeValue = "26/05/2015 14:25:00";
        const string integerValue = "5";
        const int listValue = 0;
        const string tickBoxYesValue = "True";
        const string tickBox1Value = "1";
        const string tickBoxNoValue = "0";
        const decimal decimalValue = (decimal)2.55;
        const string currencyValue = "4.66";
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            GlobalAsax.Application_Start();

        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            GlobalAsax.Application_End();
        }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestCategory("Spend Management"), TestCategory("UserDefinedFields"), TestMethod()]
        public void cUserdefined_GetUserdefinedFieldBasicsByType_ValidFieldReturned()
        {
            ICurrentUser curUser = Moqs.CurrentUser();
            cTables clsTables = new cTables(curUser.AccountID);
            cTable empUserdefinedTable = clsTables.GetTableByName("userdefined_employees");
            int userFieldId = 0;

            try
            {
                cUserDefinedField actual =
                    UserdefinedObject.New(UserdefinedObject.Template(udfTable: empUserdefinedTable, udfAttribute: cTextAttributeObject.Template(displayName: "GetUserdefinedFieldBasicsByType_TextTest")));

                Assert.IsTrue(actual.userdefineid > 0);
                userFieldId = actual.userdefineid;

                cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
                List<sFieldBasics> lst = ufields.GetUserdefinedFieldBasicsByType(empUserdefinedTable.TableID, DataType.stringVal);

                Assert.IsTrue(lst.Count > 0);
                Assert.IsTrue((from x in lst where x.FieldID == actual.attribute.fieldid select x).Any());
            }
            finally
            {
                UserdefinedObject.TearDown(userFieldId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("UserDefinedFields"), TestMethod()]
        public void cUserdefined_GetUserdefinedFieldBasicsByType_ValidFieldNotReturned()
        {
            ICurrentUser curUser = Moqs.CurrentUser();
            cTables clsTables = new cTables(curUser.AccountID);
            cTable empUserdefinedTable = clsTables.GetTableByName("userdefined_employees");
            int userFieldId = 0;

            try
            {
                cUserDefinedField actual =
                    UserdefinedObject.New(UserdefinedObject.Template(udfTable: empUserdefinedTable, udfAttribute: cTextAttributeObject.Template(displayName: "GetUserdefinedFieldBasicsByType_TextTest")));

                Assert.IsTrue(actual.userdefineid > 0);
                userFieldId = actual.userdefineid;

                cUserdefinedFields ufields = new cUserdefinedFields(curUser.AccountID);
                List<sFieldBasics> lst = ufields.GetUserdefinedFieldBasicsByType(empUserdefinedTable.TableID, DataType.dateVal);

                Assert.IsFalse((from x in lst where x.FieldID == actual.attribute.fieldid select x).Any());
            }
            finally
            {
                UserdefinedObject.TearDown(userFieldId);
            }
        }

        [TestCategory("Spend Management"), TestCategory("UserDefinedFields"), TestMethod()]
        public void cUserdefined_GetUserdefinedFieldBasicsByType_RelationshipMatchFieldOfTypeReturned()
        {
            ICurrentUser curUser = Moqs.CurrentUser();
            var clsTables = new cTables(curUser.AccountID);
            cTable empUserdefinedTable = clsTables.GetTableByName("userdefined_employees");
            int userFieldId = 0;

            try
            {
                var matchFields = new List<Guid> { new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA"), new Guid("76473C0A-DF08-40F9-8DE0-632D0111A912") };

                cUserDefinedField actual =
                    UserdefinedObject.New(UserdefinedObject.Template(udfTable: empUserdefinedTable, udfAttribute: cManyToOneRelationshipObject.Template(displayName: "GetUserdefinedFieldBasicsByType_Relationship", autoCompleteMatchFieldIDs: matchFields, autocompleteDisplayFieldID: new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"), relatedTable: clsTables.GetTableByName("employees"))));

                Assert.IsTrue(actual.userdefineid > 0);
                userFieldId = actual.userdefineid;

                var ufields = new cUserdefinedFields(curUser.AccountID);
                List<sFieldBasics> lst = ufields.GetUserdefinedFieldBasicsByType(empUserdefinedTable.TableID, DataType.stringVal);

                Assert.IsTrue((from x in lst where x.FieldID == actual.attribute.fieldid select x).Any());
            }
            finally
            {
                UserdefinedObject.TearDown(userFieldId);
            }
        }

        /// <summary>
        /// Tests that the correct datatypes and values are returned by ProcessUdfValues
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("UserDefinedFields"), TestMethod()]
        public void UdfAuditingDataProcessing()
        {
            const int integerOutputValue = 5;
            DateTime dateTimeOutputValue = new DateTime(2015, 1, 1, 13, 00, 00);
            DateTime dateOutputValue = new DateTime(2015, 1, 1, 00, 00, 00);
            DateTime timeOutputValue = new DateTime(2015, 5, 26, 14, 25, 00);
            const decimal currencyOutputValue = (decimal)4.66;
            const int yesNoOuputValue = 1;
            const int yesNoOutputValue2 = 0;
            const int listUdOutputfValue = 0;

            try
            {
                ICurrentUser curUser = Moqs.CurrentUser();
                var tables = new cTables(curUser.AccountID);
                cTable empUserdefinedTable = tables.GetTableByName("userdefined_employees");

                var userDefinedFields = new cUserdefinedFields(curUser.AccountID);
                var udfValues = new SortedList<int, object>();

                cUserDefinedField numberUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Integer);
                udfValues.Add(numberUdf.userdefineid, integerValue);

                cUserDefinedField dateTimeUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.DateTime);
                udfValues.Add(dateTimeUdf.userdefineid, dateTimeValue);

                cUserDefinedField dateOnlyUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.DateOnly);
                udfValues.Add(dateOnlyUdf.userdefineid, dateValue);

                cUserDefinedField timeOnlyUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.TimeOnly);
                udfValues.Add(timeOnlyUdf.userdefineid, timeValue);

                cUserDefinedField listUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.List);
                udfValues.Add(listUdf.userdefineid, listValue);

                cUserDefinedField currencyUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Currency);
                udfValues.Add(currencyUdf.userdefineid, currencyValue);

                cUserDefinedField yesNoUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.TickBox);
                udfValues.Add(yesNoUdf.userdefineid, tickBoxYesValue);

                cUserDefinedField yesNoUdf2 = CreateUserDefinedField(empUserdefinedTable, FieldType.TickBox);
                udfValues.Add(yesNoUdf2.userdefineid, tickBox1Value);

                cUserDefinedField yesNoUdf3 = CreateUserDefinedField(empUserdefinedTable, FieldType.TickBox);
                udfValues.Add(yesNoUdf3.userdefineid, tickBoxNoValue);

                cUserDefinedField decimalUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Number);
                udfValues.Add(decimalUdf.userdefineid, decimalValue);

                cUserDefinedField relationshipUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Relationship);
                udfValues.Add(relationshipUdf.userdefineid, GlobalTestVariables.EmployeeId);

                var fields = new cFields(curUser.AccountID);
                output = userDefinedFields.ProcessUdfValues(fields, empUserdefinedTable, udfValues);

                var integer = output.Find(x => x.UdfId == numberUdf.userdefineid);
                var integerType = integer.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(integerType) == TypeCode.Int32);
                //check correct conversion
                Assert.AreEqual(integer.Value, integerOutputValue);

                var datetime = output.Find(x => x.UdfId == dateTimeUdf.userdefineid);
                var datetimeType = datetime.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(datetimeType) == TypeCode.DateTime);
                //check correct conversion
                Assert.AreEqual(datetime.Value, dateTimeOutputValue);

                var dateOnly = output.Find(x => x.UdfId == dateOnlyUdf.userdefineid);
                var dateOnlyType = dateOnly.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(dateOnlyType) == TypeCode.DateTime);
                //check correct conversion
                Assert.AreEqual(dateOnly.Value, dateOutputValue);

                var timeOnly = output.Find(x => x.UdfId == timeOnlyUdf.userdefineid);
                var timeOnlyType = timeOnly.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(timeOnlyType) == TypeCode.DateTime);
                //check correct conversion
                Assert.AreEqual(timeOnly.Value, timeOutputValue);

                var list = output.Find(x => x.UdfId == listUdf.userdefineid);
                var listType = list.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(listType) == TypeCode.Int32);
                //check correct conversion
                Assert.AreEqual(list.Value, listUdOutputfValue);

                var currency = output.Find(x => x.UdfId == currencyUdf.userdefineid);
                var currencyType = currency.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(currencyType) == TypeCode.Decimal);
                //check correct conversion
                Assert.AreEqual(currency.Value, currencyOutputValue);

                var decimalOutcome = output.Find(x => x.UdfId == decimalUdf.userdefineid);
                var decimalType = decimalOutcome.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(decimalType) == TypeCode.Decimal);
                //check correct conversion
                Assert.AreEqual(decimalOutcome.Value, decimalValue);

                var relationship = output.Find(x => x.UdfId == relationshipUdf.userdefineid);
                var relationshipType = relationship.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(relationshipType) == TypeCode.Int32);
                //check correct conversion
                Assert.AreEqual(relationship.Value, GlobalTestVariables.EmployeeId);

                var yesNo = output.Find(x => x.UdfId == yesNoUdf.userdefineid);
                var yesNoType = yesNo.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(yesNoType) == TypeCode.Int32);
                //check correct conversion
                Assert.AreEqual(yesNo.Value, yesNoOuputValue);

                var yesNo2 = output.Find(x => x.UdfId == yesNoUdf2.userdefineid);
                var yesNoType2 = yesNo2.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(yesNoType2) == TypeCode.Int32);
                //check correct conversion
                Assert.AreEqual(yesNo2.Value, yesNoOuputValue);

                var yesNo3 = output.Find(x => x.UdfId == yesNoUdf3.userdefineid);
                var yesNoType3 = yesNo3.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(yesNoType3) == TypeCode.Int32);
                //check correct conversion
                Assert.AreEqual(yesNo3.Value, yesNoOutputValue2);

            }
            finally
            {
                if (output != null && output.Count > 0)
                {
                    foreach (var udf in output)
                    {
                        UserdefinedObject.TearDown(udf.UdfId);
                    }
                }

            }
        }

        /// <summary>
        /// Tests that null values are handled correct and the correct datatype and expected values are returned by ProcessUdfValues
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("UserDefinedFields"), TestMethod()]
        public void UdfAuditingDataProcessingNullChecks()
        {

            List<UdfAuditingDetails> output = null;

            try
            {
                ICurrentUser curUser = Moqs.CurrentUser();
                var tables = new cTables(curUser.AccountID);
                cTable empUserdefinedTable = tables.GetTableByName("userdefined_employees");
                var userDefinedFields = new cUserdefinedFields(curUser.AccountID);
                var udfValues = new SortedList<int, object>();

                cUserDefinedField numberNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Integer);
                udfValues.Add(numberNullUdf.userdefineid, DBNull.Value);

                cUserDefinedField dateTimeNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.DateTime);
                udfValues.Add(dateTimeNullUdf.userdefineid, DBNull.Value);

                cUserDefinedField dateOnlyNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.DateOnly);
                udfValues.Add(dateOnlyNullUdf.userdefineid, DBNull.Value);

                cUserDefinedField timeOnlyNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.TimeOnly);
                udfValues.Add(timeOnlyNullUdf.userdefineid, DBNull.Value);

                cUserDefinedField listNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.List);
                udfValues.Add(listNullUdf.userdefineid, DBNull.Value);

                cUserDefinedField currencyNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Currency);
                udfValues.Add(currencyNullUdf.userdefineid, DBNull.Value);

                cUserDefinedField decimalNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Number);
                udfValues.Add(decimalNullUdf.userdefineid, DBNull.Value);

                cUserDefinedField relationshipNullUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Relationship);
                udfValues.Add(relationshipNullUdf.userdefineid, DBNull.Value);

                var fields = new cFields(curUser.AccountID);
                output = userDefinedFields.ProcessUdfValues(fields, empUserdefinedTable, udfValues);

                var integerNull = output.Find(x => x.UdfId == numberNullUdf.userdefineid);
                var integerTypeNull = integerNull.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(integerTypeNull) == TypeCode.Int32);
                //check correct return for null value
                Assert.AreEqual(integerNull.Value, 0);

                var datetimeNull = output.Find(x => x.UdfId == dateTimeNullUdf.userdefineid);
                var datetimeNullType = datetimeNull.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(datetimeNullType) == TypeCode.String);
                //check correct return for null value
                Assert.AreEqual(datetimeNull.Value, string.Empty);

                var dateOnlyNull = output.Find(x => x.UdfId == dateOnlyNullUdf.userdefineid);
                var dateOnlyNullType = dateOnlyNull.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(dateOnlyNullType) == TypeCode.String);
                //check correct return for null value
                Assert.AreEqual(dateOnlyNull.Value, string.Empty);

                var listNull = output.Find(x => x.UdfId == listNullUdf.userdefineid);
                var listNullType = listNull.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(listNullType) == TypeCode.Int32);
                //check correct conversion
                Assert.AreEqual(listNull.Value, 0);

                var timeNullOnly = output.Find(x => x.UdfId == timeOnlyNullUdf.userdefineid);
                var timeOnlyNullType = timeNullOnly.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(timeOnlyNullType) == TypeCode.String);
                //check correct return for null value
                Assert.AreEqual(timeNullOnly.Value, string.Empty);

                var currencyNull = output.Find(x => x.UdfId == currencyNullUdf.userdefineid);
                var currencyNullType = currencyNull.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(currencyNullType) == TypeCode.Decimal);
                //check correct return for null value
                Assert.AreEqual(currencyNull.Value, Convert.ToDecimal(0));

                var decimalNull1 = output.Find(x => x.UdfId == decimalNullUdf.userdefineid);
                var decimalNullType = decimalNull1.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(decimalNullType) == TypeCode.Decimal);
                //check correct return for null value
                Assert.AreEqual(decimalNull1.Value, Convert.ToDecimal(0));

                var relationshipNull = output.Find(x => x.UdfId == relationshipNullUdf.userdefineid);
                var relationshipNullType = relationshipNull.Value.GetType();
                //check correct datatype
                Assert.IsTrue(Type.GetTypeCode(relationshipNullType) == TypeCode.Int32);
                //check correct return for null value
                Assert.AreEqual(relationshipNull.Value, 0);

            }
            finally
            {
                if (output != null && output.Count > 0)
                {
                    foreach (var udf in output)
                    {
                        UserdefinedObject.TearDown(udf.UdfId);
                    }
                }
            }
        }

        /// <summary>
        /// A test to compare a list of UDF values with and updated list of UDF values to ensure the differences have been correctly calculated.
        /// </summary>
        [TestCategory("Spend Management"), TestCategory("UserDefinedFields"), TestMethod()]
        public void UdfAuditingGetDifferences()
        {

            List<UdfAuditingDetails> previousValues = null;
            List<UdfAuditingDetails> updatedValues = null;

            try
            {
                ICurrentUser curUser = Moqs.CurrentUser();

                previousValues = CreatePreviousValues();
                updatedValues = CreateUpdatedValues(previousValues);
                var userdefinedFields = new cUserdefinedFields(curUser.AccountID);

                var tables = new cTables(curUser.AccountID);
                cTable empUserdefinedTable = tables.GetTableByName("userdefined_employees");
                var fields = new cFields(curUser.AccountID);

                List<UdfRecordForAudit> outcome = userdefinedFields.GetDifferences(previousValues, updatedValues,
                    empUserdefinedTable, fields);
              
                //tests the number of expected differences the method should have deduced. 
                Assert.AreEqual(outcome.Count, 8);

            }
            finally
            {
                if (previousValues != null && previousValues.Count > 0)
                {
                    foreach (var udf in previousValues)
                    {
                        UserdefinedObject.TearDown(udf.UdfId);
                    }
                }

                if (updatedValues != null && updatedValues.Count > 0)
                {
                    foreach (var udf in updatedValues)
                    {
                        UserdefinedObject.TearDown(udf.UdfId);
                    }
                }
            }
        }

        private List<UdfAuditingDetails> CreatePreviousValues()
        {
            ICurrentUser curUser = Moqs.CurrentUser();
            var tables = new cTables(curUser.AccountID);
            cTable empUserdefinedTable = tables.GetTableByName("userdefined_employees");

            var userDefinedFields = new cUserdefinedFields(curUser.AccountID);
            var udfValues = new SortedList<int, object>();

            cUserDefinedField numberUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Integer);
            udfValues.Add(numberUdf.userdefineid, integerValue);

            cUserDefinedField dateTimeUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.DateTime);
            udfValues.Add(dateTimeUdf.userdefineid, dateTimeValue);

            cUserDefinedField dateOnlyUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.DateOnly);
            udfValues.Add(dateOnlyUdf.userdefineid, dateValue);

            cUserDefinedField timeOnlyUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.DateTime, format: AttributeFormat.TimeOnly);
            udfValues.Add(timeOnlyUdf.userdefineid, timeValue);

            cUserDefinedField listUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.List);
            udfValues.Add(listUdf.userdefineid, listValue);

            cUserDefinedField currencyUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Currency);
            udfValues.Add(currencyUdf.userdefineid, currencyValue);

            cUserDefinedField yesNoUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.TickBox);
            udfValues.Add(yesNoUdf.userdefineid, tickBoxYesValue);

            cUserDefinedField yesNoUdf2 = CreateUserDefinedField(empUserdefinedTable, FieldType.TickBox);
            udfValues.Add(yesNoUdf2.userdefineid, tickBox1Value);

            cUserDefinedField yesNoUdf3 = CreateUserDefinedField(empUserdefinedTable, FieldType.TickBox);
            udfValues.Add(yesNoUdf3.userdefineid, tickBoxNoValue);

            cUserDefinedField decimalUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Number);
            udfValues.Add(decimalUdf.userdefineid, decimalValue);

            cUserDefinedField relationshipUdf = CreateUserDefinedField(empUserdefinedTable, FieldType.Relationship);
            udfValues.Add(relationshipUdf.userdefineid, GlobalTestVariables.EmployeeId);

            var fields = new cFields(curUser.AccountID);
            List<UdfAuditingDetails> previousValues = userDefinedFields.ProcessUdfValues(fields, empUserdefinedTable, udfValues);

            return previousValues;
        }

        private List<UdfAuditingDetails> CreateUpdatedValues(IEnumerable<UdfAuditingDetails> previousvalues)
        {
            ICurrentUser curUser = Moqs.CurrentUser();
            var tables = new cTables(curUser.AccountID);
            cTable empUserdefinedTable = tables.GetTableByName("userdefined_employees");

            var userDefinedFields = new cUserdefinedFields(curUser.AccountID);
            var udfValues = new SortedList<int, object>();

            foreach (var udfAuditingDetails in previousvalues)
            {
                switch (udfAuditingDetails.Field.FieldType)
                {
                    //numeric and relationship
                    case "N":
                        var userdefinedFields = new cUserdefinedFields(curUser.AccountID);
                        List<cUserDefinedField> udfForTable = userdefinedFields.GetFieldsByTable(empUserdefinedTable);

                        foreach (cUserDefinedField udfField in udfForTable.Where(udfField => udfField.userdefineid == udfAuditingDetails.UdfId))
                        {
                            switch (udfField.attribute.fieldtype)
                            {
                                case FieldType.Integer:
                                    udfValues.Add(udfAuditingDetails.UdfId, "10");
                                    break;
                                case FieldType.Relationship:
                                    udfValues.Add(udfAuditingDetails.UdfId, GlobalTestVariables.AlternativeEmployeeId);
                                    break;
                                case FieldType.List:
                                    udfValues.Add(udfAuditingDetails.UdfId, 0);
                                    break;
                                default:
                                    udfValues.Add(udfAuditingDetails.UdfId, "");
                                    break;
                            }
                        }
                        break;
                    // date time
                    case "DT":
                        udfValues.Add(udfAuditingDetails.UdfId, "01/01/2016 13:00:00");
                        break;
                    //time
                    case "T":
                        udfValues.Add(udfAuditingDetails.UdfId, "01/01/2000 13:10:00");
                        break;
                    //date
                    case "D":
                        //Leave value the same
                        udfValues.Add(udfAuditingDetails.UdfId, dateValue);
                        break;
                    // yes no
                    case "X":
                        udfValues.Add(udfAuditingDetails.UdfId, "No");
                        break;
                    //decimal
                    case "M":
                        udfValues.Add(udfAuditingDetails.UdfId, (decimal)9.99);
                        break;
                    case "C":
                        udfValues.Add(udfAuditingDetails.UdfId, "5.00");
                        break;
                }
            }

            var fields = new cFields(curUser.AccountID);
            List<UdfAuditingDetails> newValues = userDefinedFields.ProcessUdfValues(fields, empUserdefinedTable, udfValues);

            return newValues;
        }

        private cUserDefinedField CreateUserDefinedField(cTable table, FieldType fieldType, AttributeFormat format = AttributeFormat.NotSet)
        {
            cUserDefinedField userdefinedField = null;
            switch (fieldType)
            {
                case FieldType.Integer:
                    userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cNumberAttributeObject.Template(displayName: "GetUserdefinedFieldDecimal_Test", fieldType: FieldType.Integer)));
                    break;

                case FieldType.List:

                cListAttributeElement reqListItemElement = cListAttributeElementObject.Template(elementValue: 1, elementText: "TestItem", sequence: 993);   
                cListAttributeElement reqSecondListItemElement = cListAttributeElementObject.Template(elementValue: 2, elementText: "TestItem2", sequence: 994);

                    var items = new SortedList<int, cListAttributeElement>();
                    items.Add(0, reqListItemElement);
                    items.Add(1, reqSecondListItemElement);

                    userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cListAttributeObject.Template(displayName: "GetUserdefinedFieldList_Test", fieldType: FieldType.List, items: items)));
                  
                    break;

                case FieldType.DateTime:

                    switch (format)
                    {
                        case AttributeFormat.DateTime:
                            userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cDateTimeAttributeObject.Template(displayName: "GetUserdefinedFieldDateTime_Test", fieldType: FieldType.DateTime)));
                            break;

                        case AttributeFormat.DateOnly:
                            userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cDateTimeAttributeObject.Template(displayName: "GetUserdefinedFieldDateOnly_Test", fieldType: FieldType.DateTime, format: AttributeFormat.DateOnly)));
                            break;

                        case AttributeFormat.TimeOnly:
                            userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cDateTimeAttributeObject.Template(displayName: "GetUserdefinedFieldDateTimeOnly_Test", fieldType: FieldType.DateTime, format: AttributeFormat.DateTime)));
                            break;
                    }
                    break;

                case FieldType.Currency:
                    userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cNumberAttributeObject.Template(displayName: "GetUserdefinedFieldCurrency_Test", fieldType: FieldType.Currency)));
                    break;

                case FieldType.TickBox:
                    userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cTickboxAttributeObject.Template(displayName: "GetUserdefinedFieldYesNo_Test", fieldType: FieldType.TickBox)));
                    break;

                case FieldType.Relationship:
                    ICurrentUser curUser = Moqs.CurrentUser();
                    cTables tables = new cTables(curUser.AccountID);
                    var matchFields = new List<Guid> { new Guid("0F951C3E-29D1-49F0-AC13-4CFCABF21FDA"), new Guid("76473C0A-DF08-40F9-8DE0-632D0111A912") };

                    userdefinedField =
                       UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cManyToOneRelationshipObject.Template(displayName: "GetUserdefinedFieldBasicsByType_Relationship", autoCompleteMatchFieldIDs: matchFields, autocompleteDisplayFieldID: new Guid("1C45B860-DDAA-47DA-9EEC-981F59CCE795"), relatedTable: tables.GetTableByName("employees"))));
                    break;

                case FieldType.Number:
                    userdefinedField = UserdefinedObject.New(UserdefinedObject.Template(udfTable: table, udfAttribute: cNumberAttributeObject.Template(displayName: "GetUserdefinedFieldDecimal_Test", fieldType: FieldType.Number)));
                    break;
            }
            return userdefinedField;
        }
    }
}
