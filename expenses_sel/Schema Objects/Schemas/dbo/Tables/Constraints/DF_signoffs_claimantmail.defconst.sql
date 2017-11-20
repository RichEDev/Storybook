ALTER TABLE [dbo].[signoffs]
    ADD CONSTRAINT [DF_signoffs_claimantmail] DEFAULT (0) FOR [claimantmail];

