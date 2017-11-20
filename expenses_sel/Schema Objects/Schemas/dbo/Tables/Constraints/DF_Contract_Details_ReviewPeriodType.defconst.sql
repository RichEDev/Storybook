ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_ReviewPeriodType] DEFAULT ((0)) FOR [reviewPeriodType];

