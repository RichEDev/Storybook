ALTER TABLE [dbo].[mileage_thresholds]
	ADD CONSTRAINT [DF_mileage_thresholds_ppmhybrid] DEFAULT (0) FOR [ppmhybrid];
