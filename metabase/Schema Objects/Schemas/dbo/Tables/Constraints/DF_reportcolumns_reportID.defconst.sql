ALTER TABLE [dbo].[reportcolumns]
    ADD CONSTRAINT [DF_reportcolumns_reportID] DEFAULT (newid()) FOR [reportID];

