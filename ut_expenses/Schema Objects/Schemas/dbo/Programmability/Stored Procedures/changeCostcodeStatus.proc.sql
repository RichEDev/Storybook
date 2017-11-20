
CREATE PROCEDURE [dbo].[changeCostcodeStatus]
@costcodeid INT,
@archive bit,
@employeeID INT,
@delegateID INT

AS
DECLARE @count INT;

IF @archive = 1
begin
	SET @count = (select count(*) from signoffs where [include] = 4 and includeid = @costcodeid)
	IF @count > 0
		RETURN 1;
END

declare @oldarchive bit;
declare @recordTitle nvarchar(2000);
select @oldarchive = archived, @recordTitle = costcode from costcodes where costcodeid = @costcodeid;

update costcodes set archived = @archive where costcodeid = @costcodeid;

if @oldarchive <> @archive
	exec addUpdateEntryToAuditLog @employeeID, @delegateID, 1, @costcodeid, '8178629c-5908-4458-89f6-d7ee7438314d', @oldarchive, @archive, @recordtitle, null;

RETURN 0;
