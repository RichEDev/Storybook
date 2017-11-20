ALTER TABLE [dbo].[car_mileagecats]
    ADD CONSTRAINT [FK_car_mileagecats_cars] FOREIGN KEY ([carid]) REFERENCES [dbo].[cars] ([carid]) ON DELETE CASCADE ON UPDATE NO ACTION;

