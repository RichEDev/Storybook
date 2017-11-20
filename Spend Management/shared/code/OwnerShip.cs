namespace Spend_Management
{
    public class Ownership
    {
        /// <summary>
        /// Parses a string in the format "{SpendManagementElement},{PrimaryKey}" into it's corresponding object (i.e. Employee, Team, Budget Holder)
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="subAccountId"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static SpendManagementLibrary.Interfaces.IOwnership Parse(int accountId, int? subAccountId, string owner)
        {
            if (owner == null || !owner.Contains(","))
            {
                return null;
            }

            var ownerParts = owner.Split(',');
            int elementType, primaryKey;

            if (int.TryParse(ownerParts[0], out elementType) && System.Enum.IsDefined(typeof(SpendManagementElement), elementType) && int.TryParse(ownerParts[1], out primaryKey))
            {
                switch ((SpendManagementElement)elementType)
                {
                    case SpendManagementElement.BudgetHolders:
                        return new cBudgetholders(accountId).getBudgetHolderById(primaryKey);

                    case SpendManagementElement.Employees:
                        return new cEmployees(accountId).GetEmployeeById(primaryKey);

                    case SpendManagementElement.Teams:
                        return new cTeams(accountId, subAccountId).GetTeamById(primaryKey);
                }
            }

            return null;

        }
    }
}
