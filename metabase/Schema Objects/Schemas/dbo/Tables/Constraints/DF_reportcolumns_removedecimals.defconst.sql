ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_removedecimals] DEFAULT ((0)) FOR [removedecimals];

