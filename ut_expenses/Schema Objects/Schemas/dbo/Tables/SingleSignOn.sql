CREATE TABLE dbo.SingleSignOn
(
	IssuerUri			nvarchar(1000)		NOT NULL,
	PublicCertificate	varbinary(max)		NOT NULL,
	IdAttribute			nvarchar(200)		NOT NULL,
	IdLookupFieldId		uniqueidentifier	NOT NULL,
	CompanyIdAttribute	nvarchar(200)		NOT NULL,
	LoginErrorUrl		nvarchar(MAX),
	TimeoutUrl			nvarchar(MAX),
	ExitUrl				nvarchar(MAX)
)
