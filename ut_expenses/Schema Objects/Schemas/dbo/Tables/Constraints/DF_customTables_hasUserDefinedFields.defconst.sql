ALTER TABLE [dbo].[customTables]
    ADD CONSTRAINT [DF_customTables_hasUserDefinedFields] DEFAULT ((0)) FOR [hasUserDefinedFields];

