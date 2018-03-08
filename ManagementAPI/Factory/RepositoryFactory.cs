namespace ManagementAPI.Factory
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using ManagementAPI.Interface;

    public static class RepositoryFactory
    {
        public static IRepository<T> GetRepository<T>()
        
        {
            List<Type> list = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(c => c.GetInterfaces().Contains(typeof(IRepository<T>)))
                .ToList();
         
            return (IRepository<T>)Activator.CreateInstance(list.First());
        }
    }
}