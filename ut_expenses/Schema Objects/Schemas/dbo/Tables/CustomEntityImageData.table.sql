CREATE TABLE [dbo].[CustomEntityImageData]
(
    [ID] Int IDENTITY (1, 1) NOT NULL,
	[entityID] INT NOT NULL, 
    [attributeID] INT NOT NULL, 
    [fileID] UNIQUEIDENTIFIER NOT NULL, 
    [imageBinary] VARBINARY(MAX) NOT NULL, 
    [fileType] NVARCHAR(6) NOT NULL, 
    [fileName] NVARCHAR(100) NULL,
)
