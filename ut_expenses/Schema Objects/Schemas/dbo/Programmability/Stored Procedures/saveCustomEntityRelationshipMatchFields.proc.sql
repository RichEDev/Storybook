CREATE PROCEDURE dbo.saveCustomEntityRelationshipMatchFields
@attributeId int,
@fieldIdList GuidPK readonly
as
	-- clear out removed values
	delete from dbo.customEntityAttributeMatchFields where attributeId = @attributeId and fieldId not in (select ID from @fieldIdList);
	
	-- insert new values not already saved
	declare @fieldID uniqueidentifier;
	declare lp cursor for
	select ID from @fieldIdList where ID not in (select fieldId from customEntityAttributeMatchFields where attributeId = @attributeId)
	open lp
	fetch next from lp into @fieldID
	while @@FETCH_STATUS = 0
	begin
		insert into customEntityAttributeMatchFields (attributeId, fieldId)
		values (@attributeId, @fieldID);
		
		fetch next from lp into @fieldID
	end
	close lp;
	deallocate lp;
