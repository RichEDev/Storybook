


CREATE FUNCTION [dbo].[getSumOfAssignmentFailureRequests]
(
)
RETURNS INT
AS
BEGIN
	DECLARE @sumOfAssignmentFailureRequests int;
	
	IF OBJECT_ID('dbo.autoActionLogTemp','U') IS NOT NULL
	BEGIN
		SET @sumOfAssignmentFailureRequests = (SELECT SUM(assignmentFailCount) FROM autoActionLogTemp);
	END
	else 
	BEGIN
		SET @sumOfAssignmentFailureRequests = 0;
	END

	RETURN @sumOfAssignmentFailureRequests;

END


