ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_pivotorder] DEFAULT (0) FOR [pivotorder];

