	CREATE TABLE [dbo].[organisations](
		[OrganisationID]			INT				IDENTITY(1,1) NOT NULL,
		[OrganisationName]						NVARCHAR(256)	NOT NULL,
		[ParentOrganisationID]		INT				NULL,
		[PrimaryAddressID]			INT				NULL,
		[Comment]					NVARCHAR(4000)	NULL,
		[Code]						NVARCHAR(60)	NULL,
		[IsArchived]				BIT				NOT NULL,
		[CreatedOn]					DATETIME		NULL,
		[CreatedBy]					INT				NULL,
		[ModifiedOn]				DATETIME		NULL,
		[ModifiedBy]				INT				NULL,
		CONSTRAINT [PK_organisation] PRIMARY KEY CLUSTERED ([OrganisationID] ASC)
	);