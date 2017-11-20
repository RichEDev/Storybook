namespace UnitTest2012Ultimate.API.CRUD.Objects
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using EsrGo2FromNhsWcfLibrary.Base;
    using EsrGo2FromNhsWcfLibrary.Spend_Management;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// The helper.
    /// </summary>
    public static class Helper
    {
        #region Private Methods

        /// <summary>
        /// Check fields using Assert on each public field in result.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="fields">
        /// The fields.
        /// </param>
        /// <param name="recordNumber">
        /// The record number.
        /// </param>
        public static void CheckFields(object result, SortedList<string, List<object>> fields, int recordNumber = 0)
        {
            if (result != null)
            {
                var propInfo = ((DataClassBase)result).ClassFields();
                foreach (KeyValuePair<string, List<object>> keyValuePair in fields)
                {
                    bool propertyChecked = false;
                    foreach (FieldInfo propertyInfo in propInfo)
                    {
                        var resultPropName = propertyInfo.Name.ToLower();
                        if (resultPropName == keyValuePair.Key.ToLower())
                        {
                            var resultValue = propertyInfo.GetValue(result);
                            if (resultValue != null)
                            {
                                var expectedValue = keyValuePair.Value[recordNumber];
                                if (resultValue is string)
                                {
                                    CheckField((string)resultValue, (string)expectedValue, resultPropName);
                                }

                                if (resultValue is int)
                                {
                                    CheckField((int)resultValue, (int)expectedValue, resultPropName);
                                }

                                if (resultValue is long)
                                {
                                    var shouldBe = long.Parse(expectedValue.ToString());
                                    CheckField((long)resultValue, shouldBe, resultPropName);
                                }

                                if (resultValue is byte)
                                {
                                    CheckField((byte)resultValue, (byte)expectedValue, resultPropName);
                                }

                                if (resultValue is bool)
                                {
                                    CheckField((bool)resultValue, (bool)expectedValue, resultPropName);
                                }

                                if (resultValue is DateTime)
                                {
                                    CheckField((DateTime)resultValue, (DateTime)expectedValue, resultPropName);
                                }

                                if (resultValue is PwdMethod)
                                {
                                    CheckField((byte)resultValue, (byte)expectedValue, resultPropName);
                                }

                                propertyChecked = true;
                            }
                        }
                    }

                    if (!propertyChecked)
                    {
                        Assert.Fail(string.Format("The input field {0} on record {1} was not checked", keyValuePair.Key, recordNumber));
                    }
                }
            }
        }

        /// <summary>
        /// The check DateTime field.
        /// </summary>
        /// <param name="resultValue">
        /// The result value.
        /// </param>
        /// <param name="expectedValue">
        /// The expected value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public static void CheckField(DateTime resultValue, DateTime expectedValue, string propertyName)
        {
            Assert.IsTrue(resultValue == expectedValue, string.Format("{0} should be {1} but returned {2}", propertyName, expectedValue, resultValue));
        }

        /// <summary>
        /// The check byte field.
        /// </summary>
        /// <param name="resultValue">
        /// The result value.
        /// </param>
        /// <param name="expectedValue">
        /// The expected value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public static void CheckField(byte resultValue, byte expectedValue, string propertyName)
        {
            Assert.IsTrue(resultValue == expectedValue, string.Format("{0} should be {1} but returned {2}", propertyName, expectedValue, resultValue));
        }

        /// <summary>
        /// The check integer field.
        /// </summary>
        /// <param name="resultValue">
        /// The result value.
        /// </param>
        /// <param name="expectedValue">
        /// The expected value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public static void CheckField(int resultValue, int expectedValue, string propertyName)
        {
            Assert.IsTrue(resultValue == expectedValue, string.Format("{0} should be {1} but returned {2}", propertyName, expectedValue, resultValue));
        }

        /// <summary>
        /// The check long (INT64) field.
        /// </summary>
        /// <param name="resultValue">
        /// The result value.
        /// </param>
        /// <param name="expectedValue">
        /// The expected value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public static void CheckField(long resultValue, long expectedValue, string propertyName)
        {
            Assert.IsTrue(resultValue == expectedValue, string.Format("{0} should be {1} but returned {2}", propertyName, expectedValue, resultValue));
        }

        /// <summary>
        /// The check bool field.
        /// </summary>
        /// <param name="resultValue">
        /// The result value.
        /// </param>
        /// <param name="expectedValue">
        /// The expected value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public static void CheckField(bool resultValue, bool expectedValue, string propertyName)
        {
            Assert.IsTrue(resultValue == expectedValue, string.Format("{0} should be {1} but returned {2}", propertyName, expectedValue, resultValue));
        }

        /// <summary>
        /// check string is equal expected.
        /// </summary>
        /// <param name="resultValue">
        /// The result value.
        /// </param>
        /// <param name="expectedValue">
        /// The expected value.
        /// </param>
        /// <param name="propertyName">
        /// The property name.
        /// </param>
        public static void CheckField(string resultValue, string expectedValue, string propertyName)
        {
            Assert.IsTrue(resultValue == expectedValue, string.Format("{0} should be {1} but returned {2}", propertyName, expectedValue, resultValue));
        }

        #endregion
    }
}
