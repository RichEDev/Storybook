ALTER TABLE [dbo].[APICredentials]
    ADD CONSTRAINT [DF_APICredentials_Active] DEFAULT ((0)) FOR [Active];

