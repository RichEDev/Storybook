﻿ALTER TABLE [dbo].[SavedExpensesFuelReceiptToMileageAllocations]  WITH CHECK ADD  CONSTRAINT [FK_SavedExpensesFuelReceiptToMileageAllocations_savedexpenses_fuelreceiptexpenseid] FOREIGN KEY([FuelReceiptExpenseId])
			REFERENCES [dbo].[savedexpenses] ([expenseid])