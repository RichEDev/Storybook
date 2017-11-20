
CREATE PROCEDURE [dbo].[UpdateFilteringConfiguration] @MergeProjectId                  INT,
                                                     @DocumentGroupingConfigurationId INT,
                                                     @FilterColumn                    NVARCHAR(255),                                            
                                                     @SequenceOrder                   INT,
                                                     @Condition                       TINYINT,
                                                     @ValueOne                        NVARCHAR(255),
                                                     @ValueTwo                        NVARCHAR(255),
                                                     @TypeText                        NVARCHAR(255),
                                                     @FieldType                       NVARCHAR(2)
AS
    DECLARE @Id INT = 0

  BEGIN
    


      INSERT INTO dbo.DocumentFiltersConfigurationColumns
                  (GroupingId,
                   MergeProjectId,
                   FilterColumn,
                   SequenceOrder,           
                   Condition,
                   ValueOne,
                   ValueTwo,
                   TypeText,
                   FieldType)
      VALUES      ( @DocumentGroupingConfigurationId,
				    @MergeProjectId,
                    @FilterColumn,
                    @SequenceOrder,
                    @Condition,
                    @ValueOne,
                    @ValueTwo,
                    @TypeText,
                    @FieldType)

       SET @Id = (SELECT Max(FilterColumnId)
                 FROM   dbo.DocumentFiltersConfigurationColumns);

      RETURN @Id
  END 
