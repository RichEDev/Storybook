﻿CREATE TABLE [dbo].[card_transactions_allstar] (
    [transactionid]  INT             NOT NULL,
    [AccountNo]      NVARCHAR (500)  NULL,
    [InvoiceCentre]  NVARCHAR (500)  NULL,
    [CostCentre]     NVARCHAR (500)  NULL,
    [InvoiceDate]    DATETIME        NULL,
    [TransactionReg] NVARCHAR (50)   NULL,
    [Mileage]        DECIMAL (18, 2) NULL,
    [Brand]          NVARCHAR (500)  NULL,
    [Product]        NVARCHAR (500)  NULL,
    [PPL]            MONEY           NULL,
    [Gallons]        NVARCHAR (50)   NULL,
    [Net]            MONEY           NULL,
    [Vat]            MONEY           NULL,
    [VoucherNo]      NVARCHAR (500)  NULL,
    [SuppNo]         NVARCHAR (500)  NULL,
    [SuppName]       NVARCHAR (500)  NULL,
    [SuppAdd1]       NVARCHAR (500)  NULL,
    [SuppAdd2]       NVARCHAR (500)  NULL,
    [SuppAdd3]       NVARCHAR (500)  NULL,
    [SuppTown]       NVARCHAR (500)  NULL,
    [SuppCounty]     NVARCHAR (500)  NULL,
    [SuppPostcode]   NVARCHAR (50)   NULL,
    [SuppTelephone]  NVARCHAR (50)   NULL,
    [EmbossedReg]    NVARCHAR (50)   NULL,
    [EmbossedDriver] NVARCHAR (500)  NULL,
    [FilmNumber]     NVARCHAR (50)   NULL,
    [InvoiceNo]      NVARCHAR (50)   NULL,
    [Source]         NVARCHAR (500)  NULL
);

