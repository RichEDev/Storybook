
CREATE PROCEDURE [dbo].[GetCommonColumns] 
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT tableid, fieldid FROM reports_common_columns
END
