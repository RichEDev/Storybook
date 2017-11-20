namespace Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Teams
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;

    using Auto_Tests.Coded_UI_Tests.Spend_Management.Employee_Management.Employees;

    /// <summary>
    /// The teams.
    /// </summary>
    public class Teams
    {
        /// <summary>
        /// The teams sql item
        /// </summary>
        public static string SqlItems ="Select * from teams where teamid = @teamid";

        /// <summary>
        /// Initialises a new instance of the <see cref="Teams"/> class.
        /// </summary>
        public Teams()
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="Teams"/> class.
        /// </summary>
        /// <param name="teamId">
        /// The team id.
        /// </param>
        /// <param name="teamName">
        /// The team name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <param name="teamLeader">
        /// The team leader.
        /// </param>
        /// <param name="subAccountId">
        /// The sub account id.
        /// </param>
        /// <param name="teamMembers">
        /// The team Members.
        /// </param>
        public Teams(int teamId, string teamName, string description, int? teamLeader, int subAccountId, List<TeamMembers> teamMembers)
        {
            this.TeamId = teamId;
            this.TeamName = teamName;
            this.Description = description;
            this.SubAccountId = subAccountId;
            this.TeamLeader = teamLeader;
            this.TeamMembers = teamMembers;
        }

        /// <summary>
        /// Gets or sets the team id.
        /// </summary>
        internal int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the team name.
        /// </summary>
        internal string TeamName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        internal string Description { get; set; }

        /// <summary>
        /// Gets or sets the team leader.
        /// </summary>
        internal int? TeamLeader { get; set; }

        /// <summary>
        /// Gets or sets the sub account id.
        /// </summary>
        internal int SubAccountId { get; set; }

        /// <summary>
        /// Gets or sets the team members.
        /// </summary>
        internal List<TeamMembers> TeamMembers { get; set; }

        /// <summary>
        /// Gets the team grid values.
        /// </summary>
        internal List<string> TeamGridValues
        {
            get
            {
                return new List<string>
                           {
                               this.TeamName, 
                               this.Description
                           };
            }
        }

        internal string ApproverName
        {
            get
            {
                return String.Concat(this.TeamName, " (Team)");
            }
        }

    }

    /// <summary>
    /// The team members.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class TeamMembers
    {
        /// <summary>
        /// Gets the team members grid values.
        /// </summary>
        internal List<string> TeamMembersGridValues
        {
            get
            {
                return new List<string> { Employee.UserName, Employee.Title, Employee.FirstName, Employee.Surname };
            }
        }

        /// <summary>
        /// Gets or sets the team id.
        /// </summary>
        public int TeamId { get; set; }

        /// <summary>
        /// Gets or sets the emp.
        /// </summary>
        public Employees Employee { get; set; }
    }
}
