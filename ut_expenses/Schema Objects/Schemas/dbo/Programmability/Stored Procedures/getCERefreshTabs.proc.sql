CREATE PROCEDURE dbo.getCERefreshTabs (@attributeId int)
as
SET NOCOUNT ON;
declare @tabIds IntPK;
declare @tabId int;
declare @summaryAttributeId int;
declare @attFormId IntPK;
declare @summaryAttFormId int;

insert into @attFormId select distinct formid from [customEntityFormTabs] where tabid in (select tabid from [customEntityFormSections] where sectionid in (select sectionid from [customEntityFormFields] where attributeid = @attributeId));

if exists (select * from [customEntityAttributes] where attributeid = @attributeId and fieldtype = 15)
begin
	-- deleting from the summary grid, so need to refresh all grids on the form of this entity type
	declare loop_cursor cursor for
	select tabid from [customEntityFormSections] where sectionid in (select sectionid from [customEntityFormFields] where attributeid in (select otm_attributeid from [customEntityAttributeSummary] where attributeid = @attributeId))
	open loop_cursor
	fetch next from loop_cursor into @tabId
	while @@FETCH_STATUS = 0
	begin
		select @summaryAttFormId = formid from [customEntityFormTabs] where tabid = @tabId;
		
		if not exists (select * from @tabIds where c1 = @tabId) and @summaryAttFormId in (select c1 from  @attFormId)
		begin
			insert into @tabIds values (@tabId)
		end
		
		fetch next from loop_cursor into @tabId
	end
	close loop_cursor
	deallocate loop_cursor
end
else
begin
	-- deleting from the normal view, so check for summary grid on same form
	declare loop_cursor cursor for
	select attributeid from [customEntityAttributeSummary] where otm_attributeid = @attributeId
	open loop_cursor
	fetch next from loop_cursor into @summaryAttributeId
	while @@FETCH_STATUS = 0
	begin
		select distinct @summaryAttFormId = formid, @tabId = tabid from [customEntityFormTabs] where tabid in (select tabid from [customEntityFormSections] where sectionid in (select sectionid from [customEntityFormFields] where attributeid = @summaryAttributeId));

		if not exists (select * from @tabIds where c1 = @tabId) and @summaryAttFormId in (select c1 from  @attFormId)
		begin
			insert into @tabIds values (@tabId)
		end
		
		fetch next from loop_cursor into @summaryAttributeId
	end
	close loop_cursor
	deallocate loop_cursor
end

select c1 from @tabIds
