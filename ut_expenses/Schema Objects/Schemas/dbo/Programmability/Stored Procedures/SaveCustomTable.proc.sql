CREATE PROCEDURE [dbo].[SaveCustomTable] 
	@tableName nvarchar(50),
	@joinType tinyint,
	@allowReportOn bit,
	@description nvarchar(50),
	@primarykey uniqueidentifier,
	@keyField uniqueidentifier,
	@allowImport bit,
	@allowWorkflow bit,
	@allowEntityRelationship bit,
	@tableID uniqueidentifier,
	@hasUserDefinedFields bit,
	@userdefinedTable uniqueidentifier,
	@parentTableID uniqueidentifier
AS
BEGIN
	INSERT INTO [dbo].[customTables]
           ([tablename]
           ,[jointype]
           ,[allowreporton]
           ,[description]
           ,[primarykey]
           ,[keyfield]
           ,[allowimport]
           ,[amendedon]
           ,[allowworkflow]
           ,[allowentityrelationship]
           ,[tableid]
           ,[hasUserDefinedFields]
           ,[userdefined_table]
           ,[parentTableID])
     VALUES
           (@tableName, 
			@joinType, 
			@allowReportOn, 
			@description, 
			@primarykey, 
			@keyField, 
			@allowImport, 
			getutcdate(),
			@allowWorkflow, 
			@allowEntityRelationship, 
			@tableID, 
			@hasUserDefinedFields, 
			@userdefinedTable, 
			@parentTableID)
END
