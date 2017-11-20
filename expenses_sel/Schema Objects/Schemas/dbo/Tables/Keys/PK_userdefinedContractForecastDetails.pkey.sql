ALTER TABLE [dbo].[userdefinedContractForecastDetails]
    ADD CONSTRAINT [PK_userdefinedContractForecastDetails] PRIMARY KEY CLUSTERED ([forecastid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

