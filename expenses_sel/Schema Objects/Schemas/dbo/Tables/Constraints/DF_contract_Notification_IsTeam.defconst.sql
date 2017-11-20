ALTER TABLE [dbo].[contract_notification]
    ADD CONSTRAINT [DF_contract_Notification_IsTeam] DEFAULT ((0)) FOR [IsTeam];

