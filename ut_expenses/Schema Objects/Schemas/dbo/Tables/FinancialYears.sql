CREATE TABLE [dbo].[FinancialYears]
(
	[FinancialYearID] INT NOT NULL Identity, 
    [Description] NCHAR(50) NOT NULL, 
    [YearStart] DATE NOT NULL, 
    [YearEnd] DATE NOT NULL, 
    [Active] BIT NOT NULL, 
    [Primary] BIT NOT NULL
)
