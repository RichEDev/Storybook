CREATE FUNCTION [dbo].[checkForReferenceToSplitSubCatItem]
(
 @newSplitItem int,
 @parentItem int
)
RETURNS INT
AS
BEGIN

DECLARE @subCatId int = NULL;
WITH SplitSubCats (subcatid, splitsubcatid)
AS
(
-- Anchor member definition
    SELECT edh.primarysubcatid, edh.splitsubcatid
    FROM subcat_split AS edh        
    WHERE edh.primarysubcatid in(@newSplitItem)
    UNION ALL
-- Recursive member definition
    SELECT edh.primarysubcatid, edh.splitsubcatid
    FROM  subcat_split AS edh        
 INNER JOIN SplitSubCats as d on edh.primarysubcatid = d.splitsubcatid        
)
-- Statement that executes the CTE
 SELECT  @subCatId = n.primarysubcatid
    FROM    [dbo].subcat_split n
    WHERE  EXISTS (SELECT *
                   FROM SplitSubCats t
                   WHERE splitsubcatid = @parentItem
                  )

OPTION  (MAXRECURSION 1000);
IF (@subCatId IS null)
BEGIN
 RETURN 0;
END

RETURN @subCatId;
END