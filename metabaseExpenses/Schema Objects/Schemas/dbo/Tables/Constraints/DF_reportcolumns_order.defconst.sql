ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_order] DEFAULT ((1)) FOR [order];

