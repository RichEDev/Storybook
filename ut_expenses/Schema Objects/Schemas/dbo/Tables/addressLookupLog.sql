CREATE TABLE [dbo].[addressLookupLog]
(
	[AddressLookupLogId] INT IDENTITY(1,1) NOT NULL, 
    [AddressText] NVARCHAR(250) NOT NULL, 
    [Timestamp] DATETIME NOT NULL DEFAULT GETUTCDATE(),
	CONSTRAINT [PK_addressLookupLog] PRIMARY KEY CLUSTERED ([AddressLookupLogId] ASC)
)
