ALTER TABLE [dbo].[userdefined]
    ADD CONSTRAINT [DF_userdefined_relatedTable] DEFAULT (NULL) FOR [relatedTable];

