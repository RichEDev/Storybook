ALTER TABLE [dbo].[car_mileagecats]
    ADD CONSTRAINT [FK_car_mileagecats_mileage_categories] FOREIGN KEY ([mileageid]) REFERENCES [dbo].[mileage_categories] ([mileageid]) ON DELETE CASCADE ON UPDATE NO ACTION;

