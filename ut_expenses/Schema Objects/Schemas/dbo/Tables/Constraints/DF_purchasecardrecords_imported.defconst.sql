ALTER TABLE [dbo].[purchasecardrecords]
    ADD CONSTRAINT [DF_purchasecardrecords_imported] DEFAULT (0) FOR [imported];

