CREATE TABLE [dbo].[addressLabels] (
	[AddressLabelID]	INT				IDENTITY(1,1) NOT NULL,
	[EmployeeID]		INT				NULL,
	[AddressID]			INT				NOT NULL,
	[Label]				NVARCHAR(50)	NOT NULL,
	[IsPrimary]			BIT				NOT NULL CONSTRAINT def_addressLabels_IsPrimary DEFAULT (0),
	[LastUsed]			DATETIME		NOT NULL CONSTRAINT def_addressLabels_LastUsed DEFAULT(getdate()),
	CONSTRAINT [PK_addressLabel] PRIMARY KEY CLUSTERED ([AddressLabelID] ASC)
);