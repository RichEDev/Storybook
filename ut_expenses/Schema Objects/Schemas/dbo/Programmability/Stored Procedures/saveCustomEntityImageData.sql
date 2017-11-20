CREATE PROCEDURE [dbo].[SaveCustomEntityImageData]

@entityID int,
@attributeID int,
@fileID uniqueIdentifier,
@imageBinary varbinary(max),
@fileType nvarchar(6),
@fileName nvarchar(100)

as

DECLARE @count INT;

Set @count = (select count([fileID]) from CustomEntityImageData where fileID = @fileID and attributeID = @attributeID and entityID = @entityID)

if @count = 0
Begin 	
INSERT INTO dbo.CustomEntityImageData
           (entityID,
		    attributeID,
           [fileID]
           ,[imageBinary]
           ,[fileType]
		   ,[fileName])
     VALUES
           (@entityID,
           @attributeID,
           @fileID,
           @imageBinary,
           @fileType,
		   @fileName)
End
return 1;
