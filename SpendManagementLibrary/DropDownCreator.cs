namespace SpendManagementLibrary
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using BusinessLogic.ProjectCodes;

    /// <summary>
    /// Creates create down collections for the UI.
    /// </summary>
    public class DropDownCreator
    {
        public static ListItem[] GetProjectCodesListItems(bool useDesc, IEnumerable<IProjectCode> projectCodes, bool includeNone = false)
        {
            SortedList<string, IProjectCode> sortedList = new SortedList<string, IProjectCode>();

            foreach (IProjectCode projectCode in projectCodes)
            {
                if ((useDesc == false && sortedList.ContainsKey(projectCode.Name) == false) || (useDesc && sortedList.ContainsKey(projectCode.Description) == false))
                {
                    sortedList.Add(useDesc ? projectCode.Description : projectCode.Name, projectCode);
                }
            }

            List<ListItem> listItems = new List<ListItem>();

            if (includeNone)
            {
                listItems.Add(new ListItem("[None]", "0"));
            }

            foreach (KeyValuePair<string, IProjectCode> keyValuePair in sortedList)
            {
                listItems.Add(new ListItem(keyValuePair.Key, keyValuePair.Value.Id.ToString()));
            }

            return listItems.ToArray();
        }
    }
}
