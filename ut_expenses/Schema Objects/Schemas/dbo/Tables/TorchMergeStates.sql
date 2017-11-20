CREATE TABLE [dbo].[TorchMergeStates]
(
	MergeProjectId		int					not null,
	RequestNumber		uniqueidentifier	not null,
    ProgressCount		int					not null,
    TotalToProcess		int					not null,
    DocumentPath		nvarchar(500),
    DocumentUrl			nvarchar(500),
    DocumentIdentifier	uniqueidentifier	not null,
    MergeCompletionDate	datetime,
    ErrorMessage		nvarchar(max)		not null,
    [Status]			int					not null,
    OutputDocType		int					not null
)
