CREATE VIEW [dbo].[product_notes_view]
AS 
	SELECT productNotes.noteId, productId, Note, [Date], productNotes.createdBy, employees.firstname + ' ' + employees.surname AS [CreatedByName], [Primary].[fullDescription] AS [PrimaryCategory],[Secondary].[fullDescription] AS [SecondaryCategory] 
	FROM [productNotes]
        LEFT OUTER JOIN [codes_notecategory] AS [Primary] ON [Primary].[NoteCatId] = [productNotes].[NoteType]
        LEFT OUTER JOIN [codes_notecategory] AS [Secondary] ON [Secondary].[NoteCatId] = [productNotes].[NoteCatId]
		LEFT OUTER JOIN [employees] ON employees.employeeid = productNotes.createdBy

