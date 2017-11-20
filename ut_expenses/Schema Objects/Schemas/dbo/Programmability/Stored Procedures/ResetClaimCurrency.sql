CREATE PROCEDURE ResetClaimCurrency 
	@employeeid int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    update claims_base set currencyid = (select primarycurrency from employees where employeeid = @employeeid) where employeeid = @employeeid and submitted = 0 and (select count(expenseid) from savedexpenses where claimid = claims_base.claimid) = 0
END
GO
