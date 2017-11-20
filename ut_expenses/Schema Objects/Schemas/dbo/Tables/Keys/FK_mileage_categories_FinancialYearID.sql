ALTER TABLE [dbo].[mileage_categories]
	ADD CONSTRAINT [FK_mileage_categories_FinancialYearID]
	FOREIGN KEY (FinancialYearID)
	REFERENCES [FinancialYears] (FinancialYearID)
