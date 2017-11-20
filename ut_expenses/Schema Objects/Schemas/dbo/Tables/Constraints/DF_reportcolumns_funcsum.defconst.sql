ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_funcsum] DEFAULT (0) FOR [funcsum];

