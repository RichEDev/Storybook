ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_funccount] DEFAULT ((0)) FOR [funccount];

