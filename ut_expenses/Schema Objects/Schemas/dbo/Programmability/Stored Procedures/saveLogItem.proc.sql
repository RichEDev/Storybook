


--Create saveLog sp
CREATE PROCEDURE [dbo].[saveLogItem] (@logID int, @logReasonID int, @logElementID int, @logDataItem nvarchar (4000), @createdOn DateTime)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	insert into logDataItems (logID, logReasonID, logElementID, logDataItem, createdOn) values (@logID, @logReasonID, @logElementID, @logDataItem, @createdOn);
;
END


