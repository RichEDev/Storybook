ALTER TABLE [dbo].[codes_inflatormetrics]
    ADD CONSTRAINT [DF_codes_inflatormetrics_percentage] DEFAULT ((0)) FOR [percentage];

