


CREATE FUNCTION [dbo].[getSumOfAssignmentRequests]
(
)
RETURNS INT
AS
BEGIN
	DECLARE @sumOfAssignmentRequests int;
	
	IF OBJECT_ID('dbo.autoActionLogTemp','U') IS NOT NULL
	BEGIN
		SET @sumOfAssignmentRequests = (SELECT SUM(assignmentCount) FROM autoActionLogTemp);
	END
	else
	BEGIN
		SET @sumOfAssignmentRequests = 0;
	END

	RETURN @sumOfAssignmentRequests;

END


