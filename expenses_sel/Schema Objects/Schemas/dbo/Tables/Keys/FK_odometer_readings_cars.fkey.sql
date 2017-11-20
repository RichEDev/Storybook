ALTER TABLE [dbo].[odometer_readings]
    ADD CONSTRAINT [FK_odometer_readings_cars] FOREIGN KEY ([carid]) REFERENCES [dbo].[cars] ([carid]) ON DELETE CASCADE ON UPDATE CASCADE;

