CREATE PROCEDURE [dbo].[GetExtendedClaimHierarchy] @employeeid   INT,
                                                  @matchFieldID UNIQUEIDENTIFIER
AS
    DECLARE @teamid INT
    DECLARE @budgetHolderId INT

    CREATE TABLE #hierarchy
      (
         [employeeid] [INT],
         [checked]    BIT DEFAULT (0),
         PRIMARY KEY CLUSTERED ([employeeid] ASC) WITH (ignore_dup_key = on)
      );

    --Get cost code ids that belong to the employee
    DECLARE @costcodeId INT
    DECLARE acostcodeowner CURSOR FOR
      SELECT costcodeid
      FROM   costcodes
      WHERE  owneremployeeid = @employeeid

    OPEN acostcodeowner

    FETCH next FROM acostcodeowner INTO @costcodeId

    --Get EmployeeIds who have used the Costcode on a claim.
    WHILE @@FETCH_STATUS = 0
      BEGIN
          INSERT INTO #hierarchy
          SELECT DISTINCT dbo.claims_base.employeeid,
                          0
          FROM   dbo.claims_base
                 INNER JOIN dbo.savedexpenses
                         ON dbo.claims_base.claimid = dbo.savedexpenses.claimid
                 INNER JOIN dbo.savedexpenses_costcodes
                         ON dbo.savedexpenses.expenseid =
                            dbo.savedexpenses_costcodes.expenseid
                            AND costcodeid = @costcodeid

          FETCH next FROM acostcodeowner INTO @costcodeId
      END

    CLOSE acostcodeowner

    CREATE TABLE #costcodeids
      (
         [costcodeid] [INT],
         PRIMARY KEY CLUSTERED ([costcodeid] ASC) WITH (ignore_dup_key = on)
      );

    --Get costs codes that belong to a team
    DECLARE teamscccursor CURSOR FOR
      SELECT teamid
      FROM   teamemps
      WHERE  employeeid = @employeeid

    OPEN teamscccursor

    FETCH next FROM teamscccursor INTO @teamid

    WHILE @@FETCH_STATUS = 0
      BEGIN
          INSERT INTO #costcodeids
          SELECT costcodeid
          FROM   costcodes
          WHERE  ownerteamid = @teamid

          FETCH next FROM teamscccursor INTO @teamid
      END

    CLOSE teamscccursor

    DEALLOCATE teamscccursor;

    --get budget holder cost codes
    DECLARE budgetholdercccursor CURSOR FOR
      SELECT budgetholderid
      FROM   budgetholders
      WHERE  employeeid = @employeeid

    OPEN budgetholdercccursor

    FETCH next FROM budgetholdercccursor INTO @budgetHolderId

    WHILE @@FETCH_STATUS = 0
      BEGIN
          INSERT INTO #costcodeids
          SELECT costcodeid
          FROM   costcodes
          WHERE  ownerbudgetholderid = @budgetHolderId

          FETCH next FROM budgetholdercccursor INTO @budgetHolderId
      END

    CLOSE budgetholdercccursor

    DEALLOCATE budgetholdercccursor;

    --Get EmployeeIds who have used the Costcodes belonging to a team and/or budget holder.
    DECLARE @TeamBudgetCCIds INT
    DECLARE acostcodeowners CURSOR FOR
      SELECT costcodeid
      FROM   #costcodeids

    OPEN acostcodeowners

    FETCH next FROM acostcodeowners INTO @TeamBudgetCCIds

    WHILE @@FETCH_STATUS = 0
      BEGIN
          INSERT INTO #hierarchy
          SELECT DISTINCT dbo.claims_base.employeeid,
                          0
          FROM   dbo.claims_base
                 INNER JOIN dbo.savedexpenses
                         ON dbo.claims_base.claimid = dbo.savedexpenses.claimid
                 INNER JOIN dbo.savedexpenses_costcodes
                         ON dbo.savedexpenses.expenseid =
                            dbo.savedexpenses_costcodes.expenseid
                            AND costcodeid = @TeamBudgetCCIds

          FETCH next FROM acostcodeowners INTO @TeamBudgetCCIds
      END

    CLOSE acostcodeowners

    DEALLOCATE acostcodeowners;

    DROP TABLE #costcodeids

    SELECT employeeid,
           checked
    FROM   #hierarchy;

    RETURN 0  