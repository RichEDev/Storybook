CREATE TABLE [dbo].[subcat_vat_rates] (
    [vatrateid]       INT          IDENTITY (1, 1) NOT FOR REPLICATION NOT NULL,
    [subcatid]        INT          NOT NULL,
    [vatamount]       FLOAT        NOT NULL,
    [vatreceipt]      BIT          NOT NULL,
    [vatpercent]      TINYINT      NOT NULL,
    [vatlimitwithout] DECIMAL (18) NULL,
    [vatlimitwith]    DECIMAL (18) NULL,
    [daterangetype]   TINYINT      NOT NULL,
    [datevalue1]      DATETIME     NULL,
    [datevalue2]      DATETIME     NULL
);

