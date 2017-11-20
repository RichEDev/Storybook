ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_Maintenance_Percent_Y] DEFAULT ((0)) FOR [maintenancePercentY];

