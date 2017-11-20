using System;
namespace Spend_Management.shared.admin
{
    using BusinessLogic;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Reflection;
    using System.Web.Services;
    using System.Web.UI;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Cards;
    using SpendManagementLibrary.Enumerators;
    using System;

    public partial class corporatecardlogs : Page
    {
        /// <summary>
        /// Page load event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = cMisc.GetCurrentUser();
            var cardProviderId = Convert.ToInt32(Request.QueryString["cardproviderid"]);
            var corporateCards = new CorporateCards(user.AccountID);
            var corporateCard = corporateCards.GetCorporateCardById(cardProviderId);

            this.Title = $@"{corporateCard.cardprovider.cardprovider} Logs";
            this.Master.title = this.Title;

            this.ViewState["accountid"] = user.AccountID;
            this.ViewState["employeeid"] = user.EmployeeID;

            if (!IsPostBack)
            {
                var gridData = createCorporateCardLogsGrid(user, cardProviderId);
                litgrid.Text = gridData[1];

                Page.ClientScript.RegisterStartupScript(this.GetType(), "CorpCardGridVars", cGridNew.generateJS_init("CorpCardGridVars", new List<string>() { gridData[0] }, user.CurrentActiveModule), true);
            }

        }

        /// <summary>
        /// Create cGridNew
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="cardProviderId">Id of the corporate card</param>
        /// <returns></returns>
        [WebMethod(EnableSession = true)]
        protected string[] createCorporateCardLogsGrid(CurrentUser user, int cardProviderId)
        {
            var clsFields = new cFields(user.AccountID);

            var grid = new cGridNew(user.AccountID, user.EmployeeID, "gridCorporateCardLogs", this.GetImportsGridSql())
            {
                EmptyText = "No card logs to display",
                KeyField = "Date"
            };
            grid.getColumnByName("ImportId").hidden = true;
            grid.getColumnByName("CardProviderId").hidden = true;
            grid.getColumnByName("Status").HeaderText = "Import Status";
            grid.getColumnByName("Date").HeaderText = "Imported Date";
            grid.InitialiseRow += Grid_InitialiseRow;
            grid.ServiceClassForInitialiseRowEvent = "Spend_Management.shared.admin.corporatecardlogs";
            grid.ServiceClassMethodForInitialiseRowEvent = "Grid_InitialiseRow";

            grid.addFilter(clsFields.GetFieldByID(new Guid("9E3A92DB-A634-4D42-BB13-AF56A2C383EA")), ConditionType.Equals, new object[] { cardProviderId }, null, ConditionJoiner.None);
            grid.addEventColumn("viewLogs", "/shared/images/icons/history2.png", "javascript:SEL.CorporateCardLogs.showViewLogsModal('{ImportId}', '{Date}');", "View Import Log", "View Import Log");

            return grid.generateGrid();
        }

        /// <summary>
        /// Inititalise the grid rows
        /// </summary>
        /// <param name="row"></param>
        /// <param name="gridInfo"></param>
        private void Grid_InitialiseRow(cNewGridRow row, SerializableDictionary<string, object> gridInfo)
        {
            row.getCellByID("NumberOfErrors").Value =
                "<a href=\"javascript:SEL.CorporateCardLogs.showViewLogsModal('" + row.getCellByID("ImportId").Value + "', '" + row.getCellByID("Date").Value + "');\">" +
                row.getCellByID("NumberOfErrors").Value + "</a>";

            var test = Convert.ToInt32(row.getCellByID("Status").Value.ToString());
            var myEnum = (CorporateCardImportStatus)test;
            row.getCellByID("Status").Value = myEnum.GetType().GetMember(myEnum.ToString())
                .First()
                .GetCustomAttribute<DisplayAttribute>()
                .Name;
        }

        /// <summary>
        /// Gets sql for the import grid
        /// </summary>
        /// <returns>A sql select statement inside a string</returns>
        protected string GetImportsGridSql()
        {
            return "SELECT ImportId, CardProviderId, Date, [Status], [NumberOfErrors] FROM CorporateCardImportLog";
        }

        /// <summary>
        /// Close button event function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void cmdClose_Click(object sender, ImageClickEventArgs e)
        {
            Response.Redirect("~/admin/corporate_cards.aspx");
        }
    }
}