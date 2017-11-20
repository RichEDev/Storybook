ALTER TABLE [dbo].[customTables]
    ADD CONSTRAINT [DF_customTables_allowworkflow] DEFAULT ((0)) FOR [allowworkflow];

