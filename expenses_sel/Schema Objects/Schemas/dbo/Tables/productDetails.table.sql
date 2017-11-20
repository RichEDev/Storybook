﻿CREATE TABLE [dbo].[productDetails] (
    [productId]              INT            IDENTITY (1, 1) NOT NULL,
    [subAccountId]           INT            NULL,
    [productCode]            NVARCHAR (50)  NULL,
    [productName]            NVARCHAR (70)  NULL,
    [description]            NVARCHAR (MAX) NULL,
    [productCategoryId]      INT            NULL,
    [installedVersionNumber] NVARCHAR (50)  NULL,
    [dateInstalled]          DATETIME       NULL,
    [availableVersionNumber] NVARCHAR (50)  NULL,
    [numLicencedCopies]      SMALLINT       NULL,
    [totalUsage]             INT            NULL,
    [userCode]               NVARCHAR (50)  NULL,
    [callofQuantity]         SMALLINT       NULL,
    [licenceExpires]         DATETIME       NULL,
    [Notify]                 NVARCHAR (50)  NULL,
    [notifyId]               INT            NULL,
    [renewalType]            INT            NULL,
    [notifyDays]             INT            NULL,
    [Archived]               BIT            NOT NULL,
    [createdon]              DATETIME       NOT NULL,
    [createdby]              INT            NULL,
    [modifiedon]             DATETIME       NULL,
    [modifiedby]             INT            NULL
);

