ALTER TABLE [dbo].[mileage_thresholds]
	ADD CONSTRAINT [DF_mileage_thresholds_ppmelectric]
	DEFAULT 0
	FOR [ppmelectric]
