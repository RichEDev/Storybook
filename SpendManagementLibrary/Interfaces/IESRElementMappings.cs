namespace SpendManagementLibrary.Interfaces
{
    using System.Collections.Generic;
    using System.Web.UI.WebControls;

    public interface IESRElementMappings
    {
        int AccountID { get; }

        int NHSTrustID { get; }

        /// <summary>
        /// Get the ESR element by the passed in ID from the global list
        /// </summary>
        /// <param name="ElementID">Unique ID of the element</param>
        /// <returns>An element object</returns>
        cESRElement getESRElementByID(int ElementID);

        SortedList<int, cESRElement> listElements();

        /// <summary>
        /// Returns a list of all the unmapped expense items for this trust
        /// </summary>
        /// <returns></returns>
        List<cSubcat> GetUnMappedExpenseItems();

        /// <summary>
        /// Get all elements associated to the subcat passed in
        /// </summary>
        /// <param name="SubcatID">Unique ID of the expenses subcat</param>
        /// <returns>A list of elements</returns>
        List<cESRElement> getESRElementsBySubcatID(int SubcatID);

        /// <summary>
        /// Save the ESR element and its corresponding fields and subcats to the database
        /// </summary>
        /// <param name="element">Element object to save</param>
        /// <returns>The ID of the element</returns>
        int saveESRElement(cESRElement element);

        /// <summary>
        /// Delete the element from the database. The associated element fields and subcats will cascade delete in the databa.se
        /// </summary>
        /// <param name="ElementID">Unique ID of the element</param>
        void deleteESRElement(int ElementID);

        /// <summary>
        /// Get all the fields required for an ESR Inbound export for the report column dropdown
        /// </summary>
        /// <returns></returns>
        List<ListItem> CreateReportColumnDropDown();

        /// <summary>
        /// Get the Global Elements for a dropdown
        /// </summary>
        /// <returns></returns>
        List<ListItem> CreateGlobalElementDropDown();

        /// <summary>
        /// Return the global element for the ID
        /// </summary>
        /// <param name="ElementID">Global Element ID</param>
        /// <returns>Global Element from Cache</returns>
        cGlobalESRElement GetGlobalESRElementByID(int ElementID);

        SortedList<int, cGlobalESRElement> lstGlobalElements();

        /// <summary>
        /// Get the global element that matches the given string.
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        cGlobalESRElement GetGlobalESRElement(string elementName);
    }
}