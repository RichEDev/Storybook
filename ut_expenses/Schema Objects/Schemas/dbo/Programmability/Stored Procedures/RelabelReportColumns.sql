CREATE PROCEDURE [dbo].[RelabelReportColumns]
	@relabelParam nvarchar(100),
	@oldValue nvarchar(100),
	@newValue nvarchar(100)
AS
	UPDATE reportcolumns SET format = REPLACE([format], @oldValue, @newValue) FROM reportcolumns
INNER JOIN [$(targetMetabase)].dbo.fields_base AS F ON F.fieldid = reportcolumns.fieldID WHERE relabel_param = @relabelParam
RETURN 0
