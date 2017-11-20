CREATE PROCEDURE [dbo].[SaveUserdefinedGroupOrder] 
@groupOrder UserdefinedGroupOrdering READONLY,
@lastUpdated datetime out
AS
UPDATE userdefinedGroupings 
SET userdefinedGroupings.[order]=(SELECT tmpTable.displayOrder FROM @groupOrder as tmpTable 
WHERE tmpTable.userdefinedGroupID=userdefinedGroupings.userdefinedGroupID)
WHERE userdefinedGroupings.userdefinedGroupID in (select tmpTable.userdefinedGroupID from @groupOrder as tmpTable);

select @lastUpdated = GETDATE()

