ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_Maintenance_Percent_X] DEFAULT ((0)) FOR [maintenancePercentX];

