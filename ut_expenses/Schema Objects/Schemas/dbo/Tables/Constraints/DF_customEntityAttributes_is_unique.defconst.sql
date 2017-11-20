ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [DF_customEntityAttributes_is_unique] DEFAULT 0 FOR [is_unique];

