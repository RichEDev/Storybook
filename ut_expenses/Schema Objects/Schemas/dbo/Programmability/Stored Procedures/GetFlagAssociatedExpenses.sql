CREATE PROCEDURE [dbo].[GetFlagAssociatedExpenses] 
 @associatedExpenses IntPk readonly
AS
BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.
 SET NOCOUNT ON;

    -- Insert statements for procedure here
 select savedexpenses.expenseid, claims.name, savedexpenses.date, savedexpenses.refnum, subcats.subcat, savedexpenses.total, global_currencies.currencysymbol from savedexpenses inner join claims on claims.claimid = savedexpenses.claimid inner join subcats on subcats.subcatid = savedexpenses.subcatid inner join currencies on currencies.currencyid = savedexpenses.basecurrency inner join global_currencies on global_currencies.globalcurrencyid = currencies.globalcurrencyid where expenseid in (select c1 from @associatedExpenses)
END
GO


