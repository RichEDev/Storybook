ALTER TABLE [dbo].[customEntityAttributes]
    ADD CONSTRAINT [DF_customEntityAttributes_fieldid_new_1] DEFAULT newid() FOR [fieldid];

