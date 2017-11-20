ALTER TABLE [dbo].[custom_tables]
    ADD CONSTRAINT [DF_custom_tables_hasUserDefinedFields] DEFAULT ((0)) FOR [hasUserDefinedFields];

