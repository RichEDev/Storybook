ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_runtime] DEFAULT (0) FOR [runtime];

