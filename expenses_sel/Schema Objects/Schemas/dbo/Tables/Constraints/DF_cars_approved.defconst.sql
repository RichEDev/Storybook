ALTER TABLE [dbo].[cars]
    ADD CONSTRAINT [DF_cars_approved] DEFAULT ((1)) FOR [approved];

