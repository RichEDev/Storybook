ALTER TABLE [dbo].[SavedExpensesFuelReceiptToMileageAllocations]  WITH CHECK ADD  CONSTRAINT [FK_SavedExpensesFuelReceiptToMileageAllocations_savedexpenses_mileageexpenseid] FOREIGN KEY([MileageExpenseId])
REFERENCES [dbo].[savedexpenses] ([expenseid])
ON DELETE CASCADE