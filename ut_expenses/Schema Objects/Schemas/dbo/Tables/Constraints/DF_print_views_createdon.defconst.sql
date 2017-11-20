ALTER TABLE [dbo].[print_views]
    ADD CONSTRAINT [DF_print_views_createdon] DEFAULT (getdate()) FOR [createdon];

