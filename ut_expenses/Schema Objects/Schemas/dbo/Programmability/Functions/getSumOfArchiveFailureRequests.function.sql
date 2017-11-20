


CREATE FUNCTION [dbo].[getSumOfArchiveFailureRequests]
(
)
RETURNS INT
AS
BEGIN
	DECLARE @sumOfArchiveFailureRequests int;
	
	IF OBJECT_ID('dbo.autoActionLogTemp','U') IS NOT NULL
	BEGIN
		SET @sumOfArchiveFailureRequests = (SELECT SUM(archiveFailCount) FROM autoActionLogTemp);
	END
	else
	BEGIN
		SET @sumOfArchiveFailureRequests = 0;
	END

	RETURN @sumOfArchiveFailureRequests;

END


