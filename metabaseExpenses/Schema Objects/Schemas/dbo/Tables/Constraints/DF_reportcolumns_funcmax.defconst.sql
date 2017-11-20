ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_funcmax] DEFAULT ((0)) FOR [funcmax];

