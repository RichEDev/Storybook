ALTER TABLE [dbo].[employee_corporate_cards]
    ADD CONSTRAINT [DF_corporate_cards_active] DEFAULT (0) FOR [active];

