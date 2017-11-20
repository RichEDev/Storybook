




CREATE PROCEDURE [dbo].[deleteAudienceTeam] (@audienceTeamID INT,
@CUemployeeID INT,
@CUdelegateID INT)
AS
BEGIN
	declare @title1 nvarchar(500);
	select @title1 = audienceName from audiences where audienceID = (select audienceID from audienceTeams where audienceTeamID = @audienceTeamID);

	declare @recordTitle nvarchar(2000);
	set @recordTitle = (select 'Audience Team for ' + @title1);

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
	DELETE FROM audienceTeams WHERE audienceTeamID=@audienceTeamID;

	exec addDeleteEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceTeamID, @recordTitle, null;
END
