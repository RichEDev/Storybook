ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_reportcolumnid] DEFAULT (newid()) FOR [reportcolumnid];

