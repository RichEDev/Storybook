CREATE VIEW [dbo].[supplier_notes_view]
AS 
	SELECT supplierNotes.noteId, supplierId, Note, [Date], supplierNotes.createdBy, employees.firstname + ' ' + employees.surname AS [CreatedByName], [Primary].[fullDescription] AS [PrimaryCategory],[Secondary].[fullDescription] AS [SecondaryCategory] 
	FROM [supplierNotes]
        LEFT OUTER JOIN [codes_notecategory] AS [Primary] ON [Primary].[NoteCatId] = [supplierNotes].[NoteType]
        LEFT OUTER JOIN [codes_notecategory] AS [Secondary] ON [Secondary].[NoteCatId] = [supplierNotes].[NoteCatId]
        LEFT OUTER JOIN [employees] ON employees.employeeid = supplierNotes.createdBy

