CREATE PROCEDURE [dbo].[SaveCustomField]
	
	@Update bit,
	@FieldID uniqueidentifier,
	@Description nvarchar(1000),
	@Field nvarchar(500),
	@FieldCategory tinyint,
	@LookupTable uniqueidentifier,
	@LookupField uniqueidentifier,
	@Comment nvarchar(4000),
	@ViewGroupID uniqueidentifier,
	@ReLabel bit,
	@ReLabel_Param nvarchar(150),
	@DataType nvarchar(2),
	@NormalView bit,
	@IdField bit,
	@GenList bit,
	@Width int,
	@CanTotal bit,
	@PrintOut bit,
	@ValueList bit,
	@AllowImport bit,
	@Mandatory bit,
	@UseForLookup bit,
	@WorkflowUpdate bit,
	@WorkflowSearch bit,
	@Length int,
	@TableID uniqueidentifier,
	@identity uniqueidentifier OUTPUT
AS

BEGIN
	IF @Update = 0
		BEGIN
			SET @identity = (SELECT fieldid FROM custom_fields WHERE Field = @Field AND tableid = @TableID); 
			IF @identity is null
			BEGIN
				INSERT INTO custom_fields ([fieldid]
			   ,[field]
			   ,[fieldtype]
			   ,[description]
			   ,[comment]
			   ,[normalview]
			   ,[idfield]
			   ,[genlist]
			   ,[width]
			   ,[cantotal]
			   ,[printout]
			   ,[valuelist]
			   ,[allowimport]
			   ,[mandatory]
			   ,[amendedon]
			   ,[lookuptable]
			   ,[lookupfield]
			   ,[useforlookup]
			   ,[workflowUpdate]
			   ,[workflowSearch]
			   ,[length]
			   ,[tableid]
			   ,[viewgroupid]
			   ,[relabel]
			   ,[relabel_param]
			   ,[FieldCategory])
			   VALUES (@FieldID, @Field, @DataType, @Description, @Comment, @NormalView,
			   @IdField, @GenList, @Width, @CanTotal, @PrintOut, @ValueList, @AllowImport,
			   @Mandatory, getutcdate(), @LookupTable, @LookupField, @UseForLookup,
			   @WorkflowUpdate, @WorkflowSearch, @Length, @TableID, @ViewGroupID,
			   @ReLabel, @ReLabel_Param, @FieldCategory);
			   SET @identity = @FieldID; 
			END
		END
	ELSE
		BEGIN
			UPDATE custom_fields
   SET 
      [field] = @Field
      ,[fieldtype] = @DataType
      ,[description] = @Description
      ,[comment] = @Comment
      ,[normalview] = @NormalView
      ,[idfield] = @IdField
      ,[genlist] = @GenList
      ,[width] = @Width
      ,[cantotal] = @CanTotal
      ,[printout] = @PrintOut
      ,[valuelist] = @ValueList
      ,[allowimport] = @AllowImport
      ,[mandatory] = @Mandatory
      ,[amendedon] = getutcdate()
      ,[lookuptable] = @LookupTable
      ,[lookupfield] = @LookupField
      ,[useforlookup] = @UseForLookup
      ,[workflowUpdate] = @WorkflowUpdate
      ,[workflowSearch] = @WorkflowSearch
      ,[length] = @Length
      ,[tableid] = @TableID
      ,[viewgroupid] = @ViewGroupID
      ,[relabel] = @ReLabel
      ,[relabel_param] = @ReLabel_Param
      ,[FieldCategory] = @FieldCategory
 WHERE fieldid = @FieldID;
	SET @identity = @FieldID;
		END
END
