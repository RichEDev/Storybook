namespace UnitTest2012Ultimate.Administration.FilterRules
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlTypes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpendManagementLibrary;
    using ConstructorValidation;

    /// <summary>
    /// This is a test class for validating the FilterRules constructor and methods.
    /// </summary>
    [TestClass]
    public class FilterRules
    {
        [ClassInitialize]
        public static void MyClassInitialize(TestContext context)
        {
            GlobalAsax.Application_Start();
        }

        /// <summary>
        /// Tests the constructor validation for <see cref="cFilterRule">cFilterRule</see>/>
        /// </summary>
        [TestMethod]
        [TestCategory("FilterRulesConstructorValidationTests")]
        public void ValidCostcodes()
        {
            const int validFilterId = 1;
            const FilterType validParentType = FilterType.Costcode;
            const FilterType validChildType = FilterType.Department;
            var validFilterRules = new Dictionary<int, cFilterRuleValue>();
            const int validParentUdfId = 1;
            const int validChildUdfId = 1;
            const bool enabled = true;
            DateTime validCreatedByDate = DateTime.Today;
            const int validCreatedById = 1;

            var filterTypeTests = GetConstructorWrapper();

            filterTypeTests.Fail(new object[] { -1, validParentType, validChildType, validFilterRules, validParentUdfId, validChildUdfId, enabled, validCreatedByDate, validCreatedById }, typeof(ArgumentException), "Negative filter Id")
                .Fail(new object[] { validFilterId, "invalid", validChildType, validFilterRules, validParentUdfId, validChildUdfId, enabled, validCreatedByDate, validCreatedById }, typeof(ArgumentException), "Invalid parent filter type")
                .Fail(new object[] { validFilterId, validParentType, "invalid", validFilterRules, validParentUdfId, validChildUdfId, enabled, validCreatedByDate, validCreatedById }, typeof(ArgumentException), "Invalid child filter type")
                .Fail(new object[] { validFilterId, validParentType, validChildType, null, validParentUdfId, validChildUdfId, enabled, validCreatedByDate, validCreatedById }, typeof(ArgumentNullException), "Null filter rules")
                .Fail(new object[] { validFilterId, FilterType.Userdefined, validChildType, validFilterRules, -1, validChildUdfId, enabled, validCreatedByDate, validCreatedById }, typeof(ArgumentException), "Invalid parent Udf Id")
                .Fail(new object[] { validFilterId, validParentType, FilterType.Userdefined, validFilterRules, validParentUdfId, -1, enabled, validCreatedByDate, validCreatedById }, typeof(ArgumentException), "Invalid child Udf Id")
                .Fail(new object[] { validFilterId, validParentType, validChildType, validFilterRules, validParentUdfId, validChildUdfId, enabled, DateTime.Today.AddDays(1), validCreatedById }, typeof(ArgumentException), "Created on date greated than today")
                .Fail(new object[] { validFilterId, validParentType, validChildType, validFilterRules, validParentUdfId, validChildUdfId, enabled, SqlDateTime.MinValue.Value.AddDays(-1), validCreatedById }, typeof(ArgumentException), "Created on date greated than 01/01/1753")
                .Fail(new object[] { validFilterId, validParentType, validChildType, validFilterRules, validParentUdfId, validChildUdfId, enabled, validCreatedByDate, -1 }, typeof(ArgumentException), "Invalid created by Id")
                .Succeed(new object[] { validFilterId, validParentType, validChildType, validFilterRules, validParentUdfId, validChildUdfId, enabled, validCreatedByDate, validCreatedById }, "Arguements provided where expected to pass constructor validation")
                .Assert();
        }

        private static Tester<cFilterRule> GetConstructorWrapper()
        {
            return ConstructorTests<cFilterRule>
                .For(typeof(int), typeof(FilterType), typeof(FilterType), typeof(Dictionary<int, cFilterRuleValue>), typeof(int), typeof(int), typeof(bool), typeof(DateTime), typeof(int));
        }
    }
}

