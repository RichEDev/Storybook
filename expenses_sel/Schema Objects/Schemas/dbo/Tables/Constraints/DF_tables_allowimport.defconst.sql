ALTER TABLE [dbo].[custom_tables]
    ADD CONSTRAINT [DF_tables_allowimport] DEFAULT ((0)) FOR [allowimport];

