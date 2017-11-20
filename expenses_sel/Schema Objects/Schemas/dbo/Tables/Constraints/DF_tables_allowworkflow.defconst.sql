ALTER TABLE [dbo].[custom_tables]
    ADD CONSTRAINT [DF_tables_allowworkflow] DEFAULT ((0)) FOR [allowworkflow];

