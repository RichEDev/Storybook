﻿ALTER TABLE [dbo].[card_transactions_barclaycard_enhanced]
    ADD CONSTRAINT [PK_card_transactions_barclaycard_enhanced] PRIMARY KEY CLUSTERED ([transactionid] ASC) WITH (ALLOW_PAGE_LOCKS = ON, ALLOW_ROW_LOCKS = ON, PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF);

