ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_CanncellationPenalty] DEFAULT ((0)) FOR [cancellationPenalty];

