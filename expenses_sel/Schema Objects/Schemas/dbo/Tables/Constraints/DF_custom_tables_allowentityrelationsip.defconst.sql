ALTER TABLE [dbo].[custom_tables]
    ADD CONSTRAINT [DF_custom_tables_allowentityrelationsip] DEFAULT ((0)) FOR [allowentityrelationship];

