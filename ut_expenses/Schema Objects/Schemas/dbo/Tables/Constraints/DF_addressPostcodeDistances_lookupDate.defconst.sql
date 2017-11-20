ALTER TABLE [dbo].[addressPostcodeDistances]
    ADD CONSTRAINT [DF_addressPostcodeDistances_lookupDate] DEFAULT (getdate()) FOR [lookupDate];

