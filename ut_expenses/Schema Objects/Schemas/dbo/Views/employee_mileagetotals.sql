CREATE VIEW [dbo].[employee_mileagetotals]
AS
select employeeid, financial_year, SUM(mileage) AS mileagetotal FROM 
(SELECT claims_base.employeeid
 ,DATEPART(yy, dbo.getFinancialYear(DATE, FinancialYears.YearStart)) AS financial_year
 ,ISNULL(dbo.savedexpenses_journey_steps.num_miles, 0) AS mileage
 FROM dbo.claims_base
LEFT JOIN dbo.savedexpenses ON claims_base.claimid = dbo.savedexpenses.claimid
left join dbo.savedexpenses_journey_steps on dbo.savedexpenses_journey_steps.expenseid = dbo.savedexpenses.expenseid
LEFT JOIN subcats ON subcats.subcatid = savedexpenses.subcatid
LEFT JOIN cars ON cars.carid = savedexpenses.carid
LEFT JOIN mileage_categories ON savedexpenses.mileageid = mileage_categories.mileageid
LEFT JOIN FinancialYears ON FinancialYears.FinancialYearID = mileage_categories.FinancialYearID
WHERE mileage_categories.calcmilestotal = 1
 AND subcats.calculation = 3
union all
select distinct  employees.employeeid,DATEPART(yy, dbo.getFinancialYear(ISNULL(mileagetotaldate, ISNULL((select min(datesubmitted) from claims_base where datesubmitted is not null and datesubmitted > '1980-01-01'), GETDATE())), fy.YearStart)) as financial_year , mileagetotal from employees 
inner join (select  MIN(cars.carid) as carid, employeeid, car_mileagecats.mileageid from cars inner join car_mileagecats on car_mileagecats.carid = cars.carid where cars.active = 1 group by employeeid, car_mileagecats.mileageid ) as carrs on carrs.employeeid = employees.employeeid
inner join mileage_categories as mc on mc.mileageid = carrs.mileageid
inner join FinancialYears as fy on fy.FinancialYearID = mc.FinancialYearID
where mileagetotal > 0) as mileView
GROUP BY employeeid, financial_year