ALTER TABLE [dbo].[addressPostcodeDistances]
    ADD CONSTRAINT [DF_addressPostcodeDistances_distance] DEFAULT ((0)) FOR [distance];

