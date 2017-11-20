ALTER TABLE [dbo].[userdefinedContractProductDetails]
    ADD CONSTRAINT [PK_userdefinedContractProductDetails] PRIMARY KEY CLUSTERED ([contractproductid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

