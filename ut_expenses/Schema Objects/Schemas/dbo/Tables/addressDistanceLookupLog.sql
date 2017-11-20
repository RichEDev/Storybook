CREATE TABLE [dbo].[addressDistanceLookupLog]
(
	[addressDistanceLookupLogId] INT IDENTITY(1,1) NOT NULL, 
    [Origin] NVARCHAR(40) NOT NULL, 
    [Destination] NVARCHAR(40) NOT NULL, 
    [Timestamp] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	CONSTRAINT [PK_addressDistanceLookupLog] PRIMARY KEY CLUSTERED ([addressDistanceLookupLogId] ASC)
)
