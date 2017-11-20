ALTER TABLE [dbo].[tables_base]
    ADD CONSTRAINT [DF_tables_amendedon] DEFAULT (getdate()) FOR [amendedon];

