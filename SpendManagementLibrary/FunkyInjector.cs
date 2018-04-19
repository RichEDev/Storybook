namespace SpendManagementLibrary
{
    using SimpleInjector;

    /// <summary>
    /// Helper property used to access the container inside the non SOLID projects. 
    /// </summary>
    public static class FunkyInjector
    {
        /// <summary>
        /// Use me to access the container in classes where following dependency injection would potentially cause performance issues or the trailing of too many dependencies
        /// </summary>
        public static Container Container { get; set; }
    }
}
