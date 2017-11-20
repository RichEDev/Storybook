
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[saveESRExportLog] 
	@logid int,
	@logitems logitem READONLY
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @createdon datetime;
	set @createdon = GETDATE()
    -- Insert statements for procedure here
	insert into logDataItems (logID, logReasonID, logElementID, logDataItem, createdOn) (select @logid, tmp.reasonID, elementID, logItem, @createdon from @logitems as tmp)
END