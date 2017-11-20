
CREATE PROCEDURE [dbo].[saveLocationDistance]
@locationa int,
@locationb INT,
@distance DECIMAL(18,2),
@postcodeAnywhereDistance DECIMAL(18,2),
@employeeID INT,
@delegateID INT,
@mileageCalcType tinyint

AS

	DECLARE @count INT;
	declare @recordTitle nvarchar(2000);
	declare @loca nvarchar(250);
	declare @locb nvarchar(250);
	declare @distanceid int;

	set @loca = (select company from companies where companyid = @locationa);
	set @locb = (select company from companies where companyid = @locationb);
	set @recordTitle = (select @loca + ' to ' + @locb);

	SET @count = (SELECT COUNT(*) FROM location_distances WHERE (locationa = @locationa AND locationb = @locationb));

if (@count = 0)
begin
	if @mileageCalcType = 1
	BEGIN
		insert into location_distances (locationa, locationb, distance, postcodeAnywhereShortestDistance) VALUES (@locationa, @locationb, @distance, @postcodeAnywhereDistance);
	END
	ELSE
	BEGIN
		insert into location_distances (locationa, locationb, distance, postcodeAnywhereFastestDistance) VALUES (@locationa, @locationb, @distance, @postcodeAnywhereDistance);
	END
	exec addInsertEntryToAuditLog @employeeID, @delegateID, 38, null, @recordTitle, null;
end
else
begin
	declare @olddistance DECIMAL(18,2);
	declare @oldPostcodeAnywhereDistance DECIMAL(18,2);

	if @mileageCalcType = 1
	BEGIN
		select top 1 @olddistance = distance, @oldPostcodeAnywhereDistance = postcodeAnywhereShortestDistance FROM [location_distances] WHERE (locationa = @locationa AND locationb = @locationb);
		UPDATE [location_distances] SET distance = @distance, postcodeAnywhereShortestDistance = @postcodeAnywhereDistance WHERE (locationa = @locationa AND locationb = @locationb);
		
		if @oldPostcodeAnywhereDistance <> @postcodeAnywhereDistance
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 38, null, '3d44177b-928e-448c-80b6-05d7f24e5a6d', @oldPostcodeAnywhereDistance, @postcodeAnywhereDistance, @recordtitle, null;
	
	END
	ELSE
	BEGIN
		select top 1 @olddistance = distance, @oldPostcodeAnywhereDistance = postcodeAnywhereFastestDistance FROM [location_distances] WHERE (locationa = @locationa AND locationb = @locationb);
		UPDATE [location_distances] SET distance = @distance, postcodeAnywhereFastestDistance = @postcodeAnywhereDistance WHERE (locationa = @locationa AND locationb = @locationb);
		
		if @oldPostcodeAnywhereDistance <> @postcodeAnywhereDistance
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 38, null, 'c2d53a4f-c5de-495f-8ea4-0b7cb210e1c3', @oldPostcodeAnywhereDistance, @postcodeAnywhereDistance, @recordtitle, null;
	
	END
	if @olddistance <> @distance
		exec addUpdateEntryToAuditLog @employeeID, @delegateID, 38, null, 'c1c0cff2-3c7c-457a-91ec-4be328403ed4', @olddistance, @distance, @recordtitle, null;
		
end
