﻿CREATE TABLE [dbo].[print_views] (
    [order]     INT              NOT NULL,
    [createdon] DATETIME         NOT NULL,
    [createdby] INT              NULL,
    [fieldID]   UNIQUEIDENTIFIER NOT NULL
);

