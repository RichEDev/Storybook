CREATE PROCEDURE [dbo].[ApiBatchSaveEmployeeWorkAddress] @list ApiBatchSaveEmployeeWorkAddressType READONLY
AS
BEGIN

	DECLARE @retIds TABLE (Id int)

	MERGE dbo.EmployeeWorkAddresses AS Target
	USING (select * from @list) AS Source
	ON (Target.EmployeeWorkAddressId = Source.EmployeeWorkAddressId or Target.ESRAssignmentLocationId = Source.ESRAssignmentLocationId)
		WHEN MATCHED THEN
			UPDATE SET
				Target.EmployeeId = Source.EmployeeId,
				Target.AddressId = Source.AddressId,
				Target.StartDate = Source.StartDate,
				Target.EndDate = Source.EndDate,
				Target.Active = Source.Active,
				Target.Temporary = Source.Temporary,
				Target.ModifiedOn = Source.ModifiedOn,
				Target.ModifiedBy = Source.ModifiedBy,
				Target.ESRAssignmentLocationId = Source.ESRAssignmentLocationId
		WHEN NOT MATCHED BY Target THEN
			INSERT (
				EmployeeId,
				AddressId,
				StartDate,
				EndDate,
				Active,
				Temporary,
				CreatedOn,
				CreatedBy,
				ESRAssignmentLocationId
			)
			VALUES (
				Source.EmployeeId,
				Source.AddressId,
				Source.StartDate,
				Source.EndDate,
				Source.Active,
				Source.Temporary,
				Source.CreatedOn,
				Source.CreatedBy,
				Source.ESRAssignmentLocationId
			)
	OUTPUT inserted.EmployeeWorkAddressId INTO @retIds;

	select * from @retIds;

	RETURN 0;
END
