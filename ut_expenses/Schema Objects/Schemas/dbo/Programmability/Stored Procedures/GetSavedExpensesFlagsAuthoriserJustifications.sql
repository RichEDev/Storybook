CREATE PROCEDURE [dbo].[GetSavedExpensesFlagsAuthoriserJustifications] 
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
    -- Insert statements for procedure here
    
	select savedexpensesFlagsApproverJustifications.flaggeditemid, stage, approverId, justification, datestamp, delegateID, case when delegateID IS null then employees.firstname + ' ' + employees.surname else delegates.firstname + ' ' + delegates.surname end as fullName from savedExpensesFlagsApproverJustifications 
		inner join savedexpensesFlags on savedexpensesFlags.flaggeditemid = savedExpensesFlagsApproverJustifications.flaggeditemid
		inner join @expenseids ids on ids.c1 = savedexpensesFlags.expenseid
		left join employees on employees.employeeid = savedExpensesFlagsApproverJustifications.approverId
		left join employees as delegates on delegates.employeeid = savedExpensesFlagsApproverJustifications.delegateID
		 order by expenseid, savedexpensesFlagsApproverJustifications.flaggeditemid, savedexpensesFlagsApproverJustifications.stage
END


GO