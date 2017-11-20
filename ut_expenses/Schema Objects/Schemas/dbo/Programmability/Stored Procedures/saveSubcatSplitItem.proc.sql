
CREATE  PROCEDURE [dbo].[saveSubcatSplitItem]
 @primarysubcatid int,
 @splitsubcatid int,
 @employeeID INT,
 @delegateID INT
AS

BEGIN
 -- SET NOCOUNT ON added to prevent extra result sets from
 -- interfering with SELECT statements.

 SET NOCOUNT ON;
 DECLARE @title1 nvarchar(500);
 DECLARE @title2 nvarchar(500);
 DECLARE @recordTitle nvarchar(2000);
 SELECT @title1 = subcat from subcats where subcatid = @primarysubcatid;
 SELECT @title2 = subcat from subcats where subcatid = @splitsubcatid;
 DECLARE @subCatId INT
 SET  @subCatId = dbo.checkForReferenceToSplitSubCatItem(@splitsubcatid,@primarysubcatid) 
 IF(ISNULL(@subCatId,0)=0)
 BEGIN
 SET @recordTitle = (select @title1 + ' split with ' + @title2);
    insert into subcat_split (primarysubcatid, splitsubcatid) values (@primarysubcatid, @splitsubcatid);
 exec addInsertEntryToAuditLog @employeeID, @delegateID, 29, @primarysubcatid, @recordTitle, null

 return 0

 END
 ELSE
 return -2 ;

END
