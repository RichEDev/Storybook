ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_vattype] DEFAULT ((0)) FOR [vattype];

