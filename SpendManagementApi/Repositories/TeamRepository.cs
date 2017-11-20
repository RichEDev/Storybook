namespace SpendManagementApi.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SpendManagementApi.Models.Common;
    using Models.Types;
    using Utilities;
    using Spend_Management;


    /// <summary>TeamRepository manages data access for Teams.</summary>
    internal class TeamRepository : BaseRepository<Team>
    {
        private readonly cTeams _data;

        /// <summary>Creates a new TeamRepository with the passed in user.</summary>
        /// <param name="user">The user</param>
        public TeamRepository(ICurrentUser user) : base(user, x => x.Id, x => x.Label)
        {
            _data = new cTeams(User.AccountID, null);
        }

        /// <summary>Gets all the Teams within the system.</summary>
        /// <returns>A List of teams</returns>
        public override IList<Team> GetAll()
        {
            return _data.GetAllTeams().Select(b => new Team().From(b, ActionContext)).ToList();
        }

        /// <summary>Gets a single Team by it's id.</summary>
        /// <param name="id">The id of the Team to get.</param>
        /// <returns>The Team.</returns>
        public override Team Get(int id)
        {
            var item = _data.GetTeamById(id);
            if (item == null)
            {
                return null;
            }
            return new Team().From(item, ActionContext);
        }

        /// <summary>Adds a Team.</summary>
        /// <param name="item">The team to add.</param>
        /// <returns>The added team</returns>
        public override Team Add(Team item)
        {
            item = base.Add(item);
            return Save(item);
        }
       
        /// <summary>Updates a Team. You can change the team leader using PUT.</summary>
        /// <param name="item">The item to update.</param>
        /// <returns>The updated Team.</returns>
        public override Team Update(Team item)
        {
            item = base.Update(item);
            return Save(item);
        }

        /// <summary>Links employees to a team, via their Ids.</summary>
        /// <param name="id">The Id of the team.</param>
        /// <param name="employeeIds">The Ids of the employees.</param>
        /// <returns>The updated team.</returns>
        public Team LinkEmployees(int id, List<int> employeeIds)
        {
            var team = Get(id);
            if (team == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist,
                       ApiResources.ApiErrorRecordDoesntExistMessage);
            }
            team.TeamMembers = employeeIds;
            return Save(team);
        }

        /// <summary>Changes the team leader for the team. </summary>
        /// <param name="id">The Id of the team.</param>
        /// <param name="teamLeaderId">The Id of the team leader.</param>
        /// <returns>The modified team</returns>
        public Team ChangeTeamLeader(int id, int teamLeaderId)
        {
            var team = Get(id);
            if (team == null)
            {
                throw new ApiException(ApiResources.ApiErrorRecordDoesntExist,
                       ApiResources.ApiErrorRecordDoesntExistMessage);
            }
            team.TeamLeaderId = teamLeaderId;
            return Save(team);
        }
        
        /// <summary>Deletes a team, given it's ID.</summary>
        /// <param name="id">The Id of the Team to delete.</param>
        /// <returns>The deleted team.</returns>
        public override Team Delete(int id)
        {
            var item = base.Delete(id);

            var result = _data.DeleteTeam(id);
            if (result != 0)
            {
                throw new ApiException(ApiResources.ApiErrorDeleteUnsuccessful,
                    ApiResources.ApiErrorDeleteUnsuccessfulMessage);
            }

            return item;
        }



        private Team Save(Team item)
        {
            item.AccountId = User.AccountID;
            item.ModifiedById = User.EmployeeID;
            item.ModifiedOn = DateTime.UtcNow;

            // check the employees here.
            var employees = new cEmployees(User.AccountID);
            if (item.TeamMembers.Any(employeeId => employees.GetEmployeeById(employeeId) == null))
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorWrongEmployeeIdMessage);
            }

            if (item.TeamLeaderId.HasValue && employees.GetEmployeeById(item.TeamLeaderId.Value) == null)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorWrongTeamLeaderIdMessage);
            }


            // save the team
            var teamId = _data.SaveTeam(item.Id, item.Label, item.Description, item.TeamLeaderId ?? -1, User.EmployeeID);

            if (teamId == -1)
            {
                throw new ApiException(ApiResources.ApiErrorRecordAlreadyExists,
                    ApiResources.ApiErrorRecordAlreadyExistsMessage);
            }

            if (teamId < 1)
            {
                throw new ApiException(ApiResources.ApiErrorSaveUnsuccessful,
                    ApiResources.ApiErrorSaveUnsuccessfulMessage);
            }

            // get the added / updated db item
            var result = Get(teamId);
            result.Id = teamId;
            
            // work out which employees need removing / adding 
            var toRemove = result.TeamMembers.Where(i => !item.TeamMembers.Contains(i)).ToList();
            var toAdd = item.TeamMembers.Where(i => !result.TeamMembers.Contains(i)).ToList();

            // ensure that the leader is in the team
            if (item.TeamLeaderId.HasValue && !item.TeamMembers.Contains(item.TeamLeaderId.Value))
            {
                toAdd.Add(item.TeamLeaderId.Value);
                item.TeamMembers.Add(item.TeamLeaderId.Value);
            }

            // change the employees for the team.
            _data.ChangeTeamEmployees(teamId, toAdd, toRemove, User.AccountID, User.EmployeeID);
            result.TeamMembers = item.TeamMembers;

            return result;
        }

    }
}
