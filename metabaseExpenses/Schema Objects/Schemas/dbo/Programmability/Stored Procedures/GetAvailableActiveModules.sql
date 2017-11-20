CREATE PROCEDURE [dbo].[GetAvailableActiveModules]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT brandName
		,m.moduleID
	FROM modulebase m
	INNER JOIN hostnames h ON m.moduleID = h.moduleID
	GROUP BY brandName
		,m.moduleID
END