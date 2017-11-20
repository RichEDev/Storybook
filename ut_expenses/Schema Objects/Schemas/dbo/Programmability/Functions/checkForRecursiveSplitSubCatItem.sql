CREATE FUNCTION [dbo].[checkForRecursiveSplitSubCatItem]
(
	@newSplitItem int,
	@parentItem int
)
RETURNS INT
AS
BEGIN

DECLARE @recursive int = NULL;
WITH SplitSubCats (subcat, subcatid, splitsubcatid)
AS
(
-- Anchor member definition
	SELECT e.subcat, e.subcatid, edh.splitsubcatid
	FROM dbo.subcats AS e
	left JOIN subcat_split AS edh
		ON e.subcatid = edh.primarysubcatid 
	WHERE edh.primarysubcatid in(@newSplitItem)
	UNION ALL
-- Recursive member definition
	SELECT e.subcat, e.subcatid, edh.splitsubcatid
	FROM dbo.subcats AS e
	INNER JOIN subcat_split AS edh
		ON e.subcatid = edh.primarysubcatid 
	INNER JOIN SplitSubCats as d on edh.primarysubcatid = d.splitsubcatid        
)
-- Statement that executes the CTE
SELECT @recursive = subcatid FROM SplitSubCats WHERE splitsubcatid = @parentItem;

IF (@recursive IS null)
BEGIN
	RETURN 0;
END

RETURN @recursive;
END