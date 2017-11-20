namespace UnitTest2012Ultimate.ConstructorValidation
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Deals with setting up the constructor ready for testing
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ConstructorTests<T>
    {
        /// <summary>
        /// Identifies the consturctor and and returns the wrapper class 
        /// </summary>
        /// <param name="argTypes">The types of the arguments for the constructor</param>
        /// <returns>The constructor within a <see cref="ConstructorTester{T}">ConstructorTester </see> wrapper class 
        /// or <see cref="MissingConstructorTester{T}"> </see>  if the constructor is missing arguements </returns>
        public static Tester<T> For(params Type[] argTypes)
        {
            ConstructorInfo ctor = typeof(T).GetConstructor(argTypes);

            if (ctor == null)
            {
               return new MissingConstructorTester<T>();
            }
            
            return new ConstructorTester<T>(ctor);
        }
    }
}
