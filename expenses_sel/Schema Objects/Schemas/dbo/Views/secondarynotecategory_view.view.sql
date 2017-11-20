CREATE VIEW [dbo].[secondarynotecategory_view]
AS 
	SELECT [NoteCatId],[Description] FROM codes_notecategory WHERE [NoteType] > 0

