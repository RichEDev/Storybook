ALTER TABLE [dbo].[customTables]
    ADD CONSTRAINT [DF_customTables_allowimport] DEFAULT ((0)) FOR [allowimport];

