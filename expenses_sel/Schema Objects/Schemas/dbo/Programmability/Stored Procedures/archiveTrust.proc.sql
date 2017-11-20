



CREATE PROCEDURE [dbo].[archiveTrust]
	@trustID int,
	@employeeID INT,
	@delegateID INT
AS 
	DECLARE @count INT;
	DECLARE @archived BIT;

	declare @recordTitle nvarchar(2000);
	select @recordTitle = trustname from esrtrusts where trustID=@trustID;

	SET @archived = (SELECT archived FROM esrTrusts WHERE trustID=@trustID);
		
	IF @archived = 0
		 BEGIN
			UPDATE esrTrusts SET archived=1 WHERE trustID=@trustID;
			RETURN 1;
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '30e2230c-9af4-4f2b-ab56-58f71061c5fe', 0, 1, @recordtitle, null;
		END
	ELSE
		BEGIN
			UPDATE esrTrusts SET archived=0 WHERE trustID=@trustID;
			RETURN 1;
			exec addUpdateEntryToAuditLog @employeeID, @delegateID, 27, @trustID, '30e2230c-9af4-4f2b-ab56-58f71061c5fe', 1, 0, @recordtitle, null;
		END
