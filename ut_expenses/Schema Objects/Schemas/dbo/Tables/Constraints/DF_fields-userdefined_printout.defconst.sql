ALTER TABLE [dbo].[fields_userdefined]
    ADD CONSTRAINT [DF_fields-userdefined_printout] DEFAULT (0) FOR [printout];

