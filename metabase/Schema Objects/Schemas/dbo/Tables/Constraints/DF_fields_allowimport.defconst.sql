ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [DF_fields_allowimport] DEFAULT ((0)) FOR [allowimport];

