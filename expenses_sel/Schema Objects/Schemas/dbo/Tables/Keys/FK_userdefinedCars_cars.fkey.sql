ALTER TABLE [dbo].[userdefinedCars]
    ADD CONSTRAINT [FK_userdefinedCars_cars] FOREIGN KEY ([carid]) REFERENCES [dbo].[cars] ([carid]) ON DELETE CASCADE ON UPDATE NO ACTION;

