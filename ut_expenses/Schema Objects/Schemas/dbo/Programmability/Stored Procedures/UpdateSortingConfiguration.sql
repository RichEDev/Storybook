CREATE PROCEDURE [dbo].[UpdateSortingConfiguration] @MergeProjectId                  INT,
                                                     @DocumentGroupingConfigurationId INT,
                                                     @SequenceOrder                   INT,
                                                     @SortingColumn                   NVARCHAR(255),                                            
                                                     @SortingOrder                    INT
                                                     

AS
    DECLARE @Id INT = 0

  BEGIN
    
      INSERT INTO dbo.DocumentSortingConfigurationColumns
                  (GroupingId,
                   MergeProjectId,
                   SortingColumn,
                   DocumentSortTypeId,
                   SequenceOrder)
      VALUES      ( @DocumentGroupingConfigurationId,
				    @MergeProjectId,
                    @SortingColumn,
                    @SortingOrder,
                    @SequenceOrder)

       SET @Id = SCOPE_IDENTITY();


      RETURN @Id
  END 
