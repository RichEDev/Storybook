ALTER TABLE [dbo].[tables_base]
    ADD CONSTRAINT [DF_tables_base_hasUserDefinedFields] DEFAULT ((0)) FOR [hasUserDefinedFields];

