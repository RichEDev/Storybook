CREATE TABLE [dbo].[supplier_status] (
    [statusid]          INT           IDENTITY (1, 1) NOT NULL,
    [description]       NVARCHAR (50) NULL,
    [sequence]          SMALLINT      NOT NULL,
    [deny_contract_add] BIT           NOT NULL,
    [createdon]         DATETIME      NULL,
    [createdby]         INT           NULL,
    [modifiedon]        DATETIME      NULL,
    [modifiedby]        INT           NULL,
    [subaccountid]      INT           NULL,
    [archived]          BIT           NOT NULL
);

