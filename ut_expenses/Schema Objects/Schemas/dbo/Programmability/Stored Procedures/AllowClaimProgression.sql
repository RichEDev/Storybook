CREATE procedure [dbo].[AllowClaimProgression] 
 @claimID int
AS
BEGIN
 SET NOCOUNT ON;
 
 DECLARE @oneClickApproval BIT
 DECLARE @approved BIT
 DECLARE @splitApprovalStage BIT
 DECLARE @itemsApproved INT
 DECLARE @claimItemsCount INT

 SELECT @oneClickApproval=displaysinglesignoff, @splitApprovalStage=splitApprovalStage, @itemsApproved=ItemsApproved, @claimItemsCount=ClaimItemsCount, @approved=approved FROM checkandpay 
 WHERE claimid = @claimID

 if (@oneClickApproval = 1)
 BEGIN
  if(@approved = 0 AND (@splitApprovalStage = 0 OR (@splitApprovalStage = 1 AND @itemsApproved = @claimItemsCount)))
  BEGIN 
	-- can approve
   return 1
  END
  ELSE 
  BEGIN
	-- can allocate for payment
    return 2
  END
 END
 ELSE
 BEGIN
  if(@approved = 1)
  BEGIN
	-- can allocate for paymment
   return 2
  END
  ELSE IF (@itemsApproved = @claimItemsCount)
  BEGIN
	-- can approve
	RETURN 1
  END
  ELSE
  BEGIN
	-- cannot approve or allocate for paymment
   return 0
  END
 END

END