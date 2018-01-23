CREATE PROCEDURE [dbo].[getEmployeeMileageTotal]
	@employeeid int,
	@expenseDate datetime,
	@financialYearId INT
AS

declare @mileageTotal decimal;

BEGIN
	DECLARE @financialYear DateTime 
	IF @financialYearId = 0
	BEGIN
	SET @financialYear = (SELECT TOP 1 YearStart FROM FinancialYears WHERE [Primary] = 1)
	END
	ELSE
	BEGIN
	SET @financialYear = (SELECT TOP 1 YearStart FROM FinancialYears WHERE FinancialYearID = @financialYearId)
	END
	set @mileageTotal = (SELECT mileagetotal FROM employee_mileagetotals WHERE employeeid = @employeeid  AND financial_year =  DATEPART(yy,dbo.getFinancialYear(@expenseDate, @financialYear)))
	return @mileageTotal
END

