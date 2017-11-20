ALTER TABLE [dbo].[customTables]
    ADD CONSTRAINT [DF_customTables_allowentityrelationsip] DEFAULT ((0)) FOR [allowentityrelationship];

