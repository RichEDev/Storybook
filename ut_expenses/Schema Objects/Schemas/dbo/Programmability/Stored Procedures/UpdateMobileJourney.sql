CREATE PROCEDURE [dbo].[UpdateMobileJourney] 
	 @journeyId INT
	,@mileageJson NVARCHAR(max)
	,@active bit
AS
BEGIN
	IF @journeyId <> 0
		UPDATE MobileJourneys
		SET JourneyJSON = @mileageJson
		   ,Active = @active
		WHERE JourneyID = @journeyId;
END