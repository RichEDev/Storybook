ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_amountforvatlpg] DEFAULT ((0)) FOR [amountforvatlpg];

