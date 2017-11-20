using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using SpendManagementLibrary;
using System.Web.Script.Services;
using System.Text;

namespace Spend_Management
{
    public partial class aeAudience : System.Web.UI.Page
    {
        static int nAudienceID;

        /// <summary>
        /// Currently edited audience id
        /// </summary>
        public int AudienceID
        {
            get { return nAudienceID; }
            set { nAudienceID = value; }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            Master.enablenavigation = false;

            if (IsPostBack == false)
            {
                CurrentUser currentUser = cMisc.GetCurrentUser();
                currentUser.CheckAccessRole(AccessRoleType.View, SpendManagementElement.Audiences, true, true);

                int.TryParse(Request.QueryString["audienceid"], out nAudienceID);
                if (nAudienceID == 0)
                {
                    currentUser.CheckAccessRole(AccessRoleType.Add, SpendManagementElement.Audiences, true, true);
                    Title = "New Audience";
                }
                else
                {
                    currentUser.CheckAccessRole(AccessRoleType.Edit, SpendManagementElement.Audiences, true, true);
					cAudiences clsAudiences = new cAudiences(currentUser);
					cAudience reqAudience = clsAudiences.GetAudienceByID(nAudienceID);
										
					Title = "Audience: " + reqAudience.audienceName;
                    txtAudienceName.Text = reqAudience.audienceName;
                    txtAudienceDescription.Text = reqAudience.description;
                }
                Master.title = Title;
                Master.PageSubTitle = "Audience Details";                

                if (currentUser.CurrentActiveModule != Modules.expenses)
                {                    
                    budgetHolderLnkContainer.Style.Add("display", "none");
                }

                svcAudiences aud_srv = new svcAudiences();
                string[] empGridData = aud_srv.CreateEmployeesGrid(nAudienceID);
                litEmployees.Text = empGridData[1];
                string[] bhGridData = aud_srv.CreateBudgetHoldersGrid(nAudienceID);
                litBudgetHolders.Text = bhGridData[1];
                string[] teamGridData = aud_srv.CreateTeamsGrid(nAudienceID);
                litTeams.Text = teamGridData[1];

                List<string> jsBlockObjects = new List<string>();
                jsBlockObjects.Add(empGridData[0]);
                jsBlockObjects.Add(bhGridData[0]);
                jsBlockObjects.Add(teamGridData[0]);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "AudienceGridVars", "SEL.Audience.AudienceID = " + AudienceID.ToString() + ";\n" + cGridNew.generateJS_init("AudienceGridVars", jsBlockObjects, currentUser.CurrentActiveModule), true);
            }
        }

        [WebMethod(EnableSession = true)]
        public static void updateAudienceID(int audienceID)
        {
            nAudienceID = audienceID;
        }
    }
}
