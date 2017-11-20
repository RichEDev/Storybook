


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[deleteSubcatSplitItem]
	@primarysubcatid int,
	@splitsubcatid int,
	@employeeID INT,
	@delegateID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	declare @title1 nvarchar(500);
	declare @title2 nvarchar(500);
	declare @recordTitle nvarchar(2000);

	select @title1 = subcat from subcats where subcatid = @primarysubcatid;
	select @title2 = subcat from subcats where subcatid = @splitsubcatid;
	set @recordTitle = (select @title1 + ' split with ' + @title2);

	DELETE FROM subcat_split WHERE primarysubcatid=@primarysubcatid AND splitsubcatid=@splitsubcatid;

	exec addDeleteEntryToAuditLog @employeeID, @delegateID, 29, @primarysubcatid, @recordTitle, null;
END



