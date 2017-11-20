ALTER TABLE [dbo].[accessManagement]
    ADD CONSTRAINT [DF_accessmanagement_manageid] DEFAULT (newid()) FOR [manageID];

