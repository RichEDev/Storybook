CREATE TABLE [dbo].[joinViaParts] (
	[joinViaPartID]		INT IDENTITY(1,1) NOT NULL,
	[joinViaID]			INT NOT NULL,
	[relatedID]			UNIQUEIDENTIFIER NOT NULL,
	[relatedType]		TINYINT NOT NULL,
	[order]				INT NOT NULL
)


