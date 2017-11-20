ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_groupby] DEFAULT (0) FOR [groupby];

