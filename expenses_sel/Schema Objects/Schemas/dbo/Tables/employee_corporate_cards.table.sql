CREATE TABLE [dbo].[employee_corporate_cards] (
    [corporatecardid] INT           IDENTITY (1, 1) NOT NULL,
    [employeeid]      INT           NOT NULL,
    [cardnumber]      NVARCHAR (50) NOT NULL,
    [active]          BIT           NOT NULL,
    [cardproviderid]  INT           NOT NULL,
    [createdon]       DATETIME      NULL,
    [createdby]       INT           NULL,
    [modifiedon]      DATETIME      NULL,
    [modifiedby]      INT           NULL
);

