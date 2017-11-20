namespace UnitTest2012Ultimate.ConstructorValidation
{
    using System.Reflection;

    /// <summary>
    /// Builds up the criteria for a successful test case
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SuccessTest<T> : TestCase<T>
    {
        /// <summary>
        /// Holds the data relating to a successful constructor test case
        /// </summary>
        /// <param name="ctor">The constructor info</param>
        /// <param name="args">The valid argument values</param>
        /// <param name="failMessage">What message to throw if the test actually fails</param>
        public SuccessTest(ConstructorInfo ctor, object[] args, string failMessage)
            : base(ctor, args, failMessage)
        {
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
            }
            catch (System.Exception ex)
            {
                return base.Fail(string.Format("{0} occurred: {1}",
                                 ex.GetType().Name, ex.Message));
            }

            return base.Success();
        }
    }
}
