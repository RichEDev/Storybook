ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_splitpersonal] DEFAULT (0) FOR [splitpersonal];

