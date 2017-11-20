CREATE PROCEDURE [dbo].[getEmployeeMileageTotal]
	@employeeid int,
	@expenseDate datetime
AS

declare @mileageTotal decimal;

BEGIN
	set @mileageTotal = (select dbo.getMileageTotalByEmployeeid(@employeeid, dbo.getFinancialYear(@expenseDate)))
	return @mileageTotal
END

