ALTER TABLE [dbo].[reportcriteria]
    ADD CONSTRAINT [DF_reportcriteria_reportid] DEFAULT (newid()) FOR [reportid];

