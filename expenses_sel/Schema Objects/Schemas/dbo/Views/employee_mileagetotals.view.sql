
CREATE VIEW [dbo].[employee_mileagetotals]
AS

select claims_base.employeeid, dbo.getFinancialYear(date) as financial_year, dbo.getMileageTotalByEmployeeid(claims_base.employeeid, dbo.getFinancialYear(date)) as mileagetotal
	from savedexpenses
		inner join claims_base on claims_base.claimid = savedexpenses.claimid
		
	group by claims_base.employeeid, dbo.getFinancialYear(date)


