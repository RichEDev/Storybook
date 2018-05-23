using System.Collections.Generic;

namespace SpendManagementLibrary.Helpers
{
    using System.Linq;
    using System.Web.UI.WebControls;

    using BusinessLogic.Modules;

    using SpendManagementLibrary.Interfaces;

    /// <summary>
    /// A factory class to create <see cref="ListItem"/> entities from specific Spend Management Types.
    /// </summary>
    public class ListItemFactory
    {
        private readonly ICurrentUserBase _user;

        private readonly ICustomEntities _customEntities;

        /// <summary>
        /// Create a new Instance of <see cref="ListItemFactory"/>
        /// </summary>
        /// <param name="user">An instance of <see cref="ICurrentUserBase"/></param>
        /// <param name="customEntities">An instance of <see cref="ICustomEntities"/></param>
        public ListItemFactory(ICurrentUserBase user, ICustomEntities customEntities)
        {
            this._user = user;
            this._customEntities = customEntities;
        }

        /// <summary>
        /// Create a <see cref="List{T}"/> of <seealso cref="ListItem"/> based on a given <see cref="List{T}"/> of <seealso cref="cTable"/>
        /// </summary>
        /// <param name="lstItems">a <see cref="List{T}"/>of <seealso cref="cTable"/>to be used to create the list</param>
        /// <returns>A new List of <see cref="ListItem"/> with Attributes of;
        /// "data-category" - The current product name or "GreenLight" for custon emtity tables
        /// "customEntity"  - "True" if the table is a custom entity but set as System Entity</returns>
        public List<ListItem> CreateList(List<cTable> lstItems)
        {
            var result = new List<ListItem>();
            var systemTables = new List<ListItem>();
            var greenLightTables = new List<ListItem>();
            if (lstItems.Count > 0)
            {

                foreach (var table in lstItems)
                {
                    var listItem = new ListItem(table.Description.TrimStart(), table.TableID.ToString());
                    var metabase = true;
                    if (table.TableSource != cTable.TableSourceType.Metabase)
                    {
                        var customEntity = this._customEntities.getEntityByTableId(table.TableID);
                        listItem.Attributes["customEntity"] = "True";
                        if (customEntity == null || !customEntity.BuiltIn)
                        {
                            metabase = false;
                        }
                    }
                    else
                    {
                        listItem.Attributes["customEntity"] = "False";
                    }

                    if (!metabase)
                    {
                        listItem.Attributes["data-category"] = "GreenLights";
                    }
                    else
                    {
                        switch (this._user.CurrentActiveModule)
                        {
                            case Modules.CorporateDiligence:
                                listItem.Attributes["data-category"] = "Corporate Dilligence";
                                break;
                            case Modules.ESR:
                                listItem.Attributes["data-category"] = "ESR";
                                break;
                            case Modules.Greenlight:
                                listItem.Attributes["data-category"] = "GreenLights";
                                break;
                            case Modules.GreenlightWorkforce:
                                listItem.Attributes["data-category"] = "GreenLight Workforce";
                                break;
                            case Modules.SmartDiligence:
                                listItem.Attributes["data-category"] = "Smart Dilligigence";
                                break;
                            case Modules.Expenses:
                                listItem.Attributes["data-category"] = "Expenses";
                                break;
                            case Modules.Contracts:
                                listItem.Attributes["data-category"] = "Framework";
                                break;
                        }
                    }
                    if (metabase)
                    {
                        systemTables.Add(listItem);
                    }
                    else
                    {
                        greenLightTables.Add(listItem);
                    }
                }
            }

            result.AddRange(systemTables.OrderBy(l => l.Text));
            result.AddRange(greenLightTables.OrderBy(l => l.Text));

            return result;
        }
    }
}
