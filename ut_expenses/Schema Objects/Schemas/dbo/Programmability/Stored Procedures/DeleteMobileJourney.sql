CREATE PROCEDURE [dbo].[DeleteMobileJourney]
	@MobileJourneyID int
AS
	DELETE FROM [dbo].[MobileJourneys]
	WHERE [JourneyID] = @MobileJourneyID	

RETURN @@ROWCOUNT

GO