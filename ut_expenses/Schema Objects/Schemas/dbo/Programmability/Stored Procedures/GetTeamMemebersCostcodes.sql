CREATE PROCEDURE [dbo].[GetTeamMemebersCostcodes](@employeeId INT)
AS
    DECLARE @teamid INT

    CREATE TABLE #costcodeids
      (
         [costcodeid] [INT],
         PRIMARY KEY CLUSTERED ([costcodeid] ASC) WITH (ignore_dup_key = on)
      );

    DECLARE teamsCcCursor CURSOR FOR
      SELECT teamid
      FROM   teamemps
      WHERE  employeeid = @employeeId

    OPEN teamsCcCursor

    FETCH next FROM teamsCcCursor INTO @teamid

    WHILE @@FETCH_STATUS = 0
      BEGIN
          INSERT INTO #costcodeids
          SELECT costcodeid
          FROM   costcodes
          WHERE  ownerteamid = @teamid

          FETCH next FROM teamsCcCursor INTO @teamid
      END

    CLOSE teamsCcCursor

    DEALLOCATE teamsCcCursor;

    SELECT costcodeid
    FROM   #costcodeids

    DROP TABLE #costcodeids