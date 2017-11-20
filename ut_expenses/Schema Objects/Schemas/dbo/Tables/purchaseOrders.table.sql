CREATE TABLE [dbo].[purchaseOrders] (
    [purchaseOrderID]       INT            IDENTITY (1, 1)  NOT NULL,
    [supplierID]            INT            NOT NULL,
    [countryID]             INT            NOT NULL,
    [currencyID]            INT            NOT NULL,
    [parentPurchaseOrderID] INT            NULL,
    [title]                 NVARCHAR (200) NOT NULL,
    [purchaseOrderNumber]   NVARCHAR (50)  NULL,
    [purchaseOrderState]    TINYINT        NOT NULL,
    [dateOrdered]           DATETIME       NULL,
    [dateApproved]          DATETIME       NULL,
    [comments]              NVARCHAR (MAX) NULL,
    [orderType]             TINYINT        NOT NULL,
    [orderRecurrence]       TINYINT        NULL,
    [orderStartDate]        DATETIME       NULL,
    [orderEndDate]          DATETIME       NULL,
    [createdOn]             DATETIME       NOT NULL,
    [createdBy]             INT            NOT NULL,
    [modifiedOn]            DATETIME       NULL,
    [modifiedBy]            INT            NULL
);

