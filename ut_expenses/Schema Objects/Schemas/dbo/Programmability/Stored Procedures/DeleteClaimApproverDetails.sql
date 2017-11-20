Create PROCEDURE [dbo].[DeleteClaimApproverDetails]
	@ExpenseId int,
	@CheckerId int
	As
Begin
	Delete ClaimApproverDetails where SavedExpenseId=@ExpenseId and CheckerId=@CheckerId
End