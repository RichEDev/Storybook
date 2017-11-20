
CREATE VIEW [dbo].[primarynotecategory_view]
AS 
	SELECT [NoteCatId],[description] FROM codes_notecategory WHERE [NoteType] = 0

