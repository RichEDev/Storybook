ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [DF_cars_fuelcard] DEFAULT (0) FOR [fuelcard];

