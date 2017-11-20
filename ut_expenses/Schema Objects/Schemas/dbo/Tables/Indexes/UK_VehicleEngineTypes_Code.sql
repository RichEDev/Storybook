create unique index UK_VehicleEngineTypes_Code
	on dbo.VehicleEngineTypes (Code)
	where Code IS NOT NULL;
