CREATE PROCEDURE [dbo].[TorchDeleteGroupingConfiguration] 
	@MergeProjectId int = 0, 
	@ConfigurationId int = 0
AS
BEGIN
	SET NOCOUNT ON;

	delete from DocumentGroupingConfigurationColumns where MergeProjectId = @MergeProjectId and GroupingId = @ConfigurationId;
	delete from DocumentGroupingConfigurations where MergeProjectId = @MergeProjectId and GroupingId = @ConfigurationId;
	return 0;
END