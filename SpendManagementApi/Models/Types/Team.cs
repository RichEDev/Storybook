namespace SpendManagementApi.Models.Types
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using Interfaces;
    using Employees;
    using Utilities;
    using SpendManagementLibrary;
    
    /// <summary>
    /// A Team is a group of <see cref="Employee"/> Employees.
    /// </summary>
    public class Team : BaseExternalType, IApiFrontForDbObject<cTeam, Team>
    {
        /// <summary>
        /// The unique Id for this Team object.
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// The name / label for this Team object.
        /// </summary>
        [Required, MaxLength(50, ErrorMessage = ApiResources.ErrorMaxLength + @"50")]
        public string Label { get; set; }

        /// <summary>
        /// A description of this Team object.
        /// </summary>
        [Required, MaxLength(4000, ErrorMessage = ApiResources.ErrorMaxLength + @"4000")]
        public string Description { get; set; }

        /// <summary>
        /// A list of the Ids of all the Employees that are assigned to this team.
        /// </summary>
        public List<int> TeamMembers { get; set; }
        
        /// <summary>
        /// Gets the nominated team leader employee Id (NULL if unspecified)
        /// </summary>
        public int? TeamLeaderId { get; set; }

        /// <summary>
        /// Creates a new Team.
        /// </summary>
        public Team()
        {
            TeamMembers = new List<int>();
        }

        /// <summary>
        /// Converts from the DAL type to the API type.
        /// </summary>
        /// <param name="dbType">The DAL type.</param>
        /// <param name="actionContext">The IActionContext</param>
        /// <returns>This, the API type.</returns>
        public Team From(cTeam dbType, IActionContext actionContext)
        {
            Id = dbType.teamid;
            Label = dbType.teamname;
            Description = dbType.description;
            TeamMembers = dbType.teammembers.ToList();
            TeamLeaderId = dbType.teamLeaderId;

            AccountId = dbType.accountid;
            CreatedById = dbType.CreatedBy ?? -1;
            CreatedOn = dbType.CreatedOn;
            ModifiedById = dbType.ModifiedBy ?? -1;
            ModifiedOn = dbType.ModifiedOn;
            return this;
        }


        /// <summary>
        /// Converts from the API type to the DAL type.
        /// </summary>
        /// <returns>The DAL type.</returns>
        public cTeam To(IActionContext actionContext)
        {   
            return new cTeam(AccountId ?? -1, Id, Label, Description, TeamMembers, TeamLeaderId, CreatedOn, CreatedById, ModifiedOn, ModifiedById);
        }
    }

}