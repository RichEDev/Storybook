CREATE PROCEDURE [dbo].[GetSavedExpenseFlags] 
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

select savedexpensesFlags.flaggedItemId, savedexpensesFlags.expenseid, savedexpensesflags.flagid, savedexpensesFlags.flagType, savedexpensesFlags.flagDescription, savedexpensesFlags.flagText, savedexpensesFlags.duplicateExpenseID, savedexpensesFlags.flagColour, savedexpensesFlags.claimantJustification, savedexpensesFlags.exceededLimit, savedexpensesFlags.stepNumber, savedexpenses.date, savedexpenses.total, subcats.subcat, global_currencies.currencySymbol, savedexpensesFlags.claimantJustificationDelegateID, employees.firstname + ' ' + employees.surname as delegateFullName, savedexpenses.claimid, claims.submitted
 from [savedexpensesFlags] 
 inner join savedexpenses on savedexpenses.expenseid = savedexpensesFlags.expenseid
 inner join claims on claims.claimid = savedexpenses.claimid 
 inner join currencies on currencies.currencyid = savedexpenses.basecurrency
 inner join global_currencies on global_currencies.globalcurrencyid = currencies.globalcurrencyid
 inner join subcats on subcats.subcatid = savedexpenses.subcatid 
 inner join @expenseids ids on ids.c1 = savedexpenses.expenseid
 left join employees on employees.employeeid = savedexpensesFlags.claimantJustificationDelegateID
 order by savedexpensesFlags.expenseid, stepnumber
END

GO