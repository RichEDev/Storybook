ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_NoticePeriodType] DEFAULT ((0)) FOR [noticePeriodType];

