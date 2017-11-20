ALTER TABLE [dbo].[countrysubcats]
    ADD CONSTRAINT [DF_countrysubcats_subcatid] DEFAULT (0) FOR [subcatid];

