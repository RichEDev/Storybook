ALTER TABLE [dbo].[purchaseOrderRecurringScheduleDays]
    ADD CONSTRAINT [PK_purchaseOrderRecurringScheduleDays_1] PRIMARY KEY CLUSTERED ([purchaseOrderScheduleDaysID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

