ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [DF_customEntityAttributes_is_key_field] DEFAULT 0 FOR [is_key_field];

