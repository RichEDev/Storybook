ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_amountforvatd] DEFAULT ((0)) FOR [amountforvatd];

