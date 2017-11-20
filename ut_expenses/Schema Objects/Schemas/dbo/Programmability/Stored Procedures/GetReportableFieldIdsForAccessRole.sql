
CREATE PROCEDURE [dbo].[GetReportableFieldIdsForAccessRole]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
SELECT r.FieldId, accessroleid from reportingfields r inner join fields f on r.FieldID = f.fieldid
END