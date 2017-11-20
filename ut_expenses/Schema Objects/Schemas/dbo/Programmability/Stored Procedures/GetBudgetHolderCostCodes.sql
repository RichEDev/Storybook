CREATE PROCEDURE [dbo].[GetBudgetHolderCostCodes](@employeeId INT)
AS
     --Get claims where the employee is a budgetholder of a cost code 
    Declare @budgetHolderId int
    
    CREATE TABLE #costcodeIds
      (
         [costcodeid] [INT],
         PRIMARY KEY CLUSTERED ([costcodeid] ASC) WITH (ignore_dup_key = on)
      );
      
  
    DECLARE budgetHolderCcCursor CURSOR FOR
      SELECT budgetholderId
      FROM   budgetholders
      WHERE  employeeid = @employeeid

    OPEN budgetHolderCcCursor

    FETCH next FROM budgetHolderCcCursor INTO @budgetHolderId

    WHILE @@FETCH_STATUS = 0
      BEGIN
          INSERT INTO #costcodeIds
          SELECT costcodeid
          FROM   costcodes
          WHERE  OwnerBudgetHolderId = @budgetHolderId

          FETCH next FROM budgetHolderCcCursor INTO @budgetHolderId
      END

    CLOSE budgetHolderCcCursor

    DEALLOCATE budgetHolderCcCursor;
    
      
  Select costcodeid from  #costcodeIds
  
  drop table #costcodeIds
