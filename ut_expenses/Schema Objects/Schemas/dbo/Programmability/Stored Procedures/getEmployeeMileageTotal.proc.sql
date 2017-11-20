CREATE PROCEDURE [dbo].[getEmployeeMileageTotal]
	@employeeid int,
	@expenseDate datetime
AS

declare @mileageTotal decimal;

BEGIN
	DECLARE @financialYear DateTime = (SELECT TOP 1 YearStart FROM FinancialYears WHERE [Primary] = 1)
	set @mileageTotal = (SELECT mileagetotal FROM employee_mileagetotals WHERE employeeid = @employeeid  AND financial_year =  DATEPART(yy,dbo.getFinancialYear(@expenseDate, @financialYear)))
	return @mileageTotal
END

