CREATE PROCEDURE [dbo].[GetSingleSignOn]
AS
	select top 1
		IssuerUri,
		PublicCertificate,
		IdAttribute,
		IdLookupFieldId,
		CompanyIdAttribute,
		LoginErrorUrl,
		TimeoutUrl,
		ExitUrl
	from
		SingleSignOn

RETURN 0
