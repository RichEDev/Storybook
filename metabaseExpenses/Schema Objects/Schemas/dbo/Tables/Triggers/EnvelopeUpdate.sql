CREATE TRIGGER EnvelopeUpdate ON [dbo].[Envelopes] FOR UPDATE
AS
	declare @envelopeId int;
	declare @accountId int;
	declare @claimId int;
	declare @envelopeNumber nvarchar(10);
	declare @crn nvarchar(12);	
	declare @envelopeStatus tinyint;
	declare @envelopeType int;
	declare @issued datetime;
	declare @assigned datetime;
	declare @received datetime;
	declare @attached datetime;
	declare @destroyed datetime;
	declare @overpayment decimal;
	declare @physicalStateUrl nvarchar(100);
	declare @lastModifiedBy int;
	declare @declaredLostInPost bit;
	declare @output nvarchar(500);
	DECLARE @cursor CURSOR
	
	SET CONCAT_NULL_YIELDS_NULL OFF;
	
	SET @cursor = CURSOR FAST_FORWARD
	FOR	SELECT i.EnvelopeId, 
				i.AccountId,
				i.ClaimId,
				i.EnvelopeNumber,
				i.CRN,
				i.EnvelopeStatus,
				i.EnvelopeType,
				i.DateIssuedToClaimant,
				i.DateAssignedToClaim,
				i.DateReceived,
				i.DateAttachCompleted,
				i.DateDestroyed,
				i.OverpaymentCharge,
				i.PhysicalStateProofUrl,
				i.LastModifiedBy,
				i.DeclaredLostInPost
		FROM inserted i;
	
	OPEN @cursor;
	
	FETCH NEXT FROM @cursor into
		@envelopeId,
		@accountId,
		@claimId,
		@envelopeNumber,
		@crn,
		@envelopeStatus,
		@envelopeType,
		@issued,
		@assigned,
		@received,
		@attached,
		@destroyed,
		@overpayment,
		@physicalStateUrl,
		@lastModifiedBy,
		@declaredLostInPost
		
	WHILE @@FETCH_STATUS = 0
	BEGIN
		
		if update(AccountId)  set @output = 'AccountId: ' + ISNULL(convert(nvarchar(10), @accountId), 'null')  + ', '
		if update(ClaimId)  set @output = @output + 'ClaimId: ' + ISNULL(convert(nvarchar(10), @claimId), 'null') + ', '
		if update(EnvelopeNumber)  set @output = @output + 'EnvelopeNumber: ' + @envelopeNumber + ', '
		if update(EnvelopeStatus)  set @output = @output + 'EnvelopeStatus: ' + ISNULL(convert(nvarchar(3), @envelopeStatus), 'null') + ', '
		if update(EnvelopeType)  set @output = @output + 'EnvelopeType: ' + ISNULL(convert(nvarchar(10), @envelopeType), 'null') + ', '
		if update(DateIssuedToClaimant)  set @output = @output + 'Issued: ' + ISNULL(convert(nvarchar(20), @issued), 'null') + ', '
		if update(DateAssignedToClaim)  set @output = @output + 'Assigned: ' + ISNULL(convert(nvarchar(20), @assigned), 'null') + ', '
		if update(DateReceived)  set @output = @output + 'Received: ' + ISNULL(convert(nvarchar(20), @received), 'null') + ', '
		if update(DateAttachCompleted)  set @output = @output + 'Attached: ' + ISNULL(convert(nvarchar(20), @attached), 'null') + ', '
		if update(DateDestroyed)  set @output = @output + 'Destroyed: ' + ISNULL(convert(nvarchar(20), @destroyed), 'null') + ', '
		if update(OverpaymentCharge)  set @output = @output + 'Overpayment: ' + ISNULL(convert(nvarchar(10), @overpayment), 'null') + ', '
		if update(PhysicalStateProofUrl) set @output = @output + 'PhysicalStateProof: ' + @physicalStateUrl + ', ' 
		if update(DeclaredLostInPost) set @output = @output + 'DeclaredLostInPost: ' + ISNULL(convert(nvarchar(1), @declaredLostInPost), 'null') + ', ' 
					
		insert into EnvelopeHistory (
			EnvelopeId, EnvelopeStatus, Data, ModifiedBy, ModifiedOn
		) values (
			@envelopeId, @envelopeStatus, @output, @lastModifiedBy, getutcdate()
		)	
		
	FETCH NEXT FROM @cursor into
		@envelopeId,
		@accountId,
		@claimId,
		@envelopeNumber,
		@crn,
		@envelopeStatus,
		@envelopeType,
		@issued,
		@assigned,
		@received,
		@attached,
		@destroyed,
		@overpayment,
		@physicalStateUrl,
		@lastModifiedBy,
		@declaredLostInPost
	END
	
	SET CONCAT_NULL_YIELDS_NULL ON;
