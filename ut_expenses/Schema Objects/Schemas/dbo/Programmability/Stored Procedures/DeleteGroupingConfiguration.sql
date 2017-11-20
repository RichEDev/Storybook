CREATE procedure [dbo].[DeleteGroupingConfiguration]

@GroupingConfigurationId int = 0,
@MergeProjectId int = 0
 
AS

delete from dbo.DocumentGroupingConfigurationColumns where GroupingId = @GroupingConfigurationId and MergeProjectId = @MergeProjectId;
delete from dbo.DocumentGroupingConfigurations where GroupingId = @GroupingConfigurationId and MergeProjectId = @MergeProjectId;

select 0