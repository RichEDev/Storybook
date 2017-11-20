ALTER TABLE [dbo].[tables_base]
    ADD CONSTRAINT [DF_tables_allowimport] DEFAULT ((0)) FOR [allowimport];

