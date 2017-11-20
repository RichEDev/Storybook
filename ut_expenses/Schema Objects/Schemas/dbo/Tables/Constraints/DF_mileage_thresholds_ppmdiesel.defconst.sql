ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_ppmdiesel] DEFAULT ((0)) FOR [ppmdiesel];

