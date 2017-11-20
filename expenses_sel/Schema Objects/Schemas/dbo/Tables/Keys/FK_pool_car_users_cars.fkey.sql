ALTER TABLE [dbo].[pool_car_users]
    ADD CONSTRAINT [FK_pool_car_users_cars] FOREIGN KEY ([carid]) REFERENCES [dbo].[cars] ([carid]) ON DELETE NO ACTION ON UPDATE NO ACTION;

