ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [DF_customEntityAttributes_mandatory] DEFAULT 0 FOR [mandatory];

