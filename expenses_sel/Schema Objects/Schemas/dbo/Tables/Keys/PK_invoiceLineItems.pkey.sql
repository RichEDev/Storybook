﻿ALTER TABLE [dbo].[invoiceLineItems]
    ADD CONSTRAINT [PK_invoiceLineItems] PRIMARY KEY CLUSTERED ([invoiceLineItemID] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

