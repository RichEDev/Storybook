ALTER TABLE [dbo].[mileage_thresholds]
    ADD CONSTRAINT [DF_mileage_thresholds_ppmpetrol] DEFAULT ((0)) FOR [ppmpetrol];

