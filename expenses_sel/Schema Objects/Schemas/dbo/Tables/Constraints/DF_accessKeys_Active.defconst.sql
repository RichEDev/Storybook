ALTER TABLE [dbo].[accessKeys]
    ADD CONSTRAINT [DF_accessKeys_Active] DEFAULT ((0)) FOR [Active];

