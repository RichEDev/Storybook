CREATE TYPE [dbo].[FormSelectionMapping] AS TABLE
(
	[formSelectionMappingId]	int NOT NULL,
	[viewId]					int NOT NULL,
	[isAdd]						bit NOT NULL,
	[formId]					int NOT NULL,
	[listValue]					int NULL,
	[textValue]					nvarchar(4000) NULL
);