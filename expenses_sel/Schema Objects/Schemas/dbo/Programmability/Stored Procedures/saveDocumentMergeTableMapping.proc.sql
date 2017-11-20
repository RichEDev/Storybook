CREATE PROCEDURE [dbo].[saveDocumentMergeTableMapping]
@mappingId INT,
@table_mappingId INT,
@sequence INT,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @returnId int;
	declare @count int;
	declare @sequenceok bit;
	
	if exists (select sequence from document_mappings_mergetable where mappingid = @mappingId and sequence = @sequence)
		begin
			set @sequenceok = (select 0)
		end
	else
		begin
			set @sequenceok = (select 1)
		end
	
	set @count = (select count(*) from document_mappings_mergetable where table_mappingid = @table_mappingId and mappingid = @mappingId);
	if(@count = 0)
	begin
		if @sequence <= 0 or @sequenceok = 0
		begin
			set @sequence = (select MAX(sequence)+1 from document_mappings_mergetable where mappingid = @mappingId);
		end
		if @sequence > 255
			return -1;
		else
			begin
				insert into document_mappings_mergetable (mappingid, table_mappingid, sequence, createdOn, createdBy)
				values (@mappingId, @table_mappingId, @sequence, GETDATE(), @CUemployeeID);
				
				update document_mappings set isMergePart = 1, modifiedOn = GETDATE(), modifiedBy = @CUemployeeID where mappingid = @table_mappingId;
				
				return 0;
			end
	end
end
