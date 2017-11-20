ALTER TABLE [dbo].[pool_car_users]
    ADD CONSTRAINT [FK_pool_car_users_employees] FOREIGN KEY ([employeeid]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE CASCADE ON UPDATE NO ACTION;

