namespace SpendManagementLibrary.Flags
{
    using System;
    using System.Collections.Generic;
    using SpendManagementLibrary.Interfaces;

    [Serializable]
    public class JourneyDoesNotStartAndFinishAtHomeOrOfficeFlag : Flag
    {
        public JourneyDoesNotStartAndFinishAtHomeOrOfficeFlag(
               int flagid,
               FlagType flagtype,
               FlagAction action,
               string flagtext,
               List<int> associateditemroles,
               List<AssociatedExpenseItem> associatedexpenseitems,
               DateTime createdon,
               int? createdby,
               DateTime? modifiedon,
               int? modifiedby,
               string description,
               bool active,
               int accountid,
               bool claimantjustificationrequired,
               bool displayflagimmediately,
               FlagColour flaglevel,
               bool approverjustificationrequired,
               string notesforauthoriser,
               FlagInclusionType itemroleinclusiontype,
               FlagInclusionType expenseiteminclusiontype)
            : base(
                flagid,
                flagtype,
                action,
                flagtext,
                associateditemroles,
                associatedexpenseitems,
                createdon,
                createdby,
                modifiedon,
                modifiedby,
                description,
                active,
                accountid,
                claimantjustificationrequired,
                displayflagimmediately,
                flaglevel,
            approverjustificationrequired,
            notesforauthoriser,
            "Only allow journeys which start and end at home or office.",
            "Our policy does not allow journeys to start and finish at a location other than your {0} or {1}.",
            itemroleinclusiontype,
            expenseiteminclusiontype,
            true,
            false,
            false,
            false,
            false)
        {
        }

        public override List<FlaggedItem> Validate(cExpenseItem expenseItem, int employeeId, cAccountProperties properties,IDBConnection connection = null)
        {
            var flagResult = new FlaggedItem(string.Format(this.FlagDescription, properties.HomeAddressKeyword, properties.WorkAddressKeyword), this.CustomFlagText, this, this.FlagLevel, this.FlagTypeDescription, this.NotesForAuthoriser, this.AssociatedExpenseItems, this.Action, this.CustomFlagText, this.ClaimantJustificationRequired, false);
            return new List<FlaggedItem>() { flagResult };
        }


    }
}
