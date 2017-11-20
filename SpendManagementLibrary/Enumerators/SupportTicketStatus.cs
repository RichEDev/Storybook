namespace SpendManagementLibrary.Enumerators
{

    /// <summary>
    /// Represents the various statuses of a support ticket
    /// Value names are self-explanatory
    /// </summary>
    public enum SupportTicketStatus
    {
        New = 1,

        InProgress = 2,

        PendingAdministratorResponse = 3,

        PendingEmployeeResponse = 4,

        OnHold = 5,

        Closed = 6
    }
}

