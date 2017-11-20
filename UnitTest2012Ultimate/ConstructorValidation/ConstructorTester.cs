namespace UnitTest2012Ultimate.ConstructorValidation
{
    using System.Reflection;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class allows us to method chain and build up a list of constructor tests together and then execute them
    /// </summary>
    /// <typeparam name="T">The type param</typeparam>
    public class ConstructorTester<T> : Tester<T>
    {
        private readonly ConstructorInfo constructor;
        private readonly IList<TestCase<T>> testCases = new List<TestCase<T>>();

        /// <summary>
        /// Constructor to set the constructor we are testing
        /// </summary>
        /// <param name="ctor">The <see cref="ConstructorInfo">ConstructorInfo</see></param>
        public ConstructorTester(ConstructorInfo ctor)
        {
            this.constructor = ctor;
        }

        /// <summary>
        /// Add a test for invalid arugment values when calling the constructor which cause the 
        /// constructor validation to fail
        /// </summary>
        /// <param name="args">The values of the constructor arguements</param>
        /// <param name="exceptionType">The expected exception type returned from the constructor validator</param>
        /// <param name="failMessage">The fail message for the assertion</param>
        /// <returns>The reference to <see cref="Tester">Tester</see>/></returns>
        public override Tester<T> Fail(object[] args, Type exceptionType, string failMessage)
        {
            TestCase<T> testCase = new FailTest<T>(this.constructor, args, exceptionType, failMessage);
            this.testCases.Add(testCase);
            return this;
        }

        /// <summary>
        /// Adds a test for valid arugment values when calling the constructor which cause the 
        /// constructor validation to pass
        /// </summary>
        /// <param name="args"></param>
        /// <param name="failMessage"></param>
        /// <returns>The reference to <see cref="Tester">Tester</see>/></returns>
        public override Tester<T> Succeed(object[] args, string failMessage)
        {
            TestCase<T> testCase = new SuccessTest<T>(this.constructor, args, failMessage);
            this.testCases.Add(testCase);
            return this;
        }

        /// <summary>
        /// Executes the test cases collected within the testCases list
        /// </summary>
        public override void Assert()
        {
            var errors = new List<string>();
            this.ExecuteTestCases(errors);
            this.Assert(errors);
        }

        private void ExecuteTestCases(List<string> errors)
        {
            foreach (TestCase<T> testCase in this.testCases)
                ExecuteTestCase(errors, testCase);
        }

        private void ExecuteTestCase(List<string> errors, TestCase<T> testCase)
        {
            string error = testCase.Execute();
            if (!string.IsNullOrEmpty(error))
                errors.Add("    ----> " + error);
        }

        private void Assert(List<string> errors)
        {
            if (errors.Count > 0)
            {
                string error = string.Format("{0} error(s) occurred:\n{1}",
                                             errors.Count,
                                             string.Join("\n", errors.ToArray()));
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail(error);
            }
        }
    }
}
