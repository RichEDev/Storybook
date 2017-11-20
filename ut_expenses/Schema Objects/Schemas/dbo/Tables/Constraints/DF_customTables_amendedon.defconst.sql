ALTER TABLE [dbo].[customTables]
    ADD CONSTRAINT [DF_customTables_amendedon] DEFAULT (getdate()) FOR [amendedon];

