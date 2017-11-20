ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_hidden] DEFAULT ((0)) FOR [hidden];

