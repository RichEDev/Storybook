ALTER TABLE [dbo].[fields_userdefined]
    ADD CONSTRAINT [DF_fields-userdefined_cantotal] DEFAULT (0) FOR [cantotal];

