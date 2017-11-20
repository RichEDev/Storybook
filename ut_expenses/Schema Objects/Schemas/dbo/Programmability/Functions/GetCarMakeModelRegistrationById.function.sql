CREATE FUNCTION [dbo].[GetCarMakeModelRegistrationById] (@carId INT)
RETURNS NVARCHAR(154)
AS
BEGIN
	DECLARE @name nvarchar(154);
	SELECT @name = [make] + ' ' + [model] + ' (' + [registration] + ')' FROM [cars] WHERE [carid] = @carId;

	RETURN @name;
END