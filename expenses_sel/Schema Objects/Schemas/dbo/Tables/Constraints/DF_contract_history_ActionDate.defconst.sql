ALTER TABLE [dbo].[contract_history]
    ADD CONSTRAINT [DF_contract_history_ActionDate] DEFAULT (getdate()) FOR [actionDate];

