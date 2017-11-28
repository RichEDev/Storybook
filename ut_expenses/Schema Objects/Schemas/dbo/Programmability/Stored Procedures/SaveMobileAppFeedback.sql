CREATE PROCEDURE [dbo].[SaveMobileAppFeedback] 
@FeedbackCategoryId int,
@Feedback nvarchar(500), 
@Email nvarchar(100),
@MobileMetricId int,
@AppVersion nvarchar(100)
AS
BEGIN

INSERT INTO MobileAppFeedback  Values (@FeedbackCategoryId, @Feedback, @Email, @MobileMetricId, @AppVersion, GETDATE())
	
RETURN Scope_identity()

END