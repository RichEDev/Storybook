ALTER TABLE [dbo].[fields_userdefined]
    ADD CONSTRAINT [DF_fields-userdefined_genlist] DEFAULT (0) FOR [genlist];

