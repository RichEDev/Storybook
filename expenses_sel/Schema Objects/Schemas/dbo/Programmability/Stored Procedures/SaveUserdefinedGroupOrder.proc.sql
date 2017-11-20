CREATE PROCEDURE [dbo].[SaveUserdefinedGroupOrder] @groupOrder UserdefinedGroupOrdering READONLY
AS
UPDATE userdefinedGroupings SET userdefinedGroupings.[order]=(SELECT tmpTable.displayOrder FROM @groupOrder as tmpTable  WHERE tmpTable.userdefinedGroupID=userdefinedGroupings.userdefinedGroupID)
