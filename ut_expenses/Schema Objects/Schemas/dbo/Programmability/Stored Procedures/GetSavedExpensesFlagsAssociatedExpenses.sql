CREATE PROCEDURE [dbo].[GetSavedExpensesFlagsAssociatedExpenses] 
	@expenses IntPk readonly
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

declare @expenseids intPk
	declare @expenseid int
    declare employees_cursor cursor for select c1 from @expenses

	open employees_cursor

	fetch next from employees_cursor into @expenseid
		while @@fetch_status = 0
			BEGIN
			insert into @expenseids select distinct splititem from dbo.getExpenseItemIdsFromPrimary(@expenseid)
			fetch next from employees_cursor into @expenseid
		end
	close employees_cursor
deallocate employees_cursor

    select savedexpensesFlagAssociatedExpenses.flaggeditemid, savedexpenses.expenseid, claims.name, savedexpenses.date, savedexpenses.refnum, savedexpenses.total, subcats.subcat, global_currencies.currencysymbol, savedexpensesFlags.expenseid  from savedexpensesFlagAssociatedExpenses 
		inner join savedexpensesFlags on savedexpensesflags.flaggedItemId = savedexpensesFlagAssociatedExpenses.flaggedItemId
		inner join savedexpenses on savedexpenses.expenseid = savedexpensesFlagAssociatedExpenses.associatedExpenseId
		inner join claims on claims.claimid = savedexpenses.claimid
		inner join subcats on subcats.subcatid = savedexpenses.subcatid
		inner join currencies on currencies.currencyid = savedexpenses.basecurrency
		inner join global_currencies on global_currencies.globalcurrencyid = currencies.globalcurrencyid 
		inner join @expenseids ids on ids.c1 = savedexpensesFlags.expenseid
		union
	select savedexpensesFlags.flaggeditemid, savedexpenses.expenseid, claims.name, savedexpenses.date, savedexpenses.refnum, savedexpenses.total, subcats.subcat, global_currencies.currencysymbol, savedexpensesFlags.expenseid  from savedexpensesFlags 
		inner join savedexpenses on savedexpenses.expenseid = savedexpensesFlags.duplicateExpenseID
		inner join claims on claims.claimid = savedexpenses.claimid
		inner join subcats on subcats.subcatid = savedexpenses.subcatid
		inner join currencies on currencies.currencyid = savedexpenses.basecurrency 
		inner join global_currencies on global_currencies.globalcurrencyid = currencies.globalcurrencyid 
		inner join @expenseids ids on ids.c1 = savedexpensesFlags.expenseid
		where savedexpensesFlags.flagType = 1
		order by savedexpensesFlags.expenseid
END


GO