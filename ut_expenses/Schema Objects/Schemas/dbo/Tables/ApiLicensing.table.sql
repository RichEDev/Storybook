CREATE TABLE [dbo].[ApiLicensing](
	[AccountId] [int] NOT NULL,
	[TotalCalls] [int] NOT NULL,
	[FreeToday] [int] NOT NULL,
	[HourLimit] [int] NOT NULL,
	[HourRemaining] [int] NOT NULL,
	[HourLast] [datetime] NOT NULL,
	[MinuteLimit] [int] NOT NULL,
	[MinuteRemaining] [int] NOT NULL,
	[MinuteLast] [datetime] NOT NULL, 
    CONSTRAINT [PK_ApiLicensing] PRIMARY KEY ([AccountId]),
);