ALTER TABLE [dbo].[subcats]
    ADD CONSTRAINT [DF_subcats_isRelocationMileage] DEFAULT ((0)) FOR [isRelocationMileage];

