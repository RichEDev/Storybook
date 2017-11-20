ALTER TABLE [dbo].[audiences]
    ADD CONSTRAINT [DF_audiences_createdOn] DEFAULT (getdate()) FOR [createdOn];

