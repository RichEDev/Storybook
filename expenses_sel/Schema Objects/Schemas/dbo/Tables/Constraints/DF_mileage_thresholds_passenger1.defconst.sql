ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_passenger1] DEFAULT ((0)) FOR [passenger1];

