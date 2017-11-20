namespace SpendManagementApi.Models.Types
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interfaces;
    using Common;
    using SpendManagementLibrary;
    using SpendManagementLibrary.Enumerators.Expedite;
    using Spend_Management;

    /// <summary>
    /// Represents a group of stages that a claim has to go through in order to be approved and therefore paid.
    /// </summary>
    public class SignOffGroup : BaseExternalType, IRequiresValidation, IEquatable<SignOffGroup>
    {
        /// <summary>
        /// The unique Id of the signoffgroup.
        /// </summary>
        public int GroupId { get; set; }

        /// <summary>
        /// The name of the signoffgroup.
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// The description of the signoffgroup.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Whether the signoffgroup supports OneClickAuthorization.
        /// </summary>
        public bool OneClickAuthorization { get; set; }

        /// <summary>
        /// The list of stages that comprises this signoffgroup.
        /// </summary>
        public List<Stage> Stages { get; set; }

        /// <summary>
        /// Whether to notify the claimant when their envelope is received.
        /// This will only have a value if the account has ReceiptsService enabled, 
        /// and if the group contains the Scan & Attach stage, with the UI value set.
        /// </summary>
        public bool? NotifyClaimantWhenEnvelopeReceived { get; set; }

        /// <summary>
        /// Whether to notify the claimant when their envelope is not received after the configured time has passed.
        /// This will only have a value if the account has ReceiptsService enabled, 
        /// and if the group contains the Scan & Attach stage, with the UI value set.
        /// </summary>
        public bool? NotifyClaimantWhenEnvelopeNotReceived { get; set; }
        

        public void Validate(IActionContext actionContext)
        {
            if (this.GroupId > 0 && actionContext.SignoffGroups.GetGroupById(this.GroupId) == null)
            {
                throw new ApiException("Invalid Group Id", "No record available for specified id");
            }

            if (this.GroupId == 0 && actionContext.SignoffGroups.getGroupByName(this.GroupName) != null)
            {
                throw new ApiException("Invalid Group Name", "Group with name specified already exists.");
            }

            if (string.IsNullOrEmpty(this.Description))
            {
                throw new ApiException("Invalid Group Description", "Group Description is a required field");
            }

            if (Stages.Count == 0)
            {
                throw new ApiException("Invalid Stage Count", "The group must include at least 1 signoff stage");
            }

            if (this.GroupId > 0 && actionContext.SignoffGroups.getCountOfClaimsInProcessByGroupID(this.GroupId) > 0)
            {
                throw new ApiException("Save unsuccessful", "This signoff group cannot currently be amended as there are one or more claims in the approval process relating to this signoff group.");
            }

            if (!actionContext.Accounts.GetAccountByID(actionContext.AccountId).ReceiptServiceEnabled &&
                (NotifyClaimantWhenEnvelopeReceived.HasValue || NotifyClaimantWhenEnvelopeNotReceived.HasValue))
            {
                throw new ApiException("Invalid notification settings", "You may not set NotifyClaimantWhenEnvelopeReceived or NotifyClaimantWhenEnvelopeNotReceived if your account does not have the Receipts service enabled.");
            }


            this.Stages.ForEach(s => s.Validate(actionContext));

            // validate
            var stageValidity = actionContext.SignoffGroups.ValidateGroupStages(this.Cast<cGroup>());
            if (!stageValidity.Result)
            {
                throw new ApiException("Invalid Stage Error", stageValidity.Messages.Aggregate((a, b) => a + "\n" + b));
            }
        }

        public bool Equals(SignOffGroup other)
        {
            if (other == null)
            {
                return false;
            }
            return this.GroupName.Equals(other.GroupName)
                   && this.Description.Equals(other.Description)
                   && this.OneClickAuthorization.Equals(other.OneClickAuthorization)
                   && this.Stages.SequenceEqual(other.Stages);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as SignOffGroup);
        }
    }

    internal static class SignOffGroupConversion
    {
        public static TResult Cast<TResult>(this cGroup group, cEmployees employees) where TResult : SignOffGroup, new()
        {
            if (group == null)
            {
                return null;
            }
            return new TResult
                       {
                           AccountId = group.accountid,
                           CreatedById = group.createdby,
                           CreatedOn = group.createdon,
                           Description = group.description,
                           GroupId = group.groupid,
                           GroupName = group.groupname,
                           ModifiedById = group.modifiedby,
                           ModifiedOn = group.modifiedon,
                           OneClickAuthorization = group.oneclickauthorisation,
                           Stages = group.stages.Values.Select(stage => stage.Cast<Stage>(group.accountid, group.createdby, employees)).ToList(),
                           NotifyClaimantWhenEnvelopeReceived = group.NotifyClaimantWhenEnvelopeReceived,
                           NotifyClaimantWhenEnvelopeNotReceived = group.NotifyClaimantWhenEnvelopeNotReceived
                       };
        }

        public static cGroup Cast<TResult>(this SignOffGroup group) where TResult : cGroup
        {
            var stages = new SerializableDictionary<int, cStage>();
            group.Stages.ForEach(
                stage =>
                    {
                        stage.CreatedOn = DateTime.UtcNow;
                        stage.CreatedById = group.EmployeeId.Value;
                        stages.Add(stage.SignOffStage, stage.Cast<cStage>());
                    });

            return new cGroup(@group.AccountId.Value, @group.GroupId, @group.GroupName, @group.Description,
                @group.OneClickAuthorization, @group.CreatedOn, @group.CreatedById,
                (@group.ModifiedOn.HasValue) ? @group.ModifiedOn.Value : DateTime.UtcNow,
                @group.ModifiedById.HasValue ? @group.ModifiedById.Value : @group.EmployeeId.Value, stages,
                notifyClaimantWhenEnvelopeReceived: @group.NotifyClaimantWhenEnvelopeReceived, notifyClaimantWhenEnvelopeNotReceived: @group.NotifyClaimantWhenEnvelopeNotReceived);
        }
    }
}