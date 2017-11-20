ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_enableHomeToLocationMileaeg] DEFAULT ((0)) FOR [enableHomeToLocationMileage];

