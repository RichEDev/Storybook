ALTER TABLE [dbo].[contract_details]
    ADD CONSTRAINT [DF_Contract_Details_Total_Maintenance_Value] DEFAULT ((0)) FOR [totalMaintenanceValue];

