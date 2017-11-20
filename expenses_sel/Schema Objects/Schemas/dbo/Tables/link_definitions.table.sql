CREATE TABLE [dbo].[link_definitions] (
    [linkId]         INT          IDENTITY (1, 1) NOT NULL,
    [linkDefinition] VARCHAR (50) NULL,
    [isScheduleLink] INT          NOT NULL,
    [subAccountId]   INT          NULL
);

