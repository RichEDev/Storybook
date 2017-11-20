ALTER TABLE [dbo].[views]
    ADD CONSTRAINT [DF_views_createdon] DEFAULT (getdate()) FOR [createdon];

