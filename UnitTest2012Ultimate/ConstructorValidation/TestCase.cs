namespace UnitTest2012Ultimate.ConstructorValidation
{
    using System.Reflection;

    /// <summary>
    /// Builds up the basic test case
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TestCase<T>
    {
        private readonly ConstructorInfo constructor;
        private readonly object[] arguments;
        private readonly string failMessage;

        protected TestCase(ConstructorInfo ctor, object[] args, string failMessage)
        {
            this.constructor = ctor;
            this.arguments = args;
            this.failMessage = failMessage;
        }

        protected T InvokeConstructor()
        {
            try
            {
                return (T)this.constructor.Invoke(this.arguments);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
        }

        protected string Fail(string msg)
        {
            return string.Format("Test failed ({0}): {1}", this.failMessage, msg);
        }

        protected string Success()
        {
            return string.Empty;
        }

        public abstract string Execute();
    }
}
