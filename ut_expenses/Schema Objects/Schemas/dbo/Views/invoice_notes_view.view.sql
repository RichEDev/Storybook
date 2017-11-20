CREATE VIEW [dbo].[invoice_notes_view]
AS 
	SELECT invoiceNotes.noteId, invoiceId, Note, [Date], invoiceNotes.createdBy, employees.firstname + ' ' + employees.surname AS [CreatedByName], [Primary].[fullDescription] AS [PrimaryCategory],[Secondary].[fullDescription] AS [SecondaryCategory] 
	FROM [invoiceNotes]
        LEFT OUTER JOIN [codes_notecategory] AS [Primary] ON [Primary].[NoteCatId] = [invoiceNotes].[NoteType]
        LEFT OUTER JOIN [codes_notecategory] AS [Secondary] ON [Secondary].[NoteCatId] = [invoiceNotes].[NoteCatId]
		LEFT OUTER JOIN [employees] ON employees.employeeid = invoiceNotes.createdBy
