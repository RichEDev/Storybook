CREATE TABLE [dbo].[card_statements_base] (
    [statementid]    INT            IDENTITY (1, 1) NOT NULL,
    [name]           NVARCHAR (250) NOT NULL,
    [statementdate]  DATETIME       NULL,
    [cardproviderid] INT            NOT NULL,
    [CreatedOn]      DATETIME       NULL,
    [CreatedBy]      INT            NULL,
    [ModifiedOn]     DATETIME       NULL,
    [ModifiedBy]     INT            NULL,
    CONSTRAINT [PK_card_statements] PRIMARY KEY CLUSTERED ([statementid] ASC),
    CONSTRAINT [FK_card_statements_employees1] FOREIGN KEY ([ModifiedBy]) REFERENCES [dbo].[employees] ([employeeid]) ON DELETE SET NULL
);



