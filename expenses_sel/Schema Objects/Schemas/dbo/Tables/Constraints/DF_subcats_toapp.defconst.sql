ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_toapp] DEFAULT ((0)) FOR [toapp];

