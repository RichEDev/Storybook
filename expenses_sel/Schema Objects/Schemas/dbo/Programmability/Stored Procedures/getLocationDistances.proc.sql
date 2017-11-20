
CREATE PROCEDURE [dbo].[getLocationDistances](@companyid INT)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Insert statements for procedure here
    
	BEGIN
		SELECT distanceid, company, distance, postcodeAnywhereShortestDistance, postcodeAnywhereFastestDistance, postcodeanywheredistance FROM [location_distances] 
			INNER JOIN companies ON companies.companyid = location_distances.locationb
			WHERE locationa = @companyid
		--union
		--SELECT distanceid, company, distance, postcodeAnywhereShortestDistance, postcodeAnywhereFastestDistance, postcodeanywheredistance FROM [location_distances] 
			--INNER JOIN companies ON companies.companyid = location_distances.locationa
			--WHERE locationb = @companyid
	END
END
