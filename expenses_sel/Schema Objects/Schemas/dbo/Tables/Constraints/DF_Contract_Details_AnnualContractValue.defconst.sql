ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_AnnualContractValue] DEFAULT ((0)) FOR [annualContractValue];

