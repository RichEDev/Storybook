CREATE PROCEDURE dbo.saveUserdefinedMatchFields
@userdefineId int,
@fieldIdList GuidPK readonly
as
	-- clear out removed values
	delete from dbo.userdefinedMatchFields where userdefineId = @userdefineId and fieldId not in (select ID from @fieldIdList);
	
	-- insert new values not already saved
	declare @fieldID uniqueidentifier;
	declare lp cursor for
	select ID from @fieldIdList where ID not in (select fieldId from userdefinedMatchFields where userdefineId = @userdefineId)
	open lp
	fetch next from lp into @fieldID
	while @@FETCH_STATUS = 0
	begin
		insert into userdefinedMatchFields (userdefineId, fieldId)
		values (@userdefineId, @fieldID);
		
		fetch next from lp into @fieldID
	end
	close lp;
	deallocate lp;