ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [FK_subcats_mileage_categories] FOREIGN KEY ([mileageCategory]) REFERENCES [dbo].[mileage_categories] ([mileageid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

