ALTER TABLE [dbo].[savedexpenses_previous]
    ADD CONSTRAINT [DF_savedexpenses_amountpayable] DEFAULT (0) FOR [amountpayable];

