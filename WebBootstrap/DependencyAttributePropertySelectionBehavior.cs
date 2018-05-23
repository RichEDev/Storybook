namespace WebBootstrap
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Web.Services;
    using System.Web.UI;

    using BusinessLogic;

    using SimpleInjector.Advanced;

    /// <summary>
    /// SimpleInjector code, makes the <see cref="Dependency"/> act as a notice to inject objects.
    /// </summary>
    class DependencyAttributePropertySelectionBehavior : IPropertySelectionBehavior
    {
        public bool SelectProperty(Type serviceType, PropertyInfo propertyInfo)
        {
            // Makes use of the System.ComponentModel.Composition assembly
            return (typeof(Page).IsAssignableFrom(serviceType) ||
                    typeof(UserControl).IsAssignableFrom(serviceType) ||
                    typeof(WebService).IsAssignableFrom(serviceType)) &&
                   propertyInfo.GetCustomAttributes<Dependency>().Any();
        }
    }
}