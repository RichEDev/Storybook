ALTER TABLE [dbo].[codes_inflatormetrics]
    ADD CONSTRAINT [DF_codes_inflatormetrics_archived] DEFAULT ((0)) FOR [archived];

