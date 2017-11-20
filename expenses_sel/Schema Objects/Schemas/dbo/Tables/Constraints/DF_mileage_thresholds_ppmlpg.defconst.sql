ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_ppmlpg] DEFAULT ((0)) FOR [ppmlpg];

