


CREATE FUNCTION [dbo].[getSumOfArchiveRequests]
(
)
RETURNS INT
AS
BEGIN
	DECLARE @sumOfArchiveRequests int;
	
	IF OBJECT_ID('dbo.autoActionLogTemp','U') IS NOT NULL
	BEGIN
		SET @sumOfArchiveRequests = (SELECT SUM(archiveCount) FROM autoActionLogTemp);
	END
	else
	BEGIN
		SET @sumOfArchiveRequests = 0;
	END

	RETURN @sumOfArchiveRequests;

END


