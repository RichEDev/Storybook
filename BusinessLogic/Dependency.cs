namespace BusinessLogic
{
    using System;

    /// <summary>
    /// Denotes that a specific attribute/property should be used for dependency injection
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Dependency : Attribute
    {
    }
}