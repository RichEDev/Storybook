ALTER TABLE [dbo].[accountProperties]
    ADD CONSTRAINT [DF_accountProperties_createdOn] DEFAULT (getutcdate()) FOR [createdOn];

