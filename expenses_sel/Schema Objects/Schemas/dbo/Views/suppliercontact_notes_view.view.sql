CREATE VIEW [dbo].[suppliercontact_notes_view]
AS 
SELECT supplierContactNotes.noteId, contactId, Note, [Date], supplierContactNotes.createdBy, employees.firstname + ' ' + employees.surname AS [CreatedByName], [Primary].[fullDescription] AS [PrimaryCategory],[Secondary].[fullDescription] AS [SecondaryCategory] 
	FROM supplierContactNotes
        LEFT OUTER JOIN [codes_notecategory] AS [Primary] ON [Primary].[NoteCatId] = supplierContactNotes.[NoteType]
        LEFT OUTER JOIN [codes_notecategory] AS [Secondary] ON [Secondary].[NoteCatId] = supplierContactNotes.[NoteCatId]
        LEFT OUTER JOIN [employees] ON employees.employeeid = supplierContactNotes.createdBy
