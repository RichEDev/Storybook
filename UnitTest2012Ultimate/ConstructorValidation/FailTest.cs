namespace UnitTest2012Ultimate.ConstructorValidation
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Builds up the criteria for a failing test case
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FailTest<T> : TestCase<T>
    {
        private readonly Type exceptionType;

        /// <summary>
        /// Holds the data relating to a failing constructor test case
        /// </summary>
        /// <param name="ctor">The constructor info</param>
        /// <param name="args">The invalid argument(s) values</param>
        /// <param name="exceptionType">The expected exception the validator should throw </param>
        /// <param name="failMessage">The fail message for the assertion</param>
        public FailTest(ConstructorInfo ctor, object[] args, Type exceptionType, string failMessage)
            : base(ctor, args, failMessage)
        {
            this.exceptionType = exceptionType;
        }

        /// <summary>
        /// Attempt to invoke the constructor
        /// </summary>
        /// <returns>The status of the execute</returns>
        public override string Execute()
        {
            try
            {
                base.InvokeConstructor();
                //invoking constructor should have failed
                return base.Fail(string.Format("{0} not thrown when expected.",
                                 this.exceptionType.Name));
            }
            catch (Exception ex)
            {
                if (ex.GetType() != this.exceptionType)
                    return base.Fail(string.Format("{0} thrown when {1} was expected.",
                                     ex.GetType().Name, this.exceptionType.Name));
            }

            return base.Success();
        }
    }
}
