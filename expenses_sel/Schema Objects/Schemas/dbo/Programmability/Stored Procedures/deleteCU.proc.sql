CREATE PROCEDURE deleteCU
@manageID uniqueidentifier
AS
BEGIN
	DELETE FROM accessManagement WHERE manageID = @manageID;
END
