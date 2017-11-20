CREATE PROCEDURE [dbo].[SaveUserdefinedFieldsOrder] @fieldOrder UserdefinedFieldOrdering READONLY
AS
UPDATE userdefined SET 
userdefined.[order]=(
SELECT tmpTable.displayOrder FROM @fieldOrder as tmpTable  
WHERE tmpTable.userdefinedFieldID=userdefined.userdefineid)
WHERE userdefineid in (select userdefinedFieldID from @fieldOrder);
