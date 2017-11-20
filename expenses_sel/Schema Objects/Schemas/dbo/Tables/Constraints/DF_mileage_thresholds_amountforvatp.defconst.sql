ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_amountforvatp] DEFAULT ((0)) FOR [amountforvatp];

