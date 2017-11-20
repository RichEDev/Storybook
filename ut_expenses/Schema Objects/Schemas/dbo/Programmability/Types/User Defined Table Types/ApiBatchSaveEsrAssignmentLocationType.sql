CREATE TYPE [dbo].[ApiBatchSaveEsrAssignmentLocationType] AS TABLE
(
	ESRAssignmentLocationId	int,
    esrAssignID				int,
	ESRLocationId			bigint,
	StartDate				datetime,
	DeletedDateTime			datetime
);
