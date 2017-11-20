namespace UnitTest2012Ultimate.ConstructorValidation
{
    using System;

    /// <summary>
    /// Base class for the test cases
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Tester<T>
    {
        /// <summary>
        /// Holds the data relating to a failing constructor test case
        /// </summary>
        /// <param name="args">The invalid argument(s) values</param>
        /// <param name="exceptionType">The expected exception the validator should throw </param>
        /// <param name="failMessage">The fail message for the assertion</param>
        /// <returns>The object</returns>
        public abstract Tester<T> Fail(object[] args, Type exceptionType, string failMessage);
   
        /// <summary>
        /// Holds the data relating to a successful constructor test case
        /// </summary>
        /// <param name="args">The valid argument values</param>
        /// <param name="failMessage">What message to throw if the test actually fails</param>
        /// <returns>The object</returns>
        /// 
        public abstract Tester<T> Succeed(object[] args, string failMessage);
     
        /// <summary>
        /// Invoke the assertions for the constructor tests
        /// </summary>
        public abstract void Assert();
    }
}
