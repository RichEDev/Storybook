ALTER TABLE [dbo].[userdefined]
    ADD CONSTRAINT [DF_userdefined_fieldid] DEFAULT (newid()) FOR [fieldid];

