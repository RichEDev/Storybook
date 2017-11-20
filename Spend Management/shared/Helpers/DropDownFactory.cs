using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spend_Management.shared.Helpers
{
    using System.Web.UI.WebControls;

    using BusinessLogic.DataConnections;
    using BusinessLogic.Interfaces;

    public static class DropDownFactory
    {
        /// <summary>
        /// Creates a dropdown of entities. 
        /// </summary>
        /// <returns>A web ui List of ListItem representing the dropdown.</returns>
        /// <summary>
        /// Creates a drop down of entities.
        /// </summary>
        /// <typeparam name="T">The type to create a list of.  Must implement IListable and IArchivable.  Will filter out archived.</typeparam>
        /// <param name="repository">
        ///     The repository to use to get the data.
        ///     The icacherepository object.
        /// </param>
        /// <param name="useDescription">Use the description or label field.</param>
        /// <returns></returns>
        public static ListItem[] CreateDropDown<T>(IGetAll<T> repository, bool useDescription) where T : IListable, IArchivable
        {
            var entities = repository.Get().Where(x => !x.Archived);
            var items = new List<ListItem>();
            items.AddRange(entities.Select(entity => new ListItem(entity.ToString(useDescription), entity.Identifier)));

            return items.ToArray();
        }

        /// <summary>
        /// Creates a drop down of entities.
        /// </summary>
        /// <typeparam name="T">The type to create a list of.  Must implement IListable and IArchivable.  Will filter out archived.</typeparam>
        /// <param name="repository">The repository to use to get the data.</param>
        /// <param name="useDescription">Use the description or label field.</param>
        /// <returns></returns>
        //public static List<ListItem> CreateDropDownWithNoneOption<T>(IGetAll<T> repository, bool useDescription) where T : IListable, IArchivable
        //{

        //    var entities = repository.Get().Where(x => !x.Archived);
        //    var items = new List<ListItem> { new ListItem("[None]", "0") };
        //    items.AddRange(entities.Select(entity => new ListItem(entity.ToString(useDescription), entity.Identifier)));

        //    return items;
        //}

    }
}