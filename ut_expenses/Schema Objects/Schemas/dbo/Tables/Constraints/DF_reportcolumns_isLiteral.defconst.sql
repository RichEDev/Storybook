ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_isLiteral] DEFAULT (0) FOR [isLiteral];

