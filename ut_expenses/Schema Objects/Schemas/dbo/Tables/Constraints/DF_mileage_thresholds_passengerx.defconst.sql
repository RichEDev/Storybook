ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_passengerx] DEFAULT ((0)) FOR [passengerx];

