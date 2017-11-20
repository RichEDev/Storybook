CREATE procedure [dbo].[saveDocumentMergeSource]
@projectId int,
@reportSourceId uniqueidentifier,
@userId int,
@CUemployeeID int,
@CUdelegateID INT
as
begin
	declare @count int
	declare @newmergesourceid int
	set @newmergesourceid = 0;

	set @count = (select count([mergesourceid]) from document_mergesources where mergeprojectid = @projectId and reportid = @reportSourceId)

	if @count = 0
	begin
		insert into document_mergesources (mergeprojectid, reportid, createddate, createdby) values (@projectId, @reportSourceId, getutcdate(), @userId)

		set @newmergesourceid = scope_identity();

		declare @title1 nvarchar(500);
		select @title1 = project_name from document_mergeprojects where mergeprojectid = @projectId;
		declare @recordTitle nvarchar(2000);
		set @recordTitle = (select 'Merge Source - ' + cast(@reportSourceId as nvarchar(100)) + ' for ' + @title1);

		exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 63, @newmergesourceid, @recordTitle, null;

	end

	return @newmergesourceid;
end
