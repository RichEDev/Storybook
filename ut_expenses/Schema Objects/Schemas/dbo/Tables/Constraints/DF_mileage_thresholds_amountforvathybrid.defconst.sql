ALTER TABLE [dbo].[mileage_thresholds]
	ADD CONSTRAINT [DF_mileage_thresholds_amountforvathybrid]
	DEFAULT 0
	FOR [amountforvathybrid]
