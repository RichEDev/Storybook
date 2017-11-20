CREATE TABLE [dbo].[purchasecardrecords] (
    [purchasecardrecordid] INT             IDENTITY (1, 1)  NOT NULL,
    [description]          NVARCHAR (4000) NOT NULL,
    [employeeid]           INT             NOT NULL,
    [uploaddate]           DATETIME        NULL,
    [statementdate]        DATETIME        NULL,
    [uploadsuccess]        BIT             NOT NULL,
    [failurereason]        NVARCHAR (4000) NULL,
    [imported]             BIT             NOT NULL,
    [filename]             NVARCHAR (500)  NOT NULL
);

