CREATE PROCEDURE TorchArchiveGroupingConfiguration 
	@MergeProjectId int = 0, 
	@ConfigurationId int = 0,
	@Archived bit = 1
AS
BEGIN
	SET NOCOUNT ON;

	update DocumentGroupingConfigurations set Archived = @Archived where MergeProjectId = @MergeProjectId and GroupingId = @ConfigurationId
	return 0;
END