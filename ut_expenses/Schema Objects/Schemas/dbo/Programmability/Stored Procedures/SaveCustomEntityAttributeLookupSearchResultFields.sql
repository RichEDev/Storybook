
CREATE PROCEDURE [dbo].[SaveCustomEntityAttributeLookupSearchResultFields]
@attributeId int,
@fieldIdList GuidPK readonly
as
	    delete from dbo.CustomEntityAttributeLookupSearchResultFields where attributeId = @attributeId
		declare @fieldID uniqueidentifier;
		insert into customEntityAttributeMatchFields (attributeId, fieldId)
		Select @attributeId,ID from  @fieldIdList
		
