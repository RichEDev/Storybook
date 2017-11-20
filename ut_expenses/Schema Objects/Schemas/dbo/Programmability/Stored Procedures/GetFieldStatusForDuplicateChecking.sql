CREATE PROCEDURE [dbo].[GetFieldStatusForDuplicateChecking] 
	@flagID int,
	@subcatID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	CREATE TABLE #duplicateFields (
		field nvarchar(100),
		availableforChecking bit
	)
	
	declare @field nvarchar(100)
	declare @duplicatefield nvarchar(100)
	declare @sql nvarchar(max)
	declare @duplicatecheckingsource tinyint
	declare @duplicatecheckingcalculation tinyint
	DECLARE @ParmDefinition nvarchar(500);
	
	set @parmdefinition = '@subcatidIn int'
	declare field_cursor cursor for select fields.field, fields.duplicateCheckingSource, fields.DuplicateCheckingCalculation, duplicatefields.field from flagAssociatedFields left join fields on flagAssociatedFields.fieldID = fields.fieldid left join fields as duplicatefields on duplicatefields.fieldid = fields.associatedFieldForDuplicateChecking where flagID = @flagid

	open field_cursor

	fetch next from field_cursor into @field, @duplicatecheckingsource, @duplicatecheckingcalculation, @duplicatefield
		while @@fetch_status = 0
			BEGIN

				if @duplicatecheckingsource = 1
					insert into #duplicateFields (field, availableforChecking) select code, display from addscreen where code = @field
				else if @duplicatecheckingsource = 2
					begin
						 
						set @sql = 'select ''' + @field + ''',' + @duplicatefield + ' from subcats where subcatid = @subcatidIn'
						insert into #duplicateFields exec sp_executesql @sql, @parmdefinition, @subcatidIn = @subcatid
					end
				else if @duplicatecheckingsource = 3
					begin
						insert into #duplicateFields (field, availableforChecking) select @field, count(subcatid) from subcats where subcatid = @subcatID and calculation = @duplicatecheckingcalculation
					end
				fetch next from field_cursor into @field, @duplicatecheckingsource, @duplicatecheckingcalculation, @duplicatefield
			end
		close field_cursor
	deallocate field_cursor

	select count(field) from #duplicateFields where availableforChecking = 0
END


GO
