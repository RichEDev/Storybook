ALTER TABLE [dbo].[purchaseOrderRecurringScheduleMonths]
    ADD CONSTRAINT [PK_purchaseOrderRecurringScheduleMonths_1] PRIMARY KEY CLUSTERED ([purchaseOrderScheduleMonthsID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

