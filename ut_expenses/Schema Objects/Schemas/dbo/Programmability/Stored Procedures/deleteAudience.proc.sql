CREATE PROCEDURE [dbo].[deleteAudience] (@audienceID INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	declare @recordTitle nvarchar(2000);
	declare @audienceUsed int;
	select @recordTitle = audienceName from audiences where audienceID = @audienceID;



	SET NOCOUNT ON;



	EXEC @audienceUsed = dbo.CheckIfAudienceIdIsUsed @audienceId = @audienceID;
	IF @audienceUsed = -1
		BEGIN
			DELETE FROM audiences WHERE audienceID=@audienceID;
			EXEC addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceID, @recordTitle, null;
			RETURN @audienceID;
		END
	ELSE
		RETURN -1
	

	
END
