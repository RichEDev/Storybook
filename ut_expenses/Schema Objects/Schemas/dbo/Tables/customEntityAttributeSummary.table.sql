﻿CREATE TABLE [dbo].[customEntityAttributeSummary] (
    [summary_attributeid] INT     IDENTITY (1, 1) NOT NULL,
    [attributeid]         INT     NOT NULL,
    [otm_attributeid]     INT     NOT NULL,
    [order]               TINYINT NULL
);

