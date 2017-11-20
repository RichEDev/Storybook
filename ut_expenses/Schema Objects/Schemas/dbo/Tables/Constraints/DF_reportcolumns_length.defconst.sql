ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_length] DEFAULT (0) FOR [length];

