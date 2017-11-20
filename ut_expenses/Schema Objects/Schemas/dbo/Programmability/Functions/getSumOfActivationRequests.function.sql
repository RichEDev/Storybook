
CREATE FUNCTION [dbo].[getSumOfActivationRequests]
(
)
RETURNS INT
AS
BEGIN
	DECLARE @sumOfActivationRequests int;
	
	IF OBJECT_ID('dbo.autoActionLogTemp','U') IS NOT NULL
	BEGIN
		SET @sumOfActivationRequests = (SELECT SUM(activationCount) FROM autoActionLogTemp);
	END
	else
	BEGIN
		SET @sumOfActivationRequests = 0;
	END

	RETURN @sumOfActivationRequests;

END
