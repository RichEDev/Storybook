namespace expenses.Bootstrap
{
    using System;
    using System.Linq;
    using System.Reflection;
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
            if (!typeof(Page).IsAssignableFrom(serviceType))
            {
                return false;
            }

            return propertyInfo.GetCustomAttributes(false).OfType<Dependency>().Any();

        }
    }
}