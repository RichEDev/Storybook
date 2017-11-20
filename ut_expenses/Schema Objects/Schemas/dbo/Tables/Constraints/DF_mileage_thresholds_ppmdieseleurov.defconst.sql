ALTER TABLE [dbo].[mileage_thresholds]
	ADD CONSTRAINT [DF_mileage_thresholds_ppmdieseleurov]
	DEFAULT 0
	FOR [ppmdieseleurov]
