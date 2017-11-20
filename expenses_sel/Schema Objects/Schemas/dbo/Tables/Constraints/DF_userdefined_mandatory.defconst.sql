ALTER TABLE [dbo].[userdefined]
    ADD CONSTRAINT [DF_userdefined_mandatory] DEFAULT (0) FOR [mandatory];

