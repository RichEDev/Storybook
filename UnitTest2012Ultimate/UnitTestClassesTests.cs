namespace UnitTest2012Ultimate
{
    #region Usings

    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    #endregion

    /// <summary>
    ///   Testing the unit test helper classes
    /// </summary>
    [TestClass]
    public class UnitTestClassesTests
    {
        #region Enums

        /// <summary>
        ///   A test enum
        /// </summary>
        protected enum ATestEnum
        {
            /// <summary>
            ///   The value of One
            /// </summary>
            One = 0, 

            /// <summary>
            ///   The value of Two
            /// </summary>
            Two = 1, 

            /// <summary>
            ///   The value of Three
            /// </summary>
            Three = 2
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///   Gets or sets the test context which provides information about and functionality for the current test run.
        /// </summary>
        public TestContext TestContext { get; set; }

        #endregion

        #region Testing Methods
        // You can use the following additional attributes as you write your tests:
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext)
        // {
        // GlobalAsax.Application_Start();
        // }

        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup()
        // {
        // GlobalAsax.Application_End();
        // }
        // Use TestInitialize to run code before running each test
        // [TestInitialize()]
        // public void MyTestInitialize()
        // {

        // }
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup()
        // {
        // }
        #endregion Testing Methods

        #region Public Methods and Operators

        #region cCompareAssert

        #region AreEqual

        /// <summary>
        ///   String compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_BasicTypes()
        {
            cCompareAssert.AreEqual(true, true); // boolean
            cCompareAssert.AreEqual("string", "string"); // string
            cCompareAssert.AreEqual(1, 1); // integer
            cCompareAssert.AreEqual(1.0m, 1.0m); // decimal
            cCompareAssert.AreEqual(1D, 1D); // double
            cCompareAssert.AreEqual(1.0E+6, 1.0E+6); // double
            cCompareAssert.AreEqual(3F, 3F); // float
            cCompareAssert.AreEqual(2U, 2U); // uint
            cCompareAssert.AreEqual((byte)8, (byte)8); // byte
            cCompareAssert.AreEqual('\x0058', '\x0058'); // Hexadecimal Character
            cCompareAssert.AreEqual((char)88, (char)88); // Character Cast from integral type
            cCompareAssert.AreEqual('\u0058', '\u0058'); // Unicode Character
            cCompareAssert.AreEqual('X', 'X'); // Character literal
            cCompareAssert.AreEqual(ATestEnum.One, ATestEnum.One); // enums
            cCompareAssert.AreEqual(new ATestObject(), new ATestObject());
            cCompareAssert.AreEqual(new ATestObject(new ATestObject()), new ATestObject(new ATestObject()));
            cCompareAssert.AreEqual(new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject()))))), new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject()))))));
        }

        /// <summary>
        ///   String compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_String()
        {
            // truths
            cCompareAssert.AreEqual("foo", "foo");
        }

        /// <summary>
        ///   ReferenceType compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_ReferenceType()
        {
            // truths
            cCompareAssert.AreEqual(new ATestObject(), new ATestObject());
        }

        /// <summary>
        ///   String Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_StringArray()
        {
            // truths
            cCompareAssert.AreEqual(new string[0], new string[0]);
            cCompareAssert.AreEqual(new[] { "foo" }, new[] { "foo" });
            cCompareAssert.AreEqual(new[] { "foo", "bar" }, new[] { "foo", "bar" });
            cCompareAssert.AreEqual(new[] { "foo", "bar" }, new[] { "bar", "foo" });

            // as order is unimportant
        }

        /// <summary>
        ///   ReferenceType Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_ReferenceTypeArray()
        {
            // truths
            cCompareAssert.AreEqual(new ATestObject[0], new ATestObject[0]);
            cCompareAssert.AreEqual(new[] { new ATestObject() }, new[] { new ATestObject() });
            cCompareAssert.AreEqual(new[] { new ATestObject(), new ATestObject() }, new[] { new ATestObject(), new ATestObject() });
            cCompareAssert.AreEqual(new[] { new ATestObject(), new ATestObject(new ATestObject()) }, new[] { new ATestObject(new ATestObject()), new ATestObject() });

            // as order is unimportant
        }

        /// <summary>
        ///   String List compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_StringList()
        {
            // truths
            cCompareAssert.AreEqual(new List<string>(), new List<string>());
            cCompareAssert.AreEqual(new List<string> { "foo" }, new List<string> { "foo" });
            cCompareAssert.AreEqual(new List<string> { "foo", "bar" }, new List<string> { "foo", "bar" });
            cCompareAssert.AreEqual(new List<string> { "foo", "bar" }, new List<string> { "bar", "foo" });
            cCompareAssert.AreEqual(new List<string> { "foo", "bar", "bar" }, new List<string> { "bar", "foo", "bar" });
        }

        /// <summary>
        ///   ReferenceType List compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_ReferenceTypeList()
        {
            // truths
            cCompareAssert.AreEqual(new List<ATestObject>(), new List<ATestObject>());
            cCompareAssert.AreEqual(new List<ATestObject> { new ATestObject() }, new List<ATestObject> { new ATestObject() });
            cCompareAssert.AreEqual(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) });
            cCompareAssert.AreEqual(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(new ATestObject()), new ATestObject() });
            cCompareAssert.AreEqual(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(new ATestObject()), new ATestObject(), new ATestObject(new ATestObject()) });
            cCompareAssert.AreEqual(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()), null });
        }

        /// <summary>
        ///   String SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_IntStringSortedList()
        {
            // truths
            cCompareAssert.AreEqual(new SortedList<int, string>(), new SortedList<int, string>());
            cCompareAssert.AreEqual(new SortedList<int, string> { { 0, "foo" } }, new SortedList<int, string> { { 0, "foo" } });
            cCompareAssert.AreEqual(new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } }, new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } });
        }

        /// <summary>
        ///   ReferenceType SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_IntReferenceTypeSortedList()
        {
            // truths
            cCompareAssert.AreEqual(new SortedList<int, ATestObject>(), new SortedList<int, ATestObject>());
            cCompareAssert.AreEqual(new SortedList<int, ATestObject> { { 0, new ATestObject() } }, new SortedList<int, ATestObject> { { 0, new ATestObject() } });
            cCompareAssert.AreEqual(new SortedList<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new SortedList<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } });
        }

        /// <summary>
        ///   String SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_UntypedIntStringSortedList()
        {
            // truths
            cCompareAssert.AreEqual(new SortedList(), new SortedList());
            cCompareAssert.AreEqual(new SortedList { { 0, "foo" } }, new SortedList { { 0, "foo" } });
            cCompareAssert.AreEqual(new SortedList { { 0, "foo" }, { 1, "bar" } }, new SortedList { { 0, "foo" }, { 1, "bar" } });
        }

        /// <summary>
        ///   String Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_IntStringDictionary()
        {
            // truths
            cCompareAssert.AreEqual(new Dictionary<int, string>(), new Dictionary<int, string>());
            cCompareAssert.AreEqual(new Dictionary<int, string> { { 0, "foo" } }, new Dictionary<int, string> { { 0, "foo" } });
            cCompareAssert.AreEqual(new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } }, new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } });
        }

        /// <summary>
        ///   ReferenceType Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_IntReferenceTypeDictionary()
        {
            // truths
            cCompareAssert.AreEqual(new Dictionary<int, ATestObject>(), new Dictionary<int, ATestObject>());
            cCompareAssert.AreEqual(new Dictionary<int, ATestObject> { { 0, new ATestObject() } }, new Dictionary<int, ATestObject> { { 0, new ATestObject() } });
            cCompareAssert.AreEqual(new Dictionary<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new Dictionary<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } });
        }

        /// <summary>
        ///   ReferenceType Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_StringStringDictionary()
        {
            // truths
            cCompareAssert.AreEqual(new Dictionary<string, string>(), new Dictionary<string, string>());
            cCompareAssert.AreEqual(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } });
            cCompareAssert.AreEqual(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000001" } });
        }

        /// <summary>
        ///   Multi-dimension Int Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_MultiDimensionArray()
        {
            // truths
            cCompareAssert.AreEqual(new int[0, 0], new int[0, 0]);
            cCompareAssert.AreEqual(new[,] { { 1, 2 } }, new[,] { { 1, 2 } });
            cCompareAssert.AreEqual(new[,] { { 1, 2 }, { 2, 4 } }, new[,] { { 1, 2 }, { 2, 4 } });
            cCompareAssert.AreEqual(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3 }, { 2, 4, 6 } });
            cCompareAssert.AreEqual(new[, ,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } }, new[, ,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } });
        }

        /// <summary>
        ///   Jagged Int Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreEqual_JaggedArray()
        {
            // truths
            cCompareAssert.AreEqual(new int[0][], new int[0][]);
            cCompareAssert.AreEqual(new int[][] { new int[] { 1, 2 }, new int[] { 1, 2, 3 } }, new int[][] { new int[] { 1, 2 }, new int[] { 1, 2, 3 } });
            cCompareAssert.AreEqual(new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 } }, new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 } });
            cCompareAssert.AreEqual(new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6 } }, new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6 } });
            cCompareAssert.AreEqual(new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } }, new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } });
        }

        #endregion AreEqual

        #region AreNotEqual

        #endregion AreNotEqual

        #region AreNothingEqual

        /// <summary>
        ///   String compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_BasicTypes()
        {
            cCompareAssert.AreNothingEqual(true, 1);
            cCompareAssert.AreNothingEqual("foo", 1);
            cCompareAssert.AreNothingEqual(false, new ATestObject());
            cCompareAssert.AreNothingEqual(Guid.Empty, Guid.NewGuid());
        }

        /// <summary>
        ///   String compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_String()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual("foo", "bar");
        }

        /// <summary>
        ///   ReferenceType compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_ReferenceType()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new ATestObject(), new ATestObject(new ATestObject()) { AnArrayList = new ArrayList() { 1 }, AnEnum = ATestEnum.Three, AnInt16 = 7, AnInt32 = 324, AnInt64 = 35699, AnObject = (object)"oogy", ABoolean = true, AChar = 'M', ADecimal = (decimal)0.55, ADictionary = new Dictionary<string, string>() { { "stringy", "strings" } }, ADouble = 1.78, AGuid = Guid.NewGuid(), ASingle = 2f, ASortedList = new SortedList() { { 10, "oooh" } }, AString = "bar", AStringArray = new string[] { "fooby", "aarr" }, AuInt16 = 17, AuInt32 = 89, AuInt64 = 9999999 });
        }

        /// <summary>
        ///   String Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_StringArray()
        {
            // as order is unimportant

            // falsehoods
            cCompareAssert.AreNothingEqual(new[] { "foo" }, new[] { "bar" });
            cCompareAssert.AreNothingEqual(new[] { "foo" }, new[] { "foo", "bar" });
        }

        /// <summary>
        ///   ReferenceType Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_ReferenceTypeArray()
        {
            // as order is unimportant

            // falsehoods
            cCompareAssert.AreNothingEqual(new[] { new ASimpleTestObject() }, new[] { new ASimpleTestObject() { AnArrayList = new ArrayList() { 1 }, AnEnum = ATestEnum.Three, AnInt16 = 7, AnInt32 = 324, AnInt64 = 35699, AnObject = (object)"oogy", ABoolean = true, AChar = 'M', ADecimal = (decimal)0.55, ADictionary = new Dictionary<string, string>() { { "stringy", "strings" } }, ADouble = 1.78, AGuid = Guid.NewGuid(), ASingle = 2f, ASortedList = new SortedList() { { 10, "oooh" } }, AString = "bar", AStringArray = new string[] { "fooby", "aarr" }, AuInt16 = 17, AuInt32 = 89, AuInt64 = 9999999 } });
        }

        /// <summary>
        ///   String List compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_StringList()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new List<string> { "foo" }, new List<string> { "bar" });
            cCompareAssert.AreNothingEqual(new List<string> { "foo" }, new List<string> { "foo", "bar" });
            cCompareAssert.AreNothingEqual(new List<string> { "foo", "bar", "bar" }, new List<string> { "foo", "foo", "bar" });
            cCompareAssert.AreNothingEqual(new List<string> { "foo", "bar", "bar", null }, new List<string> { "foo", string.Empty, "bar", "bar" });
        }

        /// <summary>
        ///   ReferenceType List compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_ReferenceTypeList()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new List<ASimpleTestObject> { new ASimpleTestObject() }, new List<ASimpleTestObject> { new ASimpleTestObject() { AnArrayList = new ArrayList() { 1 }, AnEnum = ATestEnum.Three, AnInt16 = 7, AnInt32 = 324, AnInt64 = 35699, AnObject = (object)"oogy", ABoolean = true, AChar = 'M', ADecimal = (decimal)0.55, ADictionary = new Dictionary<string, string>() { { "stringy", "strings" } }, ADouble = 1.78, AGuid = Guid.NewGuid(), ASingle = 2f, ASortedList = new SortedList() { { 10, "oooh" } }, AString = "bar", AStringArray = new string[] { "fooby", "aarr" }, AuInt16 = 17, AuInt32 = 89, AuInt64 = 9999999 } });
            //cCompareAssert.AreNothingEqual(new List<ATestObject> { new ATestObject() }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) });
            //cCompareAssert.AreNothingEqual(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(), new ATestObject(new ATestObject()) });
            //cCompareAssert.AreNothingEqual(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()), new ATestObject() });
            //cCompareAssert.AreNothingEqual(new List<ATestObject> { new ATestObject() }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()), new ATestObject() });
            //cCompareAssert.AreNothingEqual(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject() });
            //cCompareAssert.AreNothingEqual(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), null, null, null });
        }

        /// <summary>
        ///   String SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_IntStringSortedList()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new SortedList<int, string> { { 0, "foo" } }, new SortedList<int, string> { { 0, "bar" } });
            cCompareAssert.AreNothingEqual(new SortedList<int, string> { { 0, "foo" } }, new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } });
            cCompareAssert.AreNothingEqual(new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } }, new SortedList<int, string> { { 0, "bar" }, { 1, "foo" } });
        }

        /// <summary>
        ///   ReferenceType SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_IntReferenceTypeSortedList()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new SortedList<int, ATestObject> { { 0, new ATestObject() } }, new SortedList<int, ATestObject> { { 1, new ATestObject(new ATestObject()) } });
            cCompareAssert.AreNothingEqual(new SortedList<int, ATestObject> { { 0, new ATestObject() } }, new SortedList<int, ATestObject> { { 2, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } });
            cCompareAssert.AreNothingEqual(new SortedList<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new SortedList<int, ATestObject> { {3, new ATestObject(new ATestObject()) }, { 4, new ATestObject() } });
        }

        /// <summary>
        ///   String SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_UntypedIntStringSortedList()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new SortedList { { 0, "foo" } }, new SortedList { { 0, "bar" } });
            cCompareAssert.AreNothingEqual(new SortedList { { 0, "foo" } }, new SortedList { { 0, "foo" }, { 1, "bar" } });
            cCompareAssert.AreNothingEqual(new SortedList { { 0, "foo" }, { 1, "bar" } }, new SortedList { { 0, "bar" }, { 1, "foo" } });
        }

        /// <summary>
        ///   String Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_IntStringDictionary()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new Dictionary<int, string> { { 0, "foo" } }, new Dictionary<int, string> { { 0, "bar" } });
            cCompareAssert.AreNothingEqual(new Dictionary<int, string> { { 0, "foo" } }, new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } });
            cCompareAssert.AreNothingEqual(new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } }, new Dictionary<int, string> { { 0, "bar" }, { 1, "foo" } });
        }

        /// <summary>
        ///   ReferenceType Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_IntReferenceTypeDictionary()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new Dictionary<int, ATestObject> { { 0, new ATestObject() } }, new Dictionary<int, ATestObject> { { 3, new ATestObject(new ATestObject()) } });
            cCompareAssert.AreNothingEqual(new Dictionary<int, ATestObject> { { 0, new ATestObject() } }, new Dictionary<int, ATestObject> { { 3, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } });
            cCompareAssert.AreNothingEqual(new Dictionary<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new Dictionary<int, ATestObject> { { 2, new ATestObject(new ATestObject()) }, { 4, new ATestObject() } });
        }

        /// <summary>
        ///   ReferenceType Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_StringStringDictionary()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000002" } });
            cCompareAssert.AreNothingEqual(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000002" } });
            cCompareAssert.AreNothingEqual(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000002" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000002" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000001" } });
        }

        /// <summary>
        ///   Multi-dimension Int Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_MultiDimensionArray()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new[,] { { 1, 2 } }, new[,] { { 1, 2, 3 } });
            cCompareAssert.AreNothingEqual(new[,] { { 1, 2 } }, new[,] { { 2, 1 } });
            cCompareAssert.AreNothingEqual(new[,] { { 1, 2 }, { 2, 4 } }, new[,] { { 2, 2 }, { 2, 2 } });
            cCompareAssert.AreNothingEqual(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3, -4 }, { 2, 4, 6, 10 } });
            cCompareAssert.AreNothingEqual(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3 }, { 2, 4, 8 } });
            cCompareAssert.AreNothingEqual(new[, ,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } }, new[, ,] { { { 1, 2 }, { 4, 2 } }, { { 2, 4 }, { 8, 1 } } });
            cCompareAssert.AreNothingEqual(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3, 0 }, { 2, 4, 6, 0 } });
            cCompareAssert.AreNothingEqual(new[, ,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } }, new[, ,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } }, { { 2, 4 }, { 8, 16 } } });
        }

        /// <summary>
        ///   Jagged Int Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_AreNothingEqual_JaggedArray()
        {
            // falsehoods
            cCompareAssert.AreNothingEqual(new int[][] { new int[] { 1, 2 }, new int[] { 6 } }, new int[][] { new int[] { 11, 21 } });
            //cCompareAssert.AreNothingEqual(new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 } }, new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 }, new int[] { 2, 4 } });
            //cCompareAssert.AreNothingEqual(new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6 } }, new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6, 8, 10 } });
            //cCompareAssert.AreNothingEqual(new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 }, new int[] { 8, 16 } } }, new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 }, new int[] { 8, 16 }, new int[] { 8, 16 } } });
            //cCompareAssert.AreNothingEqual(new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, -8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } }, new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } });
        }

        #endregion AreNothingEqual

        #region EqualEnough

        /// <summary>
        ///   String compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_BasicTypes()
        {
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null)); // null
            Assert.IsTrue(cCompareAssert.EqualEnough(true, true)); // boolean
            Assert.IsTrue(cCompareAssert.EqualEnough("string", "string")); // string
            Assert.IsTrue(cCompareAssert.EqualEnough(1, 1)); // integer
            Assert.IsTrue(cCompareAssert.EqualEnough(1.0m, 1.0m)); // decimal
            Assert.IsTrue(cCompareAssert.EqualEnough(1D, 1D)); // double
            Assert.IsTrue(cCompareAssert.EqualEnough(1.0E+6, 1.0E+6)); // double
            Assert.IsTrue(cCompareAssert.EqualEnough(3F, 3F)); // float
            Assert.IsTrue(cCompareAssert.EqualEnough(2U, 2U)); // uint
            Assert.IsTrue(cCompareAssert.EqualEnough((byte)8, (byte)8)); // byte
            Assert.IsTrue(cCompareAssert.EqualEnough('\x0058', '\x0058')); // Hexadecimal Character
            Assert.IsTrue(cCompareAssert.EqualEnough((char)88, (char)88)); // Character Cast from integral type
            Assert.IsTrue(cCompareAssert.EqualEnough('\u0058', '\u0058')); // Unicode Character
            Assert.IsTrue(cCompareAssert.EqualEnough('X', 'X')); // Character literal
            Assert.IsTrue(cCompareAssert.EqualEnough(ATestEnum.One, ATestEnum.One)); // enums
            Assert.IsTrue(cCompareAssert.EqualEnough(new ATestObject(), new ATestObject()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new ATestObject(new ATestObject()), new ATestObject(new ATestObject())));
            Assert.IsTrue(cCompareAssert.EqualEnough(new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject()))))), new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject(new ATestObject())))))));
            
            Assert.IsFalse(cCompareAssert.EqualEnough(true, 1));
            Assert.IsFalse(cCompareAssert.EqualEnough("foo", 1));
            Assert.IsFalse(cCompareAssert.EqualEnough(false, new ATestObject()));
            Assert.IsFalse(cCompareAssert.EqualEnough(Guid.Empty, Guid.NewGuid()));
        }

        /// <summary>
        ///   String compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_String()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(string.Empty, string.Empty));
            Assert.IsTrue(cCompareAssert.EqualEnough("foo", "foo"));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(string.Empty, null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, string.Empty));
            Assert.IsFalse(cCompareAssert.EqualEnough("foo", "bar"));
        }

        /// <summary>
        ///   ReferenceType compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_ReferenceType()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new ATestObject(), new ATestObject()));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new ATestObject(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new ATestObject(new ATestObject())));
            Assert.IsFalse(cCompareAssert.EqualEnough(new ATestObject(), new ATestObject(new ATestObject())));
        }

        /// <summary>
        ///   String Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_StringArray()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new string[0], new string[0]));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[] { "foo" }, new[] { "foo" }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[] { "foo", "bar" }, new[] { "foo", "bar" }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[] { "foo", "bar" }, new[] { "bar", "foo" }));
                
                // as order is unimportant

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new string[0], null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new string[0]));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[] { "foo" }, new[] { "bar" }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[] { "foo" }, new[] { "foo", "bar" }));
        }

        /// <summary>
        ///   ReferenceType Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_ReferenceTypeArray()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new ATestObject[0], new ATestObject[0]));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[] { new ATestObject() }, new[] { new ATestObject() }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[] { new ATestObject(), new ATestObject() }, new[] { new ATestObject(), new ATestObject() }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[] { new ATestObject(), new ATestObject(new ATestObject()) }, new[] { new ATestObject(new ATestObject()), new ATestObject() }));

            // as order is unimportant

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new ATestObject[0], null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new ATestObject[0]));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[] { new ATestObject() }, new[] { new ATestObject(new ATestObject()) }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[] { new ATestObject() }, new[] { new ATestObject(), new ATestObject(new ATestObject()) }));
        }

        /// <summary>
        ///   String List compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_StringList()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<string>(), new List<string>()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<string> { "foo" }, new List<string> { "foo" }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<string> { "foo", "bar" }, new List<string> { "foo", "bar" }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<string> { "foo", "bar" }, new List<string> { "bar", "foo" }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<string> { "foo", "bar", "bar" }, new List<string> { "bar", "foo", "bar" }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<string>(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new List<string>()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<string> { "foo" }, new List<string> { "bar" }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<string> { "foo" }, new List<string> { "foo", "bar" }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<string> { "foo", "bar", "bar" }, new List<string> { "foo", "foo", "bar" }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<string> { "foo", "bar", "bar", null }, new List<string> { "foo", string.Empty, "bar", "bar" }));
        }

        /// <summary>
        ///   ReferenceType List compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_ReferenceTypeList()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<ATestObject>(), new List<ATestObject>()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject() }, new List<ATestObject> { new ATestObject() }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(new ATestObject()), new ATestObject() }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(new ATestObject()), new ATestObject(), new ATestObject(new ATestObject()) }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()), null }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject>(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new List<ATestObject>()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject() }, new List<ATestObject> { new ATestObject(new ATestObject()) }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject() }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()) }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(), new ATestObject(new ATestObject()) }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()), new ATestObject() }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject() }, new List<ATestObject> { new ATestObject(), new ATestObject(new ATestObject()), new ATestObject(new ATestObject()), new ATestObject() }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject() }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new List<ATestObject> { new ATestObject(), null, new ATestObject(new ATestObject()), new ATestObject(new ATestObject()) }, new List<ATestObject> { new ATestObject(), null, null, null }));
        }

        /// <summary>
        ///   String SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_IntStringSortedList()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList<int, string>(), new SortedList<int, string>()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList<int, string> { { 0, "foo" } }, new SortedList<int, string> { { 0, "foo" } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } }, new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, string>(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new SortedList<int, string>()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, string> { { 0, "foo" } }, new SortedList<int, string> { { 0, "bar" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, string> { { 0, "foo" } }, new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, string> { { 0, "foo" }, { 1, "bar" } }, new SortedList<int, string> { { 0, "bar" }, { 1, "foo" } }));
        }

        /// <summary>
        ///   ReferenceType SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_IntReferenceTypeSortedList()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList<int, ATestObject>(), new SortedList<int, ATestObject>()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList<int, ATestObject> { { 0, new ATestObject() } }, new SortedList<int, ATestObject> { { 0, new ATestObject() } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new SortedList<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, ATestObject>(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new SortedList<int, ATestObject>()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, ATestObject> { { 0, new ATestObject() } }, new SortedList<int, ATestObject> { { 0, new ATestObject(new ATestObject()) } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, ATestObject> { { 0, new ATestObject() } }, new SortedList<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new SortedList<int, ATestObject> { { 0, new ATestObject(new ATestObject()) }, { 1, new ATestObject() } }));
        }

        /// <summary>
        ///   String SortedList compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_UntypedIntStringSortedList()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList(), new SortedList()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList { { 0, "foo" } }, new SortedList { { 0, "foo" } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new SortedList { { 0, "foo" }, { 1, "bar" } }, new SortedList { { 0, "foo" }, { 1, "bar" } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new SortedList()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList { { 0, "foo" } }, new SortedList { { 0, "bar" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList { { 0, "foo" } }, new SortedList { { 0, "foo" }, { 1, "bar" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new SortedList { { 0, "foo" }, { 1, "bar" } }, new SortedList { { 0, "bar" }, { 1, "foo" } }));
        }

        /// <summary>
        ///   String Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_IntStringDictionary()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<int, string>(), new Dictionary<int, string>()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<int, string> { { 0, "foo" } }, new Dictionary<int, string> { { 0, "foo" } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } }, new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, string>(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new Dictionary<int, string>()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, string> { { 0, "foo" } }, new Dictionary<int, string> { { 0, "bar" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, string> { { 0, "foo" } }, new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, string> { { 0, "foo" }, { 1, "bar" } }, new Dictionary<int, string> { { 0, "bar" }, { 1, "foo" } }));
        }

        /// <summary>
        ///   ReferenceType Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_IntReferenceTypeDictionary()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject>(), new Dictionary<int, ATestObject>()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject> { { 0, new ATestObject() } }, new Dictionary<int, ATestObject> { { 0, new ATestObject() } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new Dictionary<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject>(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new Dictionary<int, ATestObject>()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject> { { 0, new ATestObject() } }, new Dictionary<int, ATestObject> { { 0, new ATestObject(new ATestObject()) } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject> { { 0, new ATestObject() } }, new Dictionary<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject> { { 0, new ATestObject() }, { 1, new ATestObject(new ATestObject()) } }, new Dictionary<int, ATestObject> { { 0, new ATestObject(new ATestObject()) }, { 1, new ATestObject() } }));

            ATestObject obj1 = new ATestObject();
            ATestObject obj2 = new ATestObject();
            obj2.ASortedList.Add(5, obj2.ASortedList[0]);
            obj2.ASortedList.Remove(0);
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<int, ATestObject> { { 0, obj1 }, { 1, new ATestObject(new ATestObject()) } }, new Dictionary<int, ATestObject> { { 0, obj2 }, { 1, new ATestObject(new ATestObject()) } }));
        }

        /// <summary>
        ///   ReferenceType Dictionary compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_StringStringDictionary()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<string, string>(), new Dictionary<string, string>()));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000001" } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<string, string>(), null));
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new Dictionary<string, string>()));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000002" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000002" } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000001" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000002" } }, new Dictionary<string, string> { { "00000000-0000-0000-0000-000000000000", "10000000-0000-0000-0000-000000000002" }, { "00000000-0000-0000-0000-000000000001", "10000000-0000-0000-0000-000000000001" } }));
        }

        /// <summary>
        ///   Multi-dimension Int Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_MultiDimensionArray()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new int[0, 0], new int[0, 0]));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[,] { { 1, 2 } }, new[,] { { 1, 2 } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[,] { { 1, 2 }, { 2, 4 } }, new[,] { { 1, 2 }, { 2, 4 } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3 }, { 2, 4, 6 } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new[,,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } }, new[,,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new int[0, 0]));
            Assert.IsFalse(cCompareAssert.EqualEnough(new int[0, 0], null));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,] { { 1, 2 } }, new[,] { { 1, 2, 3 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,] { { 1, 2 } }, new[,] { { 2, 1 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,] { { 1, 2 }, { 2, 4 } }, new[,] { { 2, 2 }, { 2, 2 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3, -4 }, { 2, 4, 6, 10 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3 }, { 2, 4, 8 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } }, new[,,] { { { 1, 2 }, { 4, 2 } }, { { 2, 4 }, { 8, 1 } } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,] { { 1, 2, 3 }, { 2, 4, 6 } }, new[,] { { 1, 2, 3, 0 }, { 2, 4, 6, 0 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new[,,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } } }, new[,,] { { { 1, 2 }, { 4, 8 } }, { { 2, 4 }, { 8, 16 } }, { { 2, 4 }, { 8, 16 } } }));
        }

        /// <summary>
        ///   Jagged Int Array compares
        /// </summary>
        [TestMethod]
        [TestCategory("Unit Test Tests")]
        public void cCompareAssert_EqualEnough_JaggedArray()
        {
            // truths
            Assert.IsTrue(cCompareAssert.EqualEnough(null, null));
            Assert.IsTrue(cCompareAssert.EqualEnough(new int[0][], new int[0][]));
            Assert.IsTrue(cCompareAssert.EqualEnough(new int[][] { new int[] { 1, 2 }, new int[] { 1, 2, 3 } }, new int[][] { new int[] { 1, 2 }, new int[] { 1, 2, 3 } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 } }, new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6 } }, new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6 } }));
            Assert.IsTrue(cCompareAssert.EqualEnough(new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } }, new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } }));

            // falsehoods
            Assert.IsFalse(cCompareAssert.EqualEnough(null, new int[0][]));
            Assert.IsFalse(cCompareAssert.EqualEnough(new int[0][], null));
            Assert.IsFalse(cCompareAssert.EqualEnough(new int[][] { new int[] { 1, 2 }, new int[] { 1, 2, 3 } }, new int[][] { new int[] { 1, 2 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 } }, new int[][] { new int[] { 1, 2 }, new int[] { 2, 4 }, new int[] { 2, 4 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6 } }, new int[][] { new int[] { 1, 2, 3 }, new int[] { 2, 4, 6, 8, 10 } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 }, new int[] { 8, 16 } } }, new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 }, new int[] { 8, 16 }, new int[] { 8, 16 } } }));
            Assert.IsFalse(cCompareAssert.EqualEnough(new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, -8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } }, new int[][][] { new int[][] { new int[] { 1, 2 }, new int[] { 4, 8 } }, new int[][] { new int[] { 2, 4 }, new int[] { 8, 16 } } }));
        }

        #endregion EqualEnough

        #endregion cCompareAssert

        #endregion

        #region Classes
        /// <summary>
        /// A test object
        /// </summary>
        protected class ATestObject : ASimpleTestObject
        {
            /// <summary>
            /// Initialises a new instance of the <see cref="ATestObject"/> class. Test thing with default values for all the properties except ANull and AnATestObject
            /// </summary>
            /// <param name="anATestObject">
            /// The an A Test Object.
            /// </param>
            public ATestObject(ATestObject anATestObject = null)
            {
                this.ABoolean = false;
                this.AString = "foo";
                this.AnInt16 = 16;
                this.AnInt32 = 32;
                this.AnInt64 = 64;
                this.AnObject = "bar";
                this.AnEnum = ATestEnum.One;
                this.ASingle = 1f;
                this.ADouble = 1.1e+6;
                this.ADecimal = 13.37m;
                this.AChar = 'X';
                this.AGuid = Guid.Empty;
                this.AnATestObject = anATestObject;
                this.AnArrayList = new ArrayList { "Zero", "One", "Two" };
                this.ASortedList = new SortedList { { 0, "Zero" }, { 1, "One" }, { 2, "Two" } };
                this.ADictionary = new Dictionary<string, string> { { "0", "Zero" }, { "1", "One" }, { "2", "Two" } };
            }

            /// <summary>
            /// Gets a null
            /// </summary>
            public object ANull
            {
                get
                {
                    return null;
                }
            }

            /// <summary>
            /// Gets or sets an ATestObject default null
            /// </summary>
            public ATestObject AnATestObject
            {
                get;
                set;
            }
        }

        /// <summary>
        /// A test object
        /// </summary>
        protected class ASimpleTestObject
        {
            /// <summary>
            /// Initialises a new instance of the <see cref="ASimpleTestObject" /> class. Test thing with default values for all the properties
            /// </summary>
            public ASimpleTestObject()
            {
                this.ABoolean = false;
                this.AString = "foo";
                this.AnInt16 = 16;
                this.AnInt32 = 32;
                this.AnInt64 = 64;
                this.AnObject = "bar";
                this.AnEnum = ATestEnum.One;
                this.ASingle = 1f;
                this.ADouble = 1.1e+6;
                this.ADecimal = 13.37m;
                this.AChar = 'X';
                this.AGuid = Guid.Empty;
                this.AnArrayList = new ArrayList { "Zero", "One", "Two" };
                this.ASortedList = new SortedList { { 0, "Zero" }, { 1, "One" }, { 2, "Two" } };
                this.ADictionary = new Dictionary<string, string> { { "0", "Zero" }, { "1", "One" }, { "2", "Two" } };
            }

            /// <summary>
            /// Gets or sets a value indicating whether NOTHING, it's a boolean - default false
            /// </summary>
            public bool ABoolean
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a string default foo
            /// </summary>
            public string AString
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a short default -16
            /// </summary>
            public short AnInt16
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an int default -32
            /// </summary>
            public int AnInt32
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a long default -64
            /// </summary>
            public long AnInt64
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a ushort default 16
            /// </summary>
            public ushort AuInt16
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a uint default 32
            /// </summary>
            public uint AuInt32
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a ulong default 64
            /// </summary>
            public ulong AuInt64
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a float default 1f
            /// </summary>
            public float ASingle
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a double default 1.1e+6
            /// </summary>
            public double ADouble
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a decimal default 13.37m
            /// </summary>
            public decimal ADecimal
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a char default X
            /// </summary>
            public char AChar
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a Guid default "00000000-0000-0000-0000-000000000000"
            /// </summary> 
            public Guid AGuid
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an object default "bar"
            /// </summary>
            public object AnObject
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an ATestEnum default One
            /// </summary>
            public ATestEnum AnEnum
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an array of strings default "foo" "bar"
            /// </summary>
            public string[] AStringArray
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets an arraylist default "zero" "one" "two"
            /// </summary>
            public ArrayList AnArrayList
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a sorted list of int string default 0 "zero" 1 "one" 2 "two"
            /// </summary>
            public SortedList ASortedList
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets a dictionary of string string default "0" "zero" "1" "one" "2" "two"
            /// </summary>
            public Dictionary<string, string> ADictionary
            {
                get;
                set;
            }
        }
        #endregion Classes
    }
}