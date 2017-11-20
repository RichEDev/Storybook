CREATE TABLE [dbo].[rolesubcats] (
    [rolesubcatid]   INT   IDENTITY (1, 1)  NOT NULL,
    [roleid]         INT   NOT NULL,
    [subcatid]       INT   NOT NULL,
    [maximum]        MONEY NULL,
    [receiptmaximum] MONEY NULL,
    [isadditem]      BIT   NOT NULL,
    [preapproval]    BIT   NOT NULL
);

