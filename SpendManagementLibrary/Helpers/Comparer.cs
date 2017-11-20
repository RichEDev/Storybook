using System;
using System.Collections.Generic;
using System.Linq;

namespace SpendManagementLibrary
{
    /// <summary>
    /// This can be used in LINQ Distinct statements (or anywhere where an IEqualityComparer is needed)
    ///  - ie. list.Concat(list).Distinct( Compare.By(lambda => lambda.id) ).ToDictionary()
    ///  - or list.Concat(list). DistinctBy(lambda => lambda.id) .ToDictionary()
    /// </summary>
    public static class Compare
    {
        /// <summary>
        /// Overload for the Distinct method in IEnumerable comparisons such as LINQ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TIdentity"></typeparam>
        /// <param name="source"></param>
        /// <param name="identitySelector"></param>
        /// <returns></returns>
        public static IEnumerable<T> DistinctBy<T, TIdentity>(this IEnumerable<T> source, Func<T, TIdentity> identitySelector)
        {
            return source.Distinct(Compare.By(identitySelector));
        }

        public static IEqualityComparer<TSource> By<TSource, TIdentity>(Func<TSource, TIdentity> identitySelector)
        {
            return new DelegateComparer<TSource, TIdentity>(identitySelector);
        }

        private class DelegateComparer<T, TIdentity> : IEqualityComparer<T>
        {
            private readonly Func<T, TIdentity> identitySelector;

            public DelegateComparer(Func<T, TIdentity> identitySelector)
            {
                this.identitySelector = identitySelector;
            }

            public bool Equals(T x, T y)
            {
                return Equals(identitySelector(x), identitySelector(y));
            }

            public int GetHashCode(T obj)
            {
                return identitySelector(obj).GetHashCode();
            }
        }
    }
}
