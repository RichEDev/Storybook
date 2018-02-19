namespace SpendManagementApi.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Models.Common;
    using Models.Types;
    using SpendManagementLibrary;
    using Spend_Management;
    
    /// <summary>
    /// Sign off group repository
    /// </summary>
    internal class SignOffGroupsRepository : BaseRepository<SignOffGroup>, ISupportsActionContext
    {
        private readonly cGroups _signOffGroups;

        private readonly cEmployees _employees;

        public SignOffGroupsRepository(ICurrentUser user, IActionContext actionContext = null)
            : base(user, actionContext, group => group.GroupId, group => group.GroupName)
        {
            _signOffGroups = this.ActionContext.SignoffGroups;
            _employees = this.ActionContext.Employees;
        }

        public override IList<SignOffGroup> GetAll()
        {
            return
                _signOffGroups.groupList.GetValueList()
                              .Cast<cGroup>()
                              .Select(group => group.Cast<SignOffGroup>(_employees))
                              .ToList();
        }

        public override SignOffGroup Get(int signOffGroupId)
        {
            cGroup signOffGroup = _signOffGroups.GetGroupById(signOffGroupId);
            return signOffGroup.Cast<SignOffGroup>(_employees);
        }

        public override SignOffGroup Add(SignOffGroup signOffGroup)
        {
            base.Add(signOffGroup);

            signOffGroup.Validate(this.ActionContext);

            int groupId = _signOffGroups.SaveGroup(0, signOffGroup.GroupName, signOffGroup.Description, signOffGroup.OneClickAuthorization, User, 0, signOffGroup.NotifyClaimantWhenEnvelopeReceived, signOffGroup.NotifyClaimantWhenEnvelopeNotReceived);

            if (groupId <= 0)
            {
                throw new ApiException("Save unsuccessful", "The item being added could not be saved successfully");
            }

            this.UpdateStages(signOffGroup.Stages, groupId);

            return this.Get(groupId);
        }

        private void UpdateStages(List<Stage> stages, int groupId)
        {
            stages.ForEach(stage => stage.Validate(this.ActionContext));

            _signOffGroups.deleteStages(groupId);

            List<cStage> convertedStages = stages.ConvertAll(c => c.Cast<cStage>());

            convertedStages.ForEach(stage => _signOffGroups.addStage(
                        groupId,
                        stage.signofftype,
                        stage.relid,
                        (int)stage.include,
                        stage.amount,
                        stage.notify,
                        stage.onholiday,
                        stage.holidaytype,
                        stage.holidayid,
                        stage.includeid,
                        stage.claimantmail,
                        stage.singlesignoff,
                        stage.sendmail,
                        stage.displaydeclaration,
                        User.EmployeeID,
                        stage.signoffid,
                        false,
                        stage.ExtraApprovalLevels,
                        stage.FromMyLevel,
                        stage.ApproverJustificationsRequired,
                        stage.NhsAssignmentSupervisorApprovesWhenMissingCostCodeOwner,
                        stage.AllocateForPayment,
                        stage.IsPostValidationCleanupStage,
                        stage.ValidationCorrectionThreshold,
                        stage.ClaimPercentageToValidate)); //TODO: Feature flag

        }

        /// <summary> Deletes sign off group with specified id</summary>
        /// <param name="signOffGroupId">Sign Off Group Id</param>
        /// <returns>Null if object deleted successfully</returns>
        public override SignOffGroup Delete(int signOffGroupId)
        {
            if (_signOffGroups.getCountOfClaimsInProcessByGroupID(signOffGroupId) > 0)
            {
                throw new ApiException("Delete unsuccessful", "This signoff group cannot currently be deleted as there are one or more claims in the approval process relating to this signoff group.");
            }

            SignOffGroup signOffGroup = Get(signOffGroupId);

            if (signOffGroup == null)
            {
                throw new ApiException("Invalid sign off group id", "No data available for specified sign off group id");
            }

            int dataDeleted = _signOffGroups.DeleteGroup(signOffGroupId, User);

            switch (dataDeleted)
            {
                case 1:
                    throw new ApiException("Delete unsuccessful", "The signoff group cannot be deleted as it is assigned to an employee(s).");
                case 2:
                    throw new ApiException("Delete unsuccessful", "The signoff group cannot be deleted as it is assigned to an employee(s) advance group");
                case -10:
                    throw new ApiException("Delete unsuccessful", "The signoff group cannot be deleted as it is assigned to one or more GreenLights or user defined field records");                    
            }

            return this.Get(signOffGroupId);
        }

        /// <summary>
        /// Updates sign off group
        /// </summary>
        /// <param name="signOffGroup">Sign off group</param>
        /// <returns></returns>
        public override SignOffGroup Update(SignOffGroup signOffGroup)
        {
            base.Update(signOffGroup);

            signOffGroup.Validate(this.ActionContext);

            int success = _signOffGroups.SaveGroup(signOffGroup.GroupId, signOffGroup.GroupName, signOffGroup.Description, signOffGroup.OneClickAuthorization, User, 2, signOffGroup.NotifyClaimantWhenEnvelopeReceived, signOffGroup.NotifyClaimantWhenEnvelopeNotReceived);

            if (success <= 0)
            {
                throw new ApiException("Save unsuccessful", "Group could not be updated successfully. This group cannot be updated as the group name you have entered already exists.");
            }

            this.UpdateStages(signOffGroup.Stages, signOffGroup.GroupId);

            return this.Get(signOffGroup.GroupId);
        }

    }
}