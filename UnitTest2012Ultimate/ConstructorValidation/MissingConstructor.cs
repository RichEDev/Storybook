namespace UnitTest2012Ultimate.ConstructorValidation
{
    using System;

    /// <summary>
    /// This classes purpose is to unconditionally fail when Assert is called
    /// Note that Fail and Succeed methods are actually just returning this object, which allows the caller to chain more methods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MissingConstructorTester<T> : Tester<T>
    {
        /// <summary>
        /// Holds the data relating to a failing constructor test case
        /// </summary>
        /// <param name="args">The invalid argument(s) values</param>
        /// <param name="exceptionType">The expected exception the validator should throw </param>
        /// <param name="failMessage">The fail message for the assertion</param>
        /// <returns>The object</returns>
        public override Tester<T> Fail(object[] args, Type exceptionType, string failMessage)
        {
            return this;
        }

        /// <summary>
        /// Holds the data relating to a successful constructor test case
        /// </summary>
        /// <param name="args">The valid argument values</param>
        /// <param name="failMessage">What message to throw if the test actually fails</param>
        /// <returns>The object</returns>
        public override Tester<T> Succeed(object[] args, string failMessage)
        {
            return this;
        }

        /// <summary>
        /// Call the assert to throw the constructor missing constructor fail
        /// </summary>
        public override void Assert()
        {
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail("Missing constructor.");
        }
    }
}
