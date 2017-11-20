CREATE PROCEDURE [dbo].[SaveSingleSignOn]
	@IssuerUri			nvarchar(1000),
	@PublicCertificate	varbinary(max),
	@IdAttribute		nvarchar(200),
	@IdLookupFieldId	uniqueidentifier,
	@CompanyIdAttribute	nvarchar(200),
	@LoginErrorUrl		nvarchar(max),
	@TimeoutUrl			nvarchar(max),
	@ExitUrl			nvarchar(max),
	@CuEmployeeId		int,
	@CuDelegateId		int
AS

	declare @ElementId				int			= 185,
			@PublicCertificateHash	varchar(32)	= case when @PublicCertificate is null then null else master.sys.fn_repl_hash_binary(@PublicCertificate) end;

	if exists (select * from SingleSignOn)
	begin
		declare
			@PrevIssuerUri				nvarchar(1000),
			@PrevPublicCertificate		varbinary(max),
			@PrevIdAttribute			nvarchar(200),
			@PrevIdLookupFieldIdString	varchar(36),
			@PrevCompanyIdAttribute		nvarchar(200),
			@PrevLoginErrorUrl			nvarchar(max),
			@PrevTimeoutUrl				nvarchar(max),
			@PrevExitUrl				nvarchar(max)

		select top 1
			@PrevIssuerUri = IssuerUri,
			@PrevPublicCertificate = PublicCertificate,
			@PrevIdAttribute = IdAttribute,
			@PrevIdLookupFieldIdString = cast (IdLookupFieldId as varchar(36)),
			@PrevCompanyIdAttribute = CompanyIdAttribute,
			@PrevLoginErrorUrl = LoginErrorUrl,
			@PrevTimeoutUrl = TimeoutUrl,
			@PrevExitUrl = ExitUrl
		from
			SingleSignOn;

		declare	@PrevPublicCertificateHash	varchar(32)	= case when @PrevPublicCertificate is null then null else master.sys.fn_repl_hash_binary(@PrevPublicCertificate) end,
				@IdLookupFieldIdString		varchar(36)	= cast (@IdLookupFieldId as varchar(36));

		update
			SingleSignOn
		set
			IssuerUri = @IssuerUri,
			PublicCertificate = @PublicCertificate,
			IdAttribute = @IdAttribute,
			IdLookupFieldId = @IdLookupFieldId,
			CompanyIdAttribute = @CompanyIdAttribute,
			LoginErrorUrl = @LoginErrorUrl,
			TimeoutUrl = @TimeoutUrl,
			ExitUrl	= @ExitUrl;

		if NOT EXISTS(SELECT @PrevIssuerUri INTERSECT SELECT @IssuerUri) -- IF x IS DISTINCT FROM y, see http://stackoverflow.com/questions/10416789/
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, 'DFD98BAC-C3FE-4300-B887-11B9DDE4AA60', @PrevIssuerUri, @IssuerUri, 'Single Sign-on', null;
		if NOT EXISTS(SELECT @PrevPublicCertificateHash INTERSECT SELECT @PublicCertificateHash)
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '0105449B-9782-4290-83BF-EDE8252F0AF5', @PrevPublicCertificateHash, @PublicCertificateHash, 'Single Sign-on', null;
		if NOT EXISTS(SELECT @PrevIdAttribute INTERSECT SELECT @IdAttribute)
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, 'AE7CEB9D-74C9-43EA-B4C2-BEE6BEE0F57F', @PrevIdAttribute, @IdAttribute, 'Single Sign-on', null;
		if NOT EXISTS(SELECT @PrevIdLookupFieldIdString INTERSECT SELECT @IdLookupFieldIdString)
		begin
			declare @PrevValue nvarchar(4000),
					@CurrValue nvarchar(4000);
			select @PrevValue = field from fields where fieldid = @PrevIdLookupFieldIdString;
			select @CurrValue = field from fields where fieldid = @IdLookupFieldIdString;
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '24B4F27C-116B-47E7-BDA8-DDB78441BF40', @PrevValue, @CurrValue, 'Single Sign-on', null;
		end
		if NOT EXISTS(SELECT @PrevCompanyIdAttribute INTERSECT SELECT @CompanyIdAttribute)
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '94B06D26-2879-49C5-AF93-D935249278D9', @PrevCompanyIdAttribute, @CompanyIdAttribute, 'Single Sign-on', null;
		if NOT EXISTS(SELECT @PrevLoginErrorUrl INTERSECT SELECT @LoginErrorUrl)
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, 'FBFCED9D-DA87-48D7-9725-B817D04EF723', @PrevLoginErrorUrl, @LoginErrorUrl, 'Single Sign-on', null;
		if NOT EXISTS(SELECT @PrevTimeoutUrl INTERSECT SELECT @TimeoutUrl)
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '6394757D-2C20-4843-B477-DFD12D2620F8', @PrevTimeoutUrl, @TimeoutUrl, 'Single Sign-on', null;
		if NOT EXISTS(SELECT @PrevExitUrl INTERSECT SELECT @ExitUrl)
			exec addUpdateEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, '910267B6-157F-4BCE-85C1-92892AEC815B', @PrevExitUrl, @ExitUrl, 'Single Sign-on', null;

	end
	else
	begin
		insert SingleSignOn
		(IssuerUri, PublicCertificate, IdAttribute, IdLookupFieldId, CompanyIdAttribute, LoginErrorUrl, TimeoutUrl, ExitUrl)
		values
		(@IssuerUri, @PublicCertificate, @IdAttribute, @IdLookupFieldId, @CompanyIdAttribute, @LoginErrorUrl, @TimeoutUrl, @ExitUrl);

		exec addInsertEntryToAuditLog @CuEmployeeId, @CuDelegateId, @ElementId, null, 'Single Sign-on', null;

	end

RETURN 0
