CREATE PROCEDURE [dbo].[CheckIfUserIsAssignmentSupervisor] @claimId    INT,
                                                           @employeeid INT
AS
    DECLARE @hasItems BIT
    DECLARE @count INT

    SELECT @count = claimid
    FROM   savedexpenses
    WHERE  itemcheckerid = @employeeid
           AND claimid = @claimId
		   AND esrAssignID IS NOT NULL

    IF @count > 0
      SET @hasItems = 1
    ELSE
      SET @hasItems = 0

    RETURN @hasItems

