using System.Collections.Generic;
using System.Data;
using Syncfusion.DocIO.DLS;

namespace SpendManagementLibrary.DocumentMerge
{
    using System;

    public static class TorchExtensionMethods
    {
        /// <summary>
        /// Adds the delete extension method to a Datatable that will immediately delete rows specified by the filter
        /// </summary>
        /// <param name="table">The DataTable to affect</param>
        /// <param name="filter">The filter used to remove rows.</param>
        /// <returns></returns>
        public static DataTable Delete(this DataTable table, string filter)
        {
            table.Select(filter).Delete();
            return table;
        }
        public static void Delete(this IEnumerable<DataRow> rows)
        {
            foreach (var row in rows)
                row.Delete();
        }

        /// <summary>
        /// Surrounds a string with apostrophees ready for a sql like function, and then shouts "come out with your hands in the air!!!!"
        /// </summary>
        /// <param name="apostrophizeee">An outlaw string, ready to be surrounded</param>
        /// <returns>A surrendered string surrounded by apostrophes</returns>
        public static string Apostrophize(this string apostrophizeee)
        {
            return string.Format("{0}{1}{0}", "'", apostrophizeee);
        }

        /// <summary>
        /// Surrounds a string with % ready for a sql like function, and then shouts "ON THE FLOOR NOWWWWWW !!!!"
        /// </summary>
        /// <param name="likanizeee">An outlaw string, ready to be surrounded</param>
        /// <returns>A surrendered string surrounded by apostrophes</returns>
        public static string Likanize(this string likanizeee)
        {
            return string.Format("{0}{1}{0}", "%", likanizeee);
        }

        /// <summary>
        /// Removes the last occurence of the specified string
        /// </summary>
        /// <param name="original"></param>
        /// <param name="removeLastWhat"></param>
        /// <returns></returns>
        public static string RemoveLast(this string original, string removeLastWhat)
        {
            if (string.IsNullOrEmpty(original))
            {
                return string.Empty;
            }

            string returnString = original;

            if (returnString.EndsWith(removeLastWhat))
            {
                returnString = returnString.Remove(returnString.LastIndexOf(removeLastWhat, StringComparison.Ordinal));
            }

            return returnString;
        }

        /// <summary>
        /// Find all occurances of type T in the given word document.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document"></param>
        /// <param name="findType">The type to find</param>
        /// <returns></returns>
        public static List<IEntity> FindAll(this IWordDocument document, Type findType)
        {
            var result = new List<IEntity>();
            FindNext(document.ChildEntities, result, findType);
            return result;
        }

        private static void FindNext(IEntityCollectionBase childEntities, List<IEntity> result, Type findType)
        {
            foreach (IEntity childEntity in childEntities)
            {
                if (childEntity.GetType() == findType)
                {
                    result.Add(childEntity);
                }

                var compositeEntity = childEntity as ICompositeEntity;
                if (compositeEntity != null)
                {
                    FindNext(compositeEntity.ChildEntities, result, findType);
                }
            }
        }

        /// <summary>
        /// Find all occurances of items that use a specified style.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="findStyle"></param>
        /// <returns></returns>
        public static List<IEntity> FindAll(this IWordDocument document, IStyle findStyle)
        {
            var result = new List<IEntity>();

            if (findStyle != null)
            {
                var styleName = findStyle.Name;
                FindNext(document.ChildEntities, result, styleName);
            }
            return result;
        }

        private static void FindNext(EntityCollection childEntities, List<IEntity> result, string styleName)
        {
            foreach (IEntity childEntity in childEntities)
            {
                var objectWithStyle = childEntity as IStyleHolder;
                if (objectWithStyle != null)
                {
                    if (objectWithStyle.StyleName == styleName)
                    {
                        result.Add(childEntity);
                    }
                }

                var compositeEntity = childEntity as ICompositeEntity;
                if (compositeEntity != null)
                {
                    FindNext(compositeEntity.ChildEntities, result, styleName);
                }
            }
        }
    }
}
