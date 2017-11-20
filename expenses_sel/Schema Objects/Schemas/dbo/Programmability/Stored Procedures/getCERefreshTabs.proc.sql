CREATE PROCEDURE dbo.getCERefreshTabs (@attributeId int)
as
SET NOCOUNT ON;

declare @tabIds IntPK;
declare @tabId int;
declare @summaryAttributeId int;
declare @attFormId int;
declare @summaryAttFormId int;

set @attFormId = (select distinct formid from custom_entity_form_tabs where tabid in (select tabid from custom_entity_form_sections where sectionid in (select sectionid from custom_entity_form_fields where attributeid = @attributeId)));

if exists (select * from custom_entity_attributes where attributeid = @attributeId and fieldtype = 15)
begin
	-- deleting from the summary grid, so need to refresh all grids on the form of this entity type
	declare loop_cursor cursor for
	select tabid from custom_entity_form_sections where sectionid in (select sectionid from custom_entity_form_fields where attributeid in (select otm_attributeid from custom_entity_attribute_summary where attributeid = @attributeId))
	open loop_cursor
	fetch next from loop_cursor into @tabId
	while @@FETCH_STATUS = 0
	begin
		select @summaryAttFormId = formid from custom_entity_form_tabs where tabid = @tabId;
		
		if not exists (select * from @tabIds where c1 = @tabId) and @summaryAttFormId = @attFormId
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
	select attributeid from custom_entity_attribute_summary where otm_attributeid = @attributeId
	open loop_cursor
	fetch next from loop_cursor into @summaryAttributeId
	while @@FETCH_STATUS = 0
	begin
		select distinct @summaryAttFormId = formid, @tabId = tabid from custom_entity_form_tabs where tabid in (select tabid from custom_entity_form_sections where sectionid in (select sectionid from custom_entity_form_fields where attributeid = @summaryAttributeId));

		if not exists (select * from @tabIds where c1 = @tabId) and @summaryAttFormId = @attFormId
		begin
			insert into @tabIds values (@tabId)
		end
		
		fetch next from loop_cursor into @summaryAttributeId
	end
	close loop_cursor
	deallocate loop_cursor
end

select c1 from @tabIds
