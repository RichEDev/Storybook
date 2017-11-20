CREATE  PROCEDURE [dbo].[saveAudienceTeam]
@audienceID int,
@teamID int,
@CUemployeeID INT,
@CUdelegateID INT
AS
	BEGIN
		declare @title1 nvarchar(500);
		select @title1 = audienceName from audiences where audienceID = @audienceID;

		DECLARE @audienceTeamID int = -1;	
		
		declare @recordTitle nvarchar(2000);
		set @recordTitle = (select 'Audience Team for ' + @title1);
		
		IF NOT EXISTS (SELECT teamID FROM audienceTeams WHERE audienceID = @audienceID AND teamID = @teamID)
		BEGIN
			INSERT INTO audienceTeams (audienceID, teamID) VALUES (@audienceID, @teamID);		
			SET @audienceTeamID = scope_identity();
			
			UPDATE audiences SET modifiedOn = getdate() WHERE audienceID = @audienceID;

			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 101, @audienceTeamID, @recordTitle, null;
		END
		ELSE
		BEGIN
			SELECT @audienceTeamID = audienceTeamID FROM audienceTeams WHERE audienceID = @audienceID AND teamID = @teamID
		END
	END
RETURN @audienceTeamID;
