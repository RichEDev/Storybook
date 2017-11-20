CREATE TABLE [dbo].[mobileExpenseItems]
	(
	[mobileID] [int] NOT NULL IDENTITY(1, 1),
	[employeeid] [int] NOT NULL,
	[otherdetails] [nvarchar] (max) NULL,
	[reasonid] [int] NULL,
	[total] [money] NOT NULL,
	[subcatid] [int] NULL,
	[date] [date] NULL,
	[currencyid] [int] NULL,
	[miles] [decimal] (18, 2) NULL,
	[quantity] [float] NULL,
	[fromLocation] [nvarchar] (200) NULL,
	[toLocation] [nvarchar] (200) NULL,
	[allowancestartdate] [datetime] NULL,
	[allowanceenddate] [datetime] NULL,
	[itemNotes] nvarchar(max) NULL,
	[deviceTypeId] [int] NULL,
	[allowanceid] [int] NULL,
	[allowancededuct] [money] NULL,
	[mileageJourneySteps] [nvarchar](max) NULL,
	[mobileDeviceID] [int],
	[mobileExpenseID] [int]

	)