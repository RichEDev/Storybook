ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [DF_fields_amendedon] DEFAULT (getdate()) FOR [amendedon];

