CREATE TABLE [dbo].[allowancebreakdown] (
    [breakdownid] INT   IDENTITY (1, 1) NOT NULL,
    [allowanceid] INT   NOT NULL,
    [hours]       INT   NOT NULL,
    [rate]        MONEY NOT NULL,
    CONSTRAINT [PK_allowancebreakdown] PRIMARY KEY CLUSTERED ([breakdownid] ASC) WITH (FILLFACTOR = 90),
    CONSTRAINT [FK_allowancebreakdown_allowances] FOREIGN KEY ([allowanceid]) REFERENCES [dbo].[allowances] ([allowanceid]) ON DELETE CASCADE
);



