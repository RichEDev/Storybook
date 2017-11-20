CREATE PROC [dbo].[UpdateFormIdForManyToOneAttributeFieldFilter]
@attributeid INT,
@formid INT
AS
BEGIN
If EXISTS (SELECT * FROM fieldFilters WHERE attributeid = @attributeid AND formid = -1)
UPDATE fieldFilters
SET formid = @formid
WHERE attributeid = @attributeid AND formid = -1
END