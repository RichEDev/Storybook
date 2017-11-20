ALTER TABLE [dbo].[savedexpenses_current]
    ADD CONSTRAINT [DF_savedexpenses_current_amountpayable] DEFAULT ((0)) FOR [amountpayable];

