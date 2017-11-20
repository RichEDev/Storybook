CREATE PROCEDURE [dbo].[TorchGetGroupingConfigurations] 
	@MergeProjectId int = 0, 
	@IncludeArchived bit = 0
AS
BEGIN
    if @IncludeArchived = 1
    begin
	   select GroupingId, Label from DocumentGroupingConfigurations where MergeProjectId = @MergeProjectId order by Label
    end
    else
    begin
	   select GroupingId, Label from DocumentGroupingConfigurations where MergeProjectId = @MergeProjectId and Archived = 0 order by Label
    end
END