CREATE VIEW [dbo].[contract_notes_view]
AS
SELECT     contractNotes.noteId, contractId, Note, [Date], contractNotes.createdBy, employees.firstname + ' ' + employees.surname AS [CreatedByName], [Primary].[fullDescription] AS [PrimaryCategory],[Secondary].[fullDescription] AS [SecondaryCategory] 
FROM         dbo.contractNotes 
	LEFT OUTER JOIN dbo.codes_notecategory AS [Primary] ON [Primary].noteCatId = dbo.contractNotes.noteType 
	LEFT OUTER JOIN	dbo.codes_notecategory AS Secondary ON Secondary.noteCatId = dbo.contractNotes.noteCatId
	LEFT OUTER JOIN [employees] ON employees.employeeid = contractNotes.createdBy


