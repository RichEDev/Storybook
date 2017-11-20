ALTER TABLE [dbo].[mileage_dateranges]
    ADD CONSTRAINT [FK_mileage_dateranges_mileage_categories] FOREIGN KEY ([mileageid]) REFERENCES [dbo].[mileage_categories] ([mileageid]) ON DELETE CASCADE ON UPDATE NO ACTION;

