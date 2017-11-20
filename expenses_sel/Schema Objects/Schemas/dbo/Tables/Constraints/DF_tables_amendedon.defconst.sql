ALTER TABLE [dbo].[custom_tables]
    ADD CONSTRAINT [DF_tables_amendedon] DEFAULT (getdate()) FOR [amendedon];

