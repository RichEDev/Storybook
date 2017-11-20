
CREATE PROCEDURE dbo.SelectFinancialYear
AS
BEGIN
    select yearstart, yearend from FinancialYears where [Primary] = 1
END