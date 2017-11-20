CREATE TABLE [dbo].[contract_notification] (
    [notificationId] INT IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subAccountId]   INT NULL,
    [contractId]     INT NULL,
    [employeeId]     INT NULL,
    [IsTeam]         INT CONSTRAINT [DF_contract_Notification_IsTeam] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_Contract_Notification] PRIMARY KEY CLUSTERED ([notificationId] ASC),
    CONSTRAINT [FK_contract_notification_accountsSubAccounts] FOREIGN KEY ([subAccountId]) REFERENCES [dbo].[accountsSubAccounts] ([subAccountID]),
    CONSTRAINT [FK_contract_notification_contract_details] FOREIGN KEY ([contractId]) REFERENCES [dbo].[contract_details] ([contractId]) ON DELETE CASCADE
);



