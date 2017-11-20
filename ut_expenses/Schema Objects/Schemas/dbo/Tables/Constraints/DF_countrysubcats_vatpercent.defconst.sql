ALTER TABLE [dbo].[countrysubcats]
    ADD CONSTRAINT [DF_countrysubcats_vatpercent] DEFAULT (0) FOR [vatpercent];

