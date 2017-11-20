CREATE TYPE [dbo].[ApiBatchSaveEmployeeWorkAddressType] AS TABLE
(
	[EmployeeWorkAddressId]		INT			NULL,
	[EmployeeId]				INT			NULL,
	[AddressId]					INT			NULL,
	[StartDate]					DATETIME 	NULL,
	[EndDate]					DATETIME 	NULL,
	[Active]					BIT			NULL,
	[Temporary]					BIT			NULL,
	[CreatedOn]					DATETIME 	NULL,
	[CreatedBy]					INT			NULL,
	[ModifiedOn]				DATETIME 	NULL,
	[ModifiedBy]				INT			NULL,
	[ESRAssignmentLocationId]	INT			NULL
);
