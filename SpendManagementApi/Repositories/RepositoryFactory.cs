using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SpendManagementApi.Interfaces;
using SpendManagementApi.Models.Types;

namespace SpendManagementApi.Repositories
{
    /// <summary>
    /// Selects and instatiates Repositories.
    /// </summary>
    public static class RepositoryFactory
    {
        /// <summary>
        /// Gets the appropriate repository for the type passed in.
        /// </summary>
        /// <param name="args">Arguments to pass to the newly constructed repository.</param>
        /// <typeparam name="T">The underlying type of the repository to instantiate.</typeparam>
        /// <returns>The Repository.</returns>
        public static IRepository<T> GetRepository<T>(object[] args)
            where T : BaseExternalType
        {
            List<Type> list = Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(c => c.GetInterfaces().Contains(typeof(IRepository<T>)))
                    .ToList();
            if (!list.First().GetInterfaces().Contains(typeof(ISupportsActionContext)))
            {
                args = new object[1] { args[0] };
            }

            // creates an instance of the repository with arguements
            return (IRepository<T>)Activator.CreateInstance(list.First(), args);
        }

        /// <summary>
        /// Gets the appropriate repository for the type passed in.
        /// </summary>
        /// <typeparam name="T">The underlying type of the repository to instantiate.</typeparam>
        /// <returns>The Repository.</returns>
        public static IRepository<T> GetRepository<T>()
            where T : BaseExternalType
        {
            List<Type> list = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(c => c.GetInterfaces().Contains(typeof(IRepository<T>)))
                .ToList();
        
            // creates an instance of the repository without any arguements
            var instance = (IRepository<T>)Activator.CreateInstance(list.First());

            return instance;
        }
    }
}