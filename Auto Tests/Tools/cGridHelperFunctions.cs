using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITesting.HtmlControls;


namespace Auto_Tests
{
    public class GridHelpers
    {
        /// <summary>;
        /// Maps Table headers to index within Grid.
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="tableHeader"></param>
        /// <returns></returns>
        public static Dictionary<string, int> MapTableHeadersToLocation(UITestControlCollection collection, List<string> tableHeader)
        {
            Dictionary<string, int> FindIndex = new Dictionary<string, int>();

            HtmlRow htmlTableHeadersRow = (HtmlRow)collection[0];
            UITestControlCollection htmlTableHeadersCellsCollection = htmlTableHeadersRow.Cells;
            IEnumerator<UITestControl> iterator = htmlTableHeadersCellsCollection.GetEnumerator();

            while (iterator.MoveNext())
            {
                HtmlHeaderCell cell = (HtmlHeaderCell)iterator.Current;
                if (tableHeader.Contains(cell.FriendlyName))
                {
                    FindIndex.Add(cell.InnerText.Trim(), htmlTableHeadersCellsCollection.IndexOf(cell));
                }
            }
            return FindIndex;
        }

        /// <summary>
        /// Find the relevant row index that is passed in via search parameters.
        /// The search parameter in this case is usually the PK within the database which leads to each row
        /// being unique.
        /// </summary>
        /// <param name="table">look up table</param>
        /// <param name="rowsWithinTable"></param>
        /// <param name="searchParameters"> - what data to look up</param>
        /// <returns>row that data was found on if not found returns -1</returns>
        // TODO : remove default parameter and make function more generic
        public static int FindRowInGridForId(HtmlTable table, UITestControlCollection rowsWithinTable, string searchParameters, int columnIndex = 2)
        {
            foreach (UITestControl rowObj in rowsWithinTable)
            {
                HtmlRow rowObject = (HtmlRow)rowObj;
                UITestControlCollection listOfcells = rowObject.Cells;
                if (listOfcells.Count > 1)
                {
                    HtmlCell cell = listOfcells[columnIndex] as HtmlCell;
                    if (cell != null && cell.InnerText.Equals(searchParameters))
                    {
                        return rowsWithinTable.IndexOf(rowObj); 
                    }
                }
            }
            return -1;
        }
    }
}
