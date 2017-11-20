ALTER TABLE [dbo].[mileage_thresholds]
	ADD CONSTRAINT [DF_mileage_thresholds_amountforvatelectric]
	DEFAULT 0
	FOR [amountforvatelectric]
