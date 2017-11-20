
CREATE FUNCTION [dbo].[getMileageTotalByEmployeeid] 
(
	@employeeid int, @financialyear int
)
RETURNS decimal
AS
BEGIN
	declare @mileage decimal;
	declare @startmileagefinyear int;

	if exists (select mileagetotaldate from employees where employeeid = @employeeid and mileagetotaldate is not null)
	begin
		set @startmileagefinyear = (select dbo.getFinancialYear(mileagetotaldate) from employees where employeeid = @employeeid);
	end
	else
	begin
		declare @date datetime
		-- set financial year to be the financial year of the first expense item claimed
		select distinct top 1 @startmileagefinyear = dbo.getFinancialYear(savedexpenses.date), @date = date from savedexpenses 
		inner join claims on savedexpenses.claimid = claims.claimid
		where claims.employeeid = @employeeid
		order by date asc;
		
		if @startmileagefinyear is null
		begin
			set @startmileagefinyear = dbo.getFinancialYear(getdate());
		end
	end

	set @mileage = (SELECT ISNULL(SUM(dbo.savedexpenses_journey_steps.num_miles),0) AS mileagetotal
		   FROM   dbo.savedexpenses_journey_steps
				inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid INNER JOIN
					dbo.claims_base ON claims_base.claimid = dbo.savedexpenses.claimid
				inner join subcats on subcats.subcatid = savedexpenses.subcatid
				inner join cars on cars.carid = savedexpenses.carid 
				inner join mileage_categories on savedexpenses.mileageid  = mileage_categories.mileageid
			WHERE mileage_categories.calcmilestotal = 1 and subcats.calculation = 3 and claims_base.employeeid = @employeeid 
				AND (dbo.savedexpenses.date BETWEEN CAST(@financialyear as nvarchar(4)) 
						+ '/04/06' AND CAST((@financialyear + 1) AS NVARCHAR(4)) + '/04/05'))

	if @startmileagefinyear = @financialyear
	begin
		set @mileage = @mileage + (select mileagetotal from employees where employeeid = @employeeid);
	end
	
	return @mileage
END

