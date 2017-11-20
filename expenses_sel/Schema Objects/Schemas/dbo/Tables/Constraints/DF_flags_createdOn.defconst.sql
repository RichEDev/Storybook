ALTER TABLE [dbo].[flags]
    ADD CONSTRAINT [DF_flags_createdOn] DEFAULT (getdate()) FOR [createdOn];

