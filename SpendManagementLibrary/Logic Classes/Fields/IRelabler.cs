// Do not copy
namespace SpendManagementLibrary.Logic_Classes.Fields
{
    using System.Collections.Generic;

    public interface IRelabler<T> where T:IRelabel
    {
        /// <summary>
        /// Set the Description property of <see cref="T"/> based on the Relabel Param
        /// </summary>
        /// <param name="field">The <see cref="T"/>to "relabel"</param>
        /// <returns>The given <see cref="T"/>with the description re-set as necessary</returns>
        T Relabel(T field);

        /// <summary>
        /// Set the Description property of <see cref="T"/> based on the Relabel Param
        /// </summary>
        /// <param name="sortedList">The a sorted list of <see cref="T"/>to "relabel"</param>
        /// <returns>The given sortedlist of <see cref="T"/>with the description re-set as necessary</returns>
        SortedList<TK, T> Convert<TK>(SortedList<TK, T> sortedList);

        /// <summary>
        /// Set the Description property of <see cref="T"/> based on the Relabel Param
        /// </summary>
        /// <param name="list">A <see cref="List{T}"/> of <see cref="T"/>to "relabel"</param>
        /// <returns>The given <see cref="List{T}"/> of <see cref="T"/>with the description re-set as necessary</returns>
        List<T> Convert(List<T> list);

        /// <summary>
        /// Set the Description property of <see cref="T"/> based on the Relabel Param
        /// </summary>
        /// <param name="field">The <see cref="T"/>to "relabel"</param>
        /// <returns>The given <see cref="T"/>with the description re-set as necessary</returns>
        T Convert(T field);
        
    }
}