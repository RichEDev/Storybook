ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [DF_cars_active] DEFAULT 0 FOR [active];

