ALTER TABLE [dbo].[countries]
    ADD CONSTRAINT [DF_countries_archived] DEFAULT ((0)) FOR [archived];

