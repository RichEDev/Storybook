ALTER TABLE [dbo].[purchaseOrderDeliveryRecords]
    ADD CONSTRAINT [PK_purchaseOrderDeliveryRecords] PRIMARY KEY CLUSTERED ([purchaseOrderDeliveryRecordID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

