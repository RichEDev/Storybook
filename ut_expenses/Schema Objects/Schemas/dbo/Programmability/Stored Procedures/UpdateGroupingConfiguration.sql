CREATE PROCEDURE [dbo].[UpdateGroupingConfiguration]      
 @GroupingColumns dbo.StringTable READONLY,
 @GroupingConfigurationId int = 0,
 @MergeProjectId int = 0,
 @Label nvarchar(255),
 @UserId int = 0,
 @Archived bit = 0,
 @Description nvarchar(250)
AS

Declare @Id int = 0
BEGIN
    
    if @GroupingConfigurationId > 0
    begin
    if exists (select * from dbo.DocumentGroupingConfigurations where label = @label and GroupingId <> @GroupingConfigurationId and MergeProjectId = @MergeProjectId)
	BEGIN
		return -1;
	END
    set @Id = (select @GroupingConfigurationId);
    
        update dbo.DocumentGroupingConfigurations 
    set Label = @Label,
    ModifiedOn = GETDATE(), 
    ModifiedBy = @UserId, 
    Archived = @Archived ,
	Description = @description
    where GroupingId = @GroupingConfigurationId and MergeProjectId = @MergeProjectId
    
    delete from dbo.DocumentGroupingConfigurationColumns where GroupingId = @GroupingConfigurationId and MergeProjectId = @MergeProjectId;
    end
    else
    begin 

    if not exists (select * from dbo.DocumentGroupingConfigurations where Label = @Label and MergeProjectId = @MergeProjectId)
    begin
  
    insert into dbo.DocumentGroupingConfigurations (
    MergeProjectId,
    Label,
    CreatedBy,
    CreatedOn,
	[Description]
    )
    values (
    @MergeProjectId,
    @Label,
    @UserId,
    GETDATE(),
	@description
    )
        
    set @Id = (SELECT MAX(GroupingId) FROM dbo.DocumentGroupingConfigurations); 
    
    end
    else
    begin
    return -1;
    end
    end
    
    insert into dbo.DocumentGroupingConfigurationColumns (
    GroupingId,
    MergeProjectId,
    GroupingColumn,
    SequenceOrder)
    select
	@Id,
	@MergeProjectId,
	StringValue, 
	ROW_NUMBER() OVER(ORDER BY (Sequence))
	from @GroupingColumns

    Return @Id

END
