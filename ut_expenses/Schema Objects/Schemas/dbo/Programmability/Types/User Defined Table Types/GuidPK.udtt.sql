CREATE TYPE dbo.GuidPK AS TABLE 
(
	ID uniqueidentifier
    PRIMARY KEY NONCLUSTERED (ID) 
)
GO