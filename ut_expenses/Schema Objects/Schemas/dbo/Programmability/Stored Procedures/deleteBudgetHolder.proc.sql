CREATE PROCEDURE [dbo].[deleteBudgetHolder]
      @budgetholderid int,
      @employeeID int,
      @delegateID int
AS
BEGIN
      -- SET NOCOUNT ON added to prevent extra result sets from
      -- interfering with SELECT statements.
      SET NOCOUNT ON;

      declare @count int
    -- Insert statements for procedure here
      set @count = (select count(*) from signoffs where signofftype = 1 and relid = @budgetholderid)
      if @count > 0
            return -1
            
      set @count = (select count(*) from claims_base  inner join employees on employees.employeeid = claims_base.employeeid inner join groups on groups.groupid = employees.groupid inner join signoffs on signoffs.groupid = groups.groupid where claims_base.submitted = 1 and claims_base.paid = 0 and signoffs.signofftype = 1 and relid = @budgetholderid)
      if @count > 0
            return -2
            
      set @count = (select count(*) from audienceBudgetHolders where budgetholderid = @budgetholderid)
      if @count > 0
            return -3

      set @count = (select count(costcodeid) from costcodes where OwnerBudgetHolderId = @budgetholderid)
      if @count > 0
            return -4;
      
      declare @bhTableId uniqueidentifier = (select tableid from tables where tablename = 'budgetholders');
      exec @count = dbo.checkReferencedBy @bhTableId, @budgetholderid;
      if @count <> 0
            return @count;

      IF EXISTS (SELECT approvalMatrixId FROM approvalMatrices WHERE defaultApproverBudgetHolderId = @budgetholderid) OR EXISTS (SELECT approvalMatrixLevelId FROM approvalMatrixLevels WHERE approverBudgetHolderId = @budgetholderid)
			RETURN -5;

      DECLARE @budgetholder NVARCHAR(50);
      SELECT @budgetholder = budgetholder FROM budgetholders WHERE budgetholderid = @budgetholderid
      delete from budgetholders where budgetholderid = @budgetholderid
      
      EXEC addDeleteEntryToAuditLog @employeeID, @delegateID, 11, @budgetholderid, @budgetholder, null
      return 0
      
END