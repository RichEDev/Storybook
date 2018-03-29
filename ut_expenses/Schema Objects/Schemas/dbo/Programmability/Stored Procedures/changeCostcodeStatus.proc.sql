CREATE PROCEDURE [dbo].[changeCostcodeStatus] @costcodeid INT
	,@archive BIT
	,@employeeID INT
	,@delegateID INT
AS
DECLARE @count INT;

IF @archive = 1
BEGIN
	SET @count = (
			SELECT count(includeid)
			FROM signoffs
			WHERE [include] = 4
				AND includeid = @costcodeid
			)

	IF @count > 0
		RETURN - 1;

	SET @count = (
			SELECT count(costcodeid)
			FROM employee_costcodes
			WHERE costcodeid = @costcodeid
			)

	IF @count > 0
		RETURN - 2;
END

DECLARE @oldarchive BIT;
DECLARE @recordTitle NVARCHAR(2000);

SELECT @oldarchive = archived
	,@recordTitle = costcode
FROM costcodes
WHERE costcodeid = @costcodeid;

UPDATE costcodes
SET archived = @archive
WHERE costcodeid = @costcodeid;

IF @oldarchive <> @archive
	EXEC addUpdateEntryToAuditLog @employeeID
		,@delegateID
		,1
		,@costcodeid
		,'8178629c-5908-4458-89f6-d7ee7438314d'
		,@oldarchive
		,@archive
		,@recordtitle
		,NULL;

RETURN 0;