ALTER TABLE [dbo].[fields_base]
    ADD CONSTRAINT [DF_fields_base_fieldid_new_1] DEFAULT (newid()) FOR [fieldid];

