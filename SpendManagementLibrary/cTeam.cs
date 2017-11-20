using System;
using System.Collections.Generic;

namespace SpendManagementLibrary
{
    using Interfaces;

    /// <summary>
    /// cTeam class
    /// </summary>
    [Serializable]
    public class cTeam : IOwnership
    {
        /// <summary>
        /// Team Id
        /// </summary>
        private int nTeamid;
        /// <summary>
        /// Team Name
        /// </summary>
        private string sTeamname;
        /// <summary>
        /// Team Description
        /// </summary>
        private string sDescription;
        /// <summary>
        /// Current Account ID
        /// </summary>
        private int nAccountid;
        /// <summary>
        /// IDs of Team Members
        /// </summary>
        private List<int> lstMembers;
        /// <summary>
        /// Employee ID of the nominated team leader (NULL if no team leader)
        /// </summary>
        private int? nTeamLeaderId;

        /// <summary>
        /// cTeam constructor
        /// </summary>
        /// <param name="accountid">Account ID</param>
        /// <param name="teamid">Team ID</param>
        /// <param name="teamname">Team Name</param>
        /// <param name="description">Team Id</param>
        /// <param name="members">Team Member Ids</param>
        /// <param name="teamleaderid">Employee Id of nominated team leader</param>
        /// <param name="createdon">Date team created</param>
        /// <param name="createdby">Employee Id who created the team</param>
        /// <param name="modifiedon">Date the team entity was last modified</param>
        /// <param name="modifiedby">Employee Id who last modified the team entity</param>
        public cTeam(int accountid, int teamid, string teamname, string description, List<int> members, int? teamleaderid, DateTime createdon, int? createdby, DateTime? modifiedon, int? modifiedby)
        {
            nAccountid = accountid;
            nTeamid = teamid;
            sTeamname = teamname;
            sDescription = description;
            lstMembers = members;
            nTeamLeaderId = teamleaderid;
            CreatedOn = createdon;
            CreatedBy = createdby;
            ModifiedOn = modifiedon;
            ModifiedBy = modifiedby;
        }

        #region properties
        /// <summary>
        /// Gets the current account id
        /// </summary>
        public int accountid
        {
            get { return nAccountid; }
        }
        /// <summary>
        /// Gets the current team id
        /// </summary>
        public int teamid
        {
            get { return nTeamid; }
        }
        /// <summary>
        /// Gets the current team name
        /// </summary>
        public string teamname
        {
            get { return sTeamname; }
        }
        /// <summary>
        /// Gets the team description
        /// </summary>
        public string description
        {
            get { return sDescription; }
        }
        /// <summary>
        /// Gets a collection of team member ids
        /// </summary>
        public List<int> teammembers
        {
            get { return lstMembers; }
        }
        /// <summary>
        /// Gets the nominated team leader employee Id (NULL if unspecified)
        /// </summary>
        public int? teamLeaderId
        {
            get { return nTeamLeaderId; }
        }
        #endregion

        public virtual int ItemPrimaryID()
        {
            return nTeamid;
        }

        public virtual string ItemDefinition()
        {
            return string.Format("{0} (Team)", sTeamname);
        }

        public virtual SpendManagementElement ElementType()
        {
            return SpendManagementElement.Teams;
        }

        public virtual int? OwnerId()
        {
            return teamid;
        }

        public virtual string OwnerDefinition()
        {
            return string.Empty;
        }

        public virtual SpendManagementElement OwnerElementType()
        {
            return SpendManagementElement.Teams;
        }
        public string CombinedItemKey
        {
            get
            {
                return string.Format("{0},{1}", (int)this.ElementType(), this.ItemPrimaryID());
            }
        }

        public string CombinedOwnerKey
        {
            get
            {
                return string.Format("{0},{1}", (int)this.OwnerElementType(), this.OwnerId());
            }
        }


        /// <summary>
        /// Date team created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Employee Id who created the team
        /// </summary>
        public int? CreatedBy { get; set; }

        /// <summary>
        /// Date the team entity was last modified
        /// </summary>
        public DateTime? ModifiedOn { get; set; }

        /// <summary>
        /// Employee Id who last modified the team entity
        /// </summary>
        public int? ModifiedBy { get; set; }
    }
}
