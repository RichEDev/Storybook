ALTER TABLE [dbo].[countrysubcats]
    ADD CONSTRAINT [DF_countrysubcats_countryid] DEFAULT (0) FOR [countryid];

