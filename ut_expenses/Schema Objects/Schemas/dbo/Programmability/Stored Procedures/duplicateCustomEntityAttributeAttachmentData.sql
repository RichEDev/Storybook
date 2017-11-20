Create procedure [dbo].[duplicateCustomEntityAttributeAttachmentData]
(
@fileID uniqueIdentifier,
@newfileID uniqueidentifier output  
)
As

Declare @count int
select @count = COUNT(fileID) from CustomEntityImageData where fileID = @fileID

if @count > 0

Begin
set @newfileID = NEWID()

Insert into CustomEntityImageData (entityID,attributeID,imageBinary,fileType,fileName, fileID)
(Select entityID,attributeID,imageBinary,fileType, filename, @newfileID 
From CustomEntityImageData
Where fileID = @fileID)

end

else

set @newfileID =  cast(cast(0 as binary) as uniqueidentifier)



