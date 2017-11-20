ALTER TABLE [dbo].[help_text]
    ADD CONSTRAINT [DF_help_text_tooltipID] DEFAULT (newid()) FOR [tooltipID];

