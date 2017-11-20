ALTER TABLE [dbo].[groups]
    ADD CONSTRAINT [DF_groups_oneClickAuthorisation] DEFAULT ((0)) FOR [oneClickAuthorisation];

