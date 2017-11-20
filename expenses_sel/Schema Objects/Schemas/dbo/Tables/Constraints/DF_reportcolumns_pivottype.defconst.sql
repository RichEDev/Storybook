ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_pivottype] DEFAULT (0) FOR [pivottype];

