CREATE FUNCTION [dbo].[getMileageTotalByEmployeeid] 
(
 @employeeid int, @financialyear DateTime
)
RETURNS decimal
AS
BEGIN
 declare @mileage decimal;
 declare @startmileagefinyear DateTime;
 
  set @startmileagefinyear = (select dbo.getFinancialYear(mileagetotaldate, @employeeid) from employees where employeeid = @employeeid);
 
 if @startmileagefinyear is null
  begin
  -- set financial year to be the financial year of the first expense item claimed
  set @startmileagefinyear = (select top 1 dbo.getFinancialYear(savedexpenses.date, @employeeid) from savedexpenses 
  inner join claims_base on savedexpenses.claimid = claims_base.claimid
  where claims_base.employeeid = @employeeid
  order by date asc);
  
  if @startmileagefinyear is null
  begin
   set @startmileagefinyear = dbo.getFinancialYear(getdate(), @employeeid);
  end
 end

 declare @startdate datetime
 declare @enddate datetime

 
 set @startdate = (@financialyear)
 set @enddate = DATEADD(yy, 1, @financialyear)
 set @enddate = DATEADD(d, -1, @enddate)
 set @mileage = (SELECT ISNULL(SUM(dbo.savedexpenses_journey_steps.num_miles),0) AS mileagetotal
     FROM   dbo.savedexpenses_journey_steps
    inner join savedexpenses on savedexpenses.expenseid = savedexpenses_journey_steps.expenseid INNER JOIN
     dbo.claims_base ON claims_base.claimid = dbo.savedexpenses.claimid
    inner join subcats on subcats.subcatid = savedexpenses.subcatid
    inner join cars on cars.carid = savedexpenses.carid 
    inner join mileage_categories on savedexpenses.mileageid  = mileage_categories.mileageid
   WHERE mileage_categories.calcmilestotal = 1 and subcats.calculation = 3 and claims_base.employeeid = @employeeid 
    AND (dbo.savedexpenses.date BETWEEN @startdate AND @enddate))

 if @startmileagefinyear = @financialyear
 begin
  set @mileage = @mileage + (select mileagetotal from employees where employeeid = @employeeid);
 end
 
 return @mileage
END