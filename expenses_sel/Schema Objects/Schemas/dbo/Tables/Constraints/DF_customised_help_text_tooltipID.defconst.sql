ALTER TABLE [dbo].[customised_help_text]
    ADD CONSTRAINT [DF_customised_help_text_tooltipID] DEFAULT (newid()) FOR [tooltipID];

